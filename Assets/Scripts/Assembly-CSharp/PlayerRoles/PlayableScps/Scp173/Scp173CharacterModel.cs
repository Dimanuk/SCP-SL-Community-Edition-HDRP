using System;
using System.Diagnostics;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerRoles.PlayableScps.Scp173;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp173
{
    public class Scp173CharacterModel : CharacterModel
    {
        public delegate void ModelFrozen(Scp173Role target);

        [SerializeField] private float _lowestPitch;
        [SerializeField] private AudioSource[] _footstepSources;
        [SerializeField] private float _footstepOverallLoundess;
        [SerializeField] private float _footstepSwapSpeed;
        [SerializeField] private float _footstepEnableSpeed;
        [SerializeField] private float _footstepDisableSpeed;
        [SerializeField] private float _groundedSustainTime;
        [SerializeField] private float _footstepGroundedSustainMultiplier;

        private readonly Stopwatch _groundedSw = Stopwatch.StartNew();
        private int _sourcesCount;
        private float _stepSize;
        private bool _isFrozen;
        private float _currentVolume;
        private Quaternion _frozenRot;
        private Scp173Role _role;
        private Scp173MovementModule _fpc;
        private Scp173ObserversTracker _observers;

        public static event ModelFrozen OnFrozen;

        public bool Frozen
        {
            get => _isFrozen;
            set
            {
                if (value == _isFrozen) return;

                _isFrozen = value;
                if (!_isFrozen)
                {
                    transform.localRotation = Quaternion.identity;
                }
                else
                {
                    _frozenRot = transform.rotation;
                    OnFrozen?.Invoke(_role);
                }
            }
        }

        public override void SpawnObject()
        {
            base.SpawnObject();

            _role = OwnerHub?.roleManager?.CurrentRole as Scp173Role;
            if (_role != null)
            {
                _fpc = _role.FpcModule as Scp173MovementModule;
                if (_fpc != null)
                {
                    _fpc.OnGrounded += OnGrounded;
                }

                if (_role.SubroutineModule != null)
                {
                    _role.SubroutineModule.TryGetSubroutine<Scp173ObserversTracker>(out var scf);
                    _observers = scf;
                }
            }

            if (_footstepSources == null)
                return;

            _sourcesCount = _footstepSources.Length;
            _stepSize = _sourcesCount > 0 ? 1f / _sourcesCount : 1f;

            for (int i = 0; i < _sourcesCount; i++)
            {
                AudioSource source = _footstepSources[i];
                if (source == null)
                    continue;

                source.volume = 0f;
                if (source.clip != null)
                {
                    source.PlayDelayed(source.clip.length * _stepSize * i);
                }
            }
        }

        public override void ResetObject()
        {
            base.ResetObject();

            if (_fpc != null)
            {
                _fpc.OnGrounded -= OnGrounded;
            }

            if (_footstepSources != null)
            {
                for (int i = 0; i < _sourcesCount; i++)
                {
                    if (_footstepSources[i] != null)
                        _footstepSources[i].Stop();
                }
            }
        }

        private void OnGrounded()
        {
            _currentVolume = 1f;
        }

        private void UpdateFootsteps(bool isMoving, bool grounded)
        {
            float speed = isMoving ? _footstepEnableSpeed : _footstepDisableSpeed;

            if (grounded)
            {
                _groundedSw.Restart();
            }
            else if (isMoving && _groundedSw.Elapsed.TotalSeconds < _groundedSustainTime)
            {
                speed *= _footstepGroundedSustainMultiplier;
            }

            _currentVolume = Mathf.MoveTowards(
                _currentVolume,
                (isMoving && grounded) ? 1f : 0f,
                Time.deltaTime * speed);

            float pitch = Mathf.Lerp(_lowestPitch, 1f, _currentVolume);
            float phase = Time.timeSinceLevelLoad * _footstepSwapSpeed;
            float volumeBase = _currentVolume * _footstepOverallLoundess;

            if (_footstepSources == null)
                return;

            for (int i = 0; i < _sourcesCount; i++)
            {
                AudioSource source = _footstepSources[i];
                if (source == null)
                    continue;

                source.pitch = pitch;
                source.volume = Mathf.Abs(Mathf.Sin(phase + _stepSize * Mathf.PI * i)) * volumeBase;
            }
        }

        private void LateUpdate()
        {
            if (!ReferenceHub.TryGetLocalHub(out ReferenceHub localHub))
                return;

            if (_role == null || _fpc == null)
                return;

            bool frozen = PlayerRoles.PlayerRolesUtils.IsHuman(localHub)
                && _observers != null
                && _observers.IsObservedBy(localHub);

            Frozen = frozen;

            bool isMoving = !_isFrozen
                && _fpc.Motor != null
                && _fpc.Motor.Velocity != Vector3.zero;

            UpdateFootsteps(isMoving, _fpc.IsGrounded);

            if (_isFrozen)
                transform.rotation = _frozenRot;
        }
    }
}
