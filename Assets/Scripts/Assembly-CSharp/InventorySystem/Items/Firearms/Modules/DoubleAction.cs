using AudioPooling;
using CameraShaking;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using Mirror;
using System.Diagnostics;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Modules
{
    public class DoubleAction : IActionModule, IFirearmModuleBase
    {
        private enum TriggerState
        {
            Released = 0,
            Pulling = 1,
            SearLock = 2
        }

        private readonly Stopwatch _triggerStopwatch;
        private readonly Firearm _firearm;
        private readonly KeyCode _cockKey;

        private readonly float _triggerPullTime;
        private readonly int _cockingTriggerHash;
        private readonly float _cooldownAfterShot;
        private readonly float _cockingTime;

        private readonly byte _dryfireClip;
        private readonly byte _mechaClip;
        private readonly byte _cockClip;

        private bool _isCocked;
        private TriggerState _triggerState;
        private float _nextAllowedShot;

        private static readonly RecoilSettings Recoil = new RecoilSettings(0.1f, 10f, 1.02f, 3f, -0.21f);

        public float HammerPosition
        {
            get
            {
                if (_triggerState != TriggerState.Pulling)
                    return 0f;

                float elapsed = (float)_triggerStopwatch.Elapsed.TotalSeconds;
                return Mathf.Clamp01(elapsed / (_triggerPullTime / FireRateMultiplier));
            }
        }

        private float FireRateMultiplier => AttachmentsUtils.AttachmentsValue(_firearm, AttachmentParam.FireRateMultiplier);

        private bool ServerTriggerReady
        {
            get
            {
                if (_nextAllowedShot <= Time.timeSinceLevelLoad && _firearm.EquipperModule.Standby)
                    return _firearm.AmmoManagerModule.Standby;
                return false;
            }
        }

        private bool ReadyToFire
        {
            get
            {
                if (ServerTriggerReady)
                    return _firearm.AdsModule.Standby;
                return false;
            }
        }

        public FirearmStatus PredictedStatus => _firearm.Status;

        public bool Standby => _nextAllowedShot <= Time.timeSinceLevelLoad && !IsTriggerHeld;

        public float CyclicRate => FireRateMultiplier / (_cooldownAfterShot + _triggerPullTime);

        public bool IsTriggerHeld => _triggerState != TriggerState.Released;

        public bool Cocked
        {
            get => _isCocked;
            set
            {
                _isCocked = value;

                if (NetworkServer.active && _firearm.Status.Flags.HasFlagFast(FirearmStatusFlags.Cocked) != value)
                {
                    FirearmStatusFlags flags = _firearm.Status.Flags;
                    flags = value ? (flags | FirearmStatusFlags.Cocked) : (flags & ~FirearmStatusFlags.Cocked);
                    _firearm.Status = new FirearmStatus(_firearm.Status.Ammo, flags, _firearm.Status.Attachments);
                }
            }
        }

        public DoubleAction(Firearm selfRef, float triggerPullTime, float cooldownAfterShot, float cockingTime,
            string cockingTriggerName, byte dryfireClipIndex, byte mechaClipIndex, byte cockingClipIndex)
        {
            _firearm = selfRef;
            _triggerPullTime = triggerPullTime;
            _cooldownAfterShot = cooldownAfterShot;
            _cockingTime = cockingTime;
            _cockingTriggerHash = Animator.StringToHash(cockingTriggerName);
            _dryfireClip = dryfireClipIndex;
            _mechaClip = mechaClipIndex;
            _cockClip = cockingClipIndex;
            _cockKey = NewInput.GetKey(ActionName.RevolverCockHammer, KeyCode.None);
            _triggerStopwatch = new Stopwatch();

            if (NetworkServer.active)
                NetworkServer.ReplaceHandler<CockMessage>(ServerMsgReceived, true);

            _firearm.OnShotCalled += ResetCocked;
            _firearm.OnDryfired += ResetCocked;
            _firearm.OnEquippedCalled += OnEquipped;
            _firearm.OnStatusChanged += SpectatorSyncCocked;
        }

        private void SpectatorSyncCocked(FirearmStatus prev, FirearmStatus cur)
        {
            // Spectator-simulated copies have no local input driving _isCocked — mirror the
            // flag from the received status and play the cocking animation on the viewmodel.
            AnimatedFirearmViewmodel vm = _firearm.ClientViewmodel;
            if (vm == null || !vm.IsSpectator)
                return;

            bool cocked = cur.Flags.HasFlagFast(FirearmStatusFlags.Cocked);
            if (cocked == _isCocked)
                return;

            _isCocked = cocked;
            if (cocked)
                vm.AnimatorSetTrigger(_cockingTriggerHash);
        }

        private void ResetCocked() => Cocked = false;

        private void OnEquipped()
        {
            Cocked = _firearm.Status.Flags.HasFlagFast(FirearmStatusFlags.Cocked);
            _triggerState = TriggerState.Released;
        }

        private void ServerMsgReceived(NetworkConnection conn, CockMessage cock)
        {
            if (ReferenceHub.TryGetHub(conn.identity.gameObject, out ReferenceHub hub)
                && hub.inventory.CurInstance is Firearm firearm
                && firearm.ActionModule is DoubleAction doubleAction
                && !doubleAction.Cocked)
            {
                firearm.ServerSendAudioMessage(_cockClip);
                doubleAction.Cocked = true;
            }
        }

        private void CockHammer()
        {
            Cocked = true;

            _firearm.ClientViewmodel?.AnimatorSetTrigger(_cockingTriggerHash);

            float reloadSpeedMultiplier = AttachmentsUtils.AttachmentsValue(_firearm, AttachmentParam.ReloadSpeedMultiplier);
            _nextAllowedShot = Time.timeSinceLevelLoad + _cockingTime / reloadSpeedMultiplier;

            NetworkClient.Send(new CockMessage());
        }

        private ActionModuleResponse UpdateHeldTrigger()
        {
            if (_nextAllowedShot > Time.timeSinceLevelLoad)
                return ActionModuleResponse.Idle;

            if (!_isCocked)
            {
                float elapsed = (float)_triggerStopwatch.Elapsed.TotalSeconds;
                if (_triggerPullTime / FireRateMultiplier > elapsed)
                    return ActionModuleResponse.Idle;
            }

            _triggerState = TriggerState.SearLock;
            _nextAllowedShot = Time.timeSinceLevelLoad + _cooldownAfterShot / FireRateMultiplier;

            if (_firearm.Status.Ammo != 0)
            {
                CameraShakeController.AddEffect(new RecoilShake(Recoil, _firearm));
                ClientPlaySound((int)AttachmentsUtils.AttachmentsValue(_firearm, AttachmentParam.ShotClipIdOverride));
                return ActionModuleResponse.Shoot;
            }

            ClientPlaySound(_dryfireClip);
            return ActionModuleResponse.Dry;
        }

        private AudioSource ClientPlaySound(int index)
        {
            if (_firearm.AudioClips == null || index < 0 || index >= _firearm.AudioClips.Length)
                return null;

            FirearmAudioClip clip = _firearm.AudioClips[index];

            return AudioSourcePoolManager.PlaySound(
                clip.Sound,
                Vector3.zero,
                clip.MaxDistance,
                volume: 1f,
                falloffType: FalloffType.Exponential,
                channel: AudioMixerChannelType.Weapons,
                spatial: 0f);
        }

        public ActionModuleResponse DoClientsideAction(bool isTriggerPressed)
        {
            if (!ReadyToFire)
                return ActionModuleResponse.Idle;

            switch (_triggerState)
            {
                case TriggerState.Released:
                    if (isTriggerPressed && ReadyToFire)
                    {
                        _triggerState = TriggerState.Pulling;

                        if (!_isCocked)
                        {
                            _triggerStopwatch.Restart();
                            AudioSource pullSound = ClientPlaySound(_mechaClip);
                            if (pullSound != null)
                                pullSound.pitch = FireRateMultiplier;
                        }

                        return UpdateHeldTrigger();
                    }
                    break;

                case TriggerState.Pulling:
                    return UpdateHeldTrigger();

                case TriggerState.SearLock:
                    if (isTriggerPressed)
                        return ActionModuleResponse.Idle;
                    _triggerState = TriggerState.Released;
                    break;
            }

            if (Input.GetKeyDown(_cockKey)
                && Time.timeSinceLevelLoad >= _nextAllowedShot
                && _triggerState == TriggerState.Released
                && !_isCocked)
            {
                CockHammer();
            }

            return ActionModuleResponse.Idle;
        }

        public bool ServerAuthorizeShot()
        {
            if ((ServerTriggerReady || _firearm.IsLocalPlayer) && _firearm.Status.Ammo > 0)
            {
                _firearm.Status = new FirearmStatus((byte)(_firearm.Status.Ammo - 1), _firearm.Status.Flags, _firearm.Status.Attachments);
                _nextAllowedShot = Time.timeSinceLevelLoad + _cooldownAfterShot;
                _firearm.ServerSendAudioMessage((byte)AttachmentsUtils.AttachmentsValue(_firearm, AttachmentParam.ShotClipIdOverride));
                return true;
            }

            return false;
        }

        public bool ServerAuthorizeDryFire()
        {
            if (ServerTriggerReady && _firearm.Status.Ammo == 0)
            {
                _nextAllowedShot = Time.timeSinceLevelLoad + _cooldownAfterShot;
                _firearm.ServerSendAudioMessage(_dryfireClip);
                return true;
            }

            return false;
        }
    }
}
