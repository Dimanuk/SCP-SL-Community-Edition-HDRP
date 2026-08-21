using System;
using System.Runtime.InteropServices;

public static class WindowsMessageBox
{
	[Flags]
	public enum MessageBoxFlags : uint
	{
		Info = 0x40u,
		Question = 0x20u,
		Warning = 0x30u,
		Error = 0x10u
	}

	private const string MessageBoxHeader = "SCP: Secret Laboratory";

	[PreserveSig]
	private static extern int MessageBox(IntPtr h, string m, string c, MessageBoxFlags type);

	public static void Show(string text, MessageBoxFlags flags, string header = "SCP: Secret Laboratory")
	{
		int num = 0;
	}
}
