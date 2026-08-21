using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameCore;
using Mirror;
using PlayerRoles.Spectating;
using UnityEngine;
using Utils.NonAllocLINQ;
using Debug = UnityEngine.Debug;

namespace PlayerRoles.RoleAssign
{
    public static class RoleAssigner
    {
        private static readonly global::System.Diagnostics.Stopwatch LateJoinTimer = new global::System.Diagnostics.Stopwatch();

        private static readonly global::System.Collections.Generic.HashSet<string> AlreadySpawnedPlayers = new global::System.Collections.Generic.HashSet<string>();

        private const string DefaultQueue = "4014314031441404134041434414";

        private const string SpawnQueueKey = "team_respawn_queue";

        private const string LateJoinKey = "late_join_time";

        private static bool _spawned;

        private static int _prevQueueSize;

        private static global::PlayerRoles.Team[] _totalQueue;

        private static global::PlayerRoles.Team[] _humanQueue;

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += delegate
            {
                _spawned = false;
                AlreadySpawnedPlayers.Clear();
            };
            CharacterClassManager.OnInstanceModeChanged += CheckLateJoin;
            CharacterClassManager.OnRoundStarted += OnRoundStarted;
        }


        private static void OnRoundStarted()
        {
            if (!global::Mirror.NetworkServer.active)
                return;

            string text = global::GameCore.ConfigFile.ServerConfig.GetString("team_respawn_queue", "4014314031441404134041434414");
            int length = text.Length;
            if (_prevQueueSize < length)
            {
                _totalQueue = new global::PlayerRoles.Team[length];
                _humanQueue = new global::PlayerRoles.Team[length];
                _prevQueueSize = length;
            }
            int queueLength = 0;
            int num = 0;
            string text2 = text;
            for (int i = 0; i < text2.Length; i++)
            {
                global::PlayerRoles.Team team = (global::PlayerRoles.Team)(text2[i] - 48);
                if (global::System.Enum.IsDefined(typeof(global::PlayerRoles.Team), team))
                {
                    if (team != global::PlayerRoles.Team.SCPs)
                    {
                        _humanQueue[queueLength++] = team;
                    }
                    _totalQueue[num++] = team;
                }
            }
            if (num == 0)
            {
                throw new global::System.InvalidOperationException("Failed to assign roles, queue has failed to load.");
            }
            _spawned = true;
            LateJoinTimer.Restart();
            int num2 = global::Utils.NonAllocLINQ.HashsetExtensions.Count(ReferenceHub.AllHubs, (ReferenceHub x) => CheckPlayer(x));
            int num3 = 0;
            for (int num4 = 0; num4 < num2; num4++)
            {
                if (_totalQueue[num4 % num] == global::PlayerRoles.Team.SCPs)
                {
                    num3++;
                }
            }

            if (NonFacilityCompatibility.currentSceneSettings.forcedClass != PlayerRoles.RoleTypeId.None)
            {
                foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
                {
                    allHub.roleManager.ServerSetRole(NonFacilityCompatibility.currentSceneSettings.forcedClass, RoleChangeReason.LateJoin);
                }
                return;
            }

            global::PlayerRoles.RoleAssign.ScpSpawner.SpawnScps(num3);
            global::PlayerRoles.RoleAssign.HumanSpawner.SpawnHumans(_humanQueue, queueLength);
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.IsAlive())
                {
                    AlreadySpawnedPlayers.Add(allHub.characterClassManager.UserId);
                }
            }
        }

        private static void CheckLateJoin(ReferenceHub hub, ClientInstanceMode cim)
        {
            if (global::Mirror.NetworkServer.active && CheckPlayer(hub) && _spawned)
            {
                float num = global::GameCore.ConfigFile.ServerConfig.GetFloat("late_join_time");
                if (!AlreadySpawnedPlayers.Add(hub.characterClassManager.UserId) || LateJoinTimer.Elapsed.TotalSeconds > (double)num)
                {
                    hub.roleManager.ServerSetRole(global::PlayerRoles.RoleTypeId.Spectator, global::PlayerRoles.RoleChangeReason.LateJoin);
                }
                else
                {
                    global::PlayerRoles.RoleAssign.HumanSpawner.SpawnLate(hub);
                }
            }
        }

        public static bool CheckPlayer(ReferenceHub hub)
        {
            if (hub.IsAlive() || (hub.roleManager.CurrentRole is global::PlayerRoles.Spectating.SpectatorRole spectatorRole && !spectatorRole.ReadyToRespawn))
            {
                return false;
            }
            ClientInstanceMode instanceMode = hub.characterClassManager.InstanceMode;
            if (instanceMode - 1 <= ClientInstanceMode.ReadyClient)
            {
                return true;
            }
            return false;
        }
    }
}
