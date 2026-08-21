namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class ChangeColorCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "changecolor";

		public string[] Aliases { get; } = new string[2] { "changec", "ccolor" };

		public string Description { get; } = "Changes the color of the lights in the room you are currently in.";

		public string[] Usage { get; } = new string[3] { "r", "g", "b" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			if (!(sender is global::RemoteAdmin.PlayerCommandSender playerCommandSender))
			{
				response = "You must be in-game to use this command!";
				return false;
			}
			if (!global::PlayerRoles.PlayerRolesUtils.IsAlive(playerCommandSender.ReferenceHub))
			{
				response = "You need to be alive to run this command!";
				return false;
			}
			global::MapGeneration.RoomIdentifier roomIdentifier = global::MapGeneration.RoomIdUtils.RoomAtPosition(playerCommandSender.ReferenceHub.transform.position);
			FlickerableLightController flickerableLightController = ((roomIdentifier != null) ? roomIdentifier.GetComponentInChildren<FlickerableLightController>() : null);
			if (flickerableLightController == null)
			{
				response = "You are not in a room that supports changing lights color!";
				return false;
			}
			if (arguments.Count == 0)
			{
				flickerableLightController.WarheadLightColor = FlickerableLightController.DefaultWarheadColor;
				flickerableLightController.WarheadLightOverride = false;
				response = "Done! Reset warhead lights to default color.";
				return true;
			}
			if (arguments.Count < 3)
			{
				response = "Type 3 numbers, eg: 255 255 255";
				return false;
			}
			if (!float.TryParse(arguments.At(0), out var result) || !float.TryParse(arguments.At(1), out var result2) || !float.TryParse(arguments.At(2), out var result3))
			{
				response = "Invalid input. Type 3 numbers, eg: 255 255 255";
				return false;
			}
			global::UnityEngine.Color warheadLightColor = new global::UnityEngine.Color(result / 255f, result2 / 255f, result3 / 255f);
			flickerableLightController.WarheadLightColor = warheadLightColor;
			flickerableLightController.WarheadLightOverride = true;
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, $"{sender.LogName} changed color of lights in a room to {result} {result2} {result3} .", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = "Done!";
			return true;
		}
	}
}
