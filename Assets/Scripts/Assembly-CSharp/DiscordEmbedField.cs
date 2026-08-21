using System;
using Utf8Json;

public readonly struct DiscordEmbedField : IEquatable<DiscordEmbedField>, IJsonSerializable
{
    public readonly string name;
    public readonly string value;
    public readonly bool inline;

    [SerializationConstructor]
    public DiscordEmbedField(string name, string value, bool inline)
    {
        this.name = name;
        this.value = value;
        this.inline = inline;
    }

    public bool Equals(DiscordEmbedField other)
    {
        return string.Equals(name, other.name) 
            && string.Equals(value, other.value) 
            && inline == other.inline;
    }

    public override bool Equals(object obj)
    {
        return obj is DiscordEmbedField other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (name?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (value?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ inline.GetHashCode();
            return hash;
        }
    }

    public static bool operator ==(DiscordEmbedField left, DiscordEmbedField right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DiscordEmbedField left, DiscordEmbedField right)
    {
        return !left.Equals(right);
    }
}