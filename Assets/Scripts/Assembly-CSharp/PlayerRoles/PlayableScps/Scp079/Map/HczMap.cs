using System.Collections.Generic;
using MapGeneration;
using MapGeneration.Distributors;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using UnityEngine;
using UnityEngine.UI;
using Utils.NonAllocLINQ;

namespace PlayerRoles.PlayableScps.Scp079.Map
{
    public class HczMap : ProceduralZoneMap
    {
        private static readonly Color GeneratorColor = new Color(1f, 0.1f, 0f, 0.15f);

        private const RoomName RotateRoom = RoomName.HczCheckpointToEntranceZone;
        private const float AngleOffset = 180f;

        [SerializeField]
        private ProceduralZoneMap _entranceMap;

        protected override void PlaceRooms()
        {
            base.PlaceRooms();

            if (HashsetExtensions.TryGetFirst(
                    RoomIdentifier.AllRoomIdentifiers,
                    x => x.Name == RotateRoom && x.Zone == FacilityZone.HeavyContainment,
                    out RoomIdentifier first)
                && NodesByRoom.TryGetValue(first, out Image roomImage))
            {
                float z = roomImage.rectTransform.localEulerAngles.z;
                Rotate(NodesByRoom, z);
                Rotate(_entranceMap.NodesByRoom, z);
            }
        }

        public override void UpdateOpened(Scp079Camera curCam)
        {
            base.UpdateOpened(curCam);

            float pulse = Mathf.Sin(Time.timeSinceLevelLoad * Mathf.PI);

            foreach (Scp079Generator generator in Scp079Recontainer.AllGenerators)
            {
                if (!generator.Activating)
                    continue;

                RoomIdentifier room = RoomIdUtils.RoomAtPosition(generator.transform.position);
                if (NodesByRoom.TryGetValue(room, out Image roomImage))
                {
                    roomImage.color = Color.Lerp(roomImage.color, GeneratorColor, Mathf.Abs(pulse));
                }
            }
        }

        private void Rotate(Dictionary<RoomIdentifier, Image> dic, float angleDeg)
        {
            Vector3 euler = Vector3.forward * (AngleOffset - angleDeg);

            foreach (KeyValuePair<RoomIdentifier, Image> pair in dic)
            {
                RectTransform rt = pair.Value.rectTransform;

                rt.localPosition = Quaternion.Euler(euler) * rt.localPosition;
                rt.Rotate(euler, Space.Self);
            }
        }
    }
}
