namespace Discord
{
	public struct SkuPrice
	{
		public uint Amount;

		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 16)]
		public string Currency;
	}
}
