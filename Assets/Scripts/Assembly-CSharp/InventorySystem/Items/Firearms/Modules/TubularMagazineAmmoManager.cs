using InventorySystem.GUI;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.Modules;
using Mirror;
using System;
using System.Diagnostics;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Modules
{
    public class TubularMagazineAmmoManager : IAmmoManagerModule, IFirearmModuleBase
    {
        private enum CurrentAction
        {
            Idle = 0,
            Reloading = 1,
            Unloading = 2
        }

        private readonly Firearm _firearm;
        private readonly byte _numberOfChambers;
        private readonly Stopwatch _cooldownStopwatch;
        private readonly float _cooldownTime;
        private readonly int _bulletsToLoadAnimHash;
        private readonly int _animLoopLayer;
        private readonly ushort _serial;
        private readonly KeyCode[] _cancelReloadKeys;

        private byte _defaultMaxAmmo;
        private CurrentAction _currentAction;

        private CurrentAction CurAction
        {
            get
            {
                if (!_firearm.IsSpectated)
                    return _currentAction;

                RequestType reloadStateRaw = FirearmClientsideStateDatabase.GetReloadStateRaw(_serial);

                bool isCurrentlyActive = _currentAction != CurrentAction.Idle;
                bool serverWantsAction = reloadStateRaw != RequestType.ReloadStop;

                if (isCurrentlyActive == serverWantsAction)
                    return _currentAction;

                _currentAction = reloadStateRaw switch
                {
                    RequestType.Reload => CurrentAction.Reloading,
                    RequestType.Unload => CurrentAction.Unloading,
                    _ => CurrentAction.Idle
                };

                return _currentAction;
            }
            set
            {
                if (!_firearm.IsSpectated)
                    _currentAction = value;
            }
        }

        private int ChamberedRounds
        {
            get
            {
                if (_firearm.ActionModule is PumpAction pumpAction)
                    return pumpAction.ChamberedRounds;

                return _numberOfChambers;
            }
        }

        private bool ClientIsReloading
        {
            get
            {
                var viewmodel = _firearm.ClientViewmodel;
                if (viewmodel == null)
                    return false;
                return viewmodel.GetAnimatorStateInfo(_animLoopLayer).tagHash == FirearmAnimatorHashes.Reload;
            }
        }

        public byte MaxAmmo
        {
            get
            {
                float attachmentsBonus = AttachmentsUtils.AttachmentsValue(_firearm, AttachmentParam.MagazineCapacityModifier);

                int extra = _firearm.Status.Flags.HasFlagFast(FirearmStatusFlags.Cocked)
                    ? ChamberedRounds
                    : 0;

                return (byte)(_defaultMaxAmmo + attachmentsBonus + extra);
            }
            private set => _defaultMaxAmmo = value;
        }

        public bool Standby
        {
            get
            {
                if (CurAction != CurrentAction.Idle)
                    return false;

                if (_cooldownStopwatch.Elapsed.TotalSeconds <= _cooldownTime)
                    return false;

                if (!_firearm.IsLocalPlayer)
                {
                    return _firearm.ServerSideAnimator.GetCurrentAnimatorStateInfo(_animLoopLayer).tagHash
                           != FirearmAnimatorHashes.Reload;
                }

                return !ClientIsReloading;
            }
        }

        public bool ClientCanUnload => ClientModulesReady;
        public bool ClientCanReload => ClientModulesReady;

        private bool ClientModulesReady
        {
            get
            {
                if (_firearm.ActionModule is not IFirearmModuleBase actionModule || !actionModule.Standby)
                    return false;

                if (_firearm.AmmoManagerModule is not IFirearmModuleBase ammoModule || !ammoModule.Standby)
                    return false;

                return true;
            }
        }

        public TubularMagazineAmmoManager(Firearm selfRef, ushort serial, byte maxAmmo, byte numberOfChambers,
            float cooldownTime, int reloadAnimatorLayer, string bulletsToLoadParamName, params ActionName[] cancelReloadActions)
        {
            _firearm = selfRef;
            _serial = serial;
            MaxAmmo = maxAmmo;
            _numberOfChambers = numberOfChambers;
            _cooldownTime = cooldownTime;
            _animLoopLayer = reloadAnimatorLayer;
            _bulletsToLoadAnimHash = Animator.StringToHash(bulletsToLoadParamName);
            _cooldownStopwatch = Stopwatch.StartNew();

            _cancelReloadKeys = new KeyCode[cancelReloadActions.Length];
            for (int i = 0; i < _cancelReloadKeys.Length; i++)
            {
                _cancelReloadKeys[i] = NewInput.GetKey(cancelReloadActions[i]);
            }

            _firearm.OnHolsteredCalled += Holstered;
            _firearm.OnEquipUpdateCalled += EquipUpdate;
        }

        public bool ServerTryReload() => ServerHandleRequest(CurrentAction.Reloading);
        public bool ServerTryUnload() => ServerHandleRequest(CurrentAction.Unloading);

        private bool ServerHandleRequest(CurrentAction action)
        {
            if (action == CurrentAction.Idle)
                throw new InvalidOperationException("Server can only handle shotgun reload/unload requests!");

            if (action == CurAction)
            {
                return false;
            }

            if (CurAction != CurrentAction.Idle)
            {
                CurAction = CurrentAction.Idle;
                _cooldownStopwatch.Restart();
                return true;
            }

            if (!_firearm.EquipperModule.Standby || !_firearm.ActionModule.Standby)
            {
                return false;
            }

            if (_cooldownStopwatch.Elapsed.TotalSeconds < _cooldownTime)
            {
                return false;
            }

            int currentAmmo = _firearm.Status.Ammo;
            ushort curReserveAmmo = _firearm.OwnerInventory.GetCurAmmo(_firearm.AmmoType);

            if (action == CurrentAction.Reloading && (currentAmmo >= MaxAmmo || curReserveAmmo == 0))
            {
                return false;
            }

            if (action == CurrentAction.Unloading && currentAmmo <= 0)
            {
                return false;
            }

            CurAction = action;
            return true;
        }

        public void ClientReload() => ClientUpdateAction(CurrentAction.Reloading);
        public void ClientUnload() => ClientUpdateAction(CurrentAction.Unloading);

        private void ClientUpdateAction(CurrentAction newAction)
        {
            // On the server (incl. listen host) the state is owned by ServerHandleRequest;
            // the broadcast confirmation must not touch it a second time.
            if (NetworkServer.active)
                return;

            if (CurAction != CurrentAction.Idle)
            {
                if (newAction != CurAction)
                {
                    if (!_firearm.IsSpectated)
                        CurAction = CurrentAction.Idle;
                }
                return;
            }

            if (!_firearm.IsSpectated)
                CurAction = newAction;
        }

        private void EquipUpdate()
        {
            _firearm.AnimSetBool(FirearmAnimatorHashes.Reload, CurAction == CurrentAction.Reloading);
            _firearm.AnimSetBool(FirearmAnimatorHashes.Unload, CurAction == CurrentAction.Unloading);

            if (CurAction == CurrentAction.Idle)
                return;

            if (_firearm.IsLocalPlayer)
                UpdateLocalPlayerOnly();

            switch (CurAction)
            {
                case CurrentAction.Reloading:
                    UpdateReload();
                    break;

                case CurrentAction.Unloading:
                    UpdateUnload();
                    break;
            }
        }

        private void UpdateLocalPlayerOnly()
        {
            if (!InventoryGuiController.ItemsSafeForInteraction)
                return;

            foreach (KeyCode key in _cancelReloadKeys)
            {
                if (UnityEngine.Input.GetKeyDown(key))
                {
                    // Cancelling works by requesting the OPPOSITE action: the server sees a
                    // different action while one is running and resets to Idle (and rebroadcasts,
                    // so every client cancels the same way).
                    var msg = new RequestMessage(
                        _serial,
                        CurAction == CurrentAction.Reloading ? RequestType.Unload : RequestType.Reload);

                    NetworkClient.Send(msg);
                    return;
                }
            }
        }

        private void UpdateReload()
        {
            int availableAmmo = _firearm.IsLocalPlayer
                ? _firearm.OwnerInventory.GetCurAmmo(_firearm.AmmoType)
                : MaxAmmo;

            int toLoad = Mathf.Min(MaxAmmo - _firearm.Status.Ammo, availableAmmo);
            toLoad = Mathf.Min(toLoad, _numberOfChambers);

            _firearm.AnimSetInt(_bulletsToLoadAnimHash, toLoad);

            if (toLoad <= 0)
            {
                CurAction = CurrentAction.Idle;
            }
        }

        private void UpdateUnload()
        {
            if (_firearm.Status.Ammo <= 0)
            {
                CurAction = CurrentAction.Idle;
            }
        }

        private void Holstered()
        {
            if (!_firearm.IsSpectated)
                CurAction = CurrentAction.Idle;

            _firearm.ServerSideAnimator.Rebind();
        }
    }
}