using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Pinging
{
    public class DefaultPingProcessor : IPingProcessor
    {
        public float Range => 10f;

        public LayerMask Mask => VisionInformation.VisionLayerMask;
        public int IconId => 0;

        public bool TryProcess(RaycastHit hit) => true;
    }
}