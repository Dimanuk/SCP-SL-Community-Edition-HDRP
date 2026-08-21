namespace Discord
{
	public struct ImageHandle
	{
		public global::Discord.ImageType Type;

		public long Id;

		public uint Size;

		public static global::Discord.ImageHandle User(long id)
		{
			return User(id, 128u);
		}

		public static global::Discord.ImageHandle User(long id, uint size)
		{
			return new global::Discord.ImageHandle
			{
				Type = global::Discord.ImageType.User,
				Id = id,
				Size = size
			};
		}
	}
}
