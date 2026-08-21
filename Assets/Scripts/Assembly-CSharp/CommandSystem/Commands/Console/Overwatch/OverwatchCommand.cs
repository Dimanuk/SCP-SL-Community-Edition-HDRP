using System;

namespace CommandSystem.Commands.Console.Overwatch
{
    public class OverwatchCommand : ParentCommand
    {
        public override string Command { get; } = "overwatch";

        public override string[] Aliases { get; } = new[] { "ovr", "ow" };

        public override string Description { get; } = "Toggle overwatch mode.";

        public static OverwatchCommand Create()
        {
            OverwatchCommand overwatchCommand = new OverwatchCommand();
            overwatchCommand.LoadGeneratedCommands();
            return overwatchCommand;
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.Overwatch, out response))
            {
                return false;
            }

            if (ReferenceHub.LocalHub == null)
            {
                response = "You must join a server to execute this command.";
                return false;
            }

            response = string.Empty;
            if (arguments.Count != 0)
            {
                response = "SYNTAX: overwatch enable/disable";
                return false;
            }

            ServerRoles serverRoles = ReferenceHub.LocalHub.serverRoles;
            serverRoles.CmdSetOverwatchStatus((byte)(serverRoles.IsInOverwatch ? 0 : 1));
            return true;
        }

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new DisableCommand());
            RegisterCommand(new EnableCommand());
        }
    }
}
