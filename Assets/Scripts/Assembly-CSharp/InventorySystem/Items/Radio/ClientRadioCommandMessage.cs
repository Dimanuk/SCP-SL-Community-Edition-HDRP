namespace InventorySystem.Items.Radio
{
    public struct ClientRadioCommandMessage : global::Mirror.NetworkMessage
    {
        public global::InventorySystem.Items.Radio.RadioMessages.RadioCommand Command;

        public ClientRadioCommandMessage(global::InventorySystem.Items.Radio.RadioMessages.RadioCommand cmd)
        {
            Command = cmd;
        }

        public void Serialize(global::Mirror.NetworkWriter writer)
        {
            writer.WriteByte((byte)Command);
        }

        public ClientRadioCommandMessage(global::Mirror.NetworkReader reader)
        {
            Command = (global::InventorySystem.Items.Radio.RadioMessages.RadioCommand)reader.ReadByte();
        }
    }
}
