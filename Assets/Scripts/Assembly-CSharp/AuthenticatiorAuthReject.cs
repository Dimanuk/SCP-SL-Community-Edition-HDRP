public readonly struct AuthenticatiorAuthReject : global::System.IEquatable<AuthenticatiorAuthReject>, IJsonSerializable
{
    public readonly string Id;

    public readonly string Reason;

    [global::Utf8Json.SerializationConstructor]
    public AuthenticatiorAuthReject(string id, string reason)
    {
        Id = id;
        Reason = reason;
    }

    public bool Equals(AuthenticatiorAuthReject other)
    {
        if (string.Equals(Id, other.Id))
        {
            return string.Equals(Reason, other.Reason);
        }
        return false;
    }

    public override bool Equals(object obj)
    {
        if (obj is AuthenticatiorAuthReject other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (((Id != null) ? Id.GetHashCode() : 0) * 397) ^ ((Reason != null) ? Reason.GetHashCode() : 0);
    }

    public static bool operator ==(AuthenticatiorAuthReject left, AuthenticatiorAuthReject right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(AuthenticatiorAuthReject left, AuthenticatiorAuthReject right)
    {
        return !left.Equals(right);
    }
}
