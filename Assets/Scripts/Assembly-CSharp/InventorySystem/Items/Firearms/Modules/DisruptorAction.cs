using AudioPooling;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using Mirror;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Modules
{
    public class DisruptorAction : IActionModule, IFirearmModuleBase, IAmmoManagerModule
    {
        private const float StatusUpdateTime = 0.4f;
        private const float PostShotCooldown = 1.5f;
        private const float AdsCooldown = 0.1f;
        private const float DestroyTime = 3.1f;
        private const float ShotAnimTime = 2.2667f;

        private readonly Firearm _firearm;
        private readonly bool _isAmmoManager;

        private FirearmStatus _predictedStatus;
        private float _lastShotTime;
        private bool _allowLoadSound;

        public const int MaxShots = 5;
        public readonly float ShotDelay;

        public float TimeSinceLastShot => CurTime - _lastShotTime;

        private float CurTime => Time.timeSinceLevelLoad;

        public FirearmStatus PredictedStatus
        {
            get
            {
                if (TimeSinceLastShot >= ShotDelay + StatusUpdateTime || !_firearm.IsLocalPlayer)
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

        private bool IsReloading
        {
            get
            {
                if (_firearm.IsLocalPlayer)
                {
                    var vm = _firearm.ClientViewmodel;
                    if (vm == null)
                        return false;
                    return vm.ViewmodelAnimator.GetCurrentAnimatorStateInfo(0).tagHash == FirearmAnimatorHashes.Reload;
                }
                return _firearm.ServerSideAnimator.GetCurrentAnimatorStateInfo(0).tagHash == FirearmAnimatorHashes.Reload;
            }
        }

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

        private float ActualCooldown
        {
            get
            {
                if (!_firearm.IsLocalPlayer)
                {
                    return PostShotCooldown;
                }
                return PostShotCooldown + ShotDelay;
            }
        }

        public bool Standby
        {
            get
            {
                if (!_isAmmoManager)
                {
                    if (!_firearm.IsLocalPlayer || !ShotTriggered)
                    {
                        return TimeSinceLastShot > ActualCooldown;
                    }
                    return false;
                }
                return !IsReloading;
            }
        }

        public float CyclicRate { get; private set; }

        public bool IsTriggerHeld { get; set; }

        public byte MaxAmmo => 5;

        public bool ShotTriggered { get; private set; }

        public bool ClientCanReload => false;

        public bool ClientCanUnload => false;

        public bool AdsReady => TimeSinceLastShot > ShotDelay + AdsCooldown;

        public DisruptorAction(Firearm selfRef, float reloadTime, float chargeupTime, bool isAmmoManager)
        {
            selfRef.OnShotCalled += () => _allowLoadSound = true;
            selfRef.OnHolsteredCalled += () => ShotTriggered = false;

            _firearm = selfRef;
            ShotDelay = chargeupTime;
            CyclicRate = 1f / (reloadTime + chargeupTime);
            _isAmmoManager = isAmmoManager;
            _allowLoadSound = true;
        }

        private void ClientModifyPredictedAmmo(int amount)
        {
            if (!_firearm.IsLocalPlayer)
                return;

            FirearmStatus status = _predictedStatus;
            byte newAmmo = (byte)Mathf.Max(0, status.Ammo + amount);

            _predictedStatus = new FirearmStatus(newAmmo, status.Flags, status.Attachments);
        }

        public ActionModuleResponse DoClientsideAction(bool isTriggerPressed)
        {
            if (PredictedStatus.Ammo == 0)
            {
                // Must outlast the fire/discard animation (ShotAnimTime, ~2.27s) before
                // auto-switching away, or the empty weapon vanishes mid-animation.
                if (TimeSinceLastShot >= ShotDelay + DestroyTime)
                {
                    _firearm.OwnerInventory.CmdSelectItem(0);
                }
                return ActionModuleResponse.Idle;
            }

            if (ShotTriggered)
            {
                if (TimeSinceLastShot < ShotDelay)
                    return ActionModuleResponse.Idle;

                ShotTriggered = false;
                ClientModifyPredictedAmmo(-1);

                var clips = _firearm.AudioClips;
                if (clips != null && clips.Length > 0)
                {
                    var clip = clips[0];
                    if (clip.Sound != null)
                    {
                        AudioSourcePoolManager.PlaySound(clip.Sound, Vector3.zero, 1f, 1f, FalloffType.Exponential, AudioMixerChannelType.Weapons, 0f);
                    }
                }

                return ActionModuleResponse.Shoot;
            }

            if (!isTriggerPressed)
                return ActionModuleResponse.Idle;

            if (!ModulesReady)
                return ActionModuleResponse.Idle;

            if (TimeSinceLastShot < ShotDelay + PostShotCooldown)
                return ActionModuleResponse.Idle;

            NetworkClient.Send(new RequestMessage(_firearm.ItemSerial, RequestType.Reload));

            var vm = _firearm.ClientViewmodel;
            if (vm != null)
            {
                vm.AnimatorSetTrigger(FirearmAnimatorHashes.Fire);
            }

            _lastShotTime = CurTime;
            ShotTriggered = true;

            return ActionModuleResponse.Idle;
        }

        public bool ServerAuthorizeShot()
        {
            if (!_firearm.IsLocalPlayer && TimeSinceLastShot < PostShotCooldown)
            {
                return false;
            }

            if (_firearm.Status.Ammo <= 0)
            {
                _firearm.OwnerInventory.ServerRemoveItem(_firearm.ItemSerial, null);
                return false;
            }

            if (!ModulesReady)
            {
                _firearm.Owner.gameConsoleTransmission.SendToClient(
                    _firearm.OwnerInventory.connectionToClient,
                    $"Shot rejected, ammoManager={_firearm.AmmoManagerModule.Standby}, " +
                    $"equipperModule={_firearm.EquipperModule.Standby}, " +
                    $"adsModule={_firearm.AdsModule.Standby}", "gray");
                return false;
            }


            _firearm.Status = new FirearmStatus(
                (byte)(_firearm.Status.Ammo - 1),
                _firearm.Status.Flags,
                _firearm.Status.Attachments);

            _firearm.ServerSendAudioMessage(0);

            if (!_firearm.IsLocalPlayer)
            {
                _lastShotTime = CurTime;
            }

            _firearm.ServerSideAnimator.Play(FirearmAnimatorHashes.Fire, 0, ShotDelay / ShotAnimTime);

            return true;
        }

        public bool ServerTryReload()
        {
            if (!_allowLoadSound)
                return false;

            _firearm.ServerSendAudioMessage(1);
            _allowLoadSound = false;
            return false;
        }

        public bool ServerAuthorizeDryFire()
        {
            return false;
        }

        public bool ServerTryUnload()
        {
            return false;
        }

        public void ClientReload()
        {
        }

        public void ClientUnload()
        {
        }
    }
}
