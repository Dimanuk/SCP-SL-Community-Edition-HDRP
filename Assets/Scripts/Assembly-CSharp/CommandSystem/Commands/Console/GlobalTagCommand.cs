using System;
using CommandSystem;
using Mirror;

namespace CommandSystem.Commands.Console
{
    public class GlobalTagCommand : ICommand
    {
        public string Command { get; } = "globaltag";

        public string[] Aliases { get; } = new string[] { "gtag", "gtg", "gt" };

        public string Description { get; } = "Shows your global badge.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            ReferenceHub localHub = ReferenceHub.LocalHub;

            if (localHub != null)
            {
                response = "Requesting your global tag...";

                CharacterClassManager ccm = localHub.characterClassManager;
                NetworkWriterPooled writer = NetworkWriterPool.Get();
                
				ccm.CmdRequestShowTag(true);
                
                NetworkWriterPool.Return(writer);
                return true;
            }

            response = "You must join a server to execute this command.";
            return false;
        }
    }
}
