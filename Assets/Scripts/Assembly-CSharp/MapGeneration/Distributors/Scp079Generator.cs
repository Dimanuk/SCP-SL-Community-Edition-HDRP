using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Footprinting;
using Interactables;
using Interactables.Interobjects.DoorUtils;
using Interactables.Verification;
using InventorySystem.Items.Keycards;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079;
using Respawning;
using TMPro;
using UnityEngine;

namespace MapGeneration.Distributors
{
    public class Scp079Generator : SpawnableStructure, IServerInteractable, IInteractable
    {
        [Serializable]
        private class GeneratorGauge
        {
            [SerializeField]
            private Transform _gauge;

            [SerializeField]
            private Vector3 _mask;

            [SerializeField]
            private AnimationCurve _values;

            [SerializeField]
            private float _smoothing;

            public void UpdateValue(float f)
            {
                Quaternion localRotation = _gauge.transform.localRotation;
                Quaternion b = Quaternion.Euler(_mask * _values.Evaluate(f));
                _gauge.transform.localRotation = Quaternion.Lerp(localRotation, b, Time.deltaTime * _smoothing);
            }
        }

        [Serializable]
        private class GeneratorLED
        {
            [SerializeField]
            private Renderer _rend;

            [SerializeField]
            private Material _onMat;

            [SerializeField]
            private Material _offMat;

            private byte _prevValue;

            public void UpdateValue(bool b)
            {
                byte b2 = (byte)(b ? 1u : 2u);
                if (b2 != _prevValue)
                {
                    _rend.sharedMaterial = (b ? _onMat : _offMat);
                    _prevValue = b2;
                }
            }
        }

        [Flags]
        public enum GeneratorFlags : byte
        {
            None = 1,
            Unlocked = 2,
            Open = 4,
            Activating = 8,
            Engaged = 0x10
        }

        public enum GeneratorColliderId : byte
        {
            Door = 0,
            Switch = 1,
            CancelButton = 2
        }

        [SerializeField]
        private Animator _doorAnimator;

        [SerializeField]
        private Animator _leverAnimator;

        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private AudioClip _deniedClip;

        [SerializeField]
        private AudioClip _unlockClip;

        [SerializeField]
        private AudioClip _openClip;

        [SerializeField]
        private AudioClip _closeClip;

        [SerializeField]
        private AudioClip _countdownClip;

        [SerializeField]
        private Renderer _keycardRenderer;

        [SerializeField]
        private Material _lockedMaterial;

        [SerializeField]
        private Material _unlockedMaterial;

        [SerializeField]
        private Material _deniedMaterial;

        [SerializeField]
        private float _deniedCooldownTime;

        [SerializeField]
        private float _doorToggleCooldownTime;

        [SerializeField]
        private float _unlockCooldownTime;

        [SerializeField]
        private KeycardPermissions _requiredPermission;

        [SerializeField]
        private float _leverDelay;

        [SerializeField]
        private float _totalActivationTime;

        [SerializeField]
        private float _totalDeactivationTime;

        [SerializeField]
        private GeneratorGauge _localGauge;

        [SerializeField]
        private GeneratorGauge _totalGauge;

        [SerializeField]
        private GeneratorLED _onLED;

        [SerializeField]
        private GeneratorLED _offLED;

        [SerializeField]
        private GeneratorLED[] _waitLights;

        [SerializeField]
        private TextMeshProUGUI _screen;

        [Multiline]
        [SerializeField]
        private string _screenCountdown;

        [Multiline]
        [SerializeField]
        private string _screenEngaged;

        [Multiline]
        [SerializeField]
        private string _screenOffline;

        [SyncVar]
        private byte _flags;

        [SyncVar]
        private short _syncTime;

        private static readonly int DoorAnimHash = Animator.StringToHash("isOpen");

        private static readonly int LeverAnimHash = Animator.StringToHash("isOn");

        private short _prevTime;

        private byte _prevFlags;

        private float _targetCooldown;

        private float _currentTime;

        private Footprint _lastActivator;

        private readonly Stopwatch _cooldownStopwatch = new Stopwatch();

        private readonly Stopwatch _leverStopwatch = new Stopwatch();

        private const float UnlockTokenReward = 0.5f;

        private const float EngageTokenReward = 1f;

        private float DropdownSpeed => _totalActivationTime / _totalDeactivationTime;

        private bool ActivationReady
        {
            get
            {
                if (Activating)
                {
                    return _leverStopwatch.Elapsed.TotalSeconds > (double)_leverDelay;
                }
                return false;
            }
        }


        public bool IsOpen
        {
            get
            {
                return HasFlag(_flags, GeneratorFlags.Open);
            }
            set
            {
                ServerSetFlag(GeneratorFlags.Open, value);
            }
        }

        public bool IsUnlocked
        {
            get
            {
                return HasFlag(_flags, GeneratorFlags.Unlocked);
            }
            set
            {
                ServerSetFlag(GeneratorFlags.Unlocked, value);
            }
        }

        public bool Engaged
        {
            get
            {
                return HasFlag(_flags, GeneratorFlags.Engaged);
            }
            set
            {
                ServerSetFlag(GeneratorFlags.Engaged, value);
            }
        }

        public RoomIdentifier Room { get; private set; }

        public bool Activating
        {
            get
            {
                return HasFlag(_flags, GeneratorFlags.Activating);
            }
            set
            {
                ServerSetFlag(GeneratorFlags.Activating, value);
            }
        }

        public int RemainingTime => _syncTime;

        public IVerificationRule VerificationRule => StandardDistanceVerification.Default;

        public void ServerInteract(ReferenceHub ply, byte colliderId)
        {
            if ((_cooldownStopwatch.IsRunning && _cooldownStopwatch.Elapsed.TotalSeconds < (double)_targetCooldown) || (colliderId != 0 && !HasFlag(_flags, GeneratorFlags.Open)))
            {
                return;
            }
            _cooldownStopwatch.Stop();
            switch ((GeneratorColliderId)colliderId)
            {
                case GeneratorColliderId.Door:
                    if (HasFlag(_flags, GeneratorFlags.Unlocked))
                    {
                        ServerSetFlag(GeneratorFlags.Open, !HasFlag(_flags, GeneratorFlags.Open));
                        _targetCooldown = _doorToggleCooldownTime;
                    }
                    else if (!ply.serverRoles.BypassMode && (!(ply.inventory.CurInstance != null) || !(ply.inventory.CurInstance is KeycardItem keycardItem) || !keycardItem.Permissions.HasFlagFast(_requiredPermission)))
                    {
                        _targetCooldown = _unlockCooldownTime;
                        RpcDenied();
                    }
                    else
                    {
                        ServerSetFlag(GeneratorFlags.Unlocked, state: true);
                        ServerGrantTicketsConditionally(new Footprint(ply), UnlockTokenReward);
                    }
                    break;
                case GeneratorColliderId.Switch:
                    if ((!ply.IsHuman() && !Activating) || Engaged)
                    {
                        break;
                    }
                    Activating = !Activating;
                    if (Activating)
                    {
                        _leverStopwatch.Restart();
                        _lastActivator = new Footprint(ply);
                    }
                    else
                    {
                        _lastActivator = default(Footprint);
                    }
                    _targetCooldown = _doorToggleCooldownTime;
                    break;
                case GeneratorColliderId.CancelButton:
                    if (Activating && !Engaged)
                    {
                        ServerSetFlag(GeneratorFlags.Activating, state: false);
                        _targetCooldown = _unlockCooldownTime;
                        _lastActivator = default(Footprint);
                    }
                    break;
                default:
                    _targetCooldown = 1f;
                    break;
            }
            _cooldownStopwatch.Restart();
        }

        private void Start()
        {
            Scp079Recontainer.AllGenerators.Add(this);

            if (_screen != null)
            {
                _screen.text = _screenOffline;
            }

            Room = RoomIdUtils.RoomAtPosition(base.transform.position);
        }

        private void OnDestroy()
        {
            Scp079Recontainer.AllGenerators.Remove(this);
        }

        private void Update()
        {
            if (NetworkServer.active)
            {
                ServerUpdate();
            }

            if (_prevFlags == _flags)
                return;

            if (!HasFlag(_prevFlags, GeneratorFlags.Unlocked))
            {
                _keycardRenderer.sharedMaterial = _unlockedMaterial;
                _audioSource.PlayOneShot(_unlockClip);
            }

            bool activating = HasFlag(_flags, GeneratorFlags.Activating);
            if (HasFlag(_prevFlags, GeneratorFlags.Activating) != activating)
            {
                _leverAnimator.SetBool(LeverAnimHash, activating);
                _leverStopwatch.Restart();

                if (!activating && _screen != null)
                {
                    _screen.text = _screenOffline;
                }
            }

            bool engaged = HasFlag(_flags, GeneratorFlags.Engaged);
            if (HasFlag(_prevFlags, GeneratorFlags.Engaged) != engaged)
            {
                _onLED.UpdateValue(engaged);
                _offLED.UpdateValue(!engaged);

                if (engaged && _screen != null)
                {
                    _screen.text = _screenEngaged;
                }
            }

            bool open = HasFlag(_flags, GeneratorFlags.Open);
            if (HasFlag(_prevFlags, GeneratorFlags.Open) != open)
            {
                _doorAnimator.SetBool(DoorAnimHash, open);
                _audioSource.PlayOneShot(open ? _openClip : _closeClip);
            }

            _prevFlags = _flags;
        }

        private void UpdateGauges()
        {
            int engagedCount = 0;
            foreach (Scp079Generator gen in Scp079Recontainer.AllGenerators)
            {
                if (HasFlag(gen._flags, GeneratorFlags.Engaged))
                {
                    engagedCount++;
                }
            }

            if (_totalGauge != null)
            {
                _totalGauge.UpdateValue(engagedCount);
            }

            if (_localGauge != null)
            {
                _localGauge.UpdateValue(1f - (float)_syncTime / _totalActivationTime);
            }
        }

        private void LateUpdate()
        {
            UpdateGauges();

            if (_cooldownStopwatch.IsRunning && _cooldownStopwatch.Elapsed.TotalSeconds >= (double)_targetCooldown)
            {
                _keycardRenderer.sharedMaterial = HasFlag(_flags, GeneratorFlags.Unlocked) ? _unlockedMaterial : _lockedMaterial;
                _cooldownStopwatch.Stop();
            }

            bool countdownActive = ActivationReady && !Engaged;
            foreach (GeneratorLED led in _waitLights)
            {
                led.UpdateValue(countdownActive);
            }

            if (_prevTime != _syncTime)
            {
                if (countdownActive)
                {
                    _audioSource.PlayOneShot(_countdownClip);

                    if (_screen != null)
                        _screen.text = string.Format(_screenCountdown, _syncTime);
                }
                _prevTime = _syncTime;
            }
        }

        [Server]
        private void ServerGrantTicketsConditionally(Footprint ply, float reward)
        {
            if (!NetworkServer.active)
            {
                UnityEngine.Debug.LogWarning("[Server] function 'System.Void MapGeneration.Distributors.Scp079Generator::ServerGrantTicketsConditionally(Footprinting.Footprint,System.Single)' called when server was not active");
            }
            else if (ply.IsSet && ply.Role.GetFaction() == Faction.FoundationStaff)
            {
                RespawnTokensManager.GrantTokens(SpawnableTeamType.NineTailedFox, reward);
            }
        }

        [Server]
        private void ServerUpdate()
        {
            if (!NetworkServer.active)
            {
                UnityEngine.Debug.LogWarning("[Server] function 'System.Void MapGeneration.Distributors.Scp079Generator::ServerUpdate()' called when server was not active");
                return;
            }
            bool flag = _currentTime >= _totalActivationTime;
            if (!flag)
            {
                int num = Mathf.FloorToInt(_totalActivationTime - _currentTime);
                if (num != _syncTime)
                {
                    _syncTime = (short)num;
                }
            }
            if (ActivationReady)
            {
                if (flag && !Engaged)
                {
                    Engaged = true;
                    Activating = false;
                    ServerGrantTicketsConditionally(_lastActivator, EngageTokenReward);
                    return;
                }
                _currentTime += Time.deltaTime;
            }
            else
            {
                if (_currentTime == 0f || flag)
                {
                    return;
                }
                _currentTime -= DropdownSpeed * Time.deltaTime;
            }
            _currentTime = Mathf.Clamp(_currentTime, 0f, _totalActivationTime);
        }

        [ClientRpc]
        private void RpcDenied()
        {
            if (!_cooldownStopwatch.IsRunning || _cooldownStopwatch.Elapsed.TotalSeconds >= (double)_targetCooldown)
            {
                _keycardRenderer.sharedMaterial = _deniedMaterial;
                _audioSource.PlayOneShot(_deniedClip);
                _cooldownStopwatch.Restart();
                _targetCooldown = _deniedCooldownTime;
            }
        }

        private bool HasFlag(byte flags, GeneratorFlags flag)
        {
            return ((uint)flags & (uint)flag) == (uint)flag;
        }

        [Server]
        private void ServerSetFlag(GeneratorFlags flag, bool state)
        {
            if (!NetworkServer.active)
            {
                UnityEngine.Debug.LogWarning("[Server] function 'System.Void MapGeneration.Distributors.Scp079Generator::ServerSetFlag(MapGeneration.Distributors.Scp079Generator/GeneratorFlags,System.Boolean)' called when server was not active");
                return;
            }
            GeneratorFlags flags = (GeneratorFlags)_flags;
            flags = ((!state) ? ((GeneratorFlags)((uint)flags & (uint)(byte)(~(int)flag))) : (flags | flag));
            byte b = (byte)flags;
            if (b != _flags)
            {
                _flags = b;
            }
        }
    }
}
