using Mirror;
using RelativePositioning;
using UnityEngine;

namespace PlayerRoles.FirstPersonControl.NetworkMessages
{
    public struct FpcOverrideMessage : NetworkMessage
    {
        public readonly Vector3 Position;

        public readonly float DeltaRotation;

        private const float FullAngle = 360f;

        public FpcOverrideMessage(Vector3 pos, float rot)
        {
            Position = pos;
            DeltaRotation = rot;
        }

        public FpcOverrideMessage(NetworkReader reader)
        {
            Position = reader.ReadRelativePosition().Position;
            DeltaRotation = Mathf.Lerp(-360f, FullAngle, (float)(int)reader.ReadUShort() / 65535f);
        }

        public void Write(NetworkWriter writer)
        {
            writer.WriteRelativePosition(new RelativePosition(Position));
            float num;
            for (num = DeltaRotation; num < -360f; num += FullAngle)
            {
            }
            while (num > FullAngle)
            {
                num -= FullAngle;
            }
            float num2 = Mathf.InverseLerp(-360f, FullAngle, num);
            writer.WriteUShort((ushort)Mathf.RoundToInt(num2 * 65535f));
        }

        public void ProcessMessage()
        {
            if (ReferenceHub.TryGetLocalHub(out var hub) && hub.roleManager.CurrentRole is IFpcRole fpcRole && fpcRole.FpcModule.ModuleReady)
            {
                fpcRole.FpcModule.Position = Position;
                fpcRole.FpcModule.MouseLook.CurrentHorizontal += DeltaRotation;
            }
        }
    }
}
