using System;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using UnityEngine;
using Mirror;

namespace InventorySystem.Items.SwayControllers
{
    public class GoopSway : IItemSwayController
    {
        [Serializable]
        public struct GoopSwaySettings
        {
            public Transform TargetTransform;
            public float SwayIntensity;
            public float TranslationIntensity;
            public float ZAxisIntensity;
            public float SwaySmoothness;
            public float TranslationSmoothness;
            public float BobIntensity;
            public float CentrifugalIntensity;
            public int Invert;

            public GoopSwaySettings(
                        Transform targetTransform = null,
                        float swayIntensity = 1f,
                        float translationIntensity = 1f,
                        float zAxisIntensity = 1f,
                        float swaySmoothness = 1f,
                        float translationSmoothness = 1f,
                        float bobIntensity = 1f,
                        float centrifugalIntensity = 1f,
                        bool invertSway = false)
            {
                TargetTransform = targetTransform;
                SwayIntensity = swayIntensity;
                TranslationIntensity = translationIntensity;
                ZAxisIntensity = zAxisIntensity;
                SwaySmoothness = swaySmoothness;
                TranslationSmoothness = translationSmoothness;
                BobIntensity = bobIntensity;
                CentrifugalIntensity = centrifugalIntensity;
                Invert = invertSway ? -1 : 1;
            }
        }

        private const float MaximumReasonableMouseSpeed = 15f;
        private const float OverallBobMultiplier = 12f;
        private const float OverallSwayMultiplier = 0.013f;

        private readonly GoopSwaySettings _settings;
        private Vector3 _positionOffset;
        private readonly ReferenceHub _owner;
        private readonly Transform _ownerTransform;
        private readonly Transform _camTransform;

        private float _prevRotX;
        private float _prevRotY;

        private float CurRotX => _ownerTransform != null ? _ownerTransform.localEulerAngles.y : 0f;
        private float CurRotY => _camTransform != null ? _camTransform.localEulerAngles.x : 0f;

        private AnimatedCharacterModel CharModel
        {
            get
            {
                if (_owner.roleManager.CurrentRole is IFpcRole fpcRole)
                {
                    return fpcRole.FpcModule.CharacterModelInstance as AnimatedCharacterModel;
                }
                return null;
            }
        }

        public GoopSway(GoopSwaySettings settings, ReferenceHub owner)
        {
            _settings = settings;
            _owner = owner;
            _ownerTransform = owner?.transform;
            _camTransform = owner?.PlayerCameraReference;

            if (_settings.TargetTransform != null)
                _positionOffset = _settings.TargetTransform.localPosition;

            _prevRotX = CurRotX;
            _prevRotY = CurRotY;
        }

        public void UpdateSway()
        {
            if (!NetworkClient.active) return;
            if (_settings.TargetTransform == null) return;

            CameraSway(_settings.TargetTransform);
            Transition(_settings.TargetTransform);
        }

        private void GetInput(out float x, out float y)
        {
            float dot = Vector3.Dot(_ownerTransform.forward, _camTransform.forward);
            float factor = OverallSwayMultiplier / Time.deltaTime;

            x = Mathf.Clamp(Mathf.DeltaAngle(_prevRotX, CurRotX) * factor,
                -MaximumReasonableMouseSpeed, MaximumReasonableMouseSpeed);
            y = Mathf.Clamp(Mathf.DeltaAngle(CurRotY, _prevRotY) * factor * Mathf.Clamp01(dot),
                -MaximumReasonableMouseSpeed, MaximumReasonableMouseSpeed);

            _prevRotX = CurRotX;
            _prevRotY = CurRotY;
        }

        private void CameraSway(Transform tr)
        {
            if (tr == null) return;

            GetInput(out float x, out float y);

            float inv = (float)_settings.Invert;

            Quaternion targetRot =
                Quaternion.AngleAxis(_settings.SwayIntensity * x, -Vector3.up * inv) *
                Quaternion.AngleAxis(_settings.SwayIntensity * y, Vector3.right * inv) *
                Quaternion.AngleAxis(_settings.ZAxisIntensity * x, -Vector3.forward * inv);

            tr.localRotation = Quaternion.Slerp(tr.localRotation, targetRot, Time.deltaTime * _settings.SwaySmoothness);
        }

        private void Transition(Transform tr)
        {
            if (tr == null) return;
            Vector3 velocity = _owner.GetVelocity();
            Vector3 localVelocity = _ownerTransform.InverseTransformDirection(velocity);

            Vector3 targetPos = _positionOffset + new Vector3(
                -localVelocity.x * _settings.TranslationIntensity,
                -Mathf.Abs(localVelocity.z) * _settings.TranslationIntensity,
                0f);

            if (_settings.CentrifugalIntensity != 0f)
            {
                targetPos += Vector3.forward
                    * (Quaternion.Angle(Quaternion.identity, tr.localRotation) / 360f)
                    * _settings.CentrifugalIntensity;
            }

            AnimatedCharacterModel charModel = CharModel;
            Vector3 bob = (charModel != null)
                ? charModel.HeadBobPosition * (_settings.BobIntensity * OverallBobMultiplier)
                : Vector3.zero;

            Vector3 smoothed = Vector3.Slerp(tr.localPosition - bob, targetPos,
                Time.deltaTime * _settings.TranslationSmoothness);
            tr.localPosition = smoothed + bob;
        }
    }
}