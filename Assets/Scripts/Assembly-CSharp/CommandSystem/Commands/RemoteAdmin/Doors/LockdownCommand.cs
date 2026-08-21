namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class LockdownCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		private global::MEC.CoroutineHandle _lockdownHandle;

		public string Command { get; } = "lockdown";

		public string[] Aliases { get; } = new string[1] { "ld" };

		public string Description { get; } = "Locks all the doors in the facility.";

		public string[] Usage { get; } = new string[2] { "ZoneID Filter", "Duration (Optional)" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			global::MapGeneration.FacilityZone result = global::MapGeneration.FacilityZone.None;
			float result2 = 0f;
			if (arguments.Count != 0 && !global::System.Enum.TryParse<global::MapGeneration.FacilityZone>(arguments.At(0), ignoreCase: true, out result))
			{
				response = "Specified zone is invalid.";
				return false;
			}
			if (arguments.Count > 1 && !float.TryParse(arguments.At(1), out result2))
			{
				response = "Specified duration is invalid.";
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, $"{sender.LogName} has locked down the facility (Filter: {result}){((result2 == 0f) ? string.Empty : $"for {result2} seconds.")}", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			Lockdown(result, result2, result2 != -1f, out response);
			return true;
		}

		private void Lockdown(global::MapGeneration.FacilityZone zoneToAffect, float duration, bool doLock, out string commandResponse)
		{
			if (RoundSummary.SummaryActive)
			{
				commandResponse = string.Empty;
				return;
			}
			bool flag = zoneToAffect != global::MapGeneration.FacilityZone.None;
			foreach (global::Interactables.Interobjects.DoorUtils.DoorVariant allDoor in global::Interactables.Interobjects.DoorUtils.DoorVariant.AllDoors)
			{
				if (!flag || global::Interactables.Interobjects.DoorUtils.DoorVariantUtils.IsInZone(allDoor, zoneToAffect))
				{
					allDoor.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.AdminCommand, doLock);
				}
			}
			global::MEC.Timing.KillCoroutines(_lockdownHandle);
			if (duration != 0f)
			{
				_lockdownHandle = global::MEC.Timing.CallDelayed(duration, delegate
				{
					Lockdown(zoneToAffect, 0f, doLock: false, out var _);
				});
			}
			commandResponse = ((zoneToAffect == global::MapGeneration.FacilityZone.None) ? ("Locked down the facility" + ((duration == 0f) ? "." : $" for {duration} seconds.")) : string.Format("Locked down {0}{1}", zoneToAffect, (duration == 0f) ? "." : $" for {duration} seconds."));
		}
	}
}
