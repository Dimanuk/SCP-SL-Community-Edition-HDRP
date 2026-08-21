using System;
using System.Collections.Generic;
using InventorySystem.Items;
using InventorySystem.Items.MicroHID;
using InventorySystem.Items.Usables;
using PlayerRoles;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Spectating;
using UnityEngine;
using InventorySystem;

namespace PlayerRoles.PlayableScps.Scp939.Ripples
{
    public class HeldItemRippleTrigger : RippleTriggerBase
    {
        [SerializeField]
        private float _cooldown;

        private readonly AbilityCooldown _cd;
        private readonly Dictionary<ushort, bool> _usableItemStates;

        public override void SpawnObject()
        {
            base.SpawnObject();
            _usableItemStates.Clear();
            UsableItemsController.OnClientStatusReceived += ProcessUsableMessage;
        }

        public override void ResetObject()
        {
            base.ResetObject();
            _cd.Clear();
            UsableItemsController.OnClientStatusReceived -= ProcessUsableMessage;
        }
        private void ProcessUsableMessage(StatusMessage msg)
        {
            _usableItemStates[msg.ItemSerial] = msg.Status == 0;
        }

        private void Update()
        {
            if (!Owner.isLocalPlayer && !SpectatorNetworking.IsLocallySpectated(Owner))
                return;


            if (_cd == null || !_cd.IsReady)
                return;
                
            _cd.Trigger(_cooldown);
            PlayerRolesUtils.ForEachRole<HumanRole>(ProcessPlayer);
        }

        private void ProcessPlayer(ReferenceHub hub, HumanRole role)
        {
            if (hub == null)
                return;

            var inventory = hub.inventory;
            if (inventory == null)
                return;

            var curItem = inventory.CurItem;

            if (!InventoryItemLoader.TryGetItem<ItemBase>(curItem.TypeId, out var item) || item == null)
                return;

            if (item is MicroHIDItem)
            {
                if (!ProcessMicro(curItem.SerialNumber))
                    return;
            }

            else if (item is UsableItem)
            {
                if (!_usableItemStates.TryGetValue(curItem.SerialNumber, out bool isUsing) || !isUsing)
                    return;
            }
            else
            {
                return;
            }


            base.Player?.Play(role);
        }

         private bool ProcessMicro(ushort serial)
        {
            if (!MicroHIDHandler.SyncStates.TryGetValue(serial, out HidStatusMessage state))
                return false;

            if (state.MessageCode == 0)
                return false;

            return state.MessageCode != 5;
        }

        public HeldItemRippleTrigger()
        {
            _cd = new AbilityCooldown();
            _usableItemStates = new Dictionary<ushort, bool>();
        }
    }
}