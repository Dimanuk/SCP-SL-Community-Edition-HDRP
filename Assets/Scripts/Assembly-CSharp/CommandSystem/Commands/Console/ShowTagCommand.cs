using System;


namespace CommandSystem.Commands.Console
{
	public class ShowTagCommand : ICommand
	{
		public string Command { get; } = "showtag";

		public string[] Aliases { get; } = new string[] { "tag", "stag", "st" };

		public string Description { get; } = "Shows your badge.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			ReferenceHub localHub = ReferenceHub.LocalHub;

            if (localHub != null)
            {
                response = "Requesting your local tag...";

                CharacterClassManager ccm = localHub.characterClassManager;
                ccm.CmdRequestShowTag(false);
                return true;
            }

            response = "You must join a server to execute this command.";
            return false;
		}
	}
}
