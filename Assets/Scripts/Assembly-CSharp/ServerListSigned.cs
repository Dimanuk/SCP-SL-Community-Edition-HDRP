using System;

[Serializable]
public struct ServerListSigned : IEquatable<ServerListSigned>, IJsonSerializable
{
    public string payload;   
    public long timestamp;    
    public string signature;  
    public string nonce;      
    public string error;      

    public ServerListSigned(string payload, long timestamp, string signature, string nonce, string error)
    {
        this.payload = payload;
        this.timestamp = timestamp;
        this.signature = signature;
        this.nonce = nonce;
        this.error = error;
    }


    public override bool Equals(object obj)
    {
        return obj is ServerListSigned other && Equals(other);
    }

    public bool Equals(ServerListSigned other)
    {
        return payload == other.payload
            && timestamp == other.timestamp
            && signature == other.signature
            && nonce == other.nonce
            && error == other.error;
    }

    public override int GetHashCode()
    {
        const int multiplier = 397;
        int hash = 17;

        hash = hash * multiplier + (payload != null ? payload.GetHashCode() : 0);
        hash = hash * multiplier + timestamp.GetHashCode();
        hash = hash * multiplier + (signature != null ? signature.GetHashCode() : 0);
        hash = hash * multiplier + (nonce != null ? nonce.GetHashCode() : 0);
        hash = hash * multiplier + (error != null ? error.GetHashCode() : 0);

        return hash;
    }

    public static bool operator ==(ServerListSigned left, ServerListSigned right) => left.Equals(right);
    public static bool operator !=(ServerListSigned left, ServerListSigned right) => !left.Equals(right);


    public void FromJson(string json) {}
    public string ToJson() => string.Empty;
}