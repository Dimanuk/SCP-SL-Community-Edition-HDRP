using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using Mirror;
using PlayerRoles;
using UnityEngine;

namespace Respawning
{
    public static class ItemPickupTokens
    {
        private static bool _hidAlreadyPickedUp;

        private const float MicroPickupReward = 1f;
        private const float WeaponHeldReward = 0.4f;

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            InventoryExtensions.OnItemAdded += OnItemAdded;
            InventoryExtensions.OnItemRemoved += OnItemRemoved;

            CustomNetworkManager.OnClientReady += delegate
            {
                _hidAlreadyPickedUp = false;
            };
        }

        private static bool TryGetSpawnableTeam(ReferenceHub hub, out SpawnableTeamType stt)
        {
            if (NetworkServer.active && RespawnTokensManager.TryGetAssignedSpawnableTeam(hub, out stt))
                return true;

            stt = SpawnableTeamType.None;
            return false;
        }

        private static bool IsCivilian(ReferenceHub hub)
        {
            if (hub.roleManager.CurrentRole is HumanRole humanRole)
                return humanRole.AssignedSpawnableTeam == SpawnableTeamType.None;

            return false;
        }

        private static void OnItemAdded(ReferenceHub hub, ItemBase ib, ItemPickupBase ipb)
        {
            if (!TryGetSpawnableTeam(hub, out var stt))
                return;

            switch ((byte)ib.Category)
            {
                case 7:
                    if (!_hidAlreadyPickedUp)
                    {
                        _hidAlreadyPickedUp = true;
                        RespawnTokensManager.GrantTokens(stt, MicroPickupReward);
                    }
                    break;

                case 4: 
                    if (IsCivilian(hub))
                        RespawnTokensManager.GrantTokens(stt, WeaponHeldReward);
                    break;
            }
        }

        private static void OnItemRemoved(ReferenceHub hub, ItemBase ib, ItemPickupBase ipb)
        {
            if (ib.Category == ItemCategory.Firearm && IsCivilian(hub) && TryGetSpawnableTeam(hub, out var stt))
                RespawnTokensManager.RemoveTokens(stt, WeaponHeldReward);
        }
    }
}