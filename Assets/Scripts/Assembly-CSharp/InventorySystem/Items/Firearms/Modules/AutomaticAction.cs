using CameraShaking;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using Mirror;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Modules
{
    public class AutomaticAction : IActionModule, IFirearmModuleBase
    {
        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public struct RefusedShotMessage : NetworkMessage
        {
        }

        private const float ServerFirerateTolerance = 0.03f;
        private const float StatusUpdateTime = 0.4f;

        private readonly Firearm _firearm;
        private readonly Stopwatch _lastUpdateStopwatch;

        private readonly bool _semiAuto;
        private readonly bool _hasBoltLock;
        private readonly float _boltTravelTime;
        private readonly float _defaultTimeBetweenShots;
        private readonly byte _dryfireClip;
        private readonly byte _triggerClip;

        private readonly RecoilSettings _recoilSettings;
        private readonly FirearmRecoilPattern _recoilPattern;
        private readonly bool _usesRecoilPattern;
        private readonly float _gunshotRandomVal;

        private readonly Queue<float> _queuedShots;
        private readonly int _ammoConsumption;

        private FirearmStatus _predictedStatus;
        private bool _hammerReady;
        private double _lastShotTime;
        private float _serverLastSuccessfulShot;

        public FirearmStatus PredictedStatus
        {
            get
            {
                if (_lastShotTime >= StatusUpdateTime || !_firearm.IsLocalPlayer)
                {
                    _predictedStatus = _firearm.Status;
                }
                return _predictedStatus;
            }
            private set
            {
                _predictedStatus = value;
            }
        }

        private float CooldownBetweenShots => _defaultTimeBetweenShots / FireRateMultiplier;

        private float FireRateMultiplier => AttachmentsUtils.AttachmentsValue(_firearm, AttachmentParam.FireRateMultiplier);

        private byte ShotClipId => (byte)AttachmentsUtils.AttachmentsValue(_firearm, AttachmentParam.ShotClipIdOverride);

        private bool ModulesReady
        {
            get
            {
                if (_firearm.AmmoManagerModule.Standby && _firearm.EquipperModule.Standby)
                {
                    return _firearm.AdsModule.Standby;
                }
                return false;
            }
        }

        public bool Standby
        {
            get
            {
                if (_firearm.IsLocalPlayer)
                {
                    if (_lastShotTime > (double)CooldownBetweenShots)
                    {
                        return _queuedShots.Count == 0;
                    }
                    return false;
                }
                return true;
            }
        }

        public float CyclicRate => 1f / _defaultTimeBetweenShots * FireRateMultiplier * (float)_ammoConsumption;

        public bool IsTriggerHeld { get; private set; }

        public float FullautoInaccuracy
        {
            get
            {
                if (_recoilPattern == null)
                    return 0f;

                float estimatedState = _recoilPattern.GetEstimatedState(1f / _defaultTimeBetweenShots * FireRateMultiplier);
                return _recoilPattern.InaccuracyOverShots.Evaluate(estimatedState);
            }
        }

        public AutomaticAction(Firearm selfRef, bool semiAuto, float boltTravelTime, float cooldownBetweenShots,
            byte dryfireClip, byte triggerClip, float gunshotPitchRandomization,
            RecoilSettings recoilSettings, FirearmRecoilPattern recoilPattern,
            bool hasBoltLock, int consumption)
        {
            _firearm = selfRef;
            _semiAuto = semiAuto;
            _boltTravelTime = boltTravelTime;
            _defaultTimeBetweenShots = cooldownBetweenShots;
            _dryfireClip = dryfireClip;
            _triggerClip = triggerClip;
            _recoilSettings = recoilSettings;
            _recoilPattern = recoilPattern;
            _usesRecoilPattern = recoilPattern != null;
            _gunshotRandomVal = gunshotPitchRandomization;
            _hasBoltLock = hasBoltLock;
            _ammoConsumption = consumption;

            _lastShotTime = 0.4;
            _queuedShots = new Queue<float>();
            _lastUpdateStopwatch = Stopwatch.StartNew();

            if (selfRef.IsLocalPlayer)
            {
                NetworkClient.ReplaceHandler<RefusedShotMessage>(ClientShotRefused);
            }
        }

        private void ClientPlaySound(int index, bool isGunshot)
        {
            if (_firearm.AudioClips == null || index < 0 || index >= _firearm.AudioClips.Length)
                return;

            FirearmAudioClip clip = _firearm.AudioClips[index];

            AudioSource source = AudioPooling.AudioSourcePoolManager.PlaySound(
                clip.Sound,
                Vector3.zero,
                clip.MaxDistance,
                volume: 1f,
                falloffType: FalloffType.Exponential,
                channel: AudioPooling.AudioMixerChannelType.Weapons,
                spatial: 0f);

            if (isGunshot && source != null)
            {
                float pitch = 1f + Random.Range(-_gunshotRandomVal, _gunshotRandomVal);
                source.pitch = pitch;
            }
        }

        private void ClientProcessReceivedStatus(FirearmStatus oldStatus, FirearmStatus newStatus)
        {
            if (oldStatus != newStatus)
            {
                _predictedStatus = newStatus;
            }
        }

        private void ClientModifyPredictedAmmo(int amount)
        {
            if (!_firearm.IsLocalPlayer)
                return;

            FirearmStatus status = _predictedStatus;
            byte newAmmo = (byte)Mathf.Max(0, status.Ammo + amount);

            _predictedStatus = new FirearmStatus(newAmmo, status.Flags, status.Attachments);
        }

        private static void ClientShotRefused(RefusedShotMessage msg)
        {
            if (ReferenceHub.TryGetLocalHub(out var hub) &&
                hub.inventory.CurInstance is Firearm firearm &&
                firearm.ActionModule is AutomaticAction action)
            {
                // ��������������� ������������ ����������� ������
                action.ClientModifyPredictedAmmo(action._ammoConsumption);

                // ������ �������������� ��������� ������ ��� ������������ (_semiAuto == false �������� ������ ������� � ���� ���������)
                if (!action._semiAuto && action.IsTriggerHeld)
                {
                    if (!action._hammerReady)
                        action._hammerReady = true;

                    FirearmStatus st = action.PredictedStatus;
                    if (!st.Flags.HasFlagFast(FirearmStatusFlags.Cocked))
                    {
                        action._predictedStatus = new FirearmStatus(st.Ammo, st.Flags | FirearmStatusFlags.Cocked, st.Attachments);
                    }
                }
            }
        }

        public ActionModuleResponse DoClientsideAction(bool isTriggerPressed)
        {
            FirearmStatus predictedStatus = PredictedStatus;

            if (isTriggerPressed != IsTriggerHeld)
            {
                if (isTriggerPressed)
                {
                    _hammerReady = true;
                }
                IsTriggerHeld = isTriggerPressed;
            }

            // ���������� ������� ��������
            float cooldown = CooldownBetweenShots;
            double elapsed = _lastUpdateStopwatch.Elapsed.TotalSeconds;
            _lastUpdateStopwatch.Restart();

            bool wasReady = _lastShotTime >= cooldown;
            _lastShotTime += elapsed;

            // ���������� �������� � �������, ���� ������� ���������
            if (_lastShotTime >= cooldown && _hammerReady && ModulesReady)
            {
                if (_semiAuto && predictedStatus.Ammo < _ammoConsumption)
                {
                    // Semi-auto, no ammo — reset hammer, don't queue
                    _hammerReady = false;
                }
                else if (_semiAuto || IsTriggerHeld)
                {
                    // Full-auto requires trigger held; semi-auto uses _hammerReady (set on press)
                    _queuedShots.Enqueue(Time.timeSinceLevelLoad + _boltTravelTime);

                    if (_semiAuto)
                        _hammerReady = false; // One shot per trigger press for semi-auto

                    if (wasReady)
                        _lastShotTime = 0.0;
                    else
                        _lastShotTime -= cooldown;
                }
            }

            // ��������� ������� ���������
            if (_queuedShots.Count > 0)
            {
                float readyTime = _queuedShots.Peek();
                if (Time.timeSinceLevelLoad >= readyTime)
                {
                    _queuedShots.Dequeue();

                    if (predictedStatus.Ammo < _ammoConsumption)
                    {
                        if (_hasBoltLock && !predictedStatus.Flags.HasFlagFast(FirearmStatusFlags.Chambered))
                        {
                            ClientPlaySound(_triggerClip, false);
                            _hammerReady = false; // One click per trigger press, not continuous
                            return ActionModuleResponse.Idle;
                        }

                        ClientPlaySound(_dryfireClip, false);

                        _hammerReady = false;

                        FirearmStatusFlags newFlags = predictedStatus.Flags & ~FirearmStatusFlags.Cocked;
                        _predictedStatus = new FirearmStatus(predictedStatus.Ammo, newFlags, predictedStatus.Attachments);
                        return ActionModuleResponse.Dry;
                    }


                    ClientPlaySound(ShotClipId, true);
                    ClientModifyPredictedAmmo(-_ammoConsumption);

                    RecoilSettings shotRecoil = _recoilSettings;

                    if (_usesRecoilPattern && _recoilPattern != null)
                    {
                        _recoilPattern.ApplyShot(CooldownBetweenShots);
                        shotRecoil = _recoilPattern.GetRecoil(_recoilSettings);
                    }

                    CameraShakeController.AddEffect(new RecoilShake(shotRecoil, _firearm));

                    predictedStatus = PredictedStatus;
                    if (predictedStatus.Ammo < _ammoConsumption)
                    {
                        _queuedShots.Clear();
                        if (!_hasBoltLock)
                            _hammerReady = false;
                    }

                    return ActionModuleResponse.Shoot;
                }
            }

            return ActionModuleResponse.Idle;
        }

        public bool ServerAuthorizeShot()
        {
            if (_firearm.Status.Ammo < _ammoConsumption)
            {
                return false;
            }

            if (!ServerCheckFirerate())
                return false;

            if (!ModulesReady)
            {

                _firearm.Owner.gameConsoleTransmission.SendToClient(
                    $"Shot rejected, ammoManager={_firearm.AmmoManagerModule.Standby}, " +
                    $"equipperModule={_firearm.EquipperModule.Standby}, " +
                    $"adsModule={_firearm.AdsModule.Standby}", "gray");

                return false;
            }

            FirearmStatusFlags flags = _firearm.Status.Flags;

            if (_firearm.Status.Ammo - _ammoConsumption < _ammoConsumption && _boltTravelTime == 0f)
            {
                flags &= ~FirearmStatusFlags.Chambered;
            }

            byte newAmmo = (byte)(_firearm.Status.Ammo - _ammoConsumption);

            _firearm.Status = new FirearmStatus(newAmmo, flags, _firearm.Status.Attachments);
            _firearm.ServerSendAudioMessage(ShotClipId);

            return true;
        }

        public bool ServerAuthorizeDryFire()
        {
            if ((!ServerCheckFirerate() || _firearm.Status.Ammo != 0 || !ModulesReady) && !_firearm.IsLocalPlayer)
            {
                return false;
            }

            FirearmStatusFlags flags = _firearm.Status.Flags;

            if (!flags.HasFlagFast(FirearmStatusFlags.Cocked))
            {
                return false;
            }

            flags &= ~FirearmStatusFlags.Cocked;


            _firearm.Status = new FirearmStatus(0, flags, _firearm.Status.Attachments);
            _firearm.ServerSendAudioMessage(_dryfireClip);

            return true;
        }

        private bool ServerCheckFirerate()
        {
            float timeSinceLevelLoad = Time.timeSinceLevelLoad;
            float num = timeSinceLevelLoad - _serverLastSuccessfulShot;
            float limit = -ServerFirerateTolerance * CooldownBetweenShots;

            if (num < limit)
            {
                _firearm.OwnerInventory.connectionToClient.Send(new RefusedShotMessage());
                return false;
            }

            _serverLastSuccessfulShot = timeSinceLevelLoad + CooldownBetweenShots - Mathf.Min(num, CooldownBetweenShots);
            return true;
        }
    }
}
