public readonly struct NewsRaw : global::System.IEquatable<NewsRaw>, IJsonSerializable
{
    public readonly NewsList appnews;

    [global::Utf8Json.SerializationConstructor]
    public NewsRaw(NewsList appnews)
    {
        this.appnews = appnews;
    }

    public bool Equals(NewsRaw other)
    {
        return appnews == other.appnews;
    }

    public override bool Equals(object obj)
    {
        if (obj is NewsRaw other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return appnews.GetHashCode();
    }

    public static bool operator ==(NewsRaw left, NewsRaw right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(NewsRaw left, NewsRaw right)
    {
        return !left.Equals(right);
    }
}
