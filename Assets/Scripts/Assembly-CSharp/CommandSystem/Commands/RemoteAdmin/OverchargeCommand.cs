namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class OverchargeCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "overcharge";

		public string[] Aliases { get; } = new string[3] { "ocharge", "flicker", "blackout" };

		public string Description { get; } = "Turns lights off in heavy and optionally heavy and light.";

		public string[] Usage { get; } = new string[2] { "Zone ID", "Duration" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To use this, type at least 1 argument(s)! (some parameters are missing)\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			bool flag = arguments.Count >= 2;
			global::MapGeneration.FacilityZone result = global::MapGeneration.FacilityZone.None;
			if (!float.TryParse(arguments.At(flag ? 1 : 0), out var result2))
			{
				response = "Specified duration is invalid.";
				return false;
			}
			if (flag && !global::System.Enum.TryParse<global::MapGeneration.FacilityZone>(arguments.At(0), ignoreCase: true, out result))
			{
				response = "Specified zone is invalid.";
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} has turned off lights in {1} for {2} seconds.", sender.LogName, (result == global::MapGeneration.FacilityZone.None) ? "the facility" : result.ToString(), result2), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			Overcharge(result, result2, out response);
			return true;
		}

		private void Overcharge(global::MapGeneration.FacilityZone zoneToAffect, float duration, out string commandResponse)
		{
			bool flag = zoneToAffect != global::MapGeneration.FacilityZone.None;
			foreach (FlickerableLightController instance in FlickerableLightController.Instances)
			{
				if (!flag || instance.Room.Zone == zoneToAffect)
				{
					instance.ServerFlickerLights(duration);
				}
			}
			commandResponse = ((zoneToAffect == global::MapGeneration.FacilityZone.None) ? $"Turned off lights in the facility for {duration} seconds." : $"Turned off lights in {zoneToAffect} for {duration} seconds.");
		}
	}
}
