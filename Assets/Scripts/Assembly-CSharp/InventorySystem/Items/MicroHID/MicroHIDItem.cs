using System;
using System.Collections.Generic;
using System.Diagnostics;
using InventorySystem.Items.Pickups;
using PlayerRoles.FirstPersonControl;
using Scp914;
using UnityEngine;
using Mirror;
using NorthwoodLib.Pools;
using PlayerStatsSystem;
using InventorySystem.Items.Firearms.Modules;
using Utils.Networking;

namespace InventorySystem.Items.MicroHID
{
    public class MicroHIDItem : ItemBase, IEquipDequipModifier, IStaminaModifier, IItemDescription, IItemNametag, IAcquisitionConfirmationTrigger, IUpgradeTrigger
    {
        public AudioClip PowerUpClip;
        public AudioClip PowerDownClip;
        public AudioClip PrimedClip;
        public AudioClip FireClip;
        public AudioClip FireToPrimedClip;
        public AudioClip FireToPowerDownClip;

        public float RemainingEnergy;
        public HidUserInput UserInput;
        public HidState State;

        [SerializeField]
        private AnimationCurve _energyConsumtionCurve;

        public const float SoundMaxDistance = 30f;
        public const float PreFireTime = 1.7f;
        private const float StaminaUsageMultp = 2f;
        private const float MinimalTimeToSwitchState = 0.35f;
        private const float PowerupTime = 5.95f;
        private const float PowerdownTime = 3.1f;
        private const float FireEnergyConsumption = 0.13f;
        private const float EnemyDotProductThreshold = 0.75f;
        private const float FriendlyDotProductThreshold = 0.98f;
        private const float DamagePerSecond = 1150f;
        private const float DamageOmnidirectionalDistance = 0.7f;
        private const float DamageMaxDistance = 6.3f;
        private const float HitsPerSecond = 8f;

        private ItemTranslationReader _itr;
        private static LayerMask _mask;
        private float _damageTimer;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private bool _itrReady;
        private KeyCode _primaryKey;
        private KeyCode _secondaryKey;

        public override float Weight => 21.5f;
        public bool AcquisitionAlreadyReceived { get; set; }
        public bool AllowEquip => true;
        public override bool AllowHolster => State == HidState.Idle;
        public bool StaminaModifierActive => IsEquipped;
        public float StaminaUsageMultiplier => StaminaUsageMultp;
        public float StaminaRegenMultiplier => 1f;
        public bool SprintingDisabled => false;

        public float Readiness
        {
            get
            {
                switch (State)
                {
                    case HidState.Primed:
                    case HidState.Firing:
                        return 1f;
                    case HidState.PoweringUp:
                        return Mathf.Clamp01((float)_stopwatch.Elapsed.TotalSeconds / 5.95f);
                    default:
                        return 0f;
                }
            }
        }

        public string Description => TranslationReader?.Description;
        public string Name => TranslationReader?.Name;

        private ItemTranslationReader TranslationReader
        {
            get
            {
                if (!_itrReady)
                {
                    _itr = new ItemTranslationReader(ItemType.MicroHID);
                    _itrReady = true;
                }
                return _itr;
            }
        }

        public static LayerMask WallMask
        {
            get
            {
                if (_mask == 0)
                {
                    _mask = LayerMask.GetMask("Default", "Glass", "CCTV", "Door", "Locker");
                }
                return _mask;
            }
        }

        private byte EnergyToByte => (byte)Mathf.RoundToInt(Mathf.Clamp01(RemainingEnergy) * 255f);

        public static event Action<MicroHIDItem> OnStopCharging;

        public void Recharge()
        {
            RemainingEnergy = 1f;
            ServerSendStatus(HidStatusMessageType.EnergySync, EnergyToByte);
        }

        public void ServerConfirmAcqusition()
        {
            OwnerInventory.connectionToClient.Send(new HidStatusMessage
            {
                MessageType = HidStatusMessageType.EnergySync,
                Serial = ItemSerial,
                MessageCode = EnergyToByte
            });
        }

        public override void OnAdded(ItemPickupBase pickup)
        {
            if (IsLocalPlayer)
            {
                _primaryKey = NewInput.GetKey(ActionName.Shoot);
                _secondaryKey = NewInput.GetKey(ActionName.Zoom);
            }
            if (NetworkServer.active && pickup is MicroHIDPickup microHIDPickup)
            {
                RemainingEnergy = microHIDPickup.Energy;
            }
        }

        public void ServerOnUpgraded(Scp914KnobSetting setting)
        {
            RemainingEnergy = 1f;
            ServerSendStatus(HidStatusMessageType.EnergySync, EnergyToByte);
        }

        public override void OnRemoved(ItemPickupBase pickup)
        {
            if (NetworkServer.active)
            {
                if (pickup is MicroHIDPickup microHIDPickup)
                {
                    microHIDPickup.Energy = RemainingEnergy;
                }
                ServerSendStatus(HidStatusMessageType.State, 5);
            }
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

            if (pickup is MicroHIDPickup microHIDPickup)
                microHIDPickup.Energy = RemainingEnergy;

            NetworkServer.Spawn(pickup.gameObject);
            OwnerInventory.ServerRemoveItem(psi.Serial, pickup);
            pickup.PreviousOwner = new Footprinting.Footprint(Owner);
            return pickup;
        }

        public override void OnEquipped()
        {
            UserInput = HidUserInput.None;
            if (NetworkServer.active)
            {
                ServerSendStatus(HidStatusMessageType.EnergySync, EnergyToByte);
                ServerSendStatus(HidStatusMessageType.State, 0);
            }
        }

        public override void EquipUpdate()
        {
            if (IsLocalPlayer)
            {
                ExecuteClientside();
            }
            if (NetworkServer.active)
            {
                ExecuteServerside();
            }
        }

        private void ExecuteClientside()
        {
            HidUserInput lastInput = UserInput;

            if (InventorySystem.GUI.InventoryGuiController.ItemsSafeForInteraction)
            {
                if (Input.GetKey(_primaryKey))
                    UserInput = HidUserInput.Fire;
                else
                    UserInput = Input.GetKey(_secondaryKey) ? HidUserInput.Prime : HidUserInput.None;
            }
            else
            {
                UserInput = HidUserInput.None;
            }

            if (UserInput != lastInput)
            {
                OwnerInventory.connectionToServer.Send(new HidStatusMessage
                {
                    MessageType = HidStatusMessageType.State,
                    Serial = ItemSerial,
                    MessageCode = (byte)UserInput
                });
            }
        }

        private void ExecuteServerside()
        {
            HidState prevState = State;
            byte prevEnergy = EnergyToByte;
            float consumptionRate = 0f;

            switch (State)
            {
                case HidState.Idle:
                case HidState.StopSound:
                    if (RemainingEnergy > 0f && UserInput != HidUserInput.None)
                    {
                        State = HidState.PoweringUp;
                        _stopwatch.Restart();
                    }
                    break;

                case HidState.PoweringUp:
                    if ((UserInput == HidUserInput.None && _stopwatch.Elapsed.TotalSeconds >= 0.35f) || RemainingEnergy <= 0f)
                    {
                        OnStopCharging?.Invoke(this);
                        State = HidState.PoweringDown;
                        _stopwatch.Restart();
                    }
                    else if (Readiness == 1f)
                    {
                        State = (UserInput == HidUserInput.Fire) ? HidState.Firing : HidState.Primed;
                        _stopwatch.Restart();
                    }
                    consumptionRate = _energyConsumtionCurve.Evaluate((float)(_stopwatch.Elapsed.TotalSeconds / 5.95f));
                    break;

                case HidState.PoweringDown:
                    if (_stopwatch.Elapsed.TotalSeconds >= 3.1f)
                    {
                        State = HidState.Idle;
                        _stopwatch.Stop();
                        _stopwatch.Reset();
                    }
                    break;

                case HidState.Primed:
                    if ((UserInput != HidUserInput.Prime && _stopwatch.Elapsed.TotalSeconds >= 0.35f) || RemainingEnergy <= 0f)
                    {
                        if (UserInput == HidUserInput.Fire && RemainingEnergy > 0f)
                        {
                            State = HidState.Firing;
                        }
                        else
                        {
                            OnStopCharging?.Invoke(this);
                            State = HidState.PoweringDown;
                        }
                        _stopwatch.Restart();
                    }
                    else
                    {
                        consumptionRate = _energyConsumtionCurve.Evaluate(1f);
                    }
                    break;

                case HidState.Firing:
                    if (_stopwatch.Elapsed.TotalSeconds > 1.7f)
                    {
                        consumptionRate = 0.13f;
                        Fire();
                        if (RemainingEnergy == 0f || (UserInput != HidUserInput.Fire && _stopwatch.Elapsed.TotalSeconds >= 2.05f))
                        {
                            if (RemainingEnergy > 0f && UserInput == HidUserInput.Prime)
                            {
                                State = HidState.Primed;
                            }
                            else
                            {
                                OnStopCharging?.Invoke(this);
                                State = HidState.PoweringDown;
                            }
                            _stopwatch.Restart();
                        }
                    }
                    else
                    {
                        consumptionRate = _energyConsumtionCurve.Evaluate(1f);
                    }
                    break;
            }

            if (prevState != State)
            {
                ServerSendStatus(HidStatusMessageType.State, (byte)State);
            }

            if (consumptionRate > 0f)
            {
                RemainingEnergy = Mathf.Clamp01(RemainingEnergy - consumptionRate * Time.deltaTime);
                if (prevEnergy != EnergyToByte)
                {
                    ServerSendStatus(HidStatusMessageType.EnergySync, EnergyToByte);
                }
            }
        }

        private void ServerSendStatus(HidStatusMessageType msgType, byte code)
        {
            NetworkUtils.SendToAuthenticated(new HidStatusMessage
            {
                MessageType = msgType,
                Serial = ItemSerial,
                MessageCode = code
            });
        }

        private void Fire()
        {
            if (_damageTimer > 0f)
            {
                _damageTimer -= Time.deltaTime;
                return;
            }
            _damageTimer += 0.125f;

            HashSet<uint> hitIds = HashSetPool<uint>.Shared.Rent();
            bool hasHitAnything = false;
            float damageValue = 143.75f;

            foreach (ReferenceHub hub in ReferenceHub.AllHubs)
            {
                if (hub == Owner || !HitboxIdentity.CheckFriendlyFire(hub, Owner))
                    continue;

                Vector3 direction = hub.transform.position - Owner.transform.position;
                float distance = Vector3.Distance(hub.transform.position, Owner.transform.position);

                if (distance > 6.3f)
                    continue;

                bool isFriendly = HitboxIdentity.CheckFriendlyFire(hub, Owner, ignoreConfig: true);
                float dotThreshold = isFriendly ? 0.75f : 0.98f;

                bool inOmniRange = distance < 0.7f && isFriendly;
                bool inFront = Vector3.Dot(direction.normalized, Owner.PlayerCameraReference.forward) >= dotThreshold;

                if ((inOmniRange || inFront) && !Physics.Raycast(Owner.transform.position, direction, distance, WallMask) && hitIds.Add(hub.networkIdentity.netId))
                {
                    bool killed = hub.playerStats.DealDamage(new MicroHidDamageHandler(this, damageValue));
                    hasHitAnything = hasHitAnything || killed;
                }
            }

            if (Physics.Raycast(Owner.PlayerCameraReference.position, Owner.PlayerCameraReference.forward, out var hitInfo, 6.3f, StandardHitregBase.HitregMask))
            {
                if (hitInfo.collider.TryGetComponent<IDestructible>(out var destructible) && hitIds.Add(destructible.NetworkId))
                {
                    if (!(destructible is HitboxIdentity hitbox) || hitbox.TargetHub != Owner)
                    {
                        if (destructible.Damage(damageValue, new MicroHidDamageHandler(this, damageValue), hitInfo.point))
                        {
                            hasHitAnything = true;
                        }
                    }
                }
            }

            if (hasHitAnything)
            {
                Hitmarker.SendHitmarker(Owner, 1.5f);
            }

            HashSetPool<uint>.Shared.Return(hitIds);
        }
    }
}