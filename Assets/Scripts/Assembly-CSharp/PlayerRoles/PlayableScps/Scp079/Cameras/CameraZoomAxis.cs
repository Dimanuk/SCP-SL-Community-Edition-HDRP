using System;
using System.Diagnostics;
using PlayerRoles.PlayableScps.Scp079.GUI;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Cameras
{
    [Serializable]
    public class CameraZoomAxis : CameraAxisBase
    {
        private const string ScrollAxis = "Mouse ScrollWheel";

        private readonly Stopwatch _cooldownStopwatch = Stopwatch.StartNew();

        private float _lastSoundZoom;

        private Offset _unzoomedOffset;

        [SerializeField] private Transform _zoomBone;
        [SerializeField] private Offset   _zoomedOffset;

        [SerializeField] public AnimationCurve _magnificationCurve;

        [SerializeField] private float _stepSize;
        [SerializeField] private float _cooldown;

        public float CurrentZoom => _magnificationCurve.Evaluate(CurValue);

        internal override void Update(Scp079Camera cam)
        {
            if (Scp079Role.LocalInstanceActive
                && cam != null
                && cam.IsUsedByLocalPlayer
                && cam.IsActive)
            {
                UpdateInputs();
            }

            base.Update(cam);
        }

        internal override void Awake(Scp079Camera cam)
        {
            base.Awake(cam);

            _unzoomedOffset = new Offset
            {
                position = _zoomBone.localPosition,
                rotation = _zoomBone.localEulerAngles,
                scale    = _zoomBone.localScale
            };

            TargetValue = 0f;
        }

        protected override void OnValueChanged(float newValue, Scp079Camera cam)
        {
            float t = Mathf.InverseLerp(MinValue, MaxValue, newValue);

            _zoomBone.localPosition = Vector3.Lerp(_unzoomedOffset.position, _zoomedOffset.position, t);
            _zoomBone.localRotation = Quaternion.Lerp(
                Quaternion.Euler(_unzoomedOffset.rotation),
                Quaternion.Euler(_zoomedOffset.rotation), t);
            _zoomBone.localScale    = Vector3.Lerp(_unzoomedOffset.scale,    _zoomedOffset.scale,    t);

            if (!SoundEffectSource.loop && _lastSoundZoom != TargetValue)
            {
                SoundEffectSource.Play();
                _lastSoundZoom = TargetValue;
            }
        }

        private void UpdateInputs()
        {
            if (Scp079CursorManager.LockCameras)
                return;

            float scroll = SensitivitySettings.RawInput
                ? Input.GetAxisRaw(ScrollAxis)
                : Input.GetAxis(ScrollAxis);

            if (scroll == 0f)
                return;

            if (_cooldownStopwatch.Elapsed.TotalSeconds < _cooldown)
                return;

            TargetValue += (scroll > 0f) ? _stepSize : -_stepSize;
            _cooldownStopwatch.Restart();
        }
    }
}
