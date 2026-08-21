using System;
using System.Collections.Generic;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp173;
using UnityEngine;

namespace FacilitySoundtrack
{
    public class Scp173InsanitySoundtrack : SoundtrackLayerBase
    {
        public delegate void PlayerEscapeScp173(ReferenceHub hub);

        [Serializable]
        private class EncounterTrack
        {
            private float _nextUse;

            public AudioClip Clip;

            public float Cooldown;

            public bool TryUse(float currentTime)
            {
                if (currentTime < _nextUse)
                {
                    return false;
                }
                _nextUse = currentTime + Cooldown;
                return true;
            }

            public void ResetCooldown()
            {
                _nextUse = 0f;
            }
        }

        public const float PanLimit = 0.65f;

        [SerializeField]
        private float _fadeInLerp;

        [SerializeField]
        private float _fadeOutLerp;

        [SerializeField]
        private float _sustainTime;

        [SerializeField]
        private AudioSource[] _distanceAmbients;

        [SerializeField]
        private AudioSource _encounterSource;

        [SerializeField]
        private AudioClip _goneClip;

        [SerializeField]
        private EncounterTrack _closeEncounter;

        [SerializeField]
        private EncounterTrack _farEncounter;

        [SerializeField]
        private float _closeEncounterDistanceThreshold;

        [SerializeField]
        private float _distanceLerp;

        [SerializeField]
        private float _distanceCap;

        private readonly HashSet<Scp173Role> _observed173s = new HashSet<Scp173Role>();

        private AnimationCurve[] _volumeCurves;

        private int _observed173sCount;

        private int _ambientsCount;

        private bool _isActive;

        private bool _prevPlay;

        private float _stopAmbientTime;

        private float _weight;

        private float _lastDistance;

        private bool _cameraSet;

        private Camera _camera;

        private float _lastScreenPosition;

        private float CurTime => Time.timeSinceLevelLoad;

        private bool IsObserved => _observed173sCount > 0;

        public override float Weight => _weight;

        public override bool Additive => false;

        public static event PlayerEscapeScp173 OnPlayerEscapeScp173;

        private void Awake()
        {
            PlayerRoleManager.OnRoleChanged += OnRoleChanged;
            Scp173CharacterModel.OnFrozen += OnFrozen;
            _ambientsCount = _distanceAmbients.Length;
            _volumeCurves = new AnimationCurve[_ambientsCount];
            for (int i = 0; i < _ambientsCount; i++)
            {
                _volumeCurves[i] = _distanceAmbients[i].GetCustomCurve(AudioSourceCurveType.CustomRolloff);
            }
        }

        private void OnDestroy()
        {
            PlayerRoleManager.OnRoleChanged -= OnRoleChanged;
            Scp173CharacterModel.OnFrozen -= OnFrozen;
        }

        private void Update()
        {
            if (!_isActive || !ReferenceHub.TryGetLocalHub(out var lhub))
            {
                return;
            }
            if (_observed173sCount > 0)
            {
                _observed173sCount -= _observed173s.RemoveWhere((Scp173Role x) => !IsObservedBy(x, lhub));
                if (!IsObserved)
                {
                    _stopAmbientTime = CurTime + _sustainTime;
                }
            }
            bool flag = IsObserved || _stopAmbientTime > CurTime;
            bool num = flag != _prevPlay;
            _weight = Mathf.Lerp(_weight, flag ? 1 : 0, (flag ? _fadeInLerp : _fadeOutLerp) * Time.deltaTime);
            _prevPlay = flag;
            if (num && !flag)
            {
                _encounterSource.PlayOneShot(_goneClip);
                _closeEncounter.ResetCooldown();
                _farEncounter.ResetCooldown();
                Scp173InsanitySoundtrack.OnPlayerEscapeScp173?.Invoke(lhub);
            }
            if (!flag)
            {
                return;
            }
            float num2 = _distanceCap;
            foreach (Scp173Role observed in _observed173s)
            {
                float num3 = DistanceTo(observed);
                if (num3 < num2)
                {
                    num2 = num3;
                }
            }
            _lastDistance = Mathf.Lerp(_lastDistance, num2, Time.deltaTime * _distanceLerp);
        }

        private void OnRoleChanged(ReferenceHub userHub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            if (userHub.isLocalPlayer)
            {
                _isActive = userHub.IsHuman();
                _prevPlay = false;
                _weight = 0f;
                _observed173sCount = 0;
                _stopAmbientTime = 0f;
            }
        }

        private void OnFrozen(Scp173Role target)
        {
            if (_observed173s.Add(target))
            {
                _observed173sCount++;
            }
            if (DistanceTo(target) < _closeEncounterDistanceThreshold && _closeEncounter.TryUse(CurTime))
            {
                _encounterSource.PlayOneShot(_closeEncounter.Clip);
            }
            else if (_farEncounter.TryUse(CurTime))
            {
                _encounterSource.PlayOneShot(_farEncounter.Clip);
            }
        }

        private float DistanceTo(Scp173Role role)
        {
            return Vector3.Distance(role.FpcModule.Position, MainCameraController.CurrentCamera.position);
        }

        private bool IsObservedBy(Scp173Role scp173, ReferenceHub lhub)
        {
            if (scp173.SubroutineModule.TryGetSubroutine<Scp173ObserversTracker>(out var subroutine))
            {
                return subroutine.IsObservedBy(lhub);
            }
            return false;
        }

        public override void UpdateVolume(float volumeScale)
        {
            for (int i = 0; i < _ambientsCount; i++)
            {
                float num = volumeScale;
                if (num > 0f)
                {
                    float time = _lastDistance / _distanceAmbients[i].maxDistance;
                    num *= _volumeCurves[i].Evaluate(time);
                }
                _distanceAmbients[i].volume = num;
                _distanceAmbients[i].panStereo = _lastScreenPosition;
            }
        }
    }
}
