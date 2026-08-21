using System;
using System.Collections.Generic;
using Utf8Json;

public readonly struct DiscordEmbed : IEquatable<DiscordEmbed>, IJsonSerializable
{
    public readonly string title;
    public readonly string type;
    public readonly string description;
    public readonly int color;
    public readonly DiscordEmbedField[] fields;

    [SerializationConstructor]
    public DiscordEmbed(string title, string type, string description, int color, DiscordEmbedField[] fields)
    {
        this.title = title;
        this.type = type;
        this.description = description;
        this.color = color;
        this.fields = fields;
    }

    public bool Equals(DiscordEmbed other)
    {
        return string.Equals(title, other.title)
            && string.Equals(type, other.type)
            && string.Equals(description, other.description)
            && color == other.color
            && EqualityComparer<DiscordEmbedField[]>.Default.Equals(fields, other.fields);
    }

    public override bool Equals(object obj)
    {
        return obj is DiscordEmbed other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (title?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (type?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (description?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ color;
            hash = (hash * 397) ^ (fields?.GetHashCode() ?? 0);
            return hash;
        }
    }

    public static bool operator ==(DiscordEmbed left, DiscordEmbed right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DiscordEmbed left, DiscordEmbed right)
    {
        return !left.Equals(right);
    }
}