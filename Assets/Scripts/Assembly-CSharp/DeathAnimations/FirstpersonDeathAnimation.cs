using PlayerRoles;
using PlayerRoles.Spectating;

namespace DeathAnimations
{
    public abstract class FirstpersonDeathAnimation : DeathAnimation
    {
        private bool _eventAssigned;

        protected bool IsFirstperson => ReferenceHub.LocalHub?.roleManager.CurrentRole is SpectatorRole && SpectatorTargetTracker.LastTrackedPlayer == TargetRagdoll.Info.OwnerHub;

        public bool EventAssigned
        {
            set
            {
                if (value)
                {
                    PlayerRoleManager.OnRoleChanged += OnRoleChanged;
                    SpectatorTargetTracker.OnTargetChanged += OnTargetChanged;
                }
                else
                {
                    PlayerRoleManager.OnRoleChanged -= OnRoleChanged;
                    SpectatorTargetTracker.OnTargetChanged -= OnTargetChanged;
                }
                _eventAssigned = value;
            }
        }

        protected override void OnAnimationStarted()
        {
            if (IsFirstperson)
            {
                EventAssigned = true;
            }
        }

        protected override void OnAnimationEnded()
        {
            if (_eventAssigned)
            {
                EventAssigned = false;
            }
        }

        protected virtual void OnDestroy()
        {
            if (_eventAssigned)
            {
                EventAssigned = false;
            }
        }

        private void OnTargetChanged()
        {
            // While the death-cam is active (tracker alive, no spectatable target yet),
            // the target-cleared event must not end the animation. Once a real target is
            // selected, the animation ends even if the target is the ragdoll's owner —
            // otherwise a respawned target leaves the darken volume up forever.
            if (SpectatorTargetTracker.TrackerSet && SpectatorTargetTracker.CurrentTarget == null)
                return;

            if (IsPlaying)
            {
                IsPlaying = false;
                OnAnimationEnded();
            }
        }

        private void OnRoleChanged(ReferenceHub hub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            if (hub.isLocalPlayer && newRole.Team != Team.Dead)
            {
                if (IsPlaying)
                {
                    IsPlaying = false;
                    OnAnimationEnded();
                }
            }
        }
    }
}
