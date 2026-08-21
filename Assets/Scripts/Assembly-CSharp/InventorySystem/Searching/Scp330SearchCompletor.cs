using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Usables.Scp330;
using Hints;
using UnityEngine;
using System;

namespace InventorySystem.Searching
{
    public class Scp330SearchCompletor : SearchCompletor
    {
        private readonly Scp330Bag _playerBag;

        public Scp330SearchCompletor(ReferenceHub hub, ItemPickupBase targetPickup, ItemBase targetItem, double maxDistanceSquared)
            : base(hub, targetPickup, targetItem, maxDistanceSquared)
        {
            Scp330Bag.TryGetBag(hub, out _playerBag);
        }

        protected override bool ValidateAny()
        {
            if (!base.ValidateAny())
                return false;

            bool hasBag = _playerBag != null;

            if (hasBag)
            {
                if (_playerBag.Candies.Count >= 6)
                {
                    ShowOverloadHint(Hub, true);
                    return false;
                }
            }
            else
            {
                if (Hub.inventory.UserInventory.Items.Count >= 8)
                {
                    ShowOverloadHint(Hub, false);
                    return false;
                }
            }

            return true;
        }

        public static void ShowOverloadHint(ReferenceHub ply, bool hasBag)
        {
            if (!hasBag)
            {
                ply.hints.Show(new TranslationHint(
                    HintTranslations.MaxItemsAlreadyReached,
                    new HintParameter[] { new ByteHintParameter(8) },
                    new HintEffect[] { HintEffectPresets.TrailingPulseAlpha(0.5f, 1f, 0.5f, 2f, 0f, 3) }, 2f
                ));
            }
            else
            {
                ply.hints.Show(new TranslationHint(
                    HintTranslations.MaxItemCategoryAlreadyReached,
                    new HintParameter[]
                    {
                        new Scp330HintParameter(0),
                        new ByteHintParameter(6)
                    },
                    new HintEffect[] { HintEffectPresets.TrailingPulseAlpha(0.5f, 1f, 0.5f, 2f, 0f, 2) }, 2f
                ));
            }
        }

        public override void Complete()
        {
            if (TargetPickup is not Scp330Pickup scp330Pickup)
                return;

            if (Scp330Bag.ServerProcessPickup(Hub, scp330Pickup, out _))
            {
                if (scp330Pickup.StoredCandies.Count > 0)
                {
                    PickupSyncInfo info = scp330Pickup.Info;
                    info.InUse = false;
                    scp330Pickup.Info = info;
                }
                else
                {
                    scp330Pickup.DestroySelf();
                }
            }
        }
    }
}