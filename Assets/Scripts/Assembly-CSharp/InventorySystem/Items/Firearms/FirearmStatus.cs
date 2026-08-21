namespace InventorySystem.Items.Firearms
{
    public readonly struct FirearmStatus : global::System.IEquatable<global::InventorySystem.Items.Firearms.FirearmStatus>
    {
        public readonly byte Ammo;

        public readonly global::InventorySystem.Items.Firearms.FirearmStatusFlags Flags;

        public readonly uint Attachments;

        public FirearmStatus(byte ammo, global::InventorySystem.Items.Firearms.FirearmStatusFlags flags, uint attachments)
        {
            Ammo = ammo;
            Flags = flags;
            Attachments = attachments;
        }

        public override int GetHashCode()
        {
            return (int)(((uint)(Ammo * 397) ^ (Attachments - int.MaxValue)) * 397) ^ (int)Flags;
        }

        public static bool operator ==(global::InventorySystem.Items.Firearms.FirearmStatus left, global::InventorySystem.Items.Firearms.FirearmStatus right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(global::InventorySystem.Items.Firearms.FirearmStatus left, global::InventorySystem.Items.Firearms.FirearmStatus right)
        {
            return !left.Equals(right);
        }

        public bool Equals(global::InventorySystem.Items.Firearms.FirearmStatus other)
        {
            if (Ammo == other.Ammo && Flags == other.Flags)
            {
                return Attachments == other.Attachments;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is global::InventorySystem.Items.Firearms.FirearmStatus other)
            {
                return Equals(other);
            }
            return false;
        }

        public override string ToString()
        {
            bool flag = true;
            global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
            stringBuilder.Append("Ammo: (");
            stringBuilder.Append(Ammo);
            stringBuilder.Append(") Attachments: (");
            stringBuilder.Append(Attachments);
            stringBuilder.Append(") Flags: (");
            if (Flags == global::InventorySystem.Items.Firearms.FirearmStatusFlags.None)
            {
                stringBuilder.Append("none");
            }
            foreach (global::InventorySystem.Items.Firearms.FirearmStatusFlags value in global::System.Enum.GetValues(typeof(global::InventorySystem.Items.Firearms.FirearmStatusFlags)))
            {
                if ((Flags & value) == value)
                {
                    if (flag)
                    {
                        flag = false;
                    }
                    else
                    {
                        stringBuilder.Append(" | ");
                    }
                    stringBuilder.Append(value);
                }
            }
            stringBuilder.Append(")");
            string result = stringBuilder.ToString();
            global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
            return result;
        }
    }
}
