using System;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class KeyhashCommand : ICommand
    {
        public string Command { get; } = "keyhash";

        public string[] Aliases { get; } = new[] { "khash", "kh" };

        public string Description { get; } = "Displays the key hash of central server.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string keyString = Cryptography.ECDSA.KeyToString(GameCore.Console._publicKey);
            response = "SHA256 hash of Central Server Public Key: " + Cryptography.Sha.HashToString(Cryptography.Sha.Sha256(keyString));
            return true;
        }
    }
}
