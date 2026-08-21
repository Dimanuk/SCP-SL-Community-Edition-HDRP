namespace InventorySystem.Items.Firearms.Attachments
{
    public struct AttachmentsSetupPreference : global::Mirror.NetworkMessage
    {
        public ItemType Weapon;

        public uint AttachmentsCode;

        public void Deserialize(global::Mirror.NetworkReader reader)
        {
            Weapon = (ItemType)reader.ReadByte();
            AttachmentsCode = global::Mirror.NetworkReaderExtensions.ReadUInt(reader);
        }

        public void Serialize(global::Mirror.NetworkWriter writer)
        {
            writer.WriteByte((byte)Weapon);
            global::Mirror.NetworkWriterExtensions.WriteUInt(writer, AttachmentsCode);
        }
    }
}
