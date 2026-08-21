using System;
using System.Linq;
using System.Text;

namespace CommandSystem.Commands.Console.Central
{
    public class CentralCommand : ParentCommand
    {
        public override string Command { get; } = "central";

        public override string[] Aliases { get; } = new[] { "cs", "csrv" };

        public override string Description { get; } = "Display a list of central servers";

        public static CentralCommand Create()
        {
            CentralCommand centralCommand = new CentralCommand();
            centralCommand.LoadGeneratedCommands();
            return centralCommand;
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Master central server: " + CentralServer.MasterUrl);
            stringBuilder.AppendLine("Selected central server: " + CentralServer.SelectedServer + " (" + CentralServer.StandardUrl + ")");
            stringBuilder.AppendLine("All central servers: " + string.Join(", ", CentralServer.Servers));
            response = stringBuilder.ToString();
            return true;
        }

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new ChangeCommand());
            RegisterCommand(new ForceChangeCommand());
            RegisterCommand(new RandomCommand());
            RegisterCommand(new TestCommand());
        }
    }
}
