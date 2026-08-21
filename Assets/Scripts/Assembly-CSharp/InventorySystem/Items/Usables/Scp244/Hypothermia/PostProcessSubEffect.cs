using System;
using CustomPlayerEffects;
using PlayerRoles;
using PostProcessing;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace InventorySystem.Items.Usables.Scp244.Hypothermia
{
    public class PostProcessSubEffect : HypothermiaSubEffectBase
    {
        [Serializable]
        private struct IntensityOverTemp
        {
            [SerializeField]
            private AnimationCurve _effectCurve;

            [SerializeField]
            private float _scpIntensityMultiplier;

            internal float GetValue(PostProcessSubEffect fx)
            {
                if (!fx._isAlive)
                    return 0f;

                float value = _effectCurve.Evaluate(fx._temperature);

                if (fx._isSCP)
                    value *= _scpIntensityMultiplier;

                return value;
            }
        }

        [SerializeField]
        private IntensityOverTemp _weightCurve;

        [SerializeField]
        private IntensityOverTemp _refCurve;

        [SerializeField]
        private IntensityOverTemp _intenCurve;

        [SerializeField]
        private IntensityOverTemp _frostCurve;

        private PostProcessVolume _ppv;
        private VignetteRefraction _refraction;

        private bool _isAlive;
        private bool _isSCP;
        private float _temperature;

        [SerializeField]
        private TemperatureSubEffect _temp;

        public override bool IsActive => _temp != null && _temp.IsActive;

        internal override void Init(StatusEffectBase mainEffect)
        {
            base.Init(mainEffect);

            _ppv = GetComponent <PostProcessVolume>();
            if (_ppv != null && _ppv.profile != null)
                _refraction = _ppv.profile.GetSetting<VignetteRefraction>();
        }

        internal override void UpdateEffect(float curExposure)
        {
            if (!Hub.isLocalPlayer)
            {
                if (MainEffect == null || !MainEffect.IsSpectated)
                    return;
            }

            _isAlive = PlayerRolesUtils.IsAlive(Hub);
            _isSCP = PlayerRolesUtils.IsSCP(Hub, true);

            if (_temp == null)
                return;

            _temperature = _temp.CurTemperature;

            if (_ppv == null)
                return;

            _ppv.weight = _isAlive ? _weightCurve.GetValue(this) : 0f;

            if (_refraction == null)
                return;

            _refraction.RefractionPower.value = _isAlive ? _refCurve.GetValue(this) : 0f;
            _refraction.Intensity.value = _isAlive ? _intenCurve.GetValue(this) : 0f;
            _refraction.Frosting.value = _isAlive ? _frostCurve.GetValue(this) : 0f;
        }

        public override void DisableEffect()
        {
            base.DisableEffect();

            if (_ppv != null)
                _ppv.weight = 0f;
        }
    }
}