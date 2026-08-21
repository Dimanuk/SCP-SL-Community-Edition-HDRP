using System;
using System.Collections.Generic;

namespace InventorySystem
{
    public readonly struct InventoryRoleInfo : IEquatable<InventoryRoleInfo>
    {
        public readonly ItemType[] Items;
        public readonly Dictionary<ItemType, ushort> Ammo;

        public InventoryRoleInfo(ItemType[] items, Dictionary<ItemType, ushort> ammo)
        {
            Items = items != null ? (ItemType[])items.Clone() : Array.Empty<ItemType>();
            Ammo = ammo != null ? new Dictionary<ItemType, ushort>(ammo) : new Dictionary<ItemType, ushort>();
        }

        public bool Equals(InventoryRoleInfo other)
        {
            if (ReferenceEquals(Items, other.Items) && ReferenceEquals(Ammo, other.Ammo))
                return true;

            if (!AreItemsEqual(Items, other.Items))
                return false;

            if (!AreAmmoEqual(Ammo, other.Ammo))
                return false;

            return true;
        }

        public override bool Equals(object obj) => obj is InventoryRoleInfo other && Equals(other);

        public override int GetHashCode()
        {
            int hash = 17;

            if (Items != null)
            {
                foreach (ItemType item in Items)
                    hash = hash * 31 + (int)item;
            }

            if (Ammo != null)
            {
                foreach (KeyValuePair<ItemType, ushort> kvp in Ammo)
                    hash = hash * 31 + ((int)kvp.Key ^ kvp.Value.GetHashCode());
            }

            return hash;
        }

        public void Deconstruct(out ItemType[] items, out Dictionary<ItemType, ushort> ammo)
        {
            items = Items;
            ammo = Ammo;
        }

        public static bool operator ==(InventoryRoleInfo left, InventoryRoleInfo right) => left.Equals(right);
        public static bool operator !=(InventoryRoleInfo left, InventoryRoleInfo right) => !left.Equals(right);

        private static bool AreItemsEqual(ItemType[] a, ItemType[] b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a == null || b == null)
                return false;
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }

        private static bool AreAmmoEqual(Dictionary<ItemType, ushort> a, Dictionary<ItemType, ushort> b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a == null || b == null)
                return false;
            if (a.Count != b.Count)
                return false;

            foreach (KeyValuePair<ItemType, ushort> kvp in a)
            {
                if (!b.TryGetValue(kvp.Key, out ushort otherValue) || otherValue != kvp.Value)
                    return false;
            }

            return true;
        }
    }
}