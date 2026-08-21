using InventorySystem.Items;
using InventorySystem.Items.Usables.Scp244;
using UnityEngine;

namespace PlayerRoles.PlayableScps
{

    public readonly struct VisionInformation
    {
        public enum FailReason
        {
            NotOnSameFloor = 0,
            NotInDistance = 1,
            NotInView = 2,
            NotInLineOfSight = 3,
            InDarkRoom = 4,
            IsLooking = 5,
            UnkownReason = 6
        }
        public static readonly int VisionLayerMask = LayerMask.GetMask("Door", "Default", "Locker");
        public static readonly RaycastHit[] RaycastResult = new RaycastHit[1];

        public float LookingAmount { get; }
        public ReferenceHub SourceHub { get; }
        public Vector3 TargetPosition { get; }
        public float Distance { get; }
        public bool IsOnSameFloor { get; }
        public bool IsLooking { get; }
        public bool IsInDistance { get; }
        public bool IsInDarkness { get; }
        public bool IsInLineOfSight { get; }

        public VisionInformation(
            ReferenceHub sourceHub,
            Vector3 targetHub,
            bool isLooking,
            bool isOnSameFloor,
            float lookingAmount,
            float distance,
            bool isInLineOfSight,
            bool isInDarkness,
            bool isInDistance)
        {

            SourceHub = sourceHub;
            TargetPosition = targetHub;
            IsLooking = isLooking;
            LookingAmount = lookingAmount;
            Distance = distance;
            IsInLineOfSight = isInLineOfSight;
            IsInDarkness = isInDarkness;
            IsInDistance = isInDistance;
            IsOnSameFloor = isOnSameFloor;
        }

        public static VisionInformation GetVisionInformation(
            ReferenceHub source,
            Transform sourceCam,
            Vector3 target,
            float targetRadius = 0f,
            float visionTriggerDistance = 0f,
            bool checkFog = true,
            bool checkLineOfSight = true,
            int maskLayer = 0)
        {
            bool isOnSameFloor = false;
            bool flag = false; 

            if (Mathf.Abs(target.y - sourceCam.position.y) < 100f)
            {
                isOnSameFloor = true;
                flag = true;
            }

            bool isInDistance = visionTriggerDistance == 0f;

            Vector3 vector = target - sourceCam.position;
            float magnitude = vector.magnitude;

            if (flag && visionTriggerDistance > 0f)
            {
                float effectiveDistance = (!checkFog)
                    ? visionTriggerDistance
                    : ((target.y > 980f) ? visionTriggerDistance : (visionTriggerDistance / 2f));

                if (magnitude <= effectiveDistance)
                {
                    isInDistance = true;
                }
                flag = isInDistance;
            }

            float lookingAmount = 1f;
            bool isLooking = false;

            if (flag)
            {
                flag = false;

                if (magnitude < targetRadius)
                {
                    if (Vector3.Dot(source.transform.forward, (target - source.transform.position).normalized) > 0f)
                    {
                        isLooking = true;
                        lookingAmount = 1f;
                    }
                }
                else if (Scp244Utils.CheckVisibility(sourceCam.position, target))
                {
                    Vector3 localPos = sourceCam.InverseTransformPoint(target);

                    if (targetRadius != 0f)
                    {
                        localPos.x = Mathf.MoveTowards(localPos.x, 0f, targetRadius);
                        localPos.y = Mathf.MoveTowards(localPos.y, 0f, targetRadius);
                    }

                    AspectRatioSync aspectRatioSync = source.aspectRatioSync;
                    float angleX = Vector2.Angle(Vector2.up, new Vector2(localPos.x, localPos.z));

                    if (angleX < aspectRatioSync.XScreenEdge)
                    {
                        float angleY = Vector2.Angle(Vector2.up, new Vector2(localPos.y, localPos.z));
                        if (angleY < AspectRatioSync.YScreenEdge)
                        {
                            lookingAmount = (angleX + angleY) / aspectRatioSync.XplusY;
                            isLooking = true;
                        }
                    }
                }
            }

            bool isInLineOfSight = !checkLineOfSight;

            if (isLooking && checkLineOfSight)
            {
                if (maskLayer == 0)
                    maskLayer = VisionLayerMask;

                float rayDistance = isInDistance ? magnitude : vector.magnitude;
                isInLineOfSight = Physics.RaycastNonAlloc(
                    new Ray(sourceCam.position, vector.normalized),
                    RaycastResult,
                    rayDistance,
                    maskLayer) == 0;

                isLooking = isInLineOfSight;
            }

            bool isInDarkness = !CheckAttachments(source) && FlickerableLightController.IsInDarkenedRoom(target);

            isLooking = isLooking && !isInDarkness;

            return new VisionInformation(
                source,
                target,
                isLooking,
                isOnSameFloor,
                lookingAmount,
                magnitude,
                isInLineOfSight,
                isInDarkness,
                isInDistance);
        }

        private static bool CheckAttachments(ReferenceHub source)
        {

            if (source == null)
                return false;

            ItemBase curInstance = source.inventory.CurInstance;
            if (curInstance == null)
                return false;

            if (curInstance is ILightEmittingItem lightItem)
            {
                return lightItem.IsEmittingLight;
            }

            return false;
        }

        public FailReason GetFailReason()
        {
            if (!IsOnSameFloor)
                return FailReason.NotOnSameFloor;

            if (!IsInDistance)
                return FailReason.NotInDistance;

            if (LookingAmount >= 1f)
                return FailReason.NotInView;

            if (!IsInLineOfSight)
                return FailReason.NotInLineOfSight;

            if (IsInDarkness)
                return FailReason.InDarkRoom;

            if (!IsLooking)
                return FailReason.UnkownReason;

            return FailReason.IsLooking;
        }
    }
}