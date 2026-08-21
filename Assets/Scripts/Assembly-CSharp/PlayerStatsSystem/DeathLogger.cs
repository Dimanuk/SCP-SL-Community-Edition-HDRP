namespace PlayerStatsSystem
{
    public static class DeathLogger
    {
        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            global::PlayerStatsSystem.PlayerStats.OnAnyPlayerDied += HandleDeath;
        }

        private static void HandleDeath(ReferenceHub ply, global::PlayerStatsSystem.DamageHandlerBase handler)
        {
            global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
            stringBuilder.Append(ply.LoggedNameFromRefHub());
            stringBuilder.Append(", playing as ");
            stringBuilder.Append(ply.roleManager.CurrentRole.RoleName);
            stringBuilder.Append(", ");
            ServerLogs.ServerLogType type;
            if (handler is global::PlayerStatsSystem.AttackerDamageHandler attackerDamageHandler)
            {
                if (attackerDamageHandler.IsSuicide)
                {
                    type = ServerLogs.ServerLogType.Suicide;
                    stringBuilder.Append("has commited suicide.");
                }
                else
                {
                    if (attackerDamageHandler.IsFriendlyFire)
                    {
                        type = ServerLogs.ServerLogType.Teamkill;
                        stringBuilder.Append("has been teamkilled by ");
                    }
                    else
                    {
                        type = ServerLogs.ServerLogType.KillLog;
                        stringBuilder.Append("has been killed by ");
                    }
                    stringBuilder.Append(attackerDamageHandler.Attacker.Nickname);
                    stringBuilder.Append(" (");
                    stringBuilder.Append(attackerDamageHandler.Attacker.LogUserID);
                    stringBuilder.Append(") playing as: ");
                    stringBuilder.Append(global::PlayerRoles.PlayerRoleLoader.TryGetRoleTemplate<global::PlayerRoles.PlayerRoleBase>(attackerDamageHandler.Attacker.Role, out var result) ? result.RoleName : "Unknown class");
                    stringBuilder.Append(".");
                }
            }
            else
            {
                type = ServerLogs.ServerLogType.KillLog;
                stringBuilder.Append("has died.");
            }
            stringBuilder.Append(" Specific death reason: ");
            stringBuilder.Append(handler.ServerLogsText);
            stringBuilder.Append(".");
            ServerLogs.AddLog(ServerLogs.Modules.ClassChange, stringBuilder.ToString(), type);
            global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
        }
    }
}
