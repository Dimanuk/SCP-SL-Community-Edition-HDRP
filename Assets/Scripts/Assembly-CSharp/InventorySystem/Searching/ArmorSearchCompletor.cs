using Hints;
using InventorySystem.Items;
using InventorySystem.Items.Armor;
using InventorySystem.Items.Pickups;

namespace InventorySystem.Searching
{
    public class ArmorSearchCompletor : SearchCompletor
    {
        private readonly ItemType _armorType;

        public ArmorSearchCompletor(ReferenceHub hub, ItemPickupBase targetPickup, ItemBase targetItem, double maxDistanceSquared)
            : base(hub, targetPickup, targetItem, maxDistanceSquared)
        {
            _armorType = targetItem.ItemTypeId;
        }

        protected override bool ValidateAny()
        {
            if (!base.ValidateAny())
                return false;

            if (BodyArmorUtils.TryGetBodyArmorAndItsSerial(Hub.inventory, out _, out _))
                return true;

            if (Hub.inventory.UserInventory.Items.Count >= 8)
            {
                Hub.hints.Show(new TranslationHint(
                    HintTranslations.MaxItemsAlreadyReached,
                    new HintParameter[] { new ByteHintParameter(8) },
                    new HintEffect[] { HintEffectPresets.TrailingPulseAlpha(0.5f, 1f, 0.5f, 2f, 0f, 3) }, 2f));
                return false;
            }

            return true;
        }

        public override bool ValidateStart()
        {
            if (!base.ValidateStart())
                return false;

            if (TargetItem?.ItemTypeId == ItemType.None)
                throw new System.InvalidOperationException("Item has an invalid ItemType.");

            if (TargetItem.Category != ItemCategory.Armor)
                throw new System.InvalidOperationException("Item is not equippable (can be held in inventory).");

            return true;
        }

        public override void Complete()
        {
            if (BodyArmorUtils.TryGetBodyArmorAndItsSerial(Hub.inventory, out BodyArmor currentArmor, out ushort serial))
            {
                currentArmor.DontRemoveExcessOnDrop = true;
                Hub.inventory.ServerDropItem(serial);
            }

            BodyArmor armor = Hub.inventory.ServerAddItem(TargetPickup.Info.ItemId, TargetPickup.Info.Serial, TargetPickup) as BodyArmor;
            BodyArmorUtils.RemoveEverythingExceedingLimits(Hub.inventory, armor);
            TargetPickup.DestroySelf();
        }
    }
}