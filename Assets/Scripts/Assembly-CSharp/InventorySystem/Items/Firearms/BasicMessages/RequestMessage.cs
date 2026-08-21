using Mirror;

namespace InventorySystem.Items.Firearms.BasicMessages
{
    public struct RequestMessage : global::Mirror.NetworkMessage
    {
        public ushort Serial;

        public global::InventorySystem.Items.Firearms.BasicMessages.RequestType Request;

        public RequestMessage(ushort serial, global::InventorySystem.Items.Firearms.BasicMessages.RequestType request)
        {
            Serial = serial;
            Request = request;
        }

        public void Deserialize(global::Mirror.NetworkReader reader)
        {
            Serial = (ushort)reader.ReadShort();
            Request = (global::InventorySystem.Items.Firearms.BasicMessages.RequestType)reader.ReadByte();
        }

        public void Serialize(global::Mirror.NetworkWriter writer)
        {
            writer.WriteShort((short)Serial);
            writer.WriteByte((byte)Request);
        }
    }
}
