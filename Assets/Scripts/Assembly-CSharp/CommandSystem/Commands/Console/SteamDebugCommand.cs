namespace CommandSystem.Commands.Console
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class SteamDebugCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "steamdebug";

		public string[] Aliases { get; }

		public string Description { get; } = "Toggles steam debug";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			PlayerPrefsSl.Set("steam_debug", !PlayerPrefsSl.Get("steam_debug", defaultValue: false));
			response = string.Format("Steam debug: {0}", PlayerPrefsSl.Get("steam_debug", defaultValue: false));
			return true;
		}
	}
}
