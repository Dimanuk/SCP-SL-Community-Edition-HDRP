namespace Discord
{
	public struct FileStat
	{
		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 260)]
		public string Filename;

		public ulong Size;

		public ulong LastModified;
	}
}
