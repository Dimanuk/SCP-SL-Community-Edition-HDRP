using PlayerRoles.Ragdolls;
using PlayerRoles.Spectating;
using UnityEngine;

namespace DeathAnimations
{
    public abstract class DeathAnimation : MonoBehaviour
    {
        public bool IsPlaying { get; protected set; }

        public BasicRagdoll TargetRagdoll { get; protected set; }

        protected bool IsLocalPlayer => TargetRagdoll.Info.OwnerHub.isLocalPlayer;

        protected bool IsBeingSpectated => TargetRagdoll.Info.OwnerHub.IsSpectatedBy(ReferenceHub.LocalHub);

        public void StartDeathAnimation(BasicRagdoll ragdollReference)
        {
            IsPlaying = true;
            TargetRagdoll = ragdollReference;
            OnAnimationStarted();
        }

        public void KillDeathAnimation()
        {
            if (IsPlaying)
            {
                IsPlaying = false;
                OnAnimationEnded();
            }
        }

        protected virtual void OnAnimationStarted()
        {
        }

        protected virtual void OnAnimationEnded()
        {
        }
    }
}
