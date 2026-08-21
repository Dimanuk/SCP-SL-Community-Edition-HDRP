using Mirror;
using PlayerStatsSystem;
using UnityEngine;
using Utils.Networking;

namespace PlayerRoles.Ragdolls
{
    public static class RagdollDataReaderWriter
    {
        public static void WriteRagdollData(this NetworkWriter writer, RagdollData info)
        {
            writer.WriteByte((byte)info.RoleType);
            writer.WriteString(info.Nickname);
            writer.WriteDamageHandler(info.Handler);
            writer.WriteVector3(info.StartPosition);
            writer.WriteLowPrecisionQuaternion(new LowPrecisionQuaternion(info.StartRotation));
            writer.WriteDouble(info.CreationTime);
            writer.WriteReferenceHub(info.OwnerHub);
        }

        public static RagdollData ReadRagdollData(this NetworkReader reader)
        {
            RoleTypeId roleType = (RoleTypeId)reader.ReadByte();
            string nick = reader.ReadString();
            DamageHandlerBase handler = reader.ReadDamageHandler();
            Vector3 position = reader.ReadVector3();
            Quaternion value = reader.ReadLowPrecisionQuaternion().Value;
            double creationTime = reader.ReadDouble();
            return new RagdollData(reader.ReadReferenceHub(), handler, roleType, position, value, nick, creationTime);
        }
    }
}
