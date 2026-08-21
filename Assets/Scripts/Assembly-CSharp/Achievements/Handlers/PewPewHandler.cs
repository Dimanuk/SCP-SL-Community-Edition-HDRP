namespace Achievements.Handlers
{
    public class PewPewHandler : global::Achievements.AchievementHandlerBase
    {
        private const float TimeLimit = 30f;

        private const int KillsTarget = 5;

        private static readonly global::System.Collections.Generic.Dictionary<global::Footprinting.Footprint, global::System.Collections.Generic.List<global::System.Diagnostics.Stopwatch>> Kills = new global::System.Collections.Generic.Dictionary<global::Footprinting.Footprint, global::System.Collections.Generic.List<global::System.Diagnostics.Stopwatch>>();

        private static readonly global::System.Collections.Generic.HashSet<uint> AlreadyAchieved = new global::System.Collections.Generic.HashSet<uint>();

        internal override void OnInitialize()
        {
            global::PlayerStatsSystem.PlayerStats.OnAnyPlayerDied += HandleDeath;
        }

        internal override void OnRoundStarted()
        {
            Kills.Clear();
        }

        private static void HandleDeath(ReferenceHub deadPlayer, global::PlayerStatsSystem.DamageHandlerBase handler)
        {
            if (!global::Mirror.NetworkServer.active || !(handler is global::PlayerStatsSystem.FirearmDamageHandler firearmDamageHandler))
            {
                return;
            }
            global::Footprinting.Footprint attacker = firearmDamageHandler.Attacker;
            if (!HitboxIdentity.CheckFriendlyFire(attacker.Role, global::PlayerRoles.PlayerRolesUtils.GetRoleId(deadPlayer), ignoreConfig: true) || AlreadyAchieved.Contains(attacker.NetId) || attacker.Hub == null)
            {
                return;
            }
            if (Kills.TryGetValue(attacker, out var value))
            {
                while (value.Count > 0 && value[0].Elapsed.TotalSeconds > 30.0)
                {
                    value.RemoveAt(0);
                }
                value.Add(global::System.Diagnostics.Stopwatch.StartNew());
                if (value.Count >= 5)
                {
                    AlreadyAchieved.Add(attacker.NetId);
                    Kills.Remove(attacker);
                    global::Achievements.AchievementHandlerBase.ServerAchieve(attacker.Hub.networkIdentity.connectionToClient, global::Achievements.AchievementName.PewPew);
                }
            }
            else
            {
                Kills.Add(attacker, new global::System.Collections.Generic.List<global::System.Diagnostics.Stopwatch> { global::System.Diagnostics.Stopwatch.StartNew() });
            }
        }
    }
}
