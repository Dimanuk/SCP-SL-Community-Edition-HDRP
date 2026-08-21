using Interactables;
using Interactables.Interobjects.DoorUtils;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Pinging
{
    public class DoorPingProcessor : IPingProcessor
    {
        public float Range => 20f;
        public LayerMask Mask => InteractionCoordinator.RaycastMask.Mask;
        public int IconId => 1;

        public bool TryProcess(RaycastHit hit)
        {
            if (!hit.collider.TryGetComponent<InteractableCollider>(out var collider))
                return false;

            return collider?.GetComponentInParent<DoorVariant>() != null;
        }
    }
}
