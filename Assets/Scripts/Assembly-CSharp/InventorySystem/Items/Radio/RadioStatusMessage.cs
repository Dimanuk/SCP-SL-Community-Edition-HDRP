using Mirror;

namespace InventorySystem.Items.Radio
{
    public struct RadioStatusMessage : global::Mirror.NetworkMessage
    {
        public readonly global::InventorySystem.Items.Radio.RadioMessages.RadioRangeLevel Range;

        public readonly byte Battery;

        public readonly uint Owner;

        public void Serialize(global::Mirror.NetworkWriter writer)
        {
            global::Mirror.NetworkWriterExtensions.WriteSByte(writer, (sbyte)Range);
            writer.WriteByte(Battery);
            writer.WriteUInt(Owner);
        }

        public RadioStatusMessage(global::Mirror.NetworkReader reader)
        {
            Range = (global::InventorySystem.Items.Radio.RadioMessages.RadioRangeLevel)global::Mirror.NetworkReaderExtensions.ReadSByte(reader);
            Battery = reader.ReadByte();
            Owner = reader.ReadUInt();
        }

        public RadioStatusMessage(global::InventorySystem.Items.Radio.RadioItem radio)
        {
            Range = radio.RangeLevel;
            Battery = radio.BatteryPercent;
            Owner = radio.Owner.netId;
        }
    }
}
