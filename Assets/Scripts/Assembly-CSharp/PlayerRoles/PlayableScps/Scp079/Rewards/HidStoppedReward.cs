namespace PlayerRoles.PlayableScps.Scp079.Rewards
{
    public static class HidStoppedReward
    {
        private const int Reward = 50;

        private const float MinReadiness = 0.75f;

        private const float TimeTolerance = 10f;

        private const float ScpMinProximitySqr = 600f;

        private static bool _available;

        private static double _microDamageCooldown;

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            global::PlayerRoles.PlayerRoleManager.OnRoleChanged += delegate (ReferenceHub hub, global::PlayerRoles.PlayerRoleBase x, global::PlayerRoles.PlayerRoleBase y)
            {
                if (global::Mirror.NetworkServer.active && y is global::PlayerRoles.PlayableScps.Scp079.Scp079Role)
                {
                    _available = true;
                }
            };
            global::PlayerStatsSystem.PlayerStats.OnAnyPlayerDamaged += delegate (ReferenceHub hub, global::PlayerStatsSystem.DamageHandlerBase dh)
            {
                if (global::Mirror.NetworkServer.active && _available && dh is global::PlayerStatsSystem.MicroHidDamageHandler)
                {
                    _microDamageCooldown = global::Mirror.NetworkTime.time + 10.0;
                }
            };
            global::PlayerStatsSystem.PlayerStats.OnAnyPlayerDied += delegate (ReferenceHub hub, global::PlayerStatsSystem.DamageHandlerBase dh)
            {
                if (global::Mirror.NetworkServer.active && _available && hub.inventory.CurInstance is global::InventorySystem.Items.MicroHID.MicroHIDItem microHIDItem && !(microHIDItem == null) && !(microHIDItem.Readiness < 0.75f))
                {
                    TryGrant(hub);
                }
            };
            global::InventorySystem.Items.MicroHID.MicroHIDItem.OnStopCharging += delegate (global::InventorySystem.Items.MicroHID.MicroHIDItem hid)
            {
                if (_available && !(global::Mirror.NetworkTime.time < _microDamageCooldown) && !(hid.Readiness < 0.75f))
                {
                    TryGrant(hid.Owner);
                }
            };
        }

        private static void TryGrant(ReferenceHub ply)
        {
            if (ply.roleManager.CurrentRole is global::PlayerRoles.FirstPersonControl.IFpcRole fpcRole)
            {
                global::UnityEngine.Vector3 humanPos = fpcRole.FpcModule.Position;
                global::MapGeneration.RoomIdentifier roomIdentifier = global::MapGeneration.RoomIdUtils.RoomAtPositionRaycasts(humanPos);
                if (!(roomIdentifier == null) && global::Utils.NonAllocLINQ.HashsetExtensions.Any(ReferenceHub.AllHubs, (ReferenceHub x) => IsNearbyTeammate(humanPos, x)) && global::PlayerRoles.PlayableScps.Scp079.Rewards.Scp079RewardManager.GrantExpForRoom(roomIdentifier, 50, global::PlayerRoles.PlayableScps.Scp079.Scp079HudTranslation.ExpGainHidStopped))
                {
                    _available = false;
                }
            }
        }

        private static bool IsNearbyTeammate(global::UnityEngine.Vector3 attackerPos, ReferenceHub teammate)
        {
            if (!teammate.IsSCP(includeZombies: false))
            {
                return false;
            }
            if (!(teammate.roleManager.CurrentRole is global::PlayerRoles.FirstPersonControl.IFpcRole fpcRole))
            {
                return false;
            }
            return (fpcRole.FpcModule.Position - attackerPos).sqrMagnitude < 600f;
        }
    }
}
