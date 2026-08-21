using System.Runtime.CompilerServices;

using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using UnityEngine;

namespace InventorySystem.Items.Firearms
{
    public class ParticleDisruptor : Firearm, IEquipDequipModifier
    {
        private bool _tryRemoveNextFrame;

        [global::UnityEngine.Header("Balance Settings")]
        [SerializeField]
        private FirearmBaseStats _stats;

        [SerializeField]
        private ExplosionGrenade _explosionSettings;

        public GameObject ExplosionPrefab;

        public override global::InventorySystem.Items.Firearms.Modules.IAmmoManagerModule AmmoManagerModule { get; set; }

        public override global::InventorySystem.Items.Firearms.Modules.IEquipperModule EquipperModule { get; set; }

        public override global::InventorySystem.Items.Firearms.Modules.IActionModule ActionModule { get; set; }

        public override global::InventorySystem.Items.Firearms.Modules.IInspectorModule InspectorModule { get; set; }

        public override global::InventorySystem.Items.Firearms.Modules.IAdsModule AdsModule { get; set; }

        public override global::InventorySystem.Items.Firearms.Modules.IHitregModule HitregModule { get; set; }

        public override global::InventorySystem.Items.Firearms.FirearmBaseStats BaseStats => _stats;

        public override ItemType AmmoType => ItemType.None;

        public override bool AllowHolster => ActionModule != null && ActionModule.Standby;

        public bool AllowEquip => true;

        public override void OnAdded(global::InventorySystem.Items.Pickups.ItemPickupBase pickup)
        {
            base.OnAdded(pickup);
            EquipperModule = new global::InventorySystem.Items.Firearms.Modules.EventBasedEquipper(this);
            ActionModule = new global::InventorySystem.Items.Firearms.Modules.DisruptorAction(this, 5.8f, 1f, isAmmoManager: false);
            AmmoManagerModule = new global::InventorySystem.Items.Firearms.Modules.DisruptorAction(this, 0f, 0f, isAmmoManager: true);
            InspectorModule = new global::InventorySystem.Items.Firearms.Modules.SimpleInspector(this, 0);
            AdsModule = new global::InventorySystem.Items.Firearms.Modules.DisruptorAds(this, base.ItemSerial, 0.3f, 1, 2, 3);
            HitregModule = new global::InventorySystem.Items.Firearms.Modules.DisruptorHitreg(this, base.Owner, _explosionSettings);

            _tryRemoveNextFrame = true;
        }
        public override void OnEquipped()
        {
            base.OnEquipped();
            TryRemove();
        }

        public override void OnHolstered()
        {
            base.OnHolstered();
            TryRemove();
        }


        public override void OnRemoved(global::InventorySystem.Items.Pickups.ItemPickupBase pickup)
        {
            base.OnRemoved(pickup);
            if (global::Mirror.NetworkServer.active && Status.Ammo <= 0 && !(pickup == null))
            {
                pickup.DestroySelf();
            }
        }

        public override void UpdateAnims()
        {
            if (ActionModule == null) return;

            global::InventorySystem.Items.Firearms.FirearmStatusFlags flags = ActionModule.PredictedStatus.Flags;
            this.AnimSetInt(global::InventorySystem.Items.Firearms.FirearmAnimatorHashes.Ammo, ActionModule.PredictedStatus.Ammo);
            this.AnimSetBool(global::InventorySystem.Items.Firearms.FirearmAnimatorHashes.IsCocked, flags.HasFlagFast(global::InventorySystem.Items.Firearms.FirearmStatusFlags.Cocked));
            this.AnimSetBool(global::InventorySystem.Items.Firearms.FirearmAnimatorHashes.IsChambered, flags.HasFlagFast(global::InventorySystem.Items.Firearms.FirearmStatusFlags.Chambered));
            this.AnimSetBool(global::InventorySystem.Items.Firearms.FirearmAnimatorHashes.IsMagInserted, flags.HasFlagFast(global::InventorySystem.Items.Firearms.FirearmStatusFlags.MagazineInserted));
            this.AnimSetFloat(global::InventorySystem.Items.Firearms.FirearmAnimatorHashes.ReloadSpeedMultiplier, global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.AttachmentsValue(this, global::InventorySystem.Items.Firearms.Attachments.AttachmentParam.ReloadSpeedMultiplier));
        }

        private void Update()
        {
            if (_tryRemoveNextFrame)
            {
                TryRemove();
                _tryRemoveNextFrame = false;
            }
        }

        private void TryRemove()
        {
            if (global::Mirror.NetworkServer.active && Status.Ammo <= 0 && AcquisitionAlreadyReceived)
            {
                base.OwnerInventory.ServerRemoveItem(base.ItemSerial, null);
            }
        }
    }
}
