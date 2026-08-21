using System;


public class ServerConsoleSender : CommandSender
{
	public override string SenderId => "SERVER CONSOLE";

	public override string Nickname => "SERVER CONSOLE";

	public override ulong Permissions => ServerStatic.PermissionsHandler.FullPerm;

	public override byte KickPower => (byte)byte.MaxValue;

	public override bool FullPermissions => true;

	public override void RaReply(string text, bool success, bool logToConsole, string overrideDisplay)
	{
		if (logToConsole)
		{
			ServerConsole.AddLog(text, success ? ConsoleColor.Gray : ConsoleColor.Red);
		}
	}

	public override void Print(string text)
	{
		ServerConsole.AddLog(text);
	}

	public override void Print(string text, ConsoleColor c)
	{
		ServerConsole.AddLog(text, c);
	}
}
