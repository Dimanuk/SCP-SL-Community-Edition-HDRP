namespace Authenticator
{
    public readonly struct AuthenticatorPlayerObjects : global::System.IEquatable<global::Authenticator.AuthenticatorPlayerObjects>, IJsonSerializable
    {
        public readonly global::System.Collections.Generic.List<global::Authenticator.AuthenticatorPlayerObject> objects;

        [global::Utf8Json.SerializationConstructor]
        public AuthenticatorPlayerObjects(global::System.Collections.Generic.List<global::Authenticator.AuthenticatorPlayerObject> objects)
        {
            this.objects = objects;
        }

        public bool Equals(global::Authenticator.AuthenticatorPlayerObjects other)
        {
            return objects == other.objects;
        }

        public override bool Equals(object obj)
        {
            if (obj is global::Authenticator.AuthenticatorPlayerObjects other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (objects == null)
            {
                return 0;
            }
            return objects.GetHashCode();
        }

        public static bool operator ==(global::Authenticator.AuthenticatorPlayerObjects left, global::Authenticator.AuthenticatorPlayerObjects right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(global::Authenticator.AuthenticatorPlayerObjects left, global::Authenticator.AuthenticatorPlayerObjects right)
        {
            return !left.Equals(right);
        }
    }
}
