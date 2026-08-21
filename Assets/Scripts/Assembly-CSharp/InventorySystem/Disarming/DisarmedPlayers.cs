using InventorySystem.Items;

namespace InventorySystem.Disarming
{
    public static class DisarmedPlayers
    {
        public readonly struct DisarmedEntry
        {
            public readonly uint DisarmedPlayer;

            public readonly uint Disarmer;

            public DisarmedEntry(uint disarmedPlayer, uint disarmer)
            {
                DisarmedPlayer = disarmedPlayer;
                Disarmer = disarmer;
            }
        }

        public static global::System.Collections.Generic.List<global::InventorySystem.Disarming.DisarmedPlayers.DisarmedEntry> Entries = new global::System.Collections.Generic.List<global::InventorySystem.Disarming.DisarmedPlayers.DisarmedEntry>();

        private const float AutoDisarmDistanceSquared = 8100f;

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            Mirror.NetworkServer.RegisterHandler<InventorySystem.Disarming.DisarmedPlayersListMessage>((conn, msg) => { });
            Mirror.NetworkClient.RegisterHandler<InventorySystem.Disarming.DisarmedPlayersListMessage>((msg) => { });
            StaticUnityMethods.OnUpdate += Update;

            global::PlayerRoles.PlayerRoleManager.OnRoleChanged += delegate (ReferenceHub hub, global::PlayerRoles.PlayerRoleBase prevRole, global::PlayerRoles.PlayerRoleBase newRole)
            {
                if (global::Mirror.NetworkServer.active && prevRole is global::PlayerRoles.HumanRole)
                {
                    for (int i = 0; i < Entries.Count; i++)
                    {
                        if (Entries[i].DisarmedPlayer == hub.netId)
                        {
                            Entries.RemoveAt(i);
                            global::Utils.Networking.NetworkUtils.SendToAuthenticated(new global::InventorySystem.Disarming.DisarmedPlayersListMessage(Entries));
                            break;
                        }
                    }
                }
            };
        }

        private static void Update()
        {
            if (!global::Mirror.NetworkServer.active)
            {
                return;
            }
            for (int i = 0; i < Entries.Count; i++)
            {
                if (!ValidateEntry(Entries[i]))
                {
                    Entries.RemoveAt(i);
                    global::Utils.Networking.NetworkUtils.SendToAuthenticated(new global::InventorySystem.Disarming.DisarmedPlayersListMessage(Entries));
                    break;
                }
            }
        }

        private static bool ValidateEntry(global::InventorySystem.Disarming.DisarmedPlayers.DisarmedEntry entry)
        {
            if (entry.Disarmer == 0)
            {
                return true;
            }
            if (!ReferenceHub.TryGetHubNetID(entry.DisarmedPlayer, out var hub))
            {
                return false;
            }
            if (!ReferenceHub.TryGetHubNetID(entry.Disarmer, out var hub2))
            {
                return false;
            }
            if (!global::PlayerRoles.PlayerRolesUtils.IsHuman(hub) || !global::PlayerRoles.PlayerRolesUtils.IsHuman(hub2))
            {
                return false;
            }
            if (global::PlayerRoles.PlayerRolesUtils.GetFaction(hub) == global::PlayerRoles.PlayerRolesUtils.GetFaction(hub2))
            {
                return false;
            }
            global::UnityEngine.Vector3 position = (hub.roleManager.CurrentRole as global::PlayerRoles.FirstPersonControl.IFpcRole).FpcModule.Position;
            global::UnityEngine.Vector3 position2 = (hub2.roleManager.CurrentRole as global::PlayerRoles.FirstPersonControl.IFpcRole).FpcModule.Position;
            if ((position - position2).sqrMagnitude > 8100f)
            {
                return false;
            }
            hub.inventory.ServerDropEverything();
            return true;
        }

        public static bool IsDisarmed(this global::InventorySystem.Inventory inv)
        {
            foreach (global::InventorySystem.Disarming.DisarmedPlayers.DisarmedEntry entry in Entries)
            {
                if (entry.DisarmedPlayer == inv.netId)
                {
                    return true;
                }
            }
            return false;
        }

        public static void SetDisarmedStatus(this global::InventorySystem.Inventory inv, global::InventorySystem.Inventory disarmer)
        {
            bool flag;
            do
            {
                flag = true;
                for (int i = 0; i < Entries.Count; i++)
                {
                    if (Entries[i].DisarmedPlayer == inv.netId)
                    {
                        Entries.RemoveAt(i);
                        flag = false;
                        break;
                    }
                }
            }
            while (!flag);
            if (disarmer != null)
            {
                Entries.Add(new global::InventorySystem.Disarming.DisarmedPlayers.DisarmedEntry(inv.netId, disarmer.netId));
            }
        }

        public static bool CanDisarm(this ReferenceHub disarmerHub, ReferenceHub targetHub)
        {
            if (global::PlayerRoles.PlayerRolesUtils.GetFaction(disarmerHub) == global::PlayerRoles.PlayerRolesUtils.GetFaction(targetHub))
            {
                return false;
            }
            if (!global::PlayerRoles.PlayerRolesUtils.IsHuman(disarmerHub) || !global::PlayerRoles.PlayerRolesUtils.IsHuman(targetHub))
            {
                return false;
            }
            if (targetHub.interCoordinator.AnyBlocker(global::InventorySystem.Items.BlockedInteraction.BeDisarmed))
            {
                return false;
            }
            global::InventorySystem.Items.ItemBase curInstance = disarmerHub.inventory.CurInstance;
            if (curInstance != null && curInstance is global::InventorySystem.Items.IDisarmingItem disarmingItem)
            {
                return disarmingItem.AllowDisarming;
            }
            return false;
        }

        public static bool CanUndisarm(this ReferenceHub disarmerHub, ReferenceHub targetHub)
        {
            if (!targetHub.inventory.IsDisarmed())
            {
                return false;
            }
            if (!global::PlayerRoles.PlayerRolesUtils.IsHuman(disarmerHub) || !global::PlayerRoles.PlayerRolesUtils.IsHuman(targetHub))
            {
                return false;
            }
            return true;
        }
    }
}
