namespace CommandSystem.Commands
{
    [global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
    public class CassieClear : global::CommandSystem.ICommand
    {
        public string Command { get; } = "clearcassie";

        public string[] Aliases { get; } = new string[1] { "cassieclear" };

        public string Description { get; } = "Clears cassie message queue.";

        public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.Announcer, out response))
            {
                return false;
            }
            ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " ran the clearcassie command", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
            global::Respawning.RespawnEffectsController.ClearQueue();
            response = "Cleared Cassie word queue!";
            return true;
        }
    }
}
