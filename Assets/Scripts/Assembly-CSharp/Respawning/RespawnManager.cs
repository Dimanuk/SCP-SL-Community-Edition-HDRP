using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GameCore;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.Spectating;
using Respawning.NamingRules;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace Respawning
{
    public class RespawnManager : MonoBehaviour
    {
        public enum RespawnSequencePhase : byte
        {
            RespawnCooldown = 0,
            SelectingTeam = 1,
            PlayingEntryAnimations = 2,
            SpawningSelectedTeam = 3
        }

        public static readonly Dictionary<SpawnableTeamType, SpawnableTeamHandlerBase> SpawnableTeams = new Dictionary<SpawnableTeamType, SpawnableTeamHandlerBase>
        {
            [SpawnableTeamType.ChaosInsurgency] = new ChaosInsurgencySpawnHandler("maximum_CI_respawn_amount", 15, "respawn_tickets_ci_initial_count", 18),
            [SpawnableTeamType.NineTailedFox] = new NineTailedFoxSpawnHandler("maximum_MTF_respawn_amount", 15, "respawn_tickets_mtf_initial_count", 24)
        };

        public static RespawnManager Singleton;

        public SpawnableTeamType NextKnownTeam;

        public UnitNamingHud NamingManager;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private bool _prioritySpawn;

        private RespawnSequencePhase _curSequence;

        private float _timeForNextSequence;

        private bool _started;

        public int TimeTillRespawn
        {
            get
            {
                return Mathf.RoundToInt(Singleton._timeForNextSequence - (float)Singleton._stopwatch.Elapsed.TotalSeconds);
            }
            internal set
            {
                Singleton._timeForNextSequence = value;
                Singleton._stopwatch.Restart();
            }
        }

        public static event Action<SpawnableTeamType, List<ReferenceHub>> ServerOnRespawned;

        private void Awake()
        {
            Singleton = this;
        }

        private void Start()
        {
            _prioritySpawn = ConfigFile.ServerConfig.GetBool("priority_mtf_respawn", def: true);
        }

        public static RespawnSequencePhase CurrentSequence()
        {
            return Singleton._curSequence;
        }

        private bool ReadyToCommence()
        {
            if (_started)
            {
                return true;
            }
            if (RoundStart.singleton.Timer == -1)
            {
                RestartSequence();
                _started = true;
            }
            return _started;
        }

        public static string GetRemoteAdminInfoString()
        {
            StringBuilder stringBuilder = StringBuilderPool.Shared.Rent(64);
            if (!Singleton._started)
            {
                stringBuilder.Append("The respawn system is off (or waiting for the round to start).");
            }
            else
            {
                int num = Singleton.TimeTillRespawn;
                int num2 = 0;
                while (num >= 60)
                {
                    num -= 60;
                    num2++;
                }
                switch (Singleton._curSequence)
                {
                    case RespawnSequencePhase.RespawnCooldown:
                        stringBuilder.Append("Next team selection in ");
                        break;
                    case RespawnSequencePhase.PlayingEntryAnimations:
                        stringBuilder.Append("The selected team (");
                        stringBuilder.Append(Singleton.NextKnownTeam);
                        stringBuilder.Append(") will arrive in ");
                        break;
                    default:
                        stringBuilder.Append("Respawn Manager reports the code ");
                        stringBuilder.Append((int)Singleton._curSequence);
                        stringBuilder.Append(" status. Try again in ");
                        break;
                }
                stringBuilder.Append(num2);
                stringBuilder.Append("m ");
                stringBuilder.Append(num);
                stringBuilder.Append("s");
            }
            string result = stringBuilder.ToString();
            StringBuilderPool.Shared.Return(stringBuilder);
            return result;
        }

        private void RestartSequence()
        {
            _timeForNextSequence = UnityEngine.Random.Range(ConfigFile.ServerConfig.GetFloat("minimum_MTF_time_to_spawn", 280f), ConfigFile.ServerConfig.GetFloat("maximum_MTF_time_to_spawn", 350f));
            _curSequence = RespawnSequencePhase.RespawnCooldown;
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Restart();
            }
            else
            {
                _stopwatch.Start();
            }
        }

        private bool CheckSpawnable(ReferenceHub ply)
        {
            if (ply.roleManager.CurrentRole is SpectatorRole spectatorRole)
            {
                return spectatorRole.ReadyToRespawn;
            }
            return false;
        }

        private void Update()
        {
            if (!NetworkServer.active || !ReadyToCommence())
            {
                return;
            }
            if (_stopwatch.Elapsed.TotalSeconds > (double)_timeForNextSequence)
            {
                _curSequence++;
            }
            if (_curSequence == RespawnSequencePhase.SelectingTeam)
            {
                if (!ReferenceHub.AllHubs.Any(CheckSpawnable))
                {
                    RestartSequence();
                    return;
                }
                SpawnableTeamType dominatingTeam = RespawnTokensManager.DominatingTeam;
                if (!SpawnableTeams.TryGetValue(dominatingTeam, out var value))
                {
                    throw new NotImplementedException($"{dominatingTeam} was returned as dominating team despite not being implemented.");
                }
                NextKnownTeam = dominatingTeam;
                _curSequence = RespawnSequencePhase.PlayingEntryAnimations;
                _stopwatch.Restart();
                _timeForNextSequence = value.EffectTime;
                RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.Selection, dominatingTeam);
            }
            if (_curSequence == RespawnSequencePhase.SpawningSelectedTeam)
            {
                Spawn();
                RestartSequence();
            }
        }

        public void ForceSpawnTeam(SpawnableTeamType teamToSpawn)
        {
            NextKnownTeam = teamToSpawn;
            Spawn();
            RestartSequence();
        }

        public void Spawn()
        {
            if (!SpawnableTeams.TryGetValue(NextKnownTeam, out var value) || NextKnownTeam == SpawnableTeamType.None)
            {
                ServerConsole.AddLog("Fatal error. Team '" + NextKnownTeam.ToString() + "' is undefined.", ConsoleColor.Red);
                return;
            }
            List<ReferenceHub> list = ReferenceHub.AllHubs.Where(CheckSpawnable).ToList();
            if (_prioritySpawn)
            {
                list = list.OrderByDescending((ReferenceHub item) => item.roleManager.CurrentRole.ActiveTime).ToList();
            }
            else
            {
                list.ShuffleList();
            }
            int maxWaveSize = value.MaxWaveSize;
            int num = list.Count;
            if (num > maxWaveSize)
            {
                list.RemoveRange(maxWaveSize, num - maxWaveSize);
                num = maxWaveSize;
            }
            if (num > 0 && UnitNamingRule.TryGetNamingRule(NextKnownTeam, out var rule))
            {
                UnitNameMessageHandler.SendNew(NextKnownTeam, rule);
            }
            list.ShuffleList();
            List<ReferenceHub> list2 = ListPool<ReferenceHub>.Shared.Rent();
            Queue<RoleTypeId> queue = new Queue<RoleTypeId>();
            value.GenerateQueue(queue, list.Count);
            foreach (ReferenceHub item in list)
            {
                try
                {
                    RoleTypeId newRole = queue.Dequeue();
                    item.roleManager.ServerSetRole(newRole, RoleChangeReason.Respawn);
                    list2.Add(item);
                    ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Player " + item.LoggedNameFromRefHub() + " respawned as " + newRole.ToString() + ".", ServerLogs.ServerLogType.GameEvent);
                }
                catch (Exception ex)
                {
                    if (item != null)
                    {
                        ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Player " + item.LoggedNameFromRefHub() + " couldn't be spawned. Err msg: " + ex.Message, ServerLogs.ServerLogType.GameEvent);
                    }
                    else
                    {
                        ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Couldn't spawn a player - target's ReferenceHub is null.", ServerLogs.ServerLogType.GameEvent);
                    }
                }
            }
            if (list2.Count > 0)
            {
                ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "RespawnManager has successfully spawned " + list2.Count + " players as " + NextKnownTeam.ToString() + "!", ServerLogs.ServerLogType.GameEvent);
                RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.UponRespawn, NextKnownTeam);
                RespawnTokensManager.RemoveTokens(NextKnownTeam, list2.Count);
            }
            RespawnManager.ServerOnRespawned?.Invoke(NextKnownTeam, list2);
            ListPool<ReferenceHub>.Shared.Return(list2);
            NextKnownTeam = SpawnableTeamType.None;
        }
    }
}
