using Mirror;

namespace InventorySystem.Items.Usables
{
    public struct ItemCooldownMessage : global::Mirror.NetworkMessage
    {
        public ushort ItemSerial;

        public float RemainingTime;

        public ItemCooldownMessage(ushort serial, float remainingTime)
        {
            ItemSerial = serial;
            RemainingTime = remainingTime;
        }

        public void Serialize(global::Mirror.NetworkWriter writer)
        {
            writer.WriteUShort(ItemSerial);
            writer.WriteFloat(RemainingTime);
        }
    }
}
