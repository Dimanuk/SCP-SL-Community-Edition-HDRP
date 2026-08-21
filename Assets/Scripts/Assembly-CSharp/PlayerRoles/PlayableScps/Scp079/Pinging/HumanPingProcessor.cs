using System.Collections.Generic;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Pinging
{
    public class HumanPingProcessor : IPingProcessor
    {
        private const float HumanRangeSqr = 0.64f;
        private const float HumanHeight = 1.5f;
        private const int HitboxLayer = 8192;

        public float Range => 25f;
        public LayerMask Mask => VisionInformation.VisionLayerMask;
        public int IconId => 7;

        public bool TryProcess(RaycastHit hit)
        {
            return TryGetHuman(hit.point, out _);
        }

        public static bool TryGetHuman(Vector3 hitPos, out ReferenceHub best)
        {
            best = null;
            float bestDistSqr = HumanRangeSqr;

            foreach (ReferenceHub hub in ReferenceHub.AllHubs)
            {
                if (!PlayerRolesUtils.IsHuman(hub))
                    continue;

                if (hub.roleManager.CurrentRole is not FirstPersonControl.IFpcRole fpcRole)
                    continue;

                Vector3 rolePos = fpcRole.FpcModule.Position;

                float yDiff = Mathf.Abs(rolePos.y - hitPos.y);
                if (yDiff > HumanHeight)
                    continue;

                float sqrDistXZ = Misc.SqrMagnitudeIgnoreY(rolePos - hitPos);
                if (sqrDistXZ > bestDistSqr)
                    continue;

                bestDistSqr = sqrDistXZ;
                best = hub;
            }

            return best != null;
        }
    }
}
