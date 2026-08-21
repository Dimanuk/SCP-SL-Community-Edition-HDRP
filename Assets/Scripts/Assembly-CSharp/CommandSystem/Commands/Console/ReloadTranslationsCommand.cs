namespace CommandSystem.Commands.Console
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class ReloadTranslationsCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "reloadtranslations";

		public string[] Aliases { get; }

		public string Description { get; } = "Reloads game translations";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			TranslationReader.Refresh();
			response = "Translations have been reloaded! Some elements may require full level reload.";
			return true;
		}
	}
}
