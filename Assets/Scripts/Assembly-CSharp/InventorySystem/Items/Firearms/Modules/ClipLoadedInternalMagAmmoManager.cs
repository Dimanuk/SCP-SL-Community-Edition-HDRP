using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using System.Diagnostics;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Modules
{
    public class ClipLoadedInternalMagAmmoManager : IAmmoManagerModule, IFirearmModuleBase
    {
        private const float MinimalBusyTime = 0.3f;

        private readonly Firearm _firearm;
        private readonly int _reloadTriggerHash;
        private readonly int _unloadTriggerHash;
        private readonly int _defaultAnimHash;
        private readonly int _idleTagHash;
        private readonly Stopwatch _busyStopwatch;

        private bool _isBusy;
        private byte _defaultMaxAmmo;

        public byte MaxAmmo
        {
            get
            {
                return (byte)((float)(int)_defaultMaxAmmo
                    + AttachmentsUtils.AttachmentsValue(_firearm, AttachmentParam.MagazineCapacityModifier));
            }
            private set
            {
                _defaultMaxAmmo = value;
            }
        }

        public bool Standby => !_isBusy;

        private ushort UserAmmo
        {
            get
            {
                if (!_firearm.OwnerInventory.UserInventory.ReserveAmmo.TryGetValue(_firearm.AmmoType, out var value))
                {
                    return 0;
                }
                return value;
            }
        }

        public bool ClientCanUnload
        {
            get
            {
                if (!_firearm.EquipperModule.Standby || !_firearm.ActionModule.Standby)
                    return false;

                return _firearm.Status.Ammo > 0
                    || _firearm.Status.Flags.HasFlagFast(FirearmStatusFlags.MagazineInserted);
            }
        }

        public bool ClientCanReload
        {
            get
            {
                if (!_firearm.EquipperModule.Standby || !_firearm.ActionModule.Standby)
                    return false;

                return true;
            }
        }

        public ClipLoadedInternalMagAmmoManager(Firearm selfRef, byte maxAmmo)
        {
            _firearm = selfRef;
            MaxAmmo = maxAmmo;

            _reloadTriggerHash = FirearmAnimatorHashes.Reload;
            _unloadTriggerHash = FirearmAnimatorHashes.Unload;
            _idleTagHash = FirearmAnimatorHashes.Idle;

            _busyStopwatch = new Stopwatch();

            _firearm.OnEquipUpdateCalled += EquipUpdate;

            if (Mirror.NetworkServer.active)
            {
                _firearm.OnHolsteredCalled += ServerCancelReload;
                _defaultAnimHash = _firearm.ServerSideAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
            }
        }

        public bool ServerTryReload()
        {
            if (_isBusy || _firearm.Status.Ammo >= MaxAmmo)
            {
                return false;
            }

            if (!_firearm.EquipperModule.Standby || !_firearm.ActionModule.Standby)
            {
                return false;
            }

            if (UserAmmo == 0)
            {
                return false;
            }

            _isBusy = true;
            _busyStopwatch.Restart();
            _firearm.ServerSideAnimator.SetTrigger(_reloadTriggerHash);

            return true;
        }

        public bool ServerTryUnload()
        {
            if (_isBusy || (_firearm.Status.Ammo == 0 &&
                !_firearm.Status.Flags.HasFlagFast(FirearmStatusFlags.MagazineInserted)))
            {
                return false;
            }

            if (!_firearm.EquipperModule.Standby || !_firearm.ActionModule.Standby)
            {
                return false;
            }

            _isBusy = true;
            _busyStopwatch.Restart();
            _firearm.ServerSideAnimator.SetTrigger(_unloadTriggerHash);

            return true;
        }

        public void ClientReload()
        {
            if (_firearm.ClientViewmodel != null)
            {
                _firearm.ClientViewmodel.AnimatorSetTrigger(_reloadTriggerHash);
            }

            _isBusy = true;
            _busyStopwatch.Restart();
        }

        public void ClientUnload()
        {
            if (_firearm.ClientViewmodel != null)
            {
                _firearm.ClientViewmodel.AnimatorSetTrigger(_unloadTriggerHash);
            }

            _isBusy = true;
            _busyStopwatch.Restart();
        }

        private void EquipUpdate()
        {
            if (!_isBusy)
                return;

            if (_busyStopwatch.Elapsed.TotalSeconds < MinimalBusyTime)
                return;

            bool isIdle;
            if (_firearm.IsLocalPlayer)
            {
                if (_firearm.ClientViewmodel == null)
                    return;

                isIdle = _firearm.ClientViewmodel.GetAnimatorStateInfo(0).tagHash == _idleTagHash;
            }
            else
            {
                isIdle = _firearm.ServerSideAnimator.GetCurrentAnimatorStateInfo(0).tagHash == _idleTagHash;
            }

            if (isIdle)
            {
                _isBusy = false;
            }
        }

        private void ServerCancelReload()
        {
            _isBusy = false;
            _busyStopwatch.Stop();

            if (_defaultAnimHash != 0)
            {
                _firearm.ServerSideAnimator.Play(_defaultAnimHash);
            }
        }
    }
}