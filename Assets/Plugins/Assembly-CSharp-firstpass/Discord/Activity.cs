namespace Discord
{
	public struct Activity
	{
		public global::Discord.ActivityType Type;

		public long ApplicationId;

		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 128)]
		public string Name;

		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 128)]
		public string State;

		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 128)]
		public string Details;

		public global::Discord.ActivityTimestamps Timestamps;

		public global::Discord.ActivityAssets Assets;

		public global::Discord.ActivityParty Party;

		public global::Discord.ActivitySecrets Secrets;

		public bool Instance;
	}
}
