using CameraShaking;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp096
{
    public class Scp096BecomeTargetShake : IShakeEffect
    {
        private float _remaining = 1f;

        private const float FovKick = 0.9f;
        private const float LerpSpeed = 0.92f;
        private const float RemoveThreshold = 0.001f;

        public bool GetEffect(ReferenceHub ply, out ShakeEffectValues shakeValues)
        {
            _remaining = Mathf.Lerp(_remaining, 0f, LerpSpeed * Time.deltaTime);

            shakeValues = new ShakeEffectValues(
                null,
                null,
                null,
                Mathf.Lerp(1f, FovKick, _remaining)
            );

            return _remaining > RemoveThreshold;
        }
    }
}