namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class DestroyToyCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "destroytoy";

		public string[] Aliases { get; }

		public string Description { get; } = "Despawns a toy placed by an admin.";

		public string[] Usage { get; } = new string[1] { "NetID of toy to remove." };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 1 || !uint.TryParse(arguments.Array[1], out var result))
			{
				response = "Failed to parse NetID of the toy to destroy.";
				return false;
			}
			if (!global::Mirror.NetworkServer.spawned.TryGetValue(result, out var value) || !value.TryGetComponent<global::AdminToys.AdminToyBase>(out var component))
			{
				response = $"{result} is not a valid toy NetID.";
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, $"{sender.LogName} removed admin toy: {component.CommandName} ({component.netId}).", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = $"Toy {result} successfully removed.";
			global::Mirror.NetworkServer.Destroy(component.gameObject);
			return true;
		}
	}
}
