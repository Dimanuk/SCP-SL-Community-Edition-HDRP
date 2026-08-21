using Mirror;

namespace InventorySystem.Items.Usables
{
    public static class StatusMessageFunctions
    {
        public static void Serialize(this global::Mirror.NetworkWriter writer, global::InventorySystem.Items.Usables.StatusMessage value)
        {
            value.Serialize(writer);
        }

        public static global::InventorySystem.Items.Usables.StatusMessage Deserialize(this global::Mirror.NetworkReader reader)
        {
            return new global::InventorySystem.Items.Usables.StatusMessage((global::InventorySystem.Items.Usables.StatusMessage.StatusType)reader.ReadByte(), reader.ReadUShort());
        }
    }
}
