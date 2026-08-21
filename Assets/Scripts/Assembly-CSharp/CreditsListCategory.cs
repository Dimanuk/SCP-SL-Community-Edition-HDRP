using System;
using UnityEngine;

[Serializable]
public struct CreditsListCategory : IEquatable<CreditsListCategory>, IJsonSerializable
{
    public string category;
    public CreditsListMember[] members;

    public CreditsListCategory(string category, CreditsListMember[] members)
    {
        this.category = category;
        this.members = members;
    }

    public bool Equals(CreditsListCategory other)
    {
        if (!string.Equals(category, other.category))
            return false;
        return members == other.members;
    }

    public override bool Equals(object obj)
    {
        if (obj is CreditsListCategory other)
            return Equals(other);
        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + (category != null ? category.GetHashCode() : 0);
            hash = hash * 31 + (members != null ? members.GetHashCode() : 0);
            return hash;
        }
    }

    public static bool operator ==(CreditsListCategory left, CreditsListCategory right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CreditsListCategory left, CreditsListCategory right)
    {
        return !left.Equals(right);
    }
}