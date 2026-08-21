namespace Respawning
{
    public static class HumanTerminationTokens
    {
        private const float DClassKilledMilitantReward = 2f;

        private const float KilledMilitantReward = 1.5f;

        private const float MilitantDiedPenalty = 1.2f;

        private const float KilledScientistReward = 1.4f;

        private const float GuardKilledArmedDClassReward = 0.5f;

        private static readonly global::System.Collections.Generic.Dictionary<global::Respawning.SpawnableTeamType, int> NumberOfRespawns = new global::System.Collections.Generic.Dictionary<global::Respawning.SpawnableTeamType, int>();

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            global::PlayerStatsSystem.PlayerStats.OnAnyPlayerDied += OnPlayerDied;
            CustomNetworkManager.OnClientReady += NumberOfRespawns.Clear;
            global::Respawning.RespawnManager.ServerOnRespawned += delegate (global::Respawning.SpawnableTeamType team, global::System.Collections.Generic.List<ReferenceHub> list)
            {
                if (!NumberOfRespawns.TryGetValue(team, out var value))
                {
                    value = 0;
                }
                NumberOfRespawns[team] = value + 1;
            };
        }

        private static void OnPlayerDied(ReferenceHub ply, global::PlayerStatsSystem.DamageHandlerBase handler)
        {
            if (handler is global::PlayerStatsSystem.AttackerDamageHandler attackerDamageHandler && global::PlayerRoles.PlayerRolesUtils.GetFaction(attackerDamageHandler.Attacker.Role) != global::PlayerRoles.PlayerRolesUtils.GetFaction(ply))
            {
                HandleHomocide(ply, attackerDamageHandler);
            }
            else
            {
                HandleOtherMilitantDeath(ply);
            }
        }

        private static void HandleHomocide(ReferenceHub deadPly, global::PlayerStatsSystem.AttackerDamageHandler adh)
        {
            global::PlayerRoles.Team team = global::PlayerRoles.PlayerRolesUtils.GetTeam(adh.Attacker.Role);
            global::PlayerRoles.Faction faction = global::PlayerRoles.PlayerRolesUtils.GetFaction(team);
            switch (global::PlayerRoles.PlayerRolesUtils.GetTeam(deadPly))
            {
                case global::PlayerRoles.Team.FoundationForces:
                    HandleFoundationForcesHomocide(team, deadPly);
                    break;
                case global::PlayerRoles.Team.ChaosInsurgency:
                    if (faction == global::PlayerRoles.Faction.FoundationStaff)
                    {
                        global::Respawning.RespawnTokensManager.GrantTokens(global::Respawning.SpawnableTeamType.NineTailedFox, 1.5f);
                    }
                    else
                    {
                        HandleOtherMilitantDeath(deadPly);
                    }
                    break;
                case global::PlayerRoles.Team.Scientists:
                    if (faction == global::PlayerRoles.Faction.FoundationEnemy)
                    {
                        global::Respawning.RespawnTokensManager.GrantTokens(global::Respawning.SpawnableTeamType.ChaosInsurgency, 1.4f);
                    }
                    break;
                case global::PlayerRoles.Team.ClassD:
                    if (adh.Attacker.Role == global::PlayerRoles.RoleTypeId.FacilityGuard && global::Utils.NonAllocLINQ.DictionaryExtensions.Any(deadPly.inventory.UserInventory.Items, (global::System.Collections.Generic.KeyValuePair<ushort, global::InventorySystem.Items.ItemBase> x) => IsArmedCategory(x.Value.Category)))
                    {
                        global::Respawning.RespawnTokensManager.GrantTokens(global::Respawning.SpawnableTeamType.NineTailedFox, 0.5f);
                    }
                    break;
            }
        }

        private static void HandleOtherMilitantDeath(ReferenceHub deadPly)
        {
            if (!(deadPly.roleManager.CurrentRole is global::PlayerRoles.HumanRole humanRole))
            {
                return;
            }
            global::Respawning.SpawnableTeamType assignedSpawnableTeam = humanRole.AssignedSpawnableTeam;
            if (global::Respawning.RespawnTokensManager.SupportedTeams.Contains(assignedSpawnableTeam))
            {
                if (!NumberOfRespawns.TryGetValue(assignedSpawnableTeam, out var value))
                {
                    value = 1;
                }
                global::Respawning.RespawnTokensManager.RemoveTokens(assignedSpawnableTeam, 1.2f * (float)value);
            }
        }

        private static void HandleFoundationForcesHomocide(global::PlayerRoles.Team attackerTeam, ReferenceHub deadPly)
        {
            switch (attackerTeam)
            {
                case global::PlayerRoles.Team.ClassD:
                    global::Respawning.RespawnTokensManager.GrantTokens(global::Respawning.SpawnableTeamType.ChaosInsurgency, 2f);
                    return;
                case global::PlayerRoles.Team.ChaosInsurgency:
                    global::Respawning.RespawnTokensManager.GrantTokens(global::Respawning.SpawnableTeamType.ChaosInsurgency, 1.5f);
                    return;
            }
            if (global::PlayerRoles.PlayerRolesUtils.GetRoleId(deadPly) != global::PlayerRoles.RoleTypeId.FacilityGuard)
            {
                HandleOtherMilitantDeath(deadPly);
            }
        }

        private static bool IsArmedCategory(ItemCategory category)
        {
            if (category - 4 <= ItemCategory.Keycard || category == ItemCategory.MicroHID)
            {
                return true;
            }
            return false;
        }
    }
}
