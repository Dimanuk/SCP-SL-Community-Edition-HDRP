namespace CommandSystem.Commands.RemoteAdmin.Cleanup
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.Cleanup.CleanupCommand))]
	public class CorpsesCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "corpses";

		public string[] Aliases { get; } = new string[6] { "corpse", "ragdolls", "ragdoll", "r", "c", "0" };

		public string Description { get; } = "Cleans up ragdolls from the map.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			BasicRagdoll[] array = global::System.Linq.Enumerable.ToArray(global::System.Linq.Enumerable.OrderByDescending(global::UnityEngine.Object.FindObjectsOfType<BasicRagdoll>(), (BasicRagdoll r) => r.Info.CreationTime));
			int num = array.Length;
			if (arguments.Count > 0 && int.TryParse(arguments.At(0), out var result) && result < array.Length)
			{
				num = result;
			}
			for (int num2 = 0; num2 < num; num2++)
			{
				global::Mirror.NetworkServer.Destroy(array[num2].gameObject);
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, $"{sender.LogName} has force-cleaned up {num} ragdolls.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = $"{num} ragdolls have been deleted.";
			return true;
		}
	}
}
