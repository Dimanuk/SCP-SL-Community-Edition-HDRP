using InventorySystem.Items.MicroHID;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Pinging
{
    public class MicroHidPingProcesssor : IPingProcessor
    {
        private const int PickupLayerMask = 512;
        private const int HitboxLayer = 8192;

        public float Range => 25f;
        public LayerMask Mask => VisionInformation.VisionLayerMask;
        public int IconId => 6;

        public bool TryProcess(RaycastHit hit)
        {
            if (hit.collider.GetComponentInParent<MicroHIDPickup>() != null)
                return true;

            if (!HumanPingProcessor.TryGetHuman(hit.point, out ReferenceHub hub))
                return false;

            return IsHoldingMicro(hub);
        }

        private bool IsHoldingMicro(ReferenceHub hub)
        {
            return hub.inventory.CurItem.TypeId == ItemType.MicroHID;
        }
    }
}
