using Mirror;

namespace InventorySystem.Items.MicroHID
{
    public static class HidStatusMessageFunctions
    {
        public static void Serialize(this global::Mirror.NetworkWriter writer, global::InventorySystem.Items.MicroHID.HidStatusMessage value)
        {
            writer.WriteUShort(value.Serial);
            writer.WriteByte(value.MessageCode);
            writer.WriteByte((byte)value.MessageType);
        }

        public static global::InventorySystem.Items.MicroHID.HidStatusMessage Deserialize(this global::Mirror.NetworkReader reader)
        {
            return new global::InventorySystem.Items.MicroHID.HidStatusMessage
            {
                Serial = reader.ReadUShort(),
                MessageCode = reader.ReadByte(),
                MessageType = (global::InventorySystem.Items.MicroHID.HidStatusMessageType)reader.ReadByte(),
                Time = global::UnityEngine.Time.timeSinceLevelLoad
            };
        }
    }
}
