namespace CommandSystem.Commands.Shared
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.ClientCommandHandler))]
	public class HelpCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		private readonly global::System.Text.StringBuilder _helpBuilder = new global::System.Text.StringBuilder();

		private readonly global::CommandSystem.ICommandHandler _commandHandler;

		public string Command { get; } = "help";

		public string[] Aliases { get; }

		public string Description { get; } = "Returns all commands with their descriptions and aliases or displays help for specified command.";

		public string[] Usage { get; } = new string[1] { "Command (Optional)" };

		public HelpCommand(global::CommandSystem.ICommandHandler commandHandler)
		{
			_commandHandler = commandHandler;
		}

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (arguments.Count == 0)
			{
				response = GetCommandList(_commandHandler, "Command list:");
				return true;
			}
			if (_commandHandler.TryGetCommand(arguments.At(0), out var command))
			{
				string text = command.Command;
				global::System.ArraySegment<string> arraySegment = arguments.Segment(1);
				global::CommandSystem.ICommand command2;
				while (arraySegment.Count != 0 && command is global::CommandSystem.ICommandHandler commandHandler && commandHandler.TryGetCommand(arraySegment.At(0), out command2))
				{
					arraySegment = arraySegment.Segment(1);
					command = command2;
					text = text + " " + command2.Command;
				}
				response = text + " - " + ((command is global::CommandSystem.IHelpProvider helpProvider) ? helpProvider.GetHelp(arraySegment) : command.Description);
				if (command.Aliases != null && command.Aliases.Length != 0)
				{
					response = response + "\nAliases: " + string.Join(", ", command.Aliases);
				}
				if (command is global::CommandSystem.ICommandHandler handler)
				{
					response += GetCommandList(handler, "\nSubcommand list:");
				}
				try
				{
					global::System.Type type = command.GetType();
					if (type != null)
					{
						response = response + "\nImplemented in: " + type.Assembly.GetName().Name + ":" + type.FullName;
					}
				}
				catch
				{
				}
				return true;
			}
			response = "Help for " + arguments.At(0) + " isn't available!";
			return false;
		}

		private string GetCommandList(global::CommandSystem.ICommandHandler handler, string header)
		{
			_helpBuilder.Clear();
			_helpBuilder.Append(header);
			foreach (global::CommandSystem.ICommand allCommand in handler.AllCommands)
			{
				if (!(allCommand is global::CommandSystem.IHiddenCommand))
				{
					_helpBuilder.AppendLine();
					_helpBuilder.Append(allCommand.Command);
					_helpBuilder.Append(" - ");
					_helpBuilder.Append(allCommand.Description);
					if (allCommand.Aliases != null && allCommand.Aliases.Length != 0)
					{
						_helpBuilder.Append(" - Aliases: ");
						_helpBuilder.Append(string.Join(", ", allCommand.Aliases));
					}
				}
			}
			return _helpBuilder.ToString();
		}
	}
}
