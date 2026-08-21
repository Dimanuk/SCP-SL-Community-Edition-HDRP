namespace Scp914.Processors
{
    public class MicroHidItemProcessor : global::Scp914.Processors.Scp914ItemProcessor
    {
        private const float DisruptorThreshold = 0.6f;

        private const float RechargeThreshold = 0.3f;

        private global::InventorySystem.Items.Firearms.FirearmStatus GetStatusForFirearm(ItemType firearm)
        {
            if (!global::InventorySystem.InventoryItemLoader.TryGetItem<global::InventorySystem.Items.Firearms.Firearm>(firearm, out var result))
            {
                return default(global::InventorySystem.Items.Firearms.FirearmStatus);
            }
            byte ammo;
            global::InventorySystem.Items.Firearms.FirearmStatusFlags flags;
            uint attachments;
            if (result is global::InventorySystem.Items.Firearms.ParticleDisruptor)
            {
                ammo = 5;
                flags = global::InventorySystem.Items.Firearms.FirearmStatusFlags.MagazineInserted;
                attachments = global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.ValidateAttachmentsCode(result, 0u);
            }
            else
            {
                ammo = 0;
                flags = global::InventorySystem.Items.Firearms.FirearmStatusFlags.None;
                attachments = global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.GetRandomAttachmentsCode(firearm);
            }
            return new global::InventorySystem.Items.Firearms.FirearmStatus(ammo, flags, attachments);
        }

        public override global::InventorySystem.Items.ItemBase OnInventoryItemUpgraded(global::Scp914.Scp914KnobSetting setting, ReferenceHub hub, ushort serial)
        {
            if (!hub.inventory.UserInventory.Items.TryGetValue(serial, out var value))
            {
                return null;
            }
            ItemType output = GetOutput(setting);
            if (output == ItemType.MicroHID)
            {
                if (value is global::InventorySystem.Items.MicroHID.MicroHIDItem microHIDItem)
                {
                    microHIDItem.Recharge();
                    return microHIDItem;
                }
                return null;
            }
            global::InventorySystem.InventoryExtensions.ServerRemoveItem(hub.inventory, serial, null);
            global::InventorySystem.Items.ItemBase itemBase = global::InventorySystem.InventoryExtensions.ServerAddItem(hub.inventory, output, 0);
            if (itemBase is global::InventorySystem.Items.Firearms.Firearm firearm)
            {
                firearm.Status = GetStatusForFirearm(output);
            }
            return itemBase;
        }

        public override global::InventorySystem.Items.Pickups.ItemPickupBase OnPickupUpgraded(global::Scp914.Scp914KnobSetting setting, global::InventorySystem.Items.Pickups.ItemPickupBase ipb, global::UnityEngine.Vector3 newPosition)
        {
            ItemType output = GetOutput(setting);
            if (output == ItemType.MicroHID)
            {
                if (ipb is global::InventorySystem.Items.MicroHID.MicroHIDPickup microHIDPickup)
                {
                    microHIDPickup.Energy = 1f;
                }
                ipb.transform.position = newPosition;
                return ipb;
            }
            ipb.DestroySelf();
            if (!global::InventorySystem.InventoryItemLoader.AvailableItems.TryGetValue(output, out var value) || !ReferenceHub.TryGetLocalHub(out var hub))
            {
                return null;
            }
            global::InventorySystem.Items.Pickups.PickupSyncInfo psi = new global::InventorySystem.Items.Pickups.PickupSyncInfo
            {
                ItemId = output,
                Serial = global::InventorySystem.Items.ItemSerialGenerator.GenerateNext(),
                Weight = value.Weight
            };
            global::InventorySystem.Items.Pickups.ItemPickupBase itemPickupBase = global::InventorySystem.InventoryExtensions.ServerCreatePickup(hub.inventory, value, psi);
            itemPickupBase.transform.position = newPosition;
            itemPickupBase.transform.rotation = ipb.transform.rotation;
            itemPickupBase.RefreshPositionAndRotation();
            if (itemPickupBase is global::InventorySystem.Items.Firearms.FirearmPickup firearmPickup)
            {
                firearmPickup.Status = GetStatusForFirearm(value.ItemTypeId);
            }
            return itemPickupBase;
        }

        private ItemType GetOutput(global::Scp914.Scp914KnobSetting setting)
        {
            switch (setting)
            {
                case global::Scp914.Scp914KnobSetting.Rough:
                    return ItemType.GunE11SR;
                case global::Scp914.Scp914KnobSetting.OneToOne:
                case global::Scp914.Scp914KnobSetting.Fine:
                case global::Scp914.Scp914KnobSetting.VeryFine:
                    return ItemType.MicroHID;
                default:
                    {
                        float value = global::UnityEngine.Random.value;
                        if (!(value > 0.6f))
                        {
                            if (!(value > 0.3f))
                            {
                                return ItemType.None;
                            }
                            return ItemType.MicroHID;
                        }
                        return ItemType.ParticleDisruptor;
                    }
            }
        }
    }
}
