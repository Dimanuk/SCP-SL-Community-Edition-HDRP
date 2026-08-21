public readonly struct RequestSignatureResponse : global::System.IEquatable<RequestSignatureResponse>, IJsonSerializable
{
    public readonly bool success;

    public readonly string error;

    public readonly string auth;

    public readonly string badge;

    public readonly string pub;

    public readonly string nonce;

    [global::Utf8Json.SerializationConstructor]
    public RequestSignatureResponse(bool success, string error, string auth, string badge, string pub, string nonce)
    {
        this.success = success;
        this.error = error;
        this.auth = ((auth == null) ? null : global::NorthwoodLib.StringUtils.Base64Decode(auth));
        this.badge = ((badge == null) ? null : global::NorthwoodLib.StringUtils.Base64Decode(badge));
        this.pub = ((pub == null) ? null : global::NorthwoodLib.StringUtils.Base64Decode(pub));
        this.nonce = nonce;
    }

    public bool Equals(RequestSignatureResponse other)
    {
        if (success == other.success && error == other.error && auth == other.auth && badge == other.badge && pub == other.pub)
        {
            return nonce == other.nonce;
        }
        return false;
    }

    public override bool Equals(object obj)
    {
        if (obj is RequestSignatureResponse other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (((((((((success.GetHashCode() * 397) ^ ((error != null) ? error.GetHashCode() : 0)) * 397) ^ ((auth != null) ? auth.GetHashCode() : 0)) * 397) ^ ((badge != null) ? badge.GetHashCode() : 0)) * 397) ^ ((pub != null) ? pub.GetHashCode() : 0)) * 397) ^ ((nonce != null) ? nonce.GetHashCode() : 0);
    }

    public static bool operator ==(RequestSignatureResponse left, RequestSignatureResponse right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RequestSignatureResponse left, RequestSignatureResponse right)
    {
        return !left.Equals(right);
    }
}
