
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Visibility;

namespace PlayerRoles.PlayableScps.Scp096
{
	public class Scp096VisibilityController : FpcVisibilityController
    {
        private Scp096Role _role;            

        private Scp096TargetsTracker _targetsTracker;

        public override InvisibilityFlags IgnoredFlags
        {
            get
            {
                InvisibilityFlags flags = base.IgnoredFlags;
                if (HideNonTargets)
                    flags |= (InvisibilityFlags)3u;
                return flags;
            }
        }

        private bool HideNonTargets
        {
            get
            {
                Scp096RageState rageState = _role.StateController.RageState;
                return rageState == Scp096RageState.Distressed || rageState == Scp096RageState.Enraged;
            }
        }

        public override bool ValidateVisibility(ReferenceHub target)
        {
            if (!HideNonTargets)
                return base.ValidateVisibility(target);

            if (_targetsTracker == null || !_targetsTracker.HasTarget(target))
                return false;

            return base.ValidateVisibility(target);
        }

        public override void SpawnObject()
        {
            base.SpawnObject();

            if (!NetworkServer.active)
                return;

            _role = Role as Scp096Role;
            if (_role == null)
                return;

            _role.SubroutineModule.TryGetSubroutine(out _targetsTracker);
        }
    }
}
