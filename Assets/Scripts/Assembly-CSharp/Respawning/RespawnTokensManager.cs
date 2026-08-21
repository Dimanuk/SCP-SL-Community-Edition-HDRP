using System;
using System.Collections.Generic;

using PlayerRoles;
using UnityEngine;

namespace Respawning
{
	public static class RespawnTokensManager
	{
        private class TokenCounter
        {
            private float _amount;

            public global::Respawning.SpawnableTeamType Team;

            public global::Respawning.SpawnableTeamHandlerBase Handler;

            public float Amount
            {
                get
                {
                    return global::UnityEngine.Mathf.Max(0f, _amount);
                }
                set
                {
                    _amount = value;
                }
            }

            public TokenCounter(global::Respawning.SpawnableTeamType team, global::Respawning.SpawnableTeamHandlerBase handler)
            {
                Team = team;
                Handler = handler;
            }
        }

        public static readonly global::System.Collections.Generic.HashSet<global::Respawning.SpawnableTeamType> SupportedTeams = new global::System.Collections.Generic.HashSet<global::Respawning.SpawnableTeamType>();

        private static readonly bool DebugMode = false;

        private static readonly global::System.Collections.Generic.List<global::Respawning.RespawnTokensManager.TokenCounter> Counters = new global::System.Collections.Generic.List<global::Respawning.RespawnTokensManager.TokenCounter>();

        private static int _teamsCount;

		private const float TotalTokens = 100f;

		private const float AccuracyTolerance = 0.5f;

		private const float OverallMultiplier = 3.5f;

        private static float TotalAssigned
        {
            get
            {
                float total = 0f;
                ForEachCounter(delegate (global::Respawning.RespawnTokensManager.TokenCounter x)
                {
                    total += x.Amount;
                });
                return total;
            }
        }

        public static global::Respawning.SpawnableTeamType DominatingTeam
        {
            get
            {
                float num = 0f;
                int index = 0;
                for (int i = 0; i < _teamsCount; i++)
                {
                    float amount = Counters[i].Amount;
                    if (!(amount <= num))
                    {
                        num = amount;
                        index = i;
                    }
                }
                return Counters[index].Team;
            }
        }

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            global::Utils.NonAllocLINQ.DictionaryExtensions.ForEach(global::Respawning.RespawnManager.SpawnableTeams, delegate (global::System.Collections.Generic.KeyValuePair<global::Respawning.SpawnableTeamType, global::Respawning.SpawnableTeamHandlerBase> x)
            {
                Counters.Add(new global::Respawning.RespawnTokensManager.TokenCounter(x.Key, x.Value));
            });
            Counters.ForEach(delegate (global::Respawning.RespawnTokensManager.TokenCounter x)
            {
                SupportedTeams.Add(x.Team);
            });
            _teamsCount = Counters.Count;
            if (_teamsCount <= 1)
            {
                throw new global::System.NotImplementedException("Respawn tokens require multiple teams to be set up.");
            }
            CustomNetworkManager.OnClientReady += ResetTokens;
        }

        private static void ForEachCounter(global::System.Action<global::Respawning.RespawnTokensManager.TokenCounter> action)
        {
            for (int i = 0; i < _teamsCount; i++)
            {
                action(Counters[i]);
            }
        }

        private static void UpdateAccuracy(bool force = false)
        {
            float totalAssigned = TotalAssigned;
            if (!force && global::UnityEngine.Mathf.Abs(totalAssigned - 100f) < 0.5f)
            {
                return;
            }
            if (global::UnityEngine.Mathf.Approximately(totalAssigned, 0f))
            {
                ForEachCounter(delegate (global::Respawning.RespawnTokensManager.TokenCounter x)
                {
                    x.Amount = 100f;
                });
                UpdateAccuracy();
            }
            else
            {
                float multip = 100f / totalAssigned;
                ForEachCounter(delegate (global::Respawning.RespawnTokensManager.TokenCounter x)
                {
                    x.Amount *= multip;
                });
            }
        }

        public static void ResetTokens()
        {
            if (global::Mirror.NetworkServer.active)
            {
                ForEachCounter(delegate (global::Respawning.RespawnTokensManager.TokenCounter x)
                {
                    x.Amount = x.Handler.StartTokens;
                });
                UpdateAccuracy(force: true);
            }
        }

        public static void ModifyTokens(global::Respawning.SpawnableTeamType team, float amount)
        {
            if (DebugMode)
            {
                string text = new global::System.Diagnostics.StackTrace().ToString();
                text = text.Remove(0, text.LastIndexOf("RespawnTokensManager"));
                string message = $"Tokens of ({team}) modified by ({amount}) by ({text})";
                global::Utils.NonAllocLINQ.HashsetExtensions.ForEach(ReferenceHub.AllHubs, delegate (ReferenceHub x)
                {
                    x.gameConsoleTransmission.SendToClient(message, global::UnityEngine.Color.white.ToString());
                });
            }
            amount *= 3.5f;
            bool flag = false;
            float num = (0f - amount) / (float)(_teamsCount - 1);
            for (int num2 = 0; num2 < _teamsCount; num2++)
            {
                global::Respawning.RespawnTokensManager.TokenCounter tokenCounter = Counters[num2];
                if (team == tokenCounter.Team)
                {
                    flag = true;
                    tokenCounter.Amount += amount;
                }
                else
                {
                    tokenCounter.Amount += num;
                }
            }
            UpdateAccuracy();
            if (flag)
            {
                return;
            }
            throw new global::System.ArgumentException($"Cannot add tokens to {team} - team not defined.");
        }

        public static void GrantTokens(global::Respawning.SpawnableTeamType team, float tokens)
        {
            if (tokens < 0f)
            {
                throw new global::System.ArgumentOutOfRangeException("tokens", "Cannot grant negative tokens. Use RemoveTokens instead.");
            }
            ModifyTokens(team, tokens);
        }

        public static void RemoveTokens(global::Respawning.SpawnableTeamType team, float tokens)
        {
            if (tokens < 0f)
            {
                throw new global::System.ArgumentOutOfRangeException("tokens", "Cannot revoke negative tokens. Use GrantTokens instead.");
            }
            ModifyTokens(team, 0f - tokens);
        }

        public static float GetTeamDominance(global::Respawning.SpawnableTeamType team)
        {
            float totalAssigned = TotalAssigned;
            for (int i = 0; i < _teamsCount; i++)
            {
                global::Respawning.RespawnTokensManager.TokenCounter tokenCounter = Counters[i];
                if (tokenCounter.Team == team)
                {
                    return tokenCounter.Amount / totalAssigned;
                }
            }
            throw new global::System.ArgumentException("Cannot return dominance of undefined team.", "team");
        }

        public static void ForceTeamDominance(global::Respawning.SpawnableTeamType team, float val)
        {
            float totalAssigned = TotalAssigned;
            float num = global::UnityEngine.Mathf.Clamp01(val) * totalAssigned;
            float num2 = totalAssigned - num;
            float num3 = 0f;
            for (int i = 0; i < _teamsCount; i++)
            {
                global::Respawning.RespawnTokensManager.TokenCounter tokenCounter = Counters[i];
                if (tokenCounter.Team != team)
                {
                    num3 += tokenCounter.Amount;
                }
                else
                {
                    tokenCounter.Amount = num;
                }
            }
            for (int j = 0; j < _teamsCount; j++)
            {
                global::Respawning.RespawnTokensManager.TokenCounter tokenCounter2 = Counters[j];
                if (tokenCounter2.Team != team)
                {
                    float num4 = num2;
                    if (num3 > 0f)
                    {
                        num4 *= tokenCounter2.Amount / num3;
                    }
                    tokenCounter2.Amount = num4;
                }
            }
            UpdateAccuracy();
        }

        public static bool TryGetAssignedSpawnableTeam(this global::PlayerRoles.Faction faction, out global::Respawning.SpawnableTeamType stt)
        {
            switch (faction)
            {
                case global::PlayerRoles.Faction.FoundationEnemy:
                    stt = global::Respawning.SpawnableTeamType.ChaosInsurgency;
                    return true;
                case global::PlayerRoles.Faction.FoundationStaff:
                    stt = global::Respawning.SpawnableTeamType.NineTailedFox;
                    return true;
                default:
                    stt = global::Respawning.SpawnableTeamType.None;
                    return false;
            }
        }

        public static bool TryGetAssignedSpawnableTeam(this global::PlayerRoles.RoleTypeId role, out global::Respawning.SpawnableTeamType stt)
        {
            return global::PlayerRoles.PlayerRolesUtils.GetFaction(role).TryGetAssignedSpawnableTeam(out stt);
        }

        public static bool TryGetAssignedSpawnableTeam(this ReferenceHub ply, out global::Respawning.SpawnableTeamType stt)
        {
            return global::PlayerRoles.PlayerRolesUtils.GetFaction(ply).TryGetAssignedSpawnableTeam(out stt);
        }
	}
}
