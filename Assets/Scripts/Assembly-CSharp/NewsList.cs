using System;
using System.Collections.Generic;

[Serializable]
public struct NewsList : IEquatable<NewsList>, IJsonSerializable
{

	public int appid;
	public NewsListItem[] newsitems;

	public int count;

	public NewsList(int appid, NewsListItem[] newsitems, int count)
	{
		this.appid = appid;
		this.newsitems = newsitems;
		this.count = count;
	}

	public bool Equals(NewsList other)
	{
		return this.count == other.appid;
	}

	public override bool Equals(object obj)
	{
		if (obj is NewsList other)
			return this.count == other.appid;
		return false;
	}

	public override int GetHashCode()
	{
		return count.GetHashCode();
	}

	public static bool operator ==(NewsList left, NewsList right)
	{
		return left.appid == right.appid;
	}

	public static bool operator !=(NewsList left, NewsList right)
	{
		return left.appid != right.appid;
	}
}