using AudioPooling;
using CameraShaking;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using Mirror;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Modules
{
    public class PumpAction : IActionModule, IFirearmModuleBase
    {
        private const float ServerToleranceBetweenShots = 0.8f;
        private const float ServerTolerancePumpSpeed = 0.9f;
        private const int PredictionOverrideMilliseconds = 400;

        private static bool _resetEventAssigned;
        private static readonly Dictionary<ushort, int> ChamberedRoundsBySerial = new Dictionary<ushort, int>();
        private static readonly Dictionary<ushort, int> CockedHammersBySerial = new Dictionary<ushort, int>();

        private readonly Firearm _firearm;
        private readonly RecoilSettings _recoil;
        private readonly int _chambersNumber;
        private readonly float _timeBetweenShots;
        private readonly float _pumpingTime;
        private readonly int _pumpAnimHash;
        private readonly ushort _serial;
        private readonly int _triggerClip;
        private readonly int _dryfireClip;

        private readonly Stopwatch _predictedStatusStopwatch;
        private readonly Stopwatch _lastShotStopwatch;
        private readonly Stopwatch _pumpStopwatch;

        private bool _isTriggerReady;
        private FirearmStatus _predictedStatus;

        private float PumpSpeedMultiplier =>
            AttachmentsUtils.AttachmentsValue(_firearm, AttachmentParam.FireRateMultiplier) *
            AttachmentsUtils.AttachmentsValue(_firearm, AttachmentParam.ReloadSpeedMultiplier);

        private float TimeBetweenShots =>
            (_firearm.IsLocalPlayer ? _timeBetweenShots : _timeBetweenShots * ServerToleranceBetweenShots) /
            AttachmentsUtils.AttachmentsValue(_firearm, AttachmentParam.FireRateMultiplier);

        private float PumpingTime =>
            (_firearm.IsLocalPlayer ? _pumpingTime : _pumpingTime * ServerTolerancePumpSpeed) / PumpSpeedMultiplier;

        private bool ModulesReady =>
            _firearm.AmmoManagerModule.Standby &&
            _firearm.EquipperModule.Standby &&
            _firearm.AdsModule.Standby;

        private byte ShotSoundId => (byte)AttachmentsUtils.AttachmentsValue(_firearm, AttachmentParam.ShotClipIdOverride);

        public FirearmStatus PredictedStatus
        {
            get
            {
                if (NetworkServer.active)
                    return _firearm.Status;

                if (_predictedStatusStopwatch.ElapsedMilliseconds > PredictionOverrideMilliseconds)
                    _predictedStatus = _firearm.Status;

                return _predictedStatus;
            }
            private set
            {
                _predictedStatus = value;
                _predictedStatusStopwatch.Restart();
            }
        }

        public int ChamberedRounds
        {
            get => ChamberedRoundsBySerial.TryGetValue(_serial, out var value) ? value : 0;
            set => ChamberedRoundsBySerial[_serial] = value;
        }

        public int CockedHammers
        {
            get => CockedHammersBySerial.TryGetValue(_serial, out var value) ? value : 0;
            set => CockedHammersBySerial[_serial] = value;
        }

        public int LastFiredAmount { get; private set; }

        public bool IsTriggerHeld { get; private set; }

        public float CyclicRate => 1f / (_pumpingTime / _chambersNumber + _timeBetweenShots * (_chambersNumber - 1));

        public bool Standby =>
            _lastShotStopwatch.Elapsed.TotalSeconds >= TimeBetweenShots &&
            _pumpStopwatch.Elapsed.TotalSeconds >= PumpingTime;

        public int AmmoUsage => Mathf.RoundToInt(AttachmentsUtils.AttachmentsValue(_firearm, AttachmentParam.AmmoConsumptionMultiplier));

        public PumpAction(Firearm selfRef, ushort serial, int numberOfChambers, float timeBetweenShots,
            float pumpingTime, RecoilSettings recoil, string pumpTriggerName, int triggerClip, int dryfireClip)
        {
            _firearm = selfRef;
            _serial = serial;
            _chambersNumber = numberOfChambers;
            _timeBetweenShots = timeBetweenShots;
            _pumpingTime = pumpingTime;
            _recoil = recoil;
            _pumpAnimHash = Animator.StringToHash(pumpTriggerName);
            _triggerClip = triggerClip;
            _dryfireClip = dryfireClip;

            _lastShotStopwatch = Stopwatch.StartNew();
            _predictedStatusStopwatch = Stopwatch.StartNew();
            _pumpStopwatch = Stopwatch.StartNew();

            if (!ChamberedRoundsBySerial.ContainsKey(serial))
                ChamberedRounds = 0;

            if (NetworkServer.active)
                selfRef.OnEquippedCalled += ServerResync;

            // Runs for every instance of this item (owner AND spectator/observer copies alike —
            // OnEquipUpdateCalled fires regardless of IsLocalPlayer, unlike DoClientsideAction which
            // Firearm.EquipUpdate only calls for the local player). ChamberedRounds is shared by
            // serial, so once the real shooter empties the chamber every instance observes it and
            // racks its own viewmodel in lockstep — this is what makes the pump-cycling animation
            // visible to spectators, who never get a DoClientsideAction tick of their own.
            selfRef.OnEquipUpdateCalled += TickAutoPump;

            if (!_resetEventAssigned)
            {
                Inventory.OnLocalClientStarted += ChamberedRoundsBySerial.Clear;
                _resetEventAssigned = true;
            }
        }

        private void TickAutoPump()
        {
            if (!ModulesReady || !Standby)
                return;

            if (PredictedStatus.Ammo != 0 && ChamberedRounds == 0)
            {
                _pumpStopwatch.Restart();
                _firearm.AnimSetTrigger(_pumpAnimHash);
            }
        }

        public ActionModuleResponse DoClientsideAction(bool isTriggerPressed)
        {
            IsTriggerHeld = isTriggerPressed;

            if (!isTriggerPressed)
                _isTriggerReady = true;

            if (!ModulesReady || !Standby)
                return ActionModuleResponse.Idle;

            if (!_isTriggerReady || !isTriggerPressed)
                return ActionModuleResponse.Idle;

            _isTriggerReady = false;

            if (_pumpStopwatch.Elapsed.TotalSeconds < PumpingTime)
                return ActionModuleResponse.Idle;

            LastFiredAmount = Mathf.Min(ChamberedRounds, AmmoUsage);

            int preShotChamberedRounds = ChamberedRounds;
            int preShotCockedHammers = CockedHammers;
            bool anyCockedClick = false;

            for (int i = 0; i < AmmoUsage; i++)
            {
                if (ClientTryPerformShot() == ActionModuleResponse.Dry)
                    anyCockedClick = true;
            }

            ActionModuleResponse result;
            if (LastFiredAmount > 0)
            {
                var recoilShake = new RecoilShake(_recoil, _firearm, LastFiredAmount * LastFiredAmount);
                CameraShakeController.AddEffect(recoilShake);
                result = ActionModuleResponse.Shoot;
            }
            else
            {
                int clipId = anyCockedClick ? _dryfireClip : _triggerClip;
                if (_firearm.AudioClips != null && clipId >= 0 && clipId < _firearm.AudioClips.Length)
                {
                    AudioSourcePoolManager.PlaySound(
                        _firearm.AudioClips[clipId].Sound,
                        Vector3.zero,
                        1f,
                        1f,
                        FalloffType.Exponential,
                        anyCockedClick ? AudioMixerChannelType.Weapons : AudioMixerChannelType.DefaultSfx,
                        0f,
                        false);
                }
                result = anyCockedClick ? ActionModuleResponse.Dry : ActionModuleResponse.Idle;
            }

            // On a listen server DoClientsideAction is pure prediction for visuals; the real
            // consumption happens in ServerAuthorizeShot, so put the shared state back.
            if (NetworkServer.active)
            {
                ChamberedRounds = preShotChamberedRounds;
                CockedHammers = preShotCockedHammers;
            }

            return result;
        }

        public bool ServerAuthorizeDryFire()
        {
            if (ChamberedRounds > 0 || CockedHammers <= 0)
            {
                ServerResync();
                return false;
            }

            CockedHammers -= Mathf.Min(CockedHammers, AmmoUsage);
            _firearm.ServerSendAudioMessage((byte)_dryfireClip);
            return true;
        }

        public bool ServerAuthorizeShot()
        {
            if (ChamberedRounds == 0 || _firearm.Status.Ammo == 0)
            {
                ServerResync();
                return false;
            }

            if (_lastShotStopwatch.Elapsed.TotalSeconds < TimeBetweenShots ||
                _pumpStopwatch.Elapsed.TotalSeconds < PumpingTime)
            {
                return false;
            }

            LastFiredAmount = 0;
            int ammoToConsume = AmmoUsage;
            bool result = false;

            while (ammoToConsume > 0 && ChamberedRounds > 0 && _firearm.Status.Ammo > 0)
            {
                ammoToConsume--;
                ChamberedRounds--;
                CockedHammers--;
                LastFiredAmount++;

                if (ChamberedRounds > 0)
                    _lastShotStopwatch.Restart();

                _firearm.Status = new FirearmStatus(
                    (byte)(_firearm.Status.Ammo - 1),
                    _firearm.Status.Flags,
                    _firearm.Status.Attachments);

                _firearm.ServerSendAudioMessage((byte)(ShotSoundId + ChamberedRounds));

                result = true;

                if (ChamberedRounds == 0 && _firearm.Status.Ammo > 0 && !_firearm.IsLocalPlayer)
                {
                    _pumpStopwatch.Restart();
                    _firearm.AnimSetTrigger(_pumpAnimHash);
                    break;
                }
            }

            return result;
        }

        public void Pump(bool sendToClients)
        {
            int currentAmmo = PredictedStatus.Ammo;

            if (ChamberedRounds > 0)
            {
                currentAmmo = _firearm.Status.Ammo - ChamberedRounds;
                PredictedStatus = new FirearmStatus((byte)currentAmmo, PredictedStatus.Flags, PredictedStatus.Attachments);

                if (NetworkServer.active)
                    _firearm.OwnerInventory.ServerAddAmmo(_firearm.AmmoType, ChamberedRounds);
            }

            CockedHammers = _chambersNumber;
            ChamberedRounds = Mathf.Min(currentAmmo, _chambersNumber);


            if (NetworkServer.active)
            {
                _firearm.Status = new FirearmStatus(
                    (byte)currentAmmo,
                    _firearm.Status.Flags | FirearmStatusFlags.Cocked,
                    _firearm.Status.Attachments);

                if (sendToClients)
                    ServerResync();
            }
        }

        public void ClientProcessMessage(byte msgCode)
        {
            if (!NetworkServer.active)
                ChamberedRounds = msgCode;
        }

        private void ServerResync()
        {
            _firearm.OwnerInventory.connectionToClient.Send(
                new ShotgunResyncMessage(_serial, ChamberedRounds, CockedHammers));
        }

        private ActionModuleResponse ClientTryPerformShot()
        {
            if (ChamberedRounds == 0)
            {
                if (CockedHammers <= 0)
                    return ActionModuleResponse.Idle;

                CockedHammers--;
                return ActionModuleResponse.Dry;
            }

            ChamberedRounds--;
            CockedHammers--;

            if (!NetworkServer.active)
            {
                FirearmStatus ps = PredictedStatus;
                PredictedStatus = new FirearmStatus((byte)(ps.Ammo - 1), ps.Flags, ps.Attachments);

                if (ChamberedRounds > 0)
                    _lastShotStopwatch.Restart();
            }

            int clipIndex = ShotSoundId + ChamberedRounds;
            if (_firearm.AudioClips != null && clipIndex >= 0 && clipIndex < _firearm.AudioClips.Length)
            {
                AudioSourcePoolManager.PlaySound(
                    _firearm.AudioClips[clipIndex].Sound,
                    Vector3.zero,
                    1f,
                    1f,
                    FalloffType.Exponential,
                    AudioMixerChannelType.Weapons,
                    0f,
                    false);
            }

            return ActionModuleResponse.Shoot;
        }
    }
}