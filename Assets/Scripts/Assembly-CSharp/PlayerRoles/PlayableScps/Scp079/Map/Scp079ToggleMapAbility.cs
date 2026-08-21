using System;
using System.Diagnostics;
using GameObjectPools;
using Mirror;
using UnityEngine;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using PlayerRoles.PlayableScps.Scp079.GUI;
using PlayerRoles.PlayableScps.Scp079.Map;
using PlayerRoles.Spectating;
using AudioPooling;

namespace PlayerRoles.PlayableScps.Scp079.Map
{
    public class Scp079ToggleMapAbility : Scp079KeyAbilityBase, IPoolSpawnable, IPoolResettable
    {
        [SerializeField]
        private float _cooldown;

        [SerializeField]
        private AudioClip _soundOpen;

        [SerializeField]
        private AudioClip _soundClose;

        private bool _state;

        private string _openTxt;

        private string _closeTxt;

        private static bool _localInstanceReady;

        private static Scp079ToggleMapAbility _localInstance;

        private static readonly Stopwatch CooldownSw = Stopwatch.StartNew();

        public override ActionName ActivationKey => ActionName.Inventory;

        public override bool IsReady => CooldownSw.Elapsed.TotalSeconds > _cooldown;

        public override bool IsVisible
        {
            get
            {
                if (Scp079CursorManager.LockCameras)
                {
                    if (_state)
                        return !Cursor.visible;
                    return false;
                }
                return true;
            }
        }

        public override string AbilityName => _state ? _closeTxt : _openTxt;

        public override string FailMessage => null;

        public static bool MapState
        {
            get
            {
                if (_localInstanceReady)
                    return _localInstance._state;
                return false;
            }
            internal set
            {
                if (!_localInstanceReady)
                    return;

                _localInstance._state = value;
                CooldownSw.Restart();

                if (!value && _localInstance.Role.IsLocalPlayer)
                    _localInstance.ClientSendCmd();
            }
        }

        public static bool MapVisible
        {
            get
            {
                if (_localInstanceReady)
                {
                    if (!_localInstance._state)
                        return !_localInstance.IsReady;
                    return true;
                }
                return false;
            }
        }

        private void OnSpectatorTargetChanged()
        {
            if (ScpRole.IsSpectated || ScpRole.IsLocalPlayer)
            {
                _localInstance = this;
                _localInstanceReady = true;
            }
            else
            {
                _localInstanceReady = false;
            }
        }

        private void LateUpdate()
        {
            if (Role.IsLocalPlayer && _state)
                ClientSendCmd();
        }

        private void PlaySound()
        {
            AudioSourcePoolManager.PlaySound(
                _state ? _soundOpen : _soundClose,
                null,
                1f,
                1f,
                FalloffType.Exponential,
                AudioMixerChannelType.DefaultSfx,
                0f);
        }

        protected override void Trigger()
        {
            if (CurrentCamSync.CurClientSwitchState == Scp079CurrentCameraSync.ClientSwitchState.None)
            {
                _state = !_state;
                PlaySound();

                if (!_state)
                    ClientSendCmd();
            }
        }

        protected override void Start()
        {
            base.Start();
            _openTxt = Translations.Get(Scp079HudTranslation.OpenMap);
            _closeTxt = Translations.Get(Scp079HudTranslation.CloseMap);
        }

        public override void ClientWriteCmd(NetworkWriter writer)
        {
            base.ClientWriteCmd(writer);

            if (_state)
                NetworkWriterExtensions.WriteVector3(writer, Scp079MapGui.SyncVars);
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);

            if (reader.Remaining > 0)
            {
                _state = true;
                Scp079MapGui.SyncVars = NetworkReaderExtensions.ReadVector3(reader);
                ServerSendRpc(x => x.roleManager.CurrentRole is SpectatorRole);
            }
            else
            {
                _state = false;
                ServerSendRpc(x => x != Owner);
            }
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);

            if (_state)
                NetworkWriterExtensions.WriteVector3(writer, Scp079MapGui.SyncVars);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);

            bool previousState = _state;

            if (reader.Remaining > 0)
            {
                _state = true;
                Scp079MapGui.SyncVars = NetworkReaderExtensions.ReadVector3(reader);
            }
            else
            {
                _state = false;
            }

            if (previousState != _state
                && SpectatorNetworking.IsLocallySpectated(Owner)
                && ScpRole.SubroutineModule.TryGetSubroutine<Scp079CurrentCameraSync>(out var camSync)
                && camSync.CurClientSwitchState == Scp079CurrentCameraSync.ClientSwitchState.None)
            {
                PlaySound();
            }
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            SpectatorTargetTracker.OnTargetChanged += OnSpectatorTargetChanged;

            if (Role.IsLocalPlayer)
            {
                _localInstance = this;
                _localInstanceReady = true;
            }
        }

        public override void ResetObject()
        {
            base.ResetObject();
            SpectatorTargetTracker.OnTargetChanged -= OnSpectatorTargetChanged;

            _state = false;

            if (_localInstanceReady && this == _localInstance)
                _localInstanceReady = false;
        }
    }
}
