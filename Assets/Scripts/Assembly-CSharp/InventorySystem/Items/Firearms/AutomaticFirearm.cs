using CameraShaking;
using InventorySystem.Drawers;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.Pickups;
using System;
using UnityEngine;

namespace InventorySystem.Items.Firearms
{
    public class AutomaticFirearm : Firearm, IItemProgressbarDrawer, IItemDrawer
    {
        [Header("General Settings")]
        [SerializeField] private ItemType _ammoType;

        [SerializeField] private AttachmentSlot[] _animatorExposedSlots;

        [SerializeField] private byte _dryfireClipId;
        [SerializeField] private byte _triggerClipId;
        [SerializeField] private byte _adsInClip;
        [SerializeField] private byte _adsOutClip;

        [SerializeField] private float _gunshotPitchRandomization;

        [SerializeField] private FirearmBaseStats _stats;

        [SerializeField] private float _fireRate;
        [SerializeField] private float _boltTravelTime;

        public float FireRate => _fireRate;
        [SerializeField] private bool _hasBoltLock;

        [SerializeField] private RecoilSettings _recoil;
        [SerializeField] private FirearmRecoilPattern _recoilPattern;

        [SerializeField] private byte _baseMaxAmmo;
        [SerializeField] private bool _semiAutomatic;
        [SerializeField] private float _standardAdsTime;
        [SerializeField] private int _chamberSize;

        [Header("Debug")]
        [SerializeField] private bool _debugRecoilPattern;

        public override FirearmBaseStats BaseStats => _stats;

        public override IAmmoManagerModule AmmoManagerModule { get; set; }
        public override IEquipperModule EquipperModule { get; set; }
        public override IActionModule ActionModule { get; set; }
        public override IInspectorModule InspectorModule { get; set; }
        public override IAdsModule AdsModule { get; set; }
        public override IHitregModule HitregModule { get; set; }

        public override ItemType AmmoType => _ammoType;
        public FirearmRecoilPattern RecoilPattern => _recoilPattern;
        public bool ProgressbarEnabled => _debugRecoilPattern;

        public float ProgressbarMin => 0f;

        public float ProgressbarMax
        {
            get
            {
                if (AmmoManagerModule != null)
                    return AmmoManagerModule.MaxAmmo;

                return _baseMaxAmmo;
            }
        }

        public float ProgressbarValue
        {
            get
            {
                if (_recoilPattern == null)
                    return 0f;

                float recoilMod = AttachmentsUtils.AttachmentsValue(this, AttachmentParam.OverallRecoilMultiplier);
                float effectiveRate = _fireRate * recoilMod;
                return _recoilPattern.GetEstimatedState(1f / Mathf.Max(effectiveRate, 0.001f));
            }
        }

        public float ProgressbarWidth => 1f;

        public override void OnAdded(ItemPickupBase pickup)
        {
            base.OnAdded(pickup);

            AmmoManagerModule = new AutomaticAmmoManager(this, _baseMaxAmmo, 1, _chamberSize);

            EquipperModule = new EventBasedEquipper(this);

            ActionModule = new AutomaticAction(
                this,
                _semiAutomatic,
                _boltTravelTime,
                1f / _fireRate,
                _dryfireClipId,
                _triggerClipId,
                _gunshotPitchRandomization,
                _recoil,
                _recoilPattern,
                _hasBoltLock,
                Mathf.Max(1, _chamberSize));

            InspectorModule = new SimpleInspector(this, 1);
            AdsModule = new StandardAds(this, base.ItemSerial, _standardAdsTime, 2, _adsInClip, _adsOutClip);
            HitregModule = new SingleBulletHitreg(this, base.Owner, _recoilPattern);
        }

        public override void OnEquipped()
        {
            base.OnEquipped();

            if (IsLocalPlayer)
                UpdateAnims();
        }

        public override void UpdateAnims()
        {
            if (ActionModule == null)
                return;

            FirearmStatusFlags flags = ActionModule.PredictedStatus.Flags;

            this.AnimSetFloat(FirearmAnimatorHashes.DrawSpeedMultiplier,
                AttachmentsUtils.AttachmentsValue(this, AttachmentParam.DrawSpeedMultiplier));

            this.AnimSetFloat(FirearmAnimatorHashes.ReloadSpeedMultiplier,
                AttachmentsUtils.AttachmentsValue(this, AttachmentParam.ReloadSpeedMultiplier));

            this.AnimSetInt(FirearmAnimatorHashes.Ammo, ActionModule.PredictedStatus.Ammo);

            this.AnimSetBool(FirearmAnimatorHashes.IsCocked,
                flags.HasFlagFast(FirearmStatusFlags.Cocked));

            this.AnimSetBool(FirearmAnimatorHashes.IsChambered,
                flags.HasFlagFast(FirearmStatusFlags.Chambered));

            this.AnimSetBool(FirearmAnimatorHashes.IsMagInserted,
                flags.HasFlagFast(FirearmStatusFlags.MagazineInserted));

            int attachmentsCount = Attachments.Length;
            foreach (AttachmentSlot slot in _animatorExposedSlots)
            {
                int count = -1;

                for (int j = 0; j < attachmentsCount; j++)
                {
                    if (Attachments[j].Slot == slot)
                    {
                        count++;
                        if (Attachments[j].IsEnabled)
                            break;
                    }
                }

                if (count >= 0 && FirearmAnimatorHashes.Slots.TryGetValue(slot, out int hash))
                {
                    this.AnimSetInt(hash, count);
                }
            }
        }
    }
}