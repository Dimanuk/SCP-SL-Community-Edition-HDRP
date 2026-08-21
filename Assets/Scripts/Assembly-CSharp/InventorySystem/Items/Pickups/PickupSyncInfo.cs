namespace InventorySystem.Items.Pickups
{
    [global::System.Serializable]
    public struct PickupSyncInfo : global::System.IEquatable<global::InventorySystem.Items.Pickups.PickupSyncInfo>
    {
        [global::System.Flags]
        private enum PickupFlags : byte
        {
            Locked = 1,
            InUse = 2
        }

        public static global::InventorySystem.Items.Pickups.PickupSyncInfo None = new global::InventorySystem.Items.Pickups.PickupSyncInfo
        {
            ItemId = ItemType.None
        };

        public ItemType ItemId;

        public ushort Serial;

        public float Weight;

        public global::RelativePositioning.RelativePosition RelativePosition;

        public LowPrecisionQuaternion RelativeRotation;

        private global::UnityEngine.Vector3 _serverPosition;

        private global::UnityEngine.Quaternion _serverRotation;

        private global::InventorySystem.Items.Pickups.PickupSyncInfo.PickupFlags _flags;

        public global::UnityEngine.Vector3 Position
        {
            get
            {
                if (!global::Mirror.NetworkServer.active)
                {
                    return RelativePosition.Position;
                }
                return _serverPosition;
            }
        }

        public global::UnityEngine.Quaternion Rotation
        {
            get
            {
                if (!global::Mirror.NetworkServer.active)
                {
                    return global::RelativePositioning.WaypointBase.GetWorldRotation(RelativePosition.WaypointId, RelativeRotation.Value);
                }
                return _serverRotation;
            }
        }

        public byte SyncedFlags
        {
            get
            {
                return (byte)_flags;
            }
            set
            {
                _flags = (global::InventorySystem.Items.Pickups.PickupSyncInfo.PickupFlags)value;
            }
        }

        public bool Locked
        {
            get
            {
                return (_flags & global::InventorySystem.Items.Pickups.PickupSyncInfo.PickupFlags.Locked) == global::InventorySystem.Items.Pickups.PickupSyncInfo.PickupFlags.Locked;
            }
            set
            {
                _flags = (value ? (_flags | global::InventorySystem.Items.Pickups.PickupSyncInfo.PickupFlags.Locked) : (_flags & ~global::InventorySystem.Items.Pickups.PickupSyncInfo.PickupFlags.Locked));
            }
        }

        public bool InUse
        {
            get
            {
                return (_flags & global::InventorySystem.Items.Pickups.PickupSyncInfo.PickupFlags.InUse) == global::InventorySystem.Items.Pickups.PickupSyncInfo.PickupFlags.InUse;
            }
            set
            {
                _flags = (value ? (_flags | global::InventorySystem.Items.Pickups.PickupSyncInfo.PickupFlags.InUse) : (_flags & ~global::InventorySystem.Items.Pickups.PickupSyncInfo.PickupFlags.InUse));
            }
        }

        public void ServerSetPositionAndRotation(global::UnityEngine.Vector3 pos, global::UnityEngine.Quaternion rot)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                throw new global::System.InvalidOperationException("Pickup position and rotation cannot be set on client!");
            }
            _serverPosition = pos;
            _serverRotation = rot;
            RelativePosition = new global::RelativePositioning.RelativePosition(pos);
            RelativeRotation = new LowPrecisionQuaternion(global::RelativePositioning.WaypointBase.GetRelativeRotation(RelativePosition.WaypointId, rot));
        }

        public PickupSyncInfo(ItemType id, global::UnityEngine.Vector3 pos, global::UnityEngine.Quaternion rot, float weight, ushort serial = 0)
        {
            ItemId = id;
            Weight = weight;
            _flags = (global::InventorySystem.Items.Pickups.PickupSyncInfo.PickupFlags)0;
            _serverPosition = pos;
            _serverRotation = rot;
            RelativePosition = new global::RelativePositioning.RelativePosition(pos);
            RelativeRotation = new LowPrecisionQuaternion(global::RelativePositioning.WaypointBase.GetRelativeRotation(RelativePosition.WaypointId, rot));
            Serial = ((serial == 0) ? global::InventorySystem.Items.ItemSerialGenerator.GenerateNext() : serial);
        }

        public override int GetHashCode()
        {
            if (Serial == 0)
            {
                return ((((((int)ItemId * 397) ^ RelativePosition.GetHashCode()) * 397) ^ Rotation.GetHashCode()) * 397) ^ (int)_flags;
            }
            return Serial;
        }

        public static bool operator ==(global::InventorySystem.Items.Pickups.PickupSyncInfo left, global::InventorySystem.Items.Pickups.PickupSyncInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(global::InventorySystem.Items.Pickups.PickupSyncInfo left, global::InventorySystem.Items.Pickups.PickupSyncInfo right)
        {
            return !left.Equals(right);
        }

        public bool Equals(global::InventorySystem.Items.Pickups.PickupSyncInfo other)
        {
            if (ItemId == other.ItemId && Weight == other.Weight && RelativePosition == other.RelativePosition && Rotation == other.Rotation)
            {
                return _flags == other._flags;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is global::InventorySystem.Items.Pickups.PickupSyncInfo other)
            {
                return Equals(other);
            }
            return false;
        }
    }
}
