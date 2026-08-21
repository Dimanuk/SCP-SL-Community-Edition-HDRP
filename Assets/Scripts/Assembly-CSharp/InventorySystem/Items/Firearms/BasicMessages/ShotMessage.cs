using Mirror;

namespace InventorySystem.Items.Firearms.BasicMessages
{
    public struct ShotMessage : global::Mirror.NetworkMessage
    {
        public uint TargetNetId;

        public global::RelativePositioning.RelativePosition TargetPosition;

        public global::UnityEngine.Quaternion TargetRotation;

        public ushort ShooterWeaponSerial;

        public global::RelativePositioning.RelativePosition ShooterPosition;

        public global::UnityEngine.Quaternion ShooterCameraRotation;

        public void Deserialize(global::Mirror.NetworkReader reader)
        {
            TargetNetId = reader.ReadUInt();
            if (TargetNetId != 0)
            {
                TargetPosition = global::RelativePositioning.RelativePositionSerialization.ReadRelativePosition(reader);
                TargetRotation = global::UnityEngine.Quaternion.Euler((float)(int)reader.ReadByte() / 255f * 360f * global::UnityEngine.Vector3.up);
            }
            ShooterWeaponSerial = reader.ReadUShort();
            ShooterPosition = global::RelativePositioning.RelativePositionSerialization.ReadRelativePosition(reader);
            ShooterCameraRotation = global::Mirror.NetworkReaderExtensions.ReadQuaternion(reader);
        }

        public void Serialize(global::Mirror.NetworkWriter writer)
        {
            writer.WriteUInt(TargetNetId);
            if (TargetNetId != 0)
            {
                global::RelativePositioning.RelativePositionSerialization.WriteRelativePosition(writer, TargetPosition);
                writer.WriteByte((byte)global::UnityEngine.Mathf.RoundToInt(global::UnityEngine.Mathf.Clamp01(TargetRotation.eulerAngles.y / 360f) * 255f));
            }
            writer.WriteUShort(ShooterWeaponSerial);
            global::RelativePositioning.RelativePositionSerialization.WriteRelativePosition(writer, ShooterPosition);
            global::Mirror.NetworkWriterExtensions.WriteQuaternion(writer, ShooterCameraRotation);
        }
    }
}