namespace Discord
{
	public struct InputMode
	{
		public global::Discord.InputModeType Type;

		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 256)]
		public string Shortcut;
	}
}
