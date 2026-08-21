namespace Discord
{
	public struct ActivityParty
	{
		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 128)]
		public string Id;

		public global::Discord.PartySize Size;
	}
}
