namespace InventorySystem.Items.Firearms.Attachments
{
    public struct AttachmentsChangeRequest : global::Mirror.NetworkMessage
    {
        public ushort WeaponSerial;

        public uint AttachmentsCode;

        public void Deserialize(global::Mirror.NetworkReader reader)
        {
            WeaponSerial = global::Mirror.NetworkReaderExtensions.ReadUShort(reader);
            AttachmentsCode = global::Mirror.NetworkReaderExtensions.ReadUInt(reader);
        }

        public void Serialize(global::Mirror.NetworkWriter writer)
        {
            global::Mirror.NetworkWriterExtensions.WriteUShort(writer, WeaponSerial);
            global::Mirror.NetworkWriterExtensions.WriteUInt(writer, AttachmentsCode);
        }
    }
}
