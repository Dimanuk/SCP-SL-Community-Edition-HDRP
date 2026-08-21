using Hints;
using InventorySystem.Items;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Pickups;
using System;
using UnityEngine;

namespace InventorySystem.Searching
{
    public class AmmoSearchCompletor : SearchCompletor
    {
        private readonly ItemType _ammoType;

        private ushort CurrentAmmo
        {
            get => Hub.inventory.GetCurAmmo(_ammoType);
            set => Hub.inventory.ServerSetAmmo(_ammoType, value);
        }

        private ushort MaxAmmo => InventorySystem.Configs.InventoryLimits.GetAmmoLimit(_ammoType, Hub);

        public AmmoSearchCompletor(ReferenceHub hub, ItemPickupBase targetPickup, ItemBase targetItem, double maxDistanceSquared)
            : base(hub, targetPickup, targetItem, maxDistanceSquared)
        {
            _ammoType = targetItem.ItemTypeId;
        }

        protected override bool ValidateAny()
        {
            if (!base.ValidateAny())
                return false;

            ushort maxAmmo = MaxAmmo;
            if (CurrentAmmo >= maxAmmo)
            {
                Hub.hints.Show(new TranslationHint(
                    HintTranslations.MaxAmmoAlreadyReached,
                    new HintParameter[]
                    {
                        new AmmoHintParameter((byte)_ammoType),
                        new PackedULongHintParameter(maxAmmo)
                    },
                    new[] { HintEffectPresets.TrailingPulseAlpha(0.5f, 1f, 0.5f, 2f, 0f, 2) },
                    2f));
                return false;
            }

            return true;
        }

        public override void Complete()
        {
            if (!(TargetPickup is AmmoPickup ammoPickup))
            {
                Debug.LogError("The pickup needs to derive from AmmoPickup");
                return;
            }

            uint current = CurrentAmmo;
            uint amountToAdd = Math.Min(current + ammoPickup.SavedAmmo, MaxAmmo) - current;

            if (amountToAdd >= ammoPickup.SavedAmmo)
            {
                TargetPickup.DestroySelf();
            }
            else
            {
                ammoPickup.SavedAmmo = (ushort)(ammoPickup.SavedAmmo - amountToAdd);

                PickupSyncInfo info = TargetPickup.Info;
                info.InUse = false;
                TargetPickup.Info = info;

                Hub.hints.Show(new TranslationHint(
                    HintTranslations.MaxAmmoReached,
                    new HintParameter[]
                    {
                        new AmmoHintParameter((byte)_ammoType),
                        new PackedULongHintParameter(MaxAmmo)
                    },
                    HintEffectPresets.FadeInAndOut(0.25f),
                    1.5f));
            }

            CurrentAmmo += (ushort)amountToAdd;
        }
    }
}