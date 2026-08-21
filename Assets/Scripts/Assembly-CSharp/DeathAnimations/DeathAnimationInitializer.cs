using PlayerRoles.Ragdolls;
using UnityEngine;

namespace DeathAnimations
{
    public static class DeathAnimationInitializer
    {
        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            RagdollManager.OnRagdollSpawned += HandleAddedRagdoll;
            RagdollManager.OnRagdollRemoved += HandleRemovedRagdoll;
        }

        private static void HandleAddedRagdoll(BasicRagdoll rg)
        {
            var allDeathAnimations = rg.AllDeathAnimations;
            foreach (var animation in allDeathAnimations)
            {
                animation.StartDeathAnimation(rg);
            }
        }

        private static void HandleRemovedRagdoll(BasicRagdoll rg)
        {
            var allDeathAnimations = rg.AllDeathAnimations;
            foreach (var animation in allDeathAnimations)
            {
                if (animation.IsPlaying)
                {
                    animation.KillDeathAnimation();
                }
            }
        }
    }
}
