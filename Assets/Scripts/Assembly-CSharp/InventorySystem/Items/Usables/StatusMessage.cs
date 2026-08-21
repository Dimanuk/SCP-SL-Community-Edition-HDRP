using Mirror;

namespace InventorySystem.Items.Usables
{
    public struct StatusMessage : global::Mirror.NetworkMessage
    {
        public enum StatusType : byte
        {
            Start = 0,
            Cancel = 1
        }

        public global::InventorySystem.Items.Usables.StatusMessage.StatusType Status;

        public ushort ItemSerial;

        public StatusMessage(global::InventorySystem.Items.Usables.StatusMessage.StatusType status, ushort serial)
        {
            Status = status;
            ItemSerial = serial;
        }

        public void Serialize(global::Mirror.NetworkWriter writer)
        {
            writer.WriteByte((byte)Status);
            writer.WriteUShort(ItemSerial);
        }
    }
}
