namespace CommandSystem.Commands.Console
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class LennyCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "lenny";

		public string[] Aliases { get; }

		public string Description { get; } = "( \u0361° \u035cʖ \u0361°)";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			response = "<size=200>( \u0361° \u035cʖ \u0361°)</size>";
			return true;
		}
	}
}
