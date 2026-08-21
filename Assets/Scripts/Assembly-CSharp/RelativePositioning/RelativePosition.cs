using Mirror;
using Utils.Networking;

namespace RelativePositioning
{
    public struct RelativePosition : global::Mirror.NetworkMessage, global::System.IEquatable<global::RelativePositioning.RelativePosition>
    {
        private static readonly float InverseAccuracy = 0.00390625f;

        public const float Accuracy = 256f;

        public readonly short PositionX;

        public readonly short PositionY;

        public readonly short PositionZ;

        public readonly byte WaypointId;

        public global::UnityEngine.Vector3 Position => global::RelativePositioning.WaypointBase.GetWorldPosition(WaypointId, Relative);

        internal global::UnityEngine.Vector3 Relative => new global::UnityEngine.Vector3((float)PositionX * InverseAccuracy, (float)PositionY * InverseAccuracy, (float)PositionZ * InverseAccuracy);

        public RelativePosition(global::UnityEngine.Vector3 targetPos)
        {
            global::RelativePositioning.WaypointBase.GetRelativePosition(targetPos, out WaypointId, out var rel);
            PositionX = CompressPosition(rel.x);
            PositionY = CompressPosition(rel.y);
            PositionZ = CompressPosition(rel.z);
        }

        public RelativePosition(ReferenceHub hub)
            : this((hub.roleManager.CurrentRole is global::PlayerRoles.FirstPersonControl.IFpcRole fpcRole) ? fpcRole.FpcModule.Position : hub.transform.position)
        {
        }

        public RelativePosition(global::Mirror.NetworkReader reader)
        {
            WaypointId = reader.ReadByte();
            if (WaypointId > 0)
            {
                PositionX = reader.ReadShort();
                PositionY = reader.ReadShort();
                PositionZ = reader.ReadShort();
            }
            else
            {
                PositionX = 0;
                PositionY = 0;
                PositionZ = 0;
            }
        }

        public RelativePosition(byte waypoint, short x, short y, short z)
        {
            WaypointId = waypoint;
            PositionX = x;
            PositionY = y;
            PositionZ = z;
        }

        public void Write(global::Mirror.NetworkWriter writer)
        {
            writer.WriteByte(WaypointId);
            if (WaypointId > 0)
            {
                writer.WriteShort(PositionX);
                writer.WriteShort(PositionY);
                writer.WriteShort(PositionZ);
            }
        }

        private static short CompressPosition(float pos)
        {
            float num = pos * 256f;
            if (num < -32768f)
            {
                return short.MinValue;
            }
            if (num > 32767f)
            {
                return short.MaxValue;
            }
            return (short)num;
        }

        public bool Equals(global::RelativePositioning.RelativePosition other)
        {
            if (PositionX == other.PositionX && PositionY == other.PositionY && PositionZ == other.PositionZ)
            {
                return WaypointId == other.WaypointId;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is global::RelativePositioning.RelativePosition relativePosition)
            {
                return relativePosition.Equals(this);
            }
            return false;
        }

        public static bool operator ==(global::RelativePositioning.RelativePosition left, global::RelativePositioning.RelativePosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(global::RelativePositioning.RelativePosition left, global::RelativePositioning.RelativePosition right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return (((((WaypointId * 397) ^ PositionX) * 397) ^ PositionZ) * 397) ^ PositionY;
        }
    }
}
