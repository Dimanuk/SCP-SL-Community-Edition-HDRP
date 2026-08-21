using Mirror;
using System;

namespace InventorySystem.Items.Firearms.BasicMessages
{
    public struct StatusMessage : NetworkMessage, IEquatable<StatusMessage>
    {
        public ushort Serial;

        public FirearmStatus Status;

        public StatusMessage(ushort serial, FirearmStatus status)
        {
            Serial = serial;
            Status = status;
        }

        public void Deserialize(global::Mirror.NetworkReader reader)
        {
            byte ammo = reader.ReadByte();
            byte flags = reader.ReadByte();
            uint attachments = reader.ReadUInt();
            Serial = reader.ReadUShort();
            Status = new FirearmStatus(ammo, (FirearmStatusFlags)flags, attachments);
        }

        public void Serialize(global::Mirror.NetworkWriter writer)
        {
            writer.WriteByte(Status.Ammo);
            writer.WriteByte((byte)Status.Flags);
            writer.WriteUInt(Status.Attachments);
            writer.WriteUShort(Serial);
        }

        public override int GetHashCode()
        {
            return (Status.GetHashCode() * 397) ^ Serial;
        }

        public static bool operator ==(global::InventorySystem.Items.Firearms.BasicMessages.StatusMessage left, global::InventorySystem.Items.Firearms.BasicMessages.StatusMessage right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(global::InventorySystem.Items.Firearms.BasicMessages.StatusMessage left, global::InventorySystem.Items.Firearms.BasicMessages.StatusMessage right)
        {
            return !left.Equals(right);
        }

        public bool Equals(global::InventorySystem.Items.Firearms.BasicMessages.StatusMessage other)
        {
            if (Status == other.Status)
            {
                return Serial == other.Serial;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is global::InventorySystem.Items.Firearms.BasicMessages.StatusMessage other)
            {
                return Equals(other);
            }
            return false;
        }
    }
}
