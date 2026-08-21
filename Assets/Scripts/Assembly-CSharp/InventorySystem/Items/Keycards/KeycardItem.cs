using System;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;
using Interactables.Interobjects.DoorUtils;
using PlayerRoles.Spectating;
using InventorySystem.Items.Pickups;

namespace InventorySystem.Items.Keycards
{
    public class KeycardItem : ItemBase, IItemNametag
    {
        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public struct UseMessage : NetworkMessage { }

        public KeycardPermissions Permissions;
        public override float Weight => 0.01f;
        public override ItemDescriptionType DescriptionType => ItemDescriptionType.Keycard;

        private string _name = "[NONE]";
        public string Name => _name;

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += () =>
            {
                NetworkServer.ReplaceHandler<UseMessage>((conn, msg) =>
                {
                    if (ReferenceHub.TryGetHubNetID(conn.identity.netId, out var hub) &&
                        hub.inventory.CurInstance is KeycardItem)
                    {
                        SpectatorNetworking.SendToSpectatorsOf(msg, hub, includeTarget: false);
                    }
                }, requireAuthentication: true);
            };
        }

        public override void OnAdded(ItemPickupBase ipb)
        {
            base.OnAdded(ipb);
            var reader = new ItemTranslationReader(ItemTypeId);
            _itm = reader;
            _name = _itm.Name;
        }

        private ItemTranslationReader _itm;
    }
}
