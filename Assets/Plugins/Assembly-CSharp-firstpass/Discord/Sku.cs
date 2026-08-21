namespace Discord
{
	public struct Sku
	{
		public long Id;

		public global::Discord.SkuType Type;

		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 256)]
		public string Name;

		public global::Discord.SkuPrice Price;
	}
}
