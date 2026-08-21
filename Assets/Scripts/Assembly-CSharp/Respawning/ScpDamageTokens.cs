using System.Collections.Generic;

using PlayerRoles;
using PlayerStatsSystem;
using UnityEngine;

namespace Respawning
{
	public static class ScpDamageTokens
	{
        private static readonly global::System.Collections.Generic.Dictionary<uint, global::System.Collections.Generic.Dictionary<global::PlayerRoles.Faction, float>> DamageContributions = new global::System.Collections.Generic.Dictionary<uint, global::System.Collections.Generic.Dictionary<global::PlayerRoles.Faction, float>>();

        private const float HsPointDamageReward = 0.0005f;

		private const float FullHealthbarDamageReward = 4f;

		private const float TerminationReward = 3f;

		private const float MicroKillBonus = 1.5f;

        private const global::PlayerRoles.Faction DefaultRewardReceiver = global::PlayerRoles.Faction.FoundationStaff;

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            global::PlayerStatsSystem.PlayerStats.OnAnyPlayerDied += OnAnyPlayerDied;
            global::PlayerStatsSystem.PlayerStats.OnAnyPlayerDamaged += OnAnyPlayerDamaged;
            CustomNetworkManager.OnClientReady += DamageContributions.Clear;
        }

        private static void OnAnyPlayerDied(ReferenceHub hub, global::PlayerStatsSystem.DamageHandlerBase dhb)
        {
            if (!global::Mirror.NetworkServer.active || !global::PlayerRoles.PlayerRolesUtils.IsSCP(hub, includeZombies: false))
            {
                return;
            }
            if (dhb is global::PlayerStatsSystem.MicroHidDamageHandler microHidDamageHandler)
            {
                RewardFaction(global::PlayerRoles.PlayerRolesUtils.GetFaction(microHidDamageHandler.Attacker.Role), 1.5f);
            }
            if (!DamageContributions.TryGetValue(hub.netId, out var value))
            {
                return;
            }
            float num = 0f;
            global::PlayerRoles.Faction faction = global::PlayerRoles.Faction.FoundationStaff;
            foreach (global::System.Collections.Generic.KeyValuePair<global::PlayerRoles.Faction, float> item in value)
            {
                if (!(item.Value <= num))
                {
                    faction = item.Key;
                    num = item.Value;
                }
            }
            RewardFaction(faction, 3f);
            DamageContributions.Remove(hub.netId);
        }

        private static void OnAnyPlayerDamaged(ReferenceHub hub, global::PlayerStatsSystem.DamageHandlerBase dhb)
        {
            if (global::Mirror.NetworkServer.active && global::PlayerRoles.PlayerRolesUtils.IsSCP(hub, includeZombies: false))
            {
                if (dhb is global::PlayerStatsSystem.AttackerDamageHandler adh)
                {
                    HandleDamageTickets(hub, adh);
                }
                else if (dhb is global::PlayerStatsSystem.StandardDamageHandler handler)
                {
                    RegisterContribution(hub, global::PlayerRoles.Faction.FoundationStaff, handler);
                }
            }
        }

        private static void RewardFaction(global::PlayerRoles.Faction faction, float totalTokens)
        {
            if (faction.TryGetAssignedSpawnableTeam(out var stt))
            {
                global::Respawning.RespawnTokensManager.GrantTokens(stt, totalTokens);
            }
        }


        private static void HandleDamageTickets(ReferenceHub scp, global::PlayerStatsSystem.AttackerDamageHandler adh)
        {
            if (scp.roleManager.CurrentRole is global::PlayerRoles.IHealthbarRole healthbarRole)
            {
                global::PlayerRoles.Faction faction = global::PlayerRoles.PlayerRolesUtils.GetFaction(adh.Attacker.Role);
                float totalTokens = adh.DealtHealthDamage / healthbarRole.MaxHealth * 4f + adh.AbsorbedHumeDamage * 0.0005f;
                RewardFaction(faction, totalTokens);
                RegisterContribution(scp, faction, adh);
            }
        }

        private static void RegisterContribution(ReferenceHub scp, global::PlayerRoles.Faction faction, global::PlayerStatsSystem.StandardDamageHandler handler)
        {
            global::System.Collections.Generic.Dictionary<global::PlayerRoles.Faction, float> orAdd = DamageContributions.GetOrAdd(scp.netId, () => new global::System.Collections.Generic.Dictionary<global::PlayerRoles.Faction, float>());
            orAdd.TryGetValue(faction, out var value);
            orAdd[faction] = value + handler.DealtHealthDamage;
        }
	}
}
