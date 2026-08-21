namespace Authenticator
{
    public readonly struct AuthenticatorPlayerObject : global::System.IEquatable<global::Authenticator.AuthenticatorPlayerObject>, IJsonSerializable
    {
        public readonly string Id;

        public readonly string Ip;

        public readonly string RequestIp;

        public readonly string Asn;

        public readonly string AuthSerial;

        public readonly string VacSession;

        [global::Utf8Json.SerializationConstructor]
        public AuthenticatorPlayerObject(string Id, string Ip, string RequestIp, string Asn, string AuthSerial, string VacSession)
        {
            this.Id = Id;
            this.Ip = Ip;
            this.RequestIp = RequestIp;
            this.Asn = Asn;
            this.AuthSerial = AuthSerial;
            this.VacSession = VacSession;
        }

        public bool Equals(global::Authenticator.AuthenticatorPlayerObject other)
        {
            if (Id == other.Id && Ip == other.Ip && RequestIp == other.RequestIp && Asn == other.Asn && AuthSerial == other.AuthSerial)
            {
                return VacSession == other.VacSession;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is global::Authenticator.AuthenticatorPlayerObject other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (((((((((((Id != null) ? Id.GetHashCode() : 0) * 397) ^ ((Ip != null) ? Ip.GetHashCode() : 0)) * 397) ^ ((RequestIp != null) ? RequestIp.GetHashCode() : 0)) * 397) ^ ((Asn != null) ? Asn.GetHashCode() : 0)) * 397) ^ ((AuthSerial != null) ? AuthSerial.GetHashCode() : 0)) * 397) ^ ((VacSession != null) ? VacSession.GetHashCode() : 0);
        }

        public static bool operator ==(global::Authenticator.AuthenticatorPlayerObject left, global::Authenticator.AuthenticatorPlayerObject right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(global::Authenticator.AuthenticatorPlayerObject left, global::Authenticator.AuthenticatorPlayerObject right)
        {
            return !left.Equals(right);
        }
    }
}
