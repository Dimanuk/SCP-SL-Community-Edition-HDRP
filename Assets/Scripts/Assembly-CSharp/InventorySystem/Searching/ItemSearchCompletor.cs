using Hints;
using InventorySystem.Configs;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using System;
using UnityEngine;

namespace InventorySystem.Searching
{
    public class ItemSearchCompletor : SearchCompletor
    {
        private readonly ItemCategory _category;

        private sbyte CategoryCount
        {
            get
            {
                if (Hub?.inventory?.UserInventory?.Items == null)
                    return 0;

                sbyte count = 0;
                foreach (var item in Hub.inventory.UserInventory.Items.Values)
                {
                    if (item.Category == _category)
                        count++;
                }
                return count;
            }
        }

        public ItemSearchCompletor(ReferenceHub hub, ItemPickupBase targetPickup, ItemBase targetItem, double maxDistanceSquared)
            : base(hub, targetPickup, targetItem, maxDistanceSquared)
        {
            _category = targetItem.Category;
        }

        protected override bool ValidateAny()
        {
            if (!base.ValidateAny())
                return false;

            if (Hub.inventory.UserInventory.Items.Count >= 8)
            {
                Hub.hints.Show(new TranslationHint(HintTranslations.MaxItemsAlreadyReached,
                    new HintParameter[] { new ByteHintParameter(8) },
                    new HintEffect[] { HintEffectPresets.TrailingPulseAlpha(0.5f, 1f, 0.5f, 2f, 0f, 3) }, 2f));
                return false;
            }

            if (_category != ItemCategory.None)
            {
                int num = Mathf.Abs(InventoryLimits.GetCategoryLimit(_category, Hub));
                if (CategoryCount >= num)
                {
                    Hub.hints.Show(new TranslationHint(HintTranslations.MaxItemCategoryAlreadyReached,
                        new HintParameter[]
                        {
                            new ItemCategoryHintParameter(_category),
                            new ByteHintParameter((byte)num)
                        },
                        new HintEffect[] { HintEffectPresets.TrailingPulseAlpha(0.5f, 1f, 0.5f, 2f, 0f, 2) }, 2f));
                    return false;
                }
            }

            return true;
        }

        public override bool ValidateStart()
        {
            if (!base.ValidateStart())
                return false;

            if (TargetItem.ItemTypeId == ItemType.None)
                throw new InvalidOperationException("Item has an invalid ItemType.");

            if (TargetItem.Category == ItemCategory.Ammo)
                throw new InvalidOperationException("Item is not equippable (can be held in inventory).");

            return true;
        }

        public override void Complete()
        {
            ItemBase addedItem = Hub.inventory.ServerAddItem(TargetPickup.Info.ItemId, TargetPickup.Info.Serial, TargetPickup);
            if (addedItem == null)
            {
                // Inventory rejected the item (full, unknown type, etc.). Destroying the
                // pickup here would silently delete the item; leaving InUse set would make
                // it unpickable forever. Release the lock and keep the pickup on the ground.
                var info = TargetPickup.Info;
                info.InUse = false;
                TargetPickup.Info = info;
                return;
            }
            TargetPickup.DestroySelf();
            CheckCategoryLimitHint();
        }

        protected void CheckCategoryLimitHint()
        {
            sbyte categoryLimit = InventoryLimits.GetCategoryLimit(_category, Hub);
            if (_category != ItemCategory.None && categoryLimit >= 0 && CategoryCount >= categoryLimit)
            {
                Hub.hints.Show(new TranslationHint(HintTranslations.MaxItemCategoryReached,
                    new HintParameter[]
                    {
                        new ItemCategoryHintParameter(_category),
                        new ByteHintParameter((byte)categoryLimit)
                    },
                    HintEffectPresets.FadeInAndOut(0.25f), 1.5f));
            }
        }
    }
}