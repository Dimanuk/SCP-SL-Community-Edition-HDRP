using System;
using CommandSystem;
using Mirror;

namespace CommandSystem.Commands.Console
{
    public class HideTagCommand : ICommand
    {
        public string Command { get; } = "hidetag";

        public string[] Aliases { get; } = new string[] { "htag", "ht" };

        public string Description { get; } = "Hides your badge.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            ReferenceHub localHub = ReferenceHub.LocalHub;

            if (localHub != null)
            {
                response = "Hiding your tag...";

                CharacterClassManager ccm = localHub.characterClassManager;
                ccm.CmdRequestHideTag();
                return true;
            }

            response = "You must join a server to execute this command.";
            return false;
        }
    }
}