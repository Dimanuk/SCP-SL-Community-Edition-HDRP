using System;
using Mirror;
using PlayerStatsSystem;
using UnityEngine;

namespace PlayerRoles.Ragdolls
{
    [Serializable]
    public struct RagdollData : NetworkMessage, IEquatable<RagdollData>
    {
        public readonly RoleTypeId RoleType;

        public readonly string Nickname;

        public readonly DamageHandlerBase Handler;

        public readonly Vector3 StartPosition;

        public readonly Quaternion StartRotation;

        public readonly double CreationTime;

        public readonly ReferenceHub OwnerHub;

        public float ExistenceTime => (float)(NetworkTime.time - CreationTime);

        public RagdollData(ReferenceHub hub, DamageHandlerBase handler, Vector3 positionOffset, Quaternion rotationOffset)
        {
            OwnerHub = hub;
            RoleType = hub.GetRoleId();
            Transform transform = hub.transform;
            StartPosition = transform.position + positionOffset;
            StartRotation = transform.rotation * rotationOffset;
            Nickname = hub.nicknameSync.DisplayName;
            Handler = handler;
            CreationTime = NetworkTime.time;
        }

        public RagdollData(ReferenceHub hub, DamageHandlerBase handler, RoleTypeId roleType, Vector3 position, Quaternion rotation, string nick, double creationTime)
        {
            OwnerHub = hub;
            RoleType = roleType;
            StartPosition = position;
            StartRotation = rotation;
            Nickname = nick;
            Handler = handler;
            CreationTime = creationTime;
        }

        public bool Equals(RagdollData other)
        {
            if (RoleType == other.RoleType && string.Equals(Nickname, other.Nickname))
            {
                return Handler == other.Handler;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is RagdollData other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int num = (((((byte)RoleType * 397) ^ Nickname.GetHashCode()) * 397) ^ Handler.GetHashCode()) * 397;
            double creationTime = CreationTime;
            return num ^ creationTime.GetHashCode();
        }

        public static bool operator ==(RagdollData left, RagdollData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RagdollData left, RagdollData right)
        {
            return !left.Equals(right);
        }
    }
}
