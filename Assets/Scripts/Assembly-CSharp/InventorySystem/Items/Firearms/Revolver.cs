using System.Runtime.CompilerServices;

using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.Pickups;
using UnityEngine;

namespace InventorySystem.Items.Firearms
{
    public class Revolver : Firearm
    {
        private FirearmBaseStats _uncockedStats;

        private bool _uncockedStatsSet;

        [SerializeField]
        private FirearmBaseStats _stats;

        [SerializeField]
        private float _uncockedInaccuracyAddition;

        private static readonly int StockAttachmentIndex = 4;

        private global::InventorySystem.Items.Firearms.FirearmBaseStats UncockedStats
        {
            get
            {
                if (!_uncockedStatsSet)
                {
                    _uncockedStats = _stats;
                    _uncockedStats.HipInaccuracy += _uncockedInaccuracyAddition;
                    _uncockedStats.AdsInaccuracy += _uncockedInaccuracyAddition;
                    _uncockedStatsSet = true;
                }
                return _uncockedStats;
            }
        }

        public override global::InventorySystem.Items.Firearms.FirearmBaseStats BaseStats
        {
            get
            {
                if (!Status.Flags.HasFlagFast(global::InventorySystem.Items.Firearms.FirearmStatusFlags.Cocked))
                {
                    return UncockedStats;
                }
                return _stats;
            }
        }

        public override ItemType AmmoType => ItemType.Ammo44cal;

        public override global::InventorySystem.Items.Firearms.Modules.IAmmoManagerModule AmmoManagerModule { get; set; }

        public override global::InventorySystem.Items.Firearms.Modules.IEquipperModule EquipperModule { get; set; }

        public override global::InventorySystem.Items.Firearms.Modules.IActionModule ActionModule { get; set; }

        public override global::InventorySystem.Items.Firearms.Modules.IInspectorModule InspectorModule { get; set; }

        public override global::InventorySystem.Items.Firearms.Modules.IAdsModule AdsModule { get; set; }

        public override global::InventorySystem.Items.Firearms.Modules.IHitregModule HitregModule { get; set; }

        public override void OnAdded(global::InventorySystem.Items.Pickups.ItemPickupBase pickup)
        {
            base.OnAdded(pickup);
            EquipperModule = new global::InventorySystem.Items.Firearms.Modules.ChambercheckEquipper(this, "First Time Pickup", 0.45f, 1.4f);
            InspectorModule = new global::InventorySystem.Items.Firearms.Modules.SimpleInspector(this, 0);
            AmmoManagerModule = new global::InventorySystem.Items.Firearms.Modules.ClipLoadedInternalMagAmmoManager(this, 6);
            ActionModule = new global::InventorySystem.Items.Firearms.Modules.DoubleAction(this, 0.1f, 0.15f, 0.7f, "Cock", 1, 8, 2);
            AdsModule = new global::InventorySystem.Items.Firearms.Modules.StandardAds(this, base.ItemSerial, 0.2f, 1, 4, 5);
            HitregModule = new global::InventorySystem.Items.Firearms.Modules.SingleBulletHitreg(this, base.Owner);
        }

        public override void UpdateAnims()
        {
            if (ActionModule == null) return; 

            this.AnimSetFloat(global::InventorySystem.Items.Firearms.FirearmAnimatorHashes.DrawSpeedMultiplier, global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.AttachmentsValue(this, global::InventorySystem.Items.Firearms.Attachments.AttachmentParam.DrawSpeedMultiplier));
            this.AnimSetFloat(global::InventorySystem.Items.Firearms.FirearmAnimatorHashes.ReloadSpeedMultiplier, global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.AttachmentsValue(this, global::InventorySystem.Items.Firearms.Attachments.AttachmentParam.ReloadSpeedMultiplier));
            this.AnimSetBool(global::InventorySystem.Items.Firearms.FirearmAnimatorHashes.IsCocked, (ActionModule as global::InventorySystem.Items.Firearms.Modules.DoubleAction).Cocked);
            this.AnimSetBool(global::InventorySystem.Items.Firearms.FirearmAnimatorHashes.IsUnloaded, !Status.Flags.HasFlagFast(global::InventorySystem.Items.Firearms.FirearmStatusFlags.MagazineInserted));
            this.AnimSetFloat(global::InventorySystem.Items.Firearms.FirearmAnimatorHashes.Slots[global::InventorySystem.Items.Firearms.Attachments.AttachmentSlot.Ammunition], (int)AmmoManagerModule.MaxAmmo);
            this.AnimSetInt(global::InventorySystem.Items.Firearms.FirearmAnimatorHashes.Slots[global::InventorySystem.Items.Firearms.Attachments.AttachmentSlot.Stock], Attachments[StockAttachmentIndex].IsEnabled ? 1 : 0);
        }
    }
}
