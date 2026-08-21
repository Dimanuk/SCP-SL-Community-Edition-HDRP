using AudioPooling;
using CustomPlayerEffects;
using InventorySystem.Drawers;
using InventorySystem.Items.Pickups;
using Mirror;
using System;
using System.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InventorySystem.Items.ThrowableProjectiles
{
    public class ThrowableItem : ItemBase, IEquipDequipModifier, IItemDescription, IItemNametag, IItemAlertDrawer, IItemDrawer
    {
        [Serializable]
        public struct ProjectileSettings
        {
            public float StartVelocity;
            public float UpwardsFactor;
            public float TriggerTime;
            public Vector3 StartTorque;
            public Vector3 RelativePosition;
        }

        public ThrownProjectile Projectile;
        public PhantomProjectile Phantom;

        public ProjectileSettings WeakThrowSettings;
        public ProjectileSettings FullThrowSettings;

        public float ThrowingAnimTime;
        public float CancelAnimTime;

        public AudioClip ThrowClip;
        public AudioClip BeginClip;
        public AudioClip CancelClip;

        public readonly Stopwatch ThrowStopwatch = new Stopwatch();
        public readonly Stopwatch CancelStopwatch = new Stopwatch();

        [SerializeField] private float _weight;
        [SerializeField] private float _pinPullTime;
        [SerializeField] private float _postThrownAnimationTime;
        [SerializeField] private bool _repickupable;

        private float _destroyTime;
        private bool _tryFire;
        private bool _alreadyFired;
        private bool _fireWeak;
        private bool _alreadySent;

        private KeyCode _primaryKey;
        private KeyCode _secondaryKey;

        private Vector3 _releaseSpeed;
        private Scp1853 _scp1853;

        private const float ServerTimeTolerance = 0.8f;
        private const float MaxTraceTime = 0.1f;
        private const float MaxAheadTime = 0.2f;
        private const float HintBlinkRate = 9f;
        private const float HintBlinkStartTime = 1f;
        private const float HintBlinkTotalTime = 0.7f;

        private const ActionName CancelAction = ActionName.Reload;

        private static readonly Stopwatch TriggerDelay = new();

        public override float Weight => _weight;

        public override bool AllowHolster
        {
            get
            {
                if (ThrowStopwatch.IsRunning)
                    return false;

                if (CancelStopwatch.IsRunning)
                    return ReadyToCancel;

                return true;
            }
        }

        public bool AllowEquip => true;

        public string AlertText
        {
            get
            {
                if (!ReadyToThrow || _alreadyFired || !ThrowStopwatch.IsRunning)
                    return string.Empty;

                float elapsed = (float)ThrowStopwatch.Elapsed.TotalSeconds;
                float threshold = CurrentTimeTolerance * ThrowingAnimTime;

                if (elapsed < threshold - HintBlinkTotalTime)
                    return string.Empty;

                // Blink the hint in during its first moment instead of popping fully visible: while
                // within the blink window, gate visibility on a fast sine so it flashes on/off.
                float sinceVisible = elapsed - (threshold - HintBlinkTotalTime);
                if (sinceVisible < HintBlinkStartTime && Mathf.Sin(sinceVisible * HintBlinkRate) < 0f)
                    return string.Empty;

                // The keybind is wrapped so it falls OUTSIDE the white color tags — that way the
                // "[R]" inherits the surrounding HUD text colour (the player's role colour) while the
                // rest of the sentence stays white.
                string keyName = CancelKey.ToString();
                string formattedKey = $"</color>[{keyName}]<color=white>";
                string formatted = TranslationReader.GetFormatted("Facility", 41, "Press {0} to cancel the throw.", formattedKey);
                return $"<color=white>{formatted}";
            }
        }

        public string Description { get; set; }
        public string Name { get; set; }

        private float CurrentTimeTolerance => base.IsLocalPlayer ? 1f : ServerTimeTolerance;

        private bool ReadyToThrow => ThrowStopwatch.Elapsed.TotalSeconds >= CurrentTimeTolerance * ThrowingAnimTime;

        private bool ReadyToCancel => CancelStopwatch.Elapsed.TotalSeconds >= CurrentTimeTolerance * CancelAnimTime;

        private KeyCode CancelKey => NewInput.GetKey(ActionName.Reload);

        public event Action<ThrowableNetworkHandler.RequestType> OnRequestSent;

        public override void OnAdded(ItemPickupBase pickup)
        {
            _primaryKey = NewInput.GetKey(ActionName.Shoot);
            _secondaryKey = NewInput.GetKey(ActionName.Zoom);
            _scp1853 = base.Owner.playerEffectsController.GetEffect<Scp1853>();

            var translationReader = new ItemTranslationReader(ItemTypeId);
            Name = translationReader.Name;
            Description = translationReader.Description;
        }

        public override void EquipUpdate()
        {
            if (NetworkServer.active)
            {
                UpdateServer();
            }

            if (!IsLocalPlayer)
            {
                return;
            }

            if (!AllowHolster)
            {
                if (_tryFire)
                {
                    ClientUpdateTryFire();
                }
                else
                {
                    ClientUpdateAiming();
                }
            }
            else
            {
                ClientUpdateIdle();
            }
        }

        public override void OnRemoved(ItemPickupBase pickup)
        {
            if (!Mirror.NetworkServer.active || pickup == null || _alreadyFired)
                return;

            Vector3 velocity = PlayerRoles.FirstPersonControl.FpcExtensionMethods.GetVelocity(base.Owner);

            if (ThrowStopwatch.Elapsed.TotalSeconds < _pinPullTime)
            {
                if (pickup is ThrownProjectile thrown && thrown.TryGetComponent(out Rigidbody rb))
                    rb.linearVelocity = velocity;
            }
            else
            {
                ServerThrow(0f, 0f, velocity, Vector3.zero);
                pickup.Info.Locked = true;
                pickup.DestroySelf();
            }
        }

        public override void OnHolstered()
        {
            if (!base.IsLocalPlayer)
                return;

            _tryFire = false;
            _alreadySent = false;
            _alreadyFired = false; 
            _fireWeak = false;
            ThrowStopwatch.Reset();
        }

        private void ClientUpdateTryFire()
        {
            if (_alreadyFired)
                return;

            _alreadyFired = true;
            TriggerDelay.Restart();

            float pitch = _scp1853?.ItemSpeedModifier ?? 1f;
            PlaySound(BeginClip, pitch);

            var requestType = _fireWeak
                ? ThrowableNetworkHandler.RequestType.ConfirmThrowWeak
                : ThrowableNetworkHandler.RequestType.ConfirmThrowFullForce;

            OnRequestSent?.Invoke(requestType);

            _releaseSpeed = PlayerRoles.FirstPersonControl.FpcExtensionMethods.GetVelocity(base.Owner);
            _releaseSpeed.y = Mathf.Max(_releaseSpeed.y, 0f);
            _releaseSpeed = ThrowableNetworkHandler.GetLimitedVelocity(_releaseSpeed);

            if (!_alreadySent && Phantom != null)
            {
                ProjectileSettings settings = _fireWeak ? WeakThrowSettings : FullThrowSettings;

                var phantomInstance = Object.Instantiate(Phantom);
                phantomInstance.Init(base.ItemSerial, base.Owner.PlayerCameraReference, settings.RelativePosition, Projectile?.AdditionalGravity ?? 0f);

                // Give the client-side prediction phantom the same launch impulse the server applies,
                // otherwise it just drops straight down until the networked projectile replaces it.
                PropelBody(phantomInstance.Rigidbody, settings.StartTorque, _releaseSpeed, settings.StartVelocity, settings.UpwardsFactor);

                _alreadySent = true;
            }

            var message = new ThrowableNetworkHandler.ThrowableItemRequestMessage(this, requestType, _releaseSpeed);
            Mirror.NetworkClient.Send(message);
        }

        private void ClientUpdateAiming()
        {
            bool key = Input.GetKey(_primaryKey);
            bool key2 = Input.GetKey(_secondaryKey);
            bool flag = key || key2;
            if (ReadyToThrow)
            {
                if (ClientTryCancel())
                {
                    return;
                }

                if (!flag)
                {
                    _tryFire = true;
                    return;
                }
            }

            if (flag)
            {
                _fireWeak = key2 && !key;
            }
        }

        private void ClientUpdateIdle()
        {
            if (!InventorySystem.GUI.InventoryGuiController.ItemsSafeForInteraction)
                return;

            bool primaryDown = UnityEngine.Input.GetKeyDown(_primaryKey);
            bool secondaryDown = UnityEngine.Input.GetKeyDown(_secondaryKey);

            if (!primaryDown && !secondaryDown)
                return;

            ThrowStopwatch.Start();
            CancelStopwatch.Reset();

            float pitch = _scp1853?.ItemSpeedModifier ?? 1f;
            PlaySound(BeginClip, pitch);

            var message = new ThrowableNetworkHandler.ThrowableItemRequestMessage(this, ThrowableNetworkHandler.RequestType.BeginThrow);
            Mirror.NetworkClient.Send(message);

            OnRequestSent?.Invoke(ThrowableNetworkHandler.RequestType.BeginThrow);
        }

        private bool ClientTryCancel()
        {
            if (!NewInput.GetKeyDown(CancelAction) || CancelAnimTime <= 0f || !ReadyToThrow)
                return false;

            CancelStopwatch.Start();
            ThrowStopwatch.Reset();

            float pitch = _scp1853?.ItemSpeedModifier ?? 1f;
            PlaySound(CancelClip, pitch);

            OnRequestSent?.Invoke(ThrowableNetworkHandler.RequestType.CancelThrow);

            var message = new ThrowableNetworkHandler.ThrowableItemRequestMessage(this, ThrowableNetworkHandler.RequestType.CancelThrow);
            Mirror.NetworkClient.Send(message);

            return true;
        }

        private void PlaySound(AudioClip clip, float pitch = 1f)
        {
            if (clip == null) return;
            AudioSource src = AudioPooling.AudioSourcePoolManager.PlaySound(
                clip, Vector3.zero, 0f, 1f, FalloffType.Exponential, AudioMixerChannelType.DefaultSfx, 0f);
            if (src != null)
                src.pitch = pitch;
        }

        private void UpdateServer()
        {
            if (_destroyTime > 0f && Time.timeSinceLevelLoad >= _destroyTime)
            {
                base.OwnerInventory.ServerRemoveItem(base.ItemSerial, null);
            }
        }

        private void PropelBody(Rigidbody rb, Vector3 torque, Vector3 relativeVelocity, float forceAmount, float upwardFactor)
        {
            if (rb == null) return;

            float verticalFactor = 1f - Mathf.Abs(Vector3.Dot(base.Owner.PlayerCameraReference.forward, Vector3.up));

            Vector3 forward = base.Owner.PlayerCameraReference.forward;
            Vector3 upward = base.Owner.PlayerCameraReference.up * upwardFactor;
            Vector3 finalDirection = forward + upward * verticalFactor;

            rb.centerOfMass = Vector3.zero;
            rb.angularVelocity = torque;
            rb.linearVelocity = relativeVelocity + finalDirection * forceAmount;
        }

        private void ServerThrow(float forceAmount, float upwardFactor, Vector3 torque, Vector3 startVel)
        {
            if (_alreadyFired && !base.IsLocalPlayer)
                return;

            _destroyTime = Time.timeSinceLevelLoad + _postThrownAnimationTime;
            _alreadyFired = true;

            ThrownProjectile thrown = Object.Instantiate(Projectile,
                base.Owner.PlayerCameraReference.position,
                base.Owner.PlayerCameraReference.rotation);

            var info = new PickupSyncInfo(ItemTypeId, thrown.transform.position, thrown.transform.rotation, Weight, base.ItemSerial)
            {
                Locked = !_repickupable
            };

            thrown.Info = info;
            thrown.PreviousOwner = new Footprinting.Footprint(base.Owner);

            Mirror.NetworkServer.Spawn(thrown.gameObject);
            thrown.InfoReceived(default, info);

            if (thrown.TryGetComponent(out Rigidbody rb))
                PropelBody(rb, torque, startVel, forceAmount, upwardFactor);

            thrown.ServerActivate();
        }

        public void ServerProcessThrowConfirmation(bool fullForce, Vector3 startPos, Quaternion startRot, Vector3 startVel)
        {
            if (!ReadyToThrow)
                return;

            var cam = base.Owner.PlayerCameraReference;
            Vector3 origPos = cam.position;
            Quaternion origRot = cam.rotation;

            Bounds bounds = PlayerRoles.FirstPersonControl.FpcExtensionMethods.GenerateTracerBounds(base.Owner, MaxTraceTime, ignoreTeleports: false);
            bounds.Encapsulate(cam.position + PlayerRoles.FirstPersonControl.FpcExtensionMethods.GetVelocity(base.Owner) * MaxAheadTime);

            cam.SetPositionAndRotation(bounds.ClosestPoint(startPos), startRot);

            var settings = fullForce ? FullThrowSettings : WeakThrowSettings;
            startVel = ThrowableNetworkHandler.GetLimitedVelocity(startVel);

            ServerThrow(settings.StartVelocity, settings.UpwardsFactor, settings.StartTorque, startVel);

            var audioType = fullForce
                ? ThrowableNetworkHandler.RequestType.ConfirmThrowFullForce
                : ThrowableNetworkHandler.RequestType.ConfirmThrowWeak;

            Utils.Networking.NetworkUtils.SendToAuthenticated(new ThrowableNetworkHandler.ThrowableItemAudioMessage(base.ItemSerial, audioType));

            cam.SetPositionAndRotation(origPos, origRot);
        }

        public void ServerProcessInitiation()
        {
            // On a host the client-side prediction shares this instance and has already started
            // ThrowStopwatch before the looped-back request is processed, so AllowHolster would
            // reject it and spectators would never receive the BeginThrow broadcast (pin-pull
            // anim/sound). The host's own client only sends this on a real pin pull, so trust it.
            // Latent NW bug — upstream only ever runs dedicated. Same family as the Jailbird
            // host state race.
            if (!AllowHolster && !base.IsLocalPlayer)
                return;

            if (CustomPlayerEffects.UsableItemModifierEffectExtensions.TryGetSpeedMultiplier(ItemTypeId, base.Owner, out var multiplier) && multiplier <= 0f)
                return;

            ThrowStopwatch.Start();
            CancelStopwatch.Reset();

            Utils.Networking.NetworkUtils.SendToAuthenticated(new ThrowableNetworkHandler.ThrowableItemAudioMessage(
                base.ItemSerial, ThrowableNetworkHandler.RequestType.BeginThrow));
        }

        public void ServerProcessCancellation()
        {
            // Host: ClientTryCancel already started CancelStopwatch and reset ThrowStopwatch on
            // this shared instance, so both ReadyToThrow and !CancelStopwatch.IsRunning fail and
            // the CancelThrow broadcast is silently dropped for spectators/other players.
            if (base.IsLocalPlayer)
            {
                if (_alreadyFired || CancelAnimTime <= 0f)
                    return;

                Utils.Networking.NetworkUtils.SendToAuthenticated(new ThrowableNetworkHandler.ThrowableItemAudioMessage(
                    base.ItemSerial, ThrowableNetworkHandler.RequestType.CancelThrow));
                return;
            }

            if (!ReadyToThrow || _alreadyFired || CancelStopwatch.IsRunning || CancelAnimTime <= 0f)
                return;

            CancelStopwatch.Start();
            ThrowStopwatch.Reset();

            Utils.Networking.NetworkUtils.SendToAuthenticated(new ThrowableNetworkHandler.ThrowableItemAudioMessage(
                base.ItemSerial, ThrowableNetworkHandler.RequestType.CancelThrow));
        }
    }
}