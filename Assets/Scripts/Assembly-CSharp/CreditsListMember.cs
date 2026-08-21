using System;
using UnityEngine;

[Serializable]
public struct CreditsListMember : IEquatable<CreditsListMember>, IJsonSerializable
{
    public string name;  
    public string title;  
    public string color; 

    public CreditsListMember(string name, string title, string color)
    {
        this.name = name;
        this.title = title;
        this.color = color;
    }

    public bool Equals(CreditsListMember other)
    {
        return string.Equals(name, other.name) &&
               string.Equals(title, other.title) &&
               string.Equals(color, other.color);
    }

    public override bool Equals(object obj)
    {
        if (obj is CreditsListMember other)
            return Equals(other);
        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + (name != null ? name.GetHashCode() : 0);
            hash = hash * 31 + (title != null ? title.GetHashCode() : 0);
            hash = hash * 31 + (color != null ? color.GetHashCode() : 0);
            return hash;
        }
    }

    public static bool operator ==(CreditsListMember left, CreditsListMember right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CreditsListMember left, CreditsListMember right)
    {
        return !left.Equals(right);
    }
}