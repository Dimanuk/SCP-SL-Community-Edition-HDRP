using CommandSystem.Commands.Shared;

namespace CommandSystem
{
    public class ClientCommandHandler : CommandHandler
    {
        private ClientCommandHandler()
        {
        }

        public static ClientCommandHandler Create()
        {
            ClientCommandHandler clientCommandHandler = new();
            clientCommandHandler.LoadGeneratedCommands();
            return clientCommandHandler;
        }

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new HelpCommand(this));
        }
    }
}
