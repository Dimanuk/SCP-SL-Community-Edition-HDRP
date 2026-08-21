using Footprinting;
using InventorySystem.Crosshairs;
using InventorySystem.GUI;
using InventorySystem.Items.Armor;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Attachments.Components;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.Pickups;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Spectating;
using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Utils.Networking;

namespace InventorySystem.Items.Firearms
{
    public abstract class Firearm : ItemBase,
        IAcquisitionConfirmationTrigger,
        IZoomModifyingItem,
        IMovementSpeedModifier,
        IStaminaModifier,
        ICustomCrosshairItem,
        IItemNametag,
        ILightEmittingItem,
        IDisarmingItem
    {
        [SerializeField]
        private Animator _animator;

        private const float UnloadTime = 0.6f;

        private readonly Stopwatch _unloadStopwatch = new();

        private KeyCode _fireKey;
        private KeyCode _adsKey;
        private KeyCode _inspectKey;
        private KeyCode _reloadKey;
        private KeyCode _toggleFlashlightKey;

        private FirearmStatus _status;

        private Footprint _lastFootprint;
        private bool _footprintValid;

        private bool _refillAmmo;
        private bool _sendStatusNextFrame;
        private bool _prevWasReloading;
        private bool _adsToggled;
        private bool _simulatedInstanceMode;

        protected ItemPickupBase _pendingOnAddedPickup;

        public Faction FirearmAffiliation;
        public float BaseWeight;
        public float BaseLength;
        public FirearmAudioClip[] AudioClips;
        public FirearmGlobalSettingsPreset GlobalSettingsPreset;
        public Attachment[] Attachments;
        public Texture BodyIconTexture;

        public abstract FirearmBaseStats BaseStats { get; }
        public abstract ItemType AmmoType { get; }
        public abstract IAmmoManagerModule AmmoManagerModule { get; set; }
        public abstract IEquipperModule EquipperModule { get; set; }
        public abstract IActionModule ActionModule { get; set; }
        public abstract IInspectorModule InspectorModule { get; set; }
        public abstract IHitregModule HitregModule { get; set; }
        public abstract IAdsModule AdsModule { get; set; }

        public bool AcquisitionAlreadyReceived { get; set; }

        public bool AllowDisarming => true;

        public override ItemDescriptionType DescriptionType => ItemDescriptionType.Firearm;

        public float ArmorPenetration =>
            Mathf.Min(
                BaseStats.BasePenetrationPercent
                    * AttachmentsUtils.AttachmentsValue(this, AttachmentParam.PenetrationMultiplier)
                    / 100f,
                1f);

        public virtual FirearmStatus Status
        {
            get => _status;
            set
            {
                FirearmStatus prev = _status;
                if (prev == value) return;

                _status = value;

                bool attachChanged = prev.Attachments != value.Attachments;
                if (attachChanged)
                    AttachmentsUtils.ApplyAttachmentsCode(this, value.Attachments, reValidate: true);


                OnStatusChanged?.Invoke(prev, value);
                _sendStatusNextFrame = true;
            }
        }

        public Footprint Footprint
        {
            get
            {
                if (!_footprintValid)
                {
                    _footprintValid = true;
                    _lastFootprint = new Footprint(base.Owner);
                }
                return _lastFootprint;
            }
        }

        public bool IsSpectated => SpectatorNetworking.IsLocallySpectated(base.Owner);

        public bool SimulatedInstanceMode
        {
            get => _simulatedInstanceMode;
            set
            {
                bool prev = _simulatedInstanceMode;
                _simulatedInstanceMode = value;
                if (!prev && value)
                    OnSimulationModeEnabled();
            }
        }

        public float StaminaUsageMultiplier =>
            GlobalSettingsPreset.WeightToStaminaUsage.Evaluate(Weight);

        public float MovementSpeedMultiplier =>
            GlobalSettingsPreset.WeightToMovementSpeed.Evaluate(Weight);

        public float StaminaRegenMultiplier => 1f;

        public float MovementSpeedLimit
        {
            get
            {
                float baseLimit = GlobalSettingsPreset.MaxWeaponMovementSpeed;
                if (!base.IsLocalPlayer) return baseLimit;

                float adsLimit = GlobalSettingsPreset.AdsMovementSpeedCurve.Evaluate(Length);
                return Mathf.Lerp(baseLimit, adsLimit, AdsModule.ClientAdsAmount);
            }
        }

        public bool SprintingDisabled
        {
            get
            {
                if (!base.IsLocalPlayer) return false;
                return AdsModule.ClientAdsAmount >= 1f;
            }
        }

        public Animator ServerSideAnimator => _animator;

        public AnimatedFirearmViewmodel ClientViewmodel =>
            ViewModel as AnimatedFirearmViewmodel;

        public float ZoomAmount
        {
            get
            {
                float zoomValue = AttachmentsUtils.AttachmentsValue(this, AttachmentParam.AdsZoomMultiplier);
                return Mathf.Lerp(1f, zoomValue, AdsModule.ClientAdsAmount);
            }
        }

        public string Name { get; private set; }

        public bool HasViewmodel => ClientViewmodel != null;

        public float SensitivityScale
        {
            get
            {
                float target = AttachmentsUtils.AttachmentsValue(this, AttachmentParam.AdsMouseSensitivityMultiplier)
                               * AttachmentsUtils.AttachmentsValue(this, AttachmentParam.AdsZoomMultiplier);
                float reduction = SensitivitySettings.AdsReductionMultiplier;
                float t = AdsModule.ClientAdsAmount;

                if (reduction < 1f)
                    t *= reduction;
                else
                    target *= reduction;

                return 1f / Mathf.Lerp(1f, target, t);
            }
        }

        public override float Weight =>
            BaseWeight + Attachments.Where(x => x.IsEnabled).Sum(x => x.Weight);

        public float Length =>
            BaseLength + Attachments.Where(x => x.IsEnabled).Sum(x => x.Length);

        public bool IsEmittingLight
        {
            get
            {
                if (Status.Flags.HasFlagFast(FirearmStatusFlags.FlashlightEnabled))
                    return true;

                if (!AttachmentsUtils.HasAdvantageFlag(this, AttachmentDescriptiveAdvantages.NightVision))
                    return false;

                return AdsModule.ServerAds;
            }
        }

        public virtual Type CrosshairType { get; protected set; } =
            typeof(SingleBulletFirearmCrosshair);

        public bool MovementModifierActive => base.IsEquipped;
        public bool StaminaModifierActive => base.IsEquipped;

        public event Action OnEquipUpdateCalled;
        public event Action OnEquippedCalled;
        public event Action OnHolsteredCalled;
        public event Action OnShotCalled;
        public event Action OnDryfired;
        public event Action<FirearmStatus, FirearmStatus> OnStatusChanged;

        public override void OnAdded(ItemPickupBase pickup)
        {
            if (NetworkServer.active)
            {
                if (pickup is FirearmPickup firearmPickup)
                {
                    Status = new FirearmStatus(
                        firearmPickup.Status.Ammo,
                        firearmPickup.Status.Flags,
                        AttachmentsUtils.ValidateAttachmentsCode(this, firearmPickup.Status.Attachments));
                    _refillAmmo = firearmPickup.Distributed;
                }
                else
                {
                    Status = new FirearmStatus(
                        0,
                        FirearmStatusFlags.None,
                        AttachmentsUtils.ValidateAttachmentsCode(this, 0u));
                    _refillAmmo = true;
                }
                _footprintValid = false;
            }

            var reader = new ItemTranslationReader(ItemTypeId);
            Name = reader.Name;

            for (int i = 0; i < Attachments.Length; i++)
                Attachments[i].AttachmentId = i;

            if (base.IsLocalPlayer)
            {
                _fireKey = NewInput.GetKey(ActionName.Shoot, KeyCode.Mouse0);
                _adsKey = NewInput.GetKey(ActionName.Zoom, KeyCode.Mouse1);
                _inspectKey = NewInput.GetKey(ActionName.InspectItem, KeyCode.None);
                _reloadKey = NewInput.GetKey(ActionName.Reload, KeyCode.R);
                _toggleFlashlightKey = NewInput.GetKey(ActionName.ToggleFlashlight, KeyCode.T);

                FirearmBasicMessagesHandler.OnStatusMessageReceived += ProcessReceivedStatus;
                if (!NetworkServer.active
                    && FirearmBasicMessagesHandler.ReceivedStatuses != null
                    && FirearmBasicMessagesHandler.ReceivedStatuses.TryGetValue(base.ItemSerial, out FirearmStatus pendingStatus))
                {
                    Status = new FirearmStatus(
                        pendingStatus.Ammo,
                        pendingStatus.Flags,
                        AttachmentsUtils.ValidateAttachmentsCode(this, pendingStatus.Attachments));
                }
            }
        }

        public override void OnRemoved(ItemPickupBase pickup)
        {
            base.OnRemoved(pickup);

            if (base.IsLocalPlayer)
                FirearmBasicMessagesHandler.OnStatusMessageReceived -= ProcessReceivedStatus;

            if (pickup is FirearmPickup firearmPickup)
                firearmPickup.Status = Status;
        }

        public override ItemPickupBase ServerDropItem()
        {
            if (!NetworkServer.active)
                throw new InvalidOperationException("Method ServerDropItem can only be executed on the server.");

            if (PickupDropModel == null)
            {
                UnityEngine.Debug.LogError("No pickup drop model set. Could not drop the item.");
                return null;
            }

            PickupSyncInfo psi = new(ItemTypeId, Owner.transform.position, Quaternion.identity, Weight, ItemSerial);
            ItemPickupBase pickup = OwnerInventory.ServerCreatePickup(this, psi, spawn: false);

            if (pickup is FirearmPickup firearmPickup)
                firearmPickup.Status = Status;

            NetworkServer.Spawn(pickup.gameObject);
            OwnerInventory.ServerRemoveItem(psi.Serial, pickup);
            pickup.PreviousOwner = new Footprint(Owner);
            return pickup;
        }

        public override void EquipUpdate()
        {
            UpdateAnims();

            if (NetworkServer.active)
            {
                if (AmmoManagerModule != null && AmmoManagerModule.Standby && _prevWasReloading)
                    NetworkUtils.SendToAuthenticated(
                        new RequestMessage(base.ItemSerial, RequestType.ReloadStop));

                _prevWasReloading = AmmoManagerModule != null && !AmmoManagerModule.Standby;
            }

            OnEquipUpdateCalled?.Invoke();

            if (!base.IsLocalPlayer) return;

            UpdateKeys();

            if (!InventoryGuiController.ItemsSafeForInteraction) return;

            if (ActionModule == null) return;

            bool triggered = Input.GetKey(_fireKey);

            ActionModuleResponse response = ActionModule.DoClientsideAction(triggered);

            switch (response)
            {
                case ActionModuleResponse.Shoot:
                    {
                        if (HitregModule != null &&
                            HitregModule.ClientCalculateHit(out ShotMessage msg))
                        {
                            msg.ShooterWeaponSerial = base.ItemSerial;
                            NetworkClient.Send(msg);

                            OnWeaponShot();
                        }
                        break;
                    }

                case ActionModuleResponse.Dry:
                    {
                        NetworkClient.Send(
                            new RequestMessage(base.ItemSerial, RequestType.Dryfire));

                        if (!NetworkServer.active)
                            OnWeaponDryfired();
                        break;
                    }
            }
        }

        public override void AlwaysUpdate()
        {
            if (NetworkServer.active && _sendStatusNextFrame)
            {
                NetworkUtils.SendToHubsConditionally(
                    new StatusMessage(base.ItemSerial, Status),
                    x => x != Owner || !x.isLocalPlayer);
                _sendStatusNextFrame = false;
            }
        }

        public override void OnEquipped()
        {

            _animator.enabled = true;
            _animator.Rebind();

            if (AdsModule != null)
                AdsModule.ServerAds = false;

            EquipperModule?.OnEquipped();

            OnEquippedCalled?.Invoke();

            _sendStatusNextFrame = true;
            _footprintValid = false;
        }

        public override void OnHolstered()
        {

            OnHolsteredCalled?.Invoke();
            _animator.enabled = false;
            _adsToggled = false;

            if (NetworkServer.active)
            {
                BodyArmorUtils.RemoveEverythingExceedingLimits(
                    base.OwnerInventory,
                    BodyArmorUtils.TryGetBodyArmor(base.OwnerInventory, out var bodyArmor) ? bodyArmor : null,
                    removeItems: false);
            }
        }

        public virtual void OnWeaponShot() => OnShotCalled?.Invoke();
        public virtual void OnWeaponDryfired() => OnDryfired?.Invoke();

        public abstract void UpdateAnims();

        protected virtual void OnSimulationModeEnabled() { }

        protected void ApplySimulatedStatus()
        {
            if (!_simulatedInstanceMode)
                return;

            if (!NetworkServer.active)
            {
                _pendingOnAddedPickup = null;
                return;
            }

            if (_pendingOnAddedPickup is FirearmPickup firearmPickup)
            {
                Status = new FirearmStatus(
                    firearmPickup.Status.Ammo,
                    firearmPickup.Status.Flags,
                    AttachmentsUtils.ValidateAttachmentsCode(this, firearmPickup.Status.Attachments));

                _refillAmmo = firearmPickup.Distributed;
            }
            else
            {
                Status = new FirearmStatus(
                    0,
                    FirearmStatusFlags.None,
                    AttachmentsUtils.ValidateAttachmentsCode(this, 0u));
            }

            _pendingOnAddedPickup = null;
        }

        private void ProcessReceivedStatus(StatusMessage msg)
        {
            if (msg.Serial != base.ItemSerial) return;

            Status = new FirearmStatus(
                msg.Status.Ammo,
                msg.Status.Flags,
                AttachmentsUtils.ValidateAttachmentsCode(this, msg.Status.Attachments));
        }

        private void UpdateKeys()
        {
            bool ads;
            if (GameMenuToggles.AdsToggleEnabled)
            {
                if (Input.GetKeyDown(_adsKey))
                    _adsToggled = !_adsToggled;
                ads = _adsToggled;
            }
            else
            {
                ads = Input.GetKey(_adsKey);
            }

            AdsModule?.ClientUpdateAds(ads);

            if (!InventoryGuiController.ItemsSafeForInteraction) return;

            if (Input.GetKeyDown(_reloadKey))
                _unloadStopwatch.Restart();

            if (Input.GetKeyDown(_toggleFlashlightKey))
            {
                NetworkClient.Send(new RequestMessage(base.ItemSerial, RequestType.ToggleFlashlight));
            }

            if (Input.GetKeyDown(_inspectKey) && InspectorModule != null && InspectorModule.CanInspect)
            {
                InspectorModule.OnInspect();
                NetworkClient.Send(new RequestMessage(base.ItemSerial, RequestType.Inspect));
                _adsToggled = false;
            }

            if (_unloadStopwatch.IsRunning)
            {
                if (Input.GetKey(_reloadKey))
                {
                    if (AmmoManagerModule != null && AmmoManagerModule.ClientCanUnload
                        && _unloadStopwatch.Elapsed.TotalSeconds > UnloadTime)
                    {
                        NetworkClient.Send(new RequestMessage(base.ItemSerial, RequestType.Unload));
                        _unloadStopwatch.Stop();
                        _adsToggled = false; 
                    }
                }
                else
                {
                    if (AmmoManagerModule != null && AmmoManagerModule.ClientCanReload)
                    {
                        NetworkClient.Send(new RequestMessage(base.ItemSerial, RequestType.Reload));
                        _adsToggled = false;
                    }
                    _unloadStopwatch.Stop();
                }
            }
        }

        public virtual void ServerConfirmAcqusition()
        {
            if (_refillAmmo && AmmoManagerModule != null)
            {
                Status = new FirearmStatus(
                    AmmoManagerModule.MaxAmmo,
                    Status.Flags,
                    Status.Attachments);
            }
            else
            {
                base.OwnerInventory.connectionToClient
                    .Send(new StatusMessage(base.ItemSerial, Status));
            }
        }
    }
}
