public readonly struct QueryRaReply : global::System.IEquatable<QueryRaReply>, IJsonSerializable
{
    public readonly string Text;

    public readonly bool Success;

    public readonly bool LogToConsole;

    public readonly string OverrideDisplay;

    [global::Utf8Json.SerializationConstructor]
    public QueryRaReply(string text, bool success, bool logToConsole, string overrideDisplay)
    {
        Text = text;
        Success = success;
        LogToConsole = logToConsole;
        OverrideDisplay = overrideDisplay;
    }

    public bool Equals(QueryRaReply other)
    {
        if (string.Equals(Text, other.Text) && Success == other.Success && LogToConsole == other.LogToConsole)
        {
            return string.Equals(OverrideDisplay, other.OverrideDisplay);
        }
        return false;
    }

    public override bool Equals(object obj)
    {
        if (obj is QueryRaReply other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (((((((Text != null) ? Text.GetHashCode() : 0) * 397) ^ Success.GetHashCode()) * 397) ^ LogToConsole.GetHashCode()) * 397) ^ ((OverrideDisplay != null) ? OverrideDisplay.GetHashCode() : 0);
    }

    public static bool operator ==(QueryRaReply left, QueryRaReply right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(QueryRaReply left, QueryRaReply right)
    {
        return !left.Equals(right);
    }
}
