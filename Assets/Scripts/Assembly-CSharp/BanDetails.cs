public class BanDetails
{
    public string OriginalName;

    public string Id;

    public long Expires;

    public string Reason;

    public string Issuer;

    public long IssuanceTime;

    public override string ToString()
    {
        return OriginalName.Replace(";", ":") + ";" + Id.Replace(";", ":") + ";" + global::System.Convert.ToString(Expires) + ";" + Reason.Replace(";", ":") + ";" + Issuer.Replace(";", ":") + ";" + global::System.Convert.ToString(IssuanceTime);
    }
}
