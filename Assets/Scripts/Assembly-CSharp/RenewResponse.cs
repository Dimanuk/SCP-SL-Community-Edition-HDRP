public readonly struct RenewResponse : global::System.IEquatable<RenewResponse>, IJsonSerializable
{
    public readonly bool success;

    public readonly string error;

    public readonly string id;

    public readonly string nonce;

    public readonly string country;

    public readonly byte flags;

    public readonly long expiration;

    public readonly string preauth;

    public readonly string globalBan;

    public readonly ushort lifetime;

    [global::Utf8Json.SerializationConstructor]
    public RenewResponse(bool success, string error, string id, string nonce, string country, byte flags, long expiration, string preauth, string globalBan, ushort lifetime)
    {
        this.success = success;
        this.error = error;
        this.id = id;
        this.country = country;
        this.nonce = nonce;
        this.flags = flags;
        this.expiration = expiration;
        this.preauth = preauth;
        this.globalBan = globalBan;
        this.lifetime = lifetime;
    }

    public bool Equals(RenewResponse other)
    {
        if (success == other.success && error == other.error && id == other.id && nonce == other.nonce && country == other.country && flags == other.flags && expiration == other.expiration && preauth == other.preauth && globalBan == other.globalBan)
        {
            return lifetime == other.lifetime;
        }
        return false;
    }

    public override bool Equals(object obj)
    {
        if (obj is RenewResponse other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (((((((((((((((((success.GetHashCode() * 397) ^ ((error != null) ? error.GetHashCode() : 0)) * 397) ^ ((id != null) ? id.GetHashCode() : 0)) * 397) ^ ((nonce != null) ? nonce.GetHashCode() : 0)) * 397) ^ ((country != null) ? country.GetHashCode() : 0)) * 397) ^ flags.GetHashCode()) * 397) ^ expiration.GetHashCode()) * 397) ^ ((preauth != null) ? preauth.GetHashCode() : 0)) * 397) ^ ((globalBan != null) ? globalBan.GetHashCode() : 0)) * 397) ^ lifetime.GetHashCode();
    }

    public static bool operator ==(RenewResponse left, RenewResponse right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RenewResponse left, RenewResponse right)
    {
        return !left.Equals(right);
    }
}
