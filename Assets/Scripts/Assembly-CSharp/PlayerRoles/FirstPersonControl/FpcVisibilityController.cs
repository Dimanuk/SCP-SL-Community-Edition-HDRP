using CustomPlayerEffects;
using PlayerRoles.Visibility;
using UnityEngine;

namespace PlayerRoles.FirstPersonControl
{
    public class FpcVisibilityController : VisibilityController
    {
        private const int SurfaceHeight = 800;

        private Invisible _invisibleEffect;

        protected virtual int NormalMaxRangeSqr => 1100;

        protected virtual int SurfaceMaxRangeSqr => 4900;

        public override InvisibilityFlags GetActiveFlags(ReferenceHub observer)
        {
            InvisibilityFlags invisibilityFlags = base.GetActiveFlags(observer);
            if (_invisibleEffect.IsEnabled)
            {
                invisibilityFlags |= InvisibilityFlags.Scp268;
            }
            if (observer.roleManager.CurrentRole is not IFpcRole fpcRole || !(base.Owner.roleManager.CurrentRole is IFpcRole fpcRole2))
            {
                return invisibilityFlags;
            }
            Vector3 position = fpcRole.FpcModule.Position;
            Vector3 position2 = fpcRole2.FpcModule.Position;
            float num = ((Mathf.Min(position.y, position2.y) > SurfaceHeight) ? SurfaceMaxRangeSqr : NormalMaxRangeSqr);
            if ((position - position2).sqrMagnitude > num)
            {
                invisibilityFlags |= InvisibilityFlags.OutOfRange;
            }
            return invisibilityFlags;
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            _invisibleEffect = base.Owner.playerEffectsController.GetEffect<Invisible>();
        }
    }
}
