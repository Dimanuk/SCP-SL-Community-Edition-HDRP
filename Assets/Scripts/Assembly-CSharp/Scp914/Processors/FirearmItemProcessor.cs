namespace Scp914.Processors
{
    public class FirearmItemProcessor : global::Scp914.Processors.Scp914ItemProcessor
    {
        [global::System.Serializable]
        private struct FirearmOutput
        {
            [global::UnityEngine.Range(0f, 1f)]
            public float Chance;

            public ItemType[] TargetItems;
        }

        [global::UnityEngine.SerializeField]
        private global::Scp914.Processors.FirearmItemProcessor.FirearmOutput[] _roughOutputs;

        [global::UnityEngine.SerializeField]
        private global::Scp914.Processors.FirearmItemProcessor.FirearmOutput[] _coarseOutputs;

        [global::UnityEngine.SerializeField]
        private global::Scp914.Processors.FirearmItemProcessor.FirearmOutput[] _fineOutputs;

        [global::UnityEngine.SerializeField]
        private global::Scp914.Processors.FirearmItemProcessor.FirearmOutput[] _veryFineOutputs;

        private static readonly ItemType[] None = new ItemType[1] { ItemType.None };

        public override global::InventorySystem.Items.ItemBase OnInventoryItemUpgraded(global::Scp914.Scp914KnobSetting setting, ReferenceHub hub, ushort serial)
        {
            if (!hub.inventory.UserInventory.Items.TryGetValue(serial, out var value))
            {
                return null;
            }
            global::InventorySystem.Items.ItemBase itemBase = null;
            ItemType[] items = GetItems(setting, value.ItemTypeId);
            foreach (ItemType newType in items)
            {
                if (itemBase == null)
                {
                    itemBase = UpgradePlayer(newType, value, hub, serial);
                }
                else
                {
                    UpgradePlayer(newType, value, hub, serial);
                }
            }
            return itemBase;
        }

        public override global::InventorySystem.Items.Pickups.ItemPickupBase OnPickupUpgraded(global::Scp914.Scp914KnobSetting setting, global::InventorySystem.Items.Pickups.ItemPickupBase ipb, global::UnityEngine.Vector3 newPos)
        {
            global::InventorySystem.Items.Pickups.ItemPickupBase itemPickupBase = null;
            ItemType[] items = GetItems(setting, ipb.Info.ItemId);
            foreach (ItemType newType in items)
            {
                if (itemPickupBase == null)
                {
                    itemPickupBase = UpgradePickup(newType, ipb, newPos);
                }
                else
                {
                    UpgradePickup(newType, ipb, newPos);
                }
            }
            return itemPickupBase;
        }

        private global::InventorySystem.Items.ItemBase UpgradePlayer(ItemType newType, global::InventorySystem.Items.ItemBase item, ReferenceHub hub, ushort serial)
        {
            if (!(item is global::InventorySystem.Items.Firearms.Firearm firearm))
            {
                throw new global::System.InvalidOperationException("FirearmItemProcessor can't be used for non-firearm items, such as " + item.ItemTypeId);
            }
            if (global::InventorySystem.InventoryItemLoader.AvailableItems.TryGetValue(newType, out var value) && value is global::InventorySystem.Items.Firearms.Firearm firearm2)
            {
                global::Scp914.Processors.AmmoItemProcessor.ExchangeAmmo(firearm.AmmoType, firearm2.AmmoType, firearm.Status.Ammo, out var exchangedAmmo, out var change);
                global::InventorySystem.InventoryExtensions.ServerAddAmmo(hub.inventory, firearm.AmmoType, change);
                global::InventorySystem.InventoryExtensions.ServerAddAmmo(hub.inventory, firearm2.AmmoType, exchangedAmmo);
            }
            if (newType == item.ItemTypeId)
            {
                uint randomAttachmentsCode = global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.GetRandomAttachmentsCode(newType);
                firearm.Status = new global::InventorySystem.Items.Firearms.FirearmStatus(0, global::InventorySystem.Items.Firearms.FirearmStatusFlags.None, randomAttachmentsCode);
                global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.ApplyAttachmentsCode(firearm, randomAttachmentsCode, reValidate: false);
            }
            else
            {
                global::InventorySystem.InventoryExtensions.ServerRemoveItem(hub.inventory, serial, null);
                if (global::InventorySystem.InventoryItemLoader.AvailableItems.ContainsKey(newType))
                {
                    return global::InventorySystem.InventoryExtensions.ServerAddItem(hub.inventory, newType, 0);
                }
            }
            return item;
        }

        private global::InventorySystem.Items.Pickups.ItemPickupBase UpgradePickup(ItemType newType, global::InventorySystem.Items.Pickups.ItemPickupBase ipb, global::UnityEngine.Vector3 newPos)
        {
            if (!global::InventorySystem.InventoryItemLoader.AvailableItems.TryGetValue(newType, out var value))
            {
                if (newType == ItemType.None)
                {
                    ipb.DestroySelf();
                    return null;
                }
                ipb.transform.position = newPos;
                return ipb;
            }
            if (!(ipb is global::InventorySystem.Items.Firearms.FirearmPickup firearmPickup))
            {
                throw new global::System.InvalidOperationException("FirearmItemProcessor can't be used for non-firearm items, such as " + value.ItemTypeId);
            }
            uint attachments = 0u;
            if (value is global::InventorySystem.Items.Firearms.Firearm firearm && global::InventorySystem.InventoryItemLoader.AvailableItems.TryGetValue(firearmPickup.Info.ItemId, out var value2) && value2 is global::InventorySystem.Items.Firearms.Firearm firearm2)
            {
                global::Scp914.Processors.AmmoItemProcessor.ExchangeAmmo(firearm2.AmmoType, firearm.AmmoType, firearmPickup.Status.Ammo, out var exchangedAmmo, out var change);
                global::Scp914.Processors.AmmoItemProcessor.CreateAmmoPickup(firearm2.AmmoType, change, newPos);
                global::Scp914.Processors.AmmoItemProcessor.CreateAmmoPickup(firearm.AmmoType, exchangedAmmo, newPos);
                attachments = global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.ValidateAttachmentsCode(firearm, 0u);
            }
            if (newType == ipb.Info.ItemId)
            {
                firearmPickup.Status = new global::InventorySystem.Items.Firearms.FirearmStatus(0, global::InventorySystem.Items.Firearms.FirearmStatusFlags.None, global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.GetRandomAttachmentsCode(newType));
                firearmPickup.transform.position = newPos;
                return firearmPickup;
            }
            global::InventorySystem.Items.Pickups.PickupSyncInfo psi = new global::InventorySystem.Items.Pickups.PickupSyncInfo
            {
                ItemId = newType,
                Serial = global::InventorySystem.Items.ItemSerialGenerator.GenerateNext(),
                Weight = value.Weight
            };
            global::InventorySystem.Items.Pickups.ItemPickupBase itemPickupBase = global::InventorySystem.InventoryExtensions.ServerCreatePickup(ReferenceHub.LocalHub.inventory, value, psi);
            itemPickupBase.transform.position = newPos;
            itemPickupBase.transform.rotation = ipb.transform.rotation;
            if (itemPickupBase is global::InventorySystem.Items.Firearms.FirearmPickup firearmPickup2)
            {
                firearmPickup2.Status = new global::InventorySystem.Items.Firearms.FirearmStatus(0, global::InventorySystem.Items.Firearms.FirearmStatusFlags.None, attachments);
            }
            ipb.DestroySelf();
            return itemPickupBase;
        }

        private ItemType[] GetItems(global::Scp914.Scp914KnobSetting setting, ItemType input)
        {
            global::Scp914.Processors.FirearmItemProcessor.FirearmOutput[] array;
            switch (setting)
            {
                case global::Scp914.Scp914KnobSetting.OneToOne:
                    return new ItemType[1] { input };
                case global::Scp914.Scp914KnobSetting.Rough:
                    array = _roughOutputs;
                    break;
                case global::Scp914.Scp914KnobSetting.Coarse:
                    array = _coarseOutputs;
                    break;
                case global::Scp914.Scp914KnobSetting.Fine:
                    array = _fineOutputs;
                    break;
                case global::Scp914.Scp914KnobSetting.VeryFine:
                    array = _veryFineOutputs;
                    break;
                default:
                    return None;
            }
            float value = global::UnityEngine.Random.value;
            float num = 0f;
            global::Scp914.Processors.FirearmItemProcessor.FirearmOutput[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                global::Scp914.Processors.FirearmItemProcessor.FirearmOutput firearmOutput = array2[i];
                num += firearmOutput.Chance;
                if (num >= value)
                {
                    return firearmOutput.TargetItems;
                }
            }
            return None;
        }
    }
}
