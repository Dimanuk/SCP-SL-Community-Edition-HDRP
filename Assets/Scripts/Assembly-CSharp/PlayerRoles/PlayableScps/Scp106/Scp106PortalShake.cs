using CameraShaking;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp106
{
    public class Scp106PortalShake : IShakeEffect
    {
        private const float StalkHeight = -0.75f;
        private const float NormalizationLerp = 300f;

        public Scp106Model _model;
        public Scp106Role _role;

        public static Scp106PortalShake _latestEffect;

        public Scp106PortalShake(Scp106Role role, Scp106Model model)
        {
            _role = role;
            _model = model;
            _latestEffect = this;
        }

        public bool GetEffect(ReferenceHub ply, out ShakeEffectValues shakeValues)
        {
            shakeValues = ShakeEffectValues.None;

            if (_model == null || !_model.IsTracked || _latestEffect != this)
                return false;

            if (!_role.TryGetOwner(out ReferenceHub owner))
                return false;

            Scp106SinkholeController sinkhole = _role.Sinkhole;
            if (sinkhole == null)
                return false;

            float verticalLook = 0f;
            if (sinkhole.IsDuringAnimation)
            {
                verticalLook = Time.deltaTime * (owner.PlayerCameraReference.localRotation.x - _model.StalkCameraTarget.localRotation.x) * NormalizationLerp;
            }

            Vector3 positionOffset = _model.StalkCameraTarget.position - owner.PlayerCameraReference.position;
            positionOffset.y = sinkhole.NormalizedState * StalkHeight;

            shakeValues = new ShakeEffectValues(null, null, positionOffset, 1f, verticalLook);
            return true;
        }
    }
}
