using PlayerRoles.Spectating;
using PlayerStatsSystem;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace CustomPlayerEffects
{
    public class CardiacHealthIndicator : SubEffectBase
    {
        [SerializeField]
        private AnimationCurve _healthToWeight;

        [SerializeField]
        private PostProcessVolume _ppv;

        [SerializeField]
        private float _speedMultiplier;

        private HealthStat _hp;
        private float _targetWeight;

        public override bool IsActive => MainEffect.IsEnabled;

        public void SetTargetWeight(float value, bool forceWeight = false)
        {
            _targetWeight = value;
            if (forceWeight)
                _ppv.weight = value;
        }

        public override void DisableEffect()
        {
            base.DisableEffect();
            _targetWeight = 0f;
            _ppv.weight = 0f;
        }

        internal override void UpdateEffect()
        {
            var hub = MainEffect.Hub;

            if (!hub.isLocalPlayer && !SpectatorNetworking.IsLocallySpectated(hub))
                return;

            if (_healthToWeight != null)
                _targetWeight = _healthToWeight.Evaluate(_hp.NormalizedValue);
        }

        internal override void Init(StatusEffectBase mainEffect)
        {
            base.Init(mainEffect);
            _hp = mainEffect.Hub.playerStats.GetModule<HealthStat>();
        }

        private void Update()
        {
            if (_ppv == null)
                return;

            // When the effect is no longer active, drive the target back to 0 so the
            // post-process fades out instead of freezing at its last value.
            if (!IsActive)
            {
                if (_targetWeight == 0f && _ppv.weight == 0f)
                    return;

                _targetWeight = 0f;
            }

            _ppv.weight = Mathf.Clamp01(Mathf.MoveTowards(_ppv.weight, _targetWeight, Time.deltaTime * _speedMultiplier));
        }
    }
}
