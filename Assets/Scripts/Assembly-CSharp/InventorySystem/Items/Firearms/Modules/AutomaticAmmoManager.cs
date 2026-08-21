using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using System.Diagnostics;
using UnityEngine;
using Mirror;

namespace InventorySystem.Items.Firearms.Modules
{
    public class AutomaticAmmoManager : IAmmoManagerModule, IFirearmModuleBase
    {
        private const float MinimalBusyTime = 0.3f;

        private readonly Firearm _firearm;
        private readonly int _reloadTriggerHash;
        private readonly int _unloadTriggerHash;
        private readonly int _idleTagHash;
        private readonly Stopwatch _busyStopwatch;
        private readonly int _reloadAnimsLayer;
        private readonly int _chamberSize;

        private int _defaultAnimHash;

        private bool _isBusy;
        private byte _defaultMaxAmmo;

        public int ChamberedAmount
        {
            get
            {
                if (!_firearm.Status.Flags.HasFlagFast(FirearmStatusFlags.Chambered))
                    return 0;
                return _chamberSize;
            }
        }

        public byte MaxAmmo
        {
            get
            {
                return (byte)((float)(int)_defaultMaxAmmo
                    + AttachmentsUtils.AttachmentsValue(_firearm, AttachmentParam.MagazineCapacityModifier)
                    + (float)ChamberedAmount);
            }
            private set
            {
                _defaultMaxAmmo = value;
            }
        }

        public bool Standby
        {
            get
            {
                if (_isBusy)
                    return _firearm.IsSpectated;
                return true;
            }
        }

        private ushort UserAmmo
        {
            get
            {
                if (!_firearm.OwnerInventory.UserInventory.ReserveAmmo.TryGetValue(_firearm.AmmoType, out var value))
                    return 0;
                return value;
            }
        }

        public bool ClientCanUnload => true;

        public bool ClientCanReload => true;

        public AutomaticAmmoManager(Firearm selfRef, byte maxAmmo, int reloadAnimsLayer, int chamberSize)
        {
            _firearm = selfRef;
            MaxAmmo = maxAmmo;
            _reloadTriggerHash = FirearmAnimatorHashes.Reload;
            _unloadTriggerHash = FirearmAnimatorHashes.Unload;
            _idleTagHash = FirearmAnimatorHashes.Idle;
            _busyStopwatch = new Stopwatch();
            _reloadAnimsLayer = reloadAnimsLayer;
            _chamberSize = chamberSize;

            _firearm.OnEquipUpdateCalled += EquipUpdate;
            _firearm.OnHolsteredCalled += CancelReload;

            if (NetworkServer.active)
            {
                var anim = selfRef.ServerSideAnimator;
                if (anim != null && anim.isActiveAndEnabled)
                    _defaultAnimHash = anim.GetCurrentAnimatorStateInfo(reloadAnimsLayer).fullPathHash;
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

            if (UserAmmo < Mathf.Max(1, _chamberSize))
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
            if (_isBusy || _firearm.Status.Ammo == 0)
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
                _firearm.ClientViewmodel.AnimatorSetTrigger(_reloadTriggerHash);

            _isBusy = true;
            _busyStopwatch.Restart();
        }

        public void ClientUnload()
        {
            if (_firearm.ClientViewmodel != null)
                _firearm.ClientViewmodel.AnimatorSetTrigger(_unloadTriggerHash);

            _isBusy = true;
            _busyStopwatch.Restart();
        }

        private void EquipUpdate()
        {
            if (!_isBusy)
                return;

            if (_busyStopwatch.Elapsed.TotalSeconds < MinimalBusyTime)
                return;

            bool shouldReset;

            if (_firearm.IsLocalPlayer)
            {
                var viewmodel = _firearm.ClientViewmodel;
                if (viewmodel == null)
                    return;

                var stateInfo = viewmodel.GetAnimatorStateInfo(_reloadAnimsLayer);
                shouldReset = stateInfo.tagHash == _idleTagHash;
            }
            else
            {
                var serverAnimator = _firearm.ServerSideAnimator;
                if (serverAnimator == null || !serverAnimator.isActiveAndEnabled)
                    return;

                var stateInfo = serverAnimator.GetCurrentAnimatorStateInfo(_reloadAnimsLayer);
                shouldReset = stateInfo.tagHash == _idleTagHash;
            }

            if (shouldReset)
            {
                _isBusy = false;
            }
        }

        private void CancelReload()
        {
            _isBusy = false;
            _busyStopwatch.Stop();

            if (NetworkServer.active && _defaultAnimHash != 0)
            {
                var serverAnimator = _firearm.ServerSideAnimator;
                if (serverAnimator != null && serverAnimator.isActiveAndEnabled)
                    serverAnimator.Play(_defaultAnimHash);
            }
        }
    }
}
