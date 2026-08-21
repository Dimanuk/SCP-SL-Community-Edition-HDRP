namespace CommandSystem.Commands.RemoteAdmin.Cleanup
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.Cleanup.CleanupCommand))]
	public class ItemsCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "items";

		public string[] Aliases { get; } = new string[3] { "item", "i", "1" };

		public string Description { get; } = "Cleans up items from the map.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			global::InventorySystem.Items.Pickups.ItemPickupBase[] array = global::UnityEngine.Object.FindObjectsOfType<global::InventorySystem.Items.Pickups.ItemPickupBase>();
			int num = array.Length;
			if (arguments.Count > 0 && int.TryParse(arguments.At(0), out var result) && result < array.Length)
			{
				num = result;
			}
			for (int i = 0; i < num; i++)
			{
				global::Mirror.NetworkServer.Destroy(array[i].gameObject);
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, $"{sender.LogName} has force-cleaned up {num} items.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = $"{num} items have been deleted.";
			return true;
		}
	}
}
