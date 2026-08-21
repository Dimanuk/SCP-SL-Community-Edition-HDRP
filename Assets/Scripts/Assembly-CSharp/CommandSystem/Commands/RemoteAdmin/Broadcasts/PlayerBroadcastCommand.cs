namespace CommandSystem.Commands.RemoteAdmin.Broadcasts
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class PlayerBroadcastCommand : global::CommandSystem.Commands.RemoteAdmin.Broadcasts.BroadcastCommandBase
	{
		public override string Command { get; } = "playerbroadcast";

		public override string[] Aliases { get; } = new string[1] { "pbc" };

		public override string Description { get; } = "Sends an administrative broadcast to specific player(s).";

		public override string[] Usage { get; } = new string[4] { "%player%", "Duration", "BroadcastFlag (Optional)", "Message" };

		public override bool OnExecute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			string[] newargs;
			global::System.Collections.Generic.List<ReferenceHub> list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
			if (newargs == null || newargs.Length < MinimumArguments)
			{
				response = "To execute this command provide at least 3 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string text = newargs[0];
			if (!IsValidDuration(text, out var time))
			{
				response = "Invalid argument for duration: " + text + " Usage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			Broadcast.BroadcastFlags broadcastFlag;
			bool flag = HasInputFlag(newargs[1], out broadcastFlag, newargs.Length);
			string text2 = global::Utils.RAUtils.FormatArguments(newargs.Segment(1), flag ? 1 : 0);
			global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
			Broadcast singleton = Broadcast.Singleton;
			int num = 0;
			foreach (ReferenceHub item in list)
			{
				if (num++ != 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(item.LoggedNameFromRefHub());
				singleton.TargetAddElement(item.connectionToClient, text2, time, broadcastFlag);
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, $"{sender.LogName} broadcast text \"{text2}\" to {num} players. Duration: {time} seconds. Affected players: {stringBuilder}. Broadcast Flag: {broadcastFlag}.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
			response = $"Broadcast sent to {num} players.";
			return true;
		}
	}
}
