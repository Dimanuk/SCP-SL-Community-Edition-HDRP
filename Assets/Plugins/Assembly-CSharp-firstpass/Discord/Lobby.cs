namespace Discord
{
	public struct Lobby
	{
		public long Id;

		public global::Discord.LobbyType Type;

		public long OwnerId;

		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 128)]
		public string Secret;

		public uint Capacity;

		public bool Locked;
	}
}
