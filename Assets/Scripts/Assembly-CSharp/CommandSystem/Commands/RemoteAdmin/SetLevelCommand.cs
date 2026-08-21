namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class SetLevelCommand : global::CommandSystem.Commands.RemoteAdmin.Scp079CommandBase
	{
		public override string Command { get; } = "setlevel";

		public override string[] Aliases { get; } = new string[3] { "settier", "level", "lvl" };

		public override string Description { get; } = "Sets the level of the player playing as SCP-079.";

		public override string[] Usage { get; } = new string[2] { "%player%", "New Level" };

		public override bool OnExecute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, int input, out string response)
		{
			string[] newargs;
			global::System.Collections.Generic.List<ReferenceHub> list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
			global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
			int input2 = 0;
			int num = 0;
			bool flag = input-- <= 1;
			input--;
			foreach (ReferenceHub item in list)
			{
				if (item.roleManager.CurrentRole is global::PlayerRoles.PlayableScps.Scp079.Scp079Role scp079Role && scp079Role.SubroutineModule.TryGetSubroutine<global::PlayerRoles.PlayableScps.Scp079.Scp079TierManager>(out var subroutine))
				{
					if (!flag)
					{
						input2 = subroutine.AbsoluteThresholds[global::UnityEngine.Mathf.Clamp(input, 0, subroutine.AbsoluteThresholds.Length - 1)];
					}
					ApplyChanges(subroutine, input2);
					num++;
				}
			}
			if (num > 0)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} used \"{1} ({2})\" command on player{3}{4}.", sender.LogName, Command, input, (num == 1) ? " " : "s ", global::NorthwoodLib.Pools.StringBuilderPool.Shared.ToStringReturn(stringBuilder)), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			}
			global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}

		public override void ApplyChanges(global::PlayerRoles.PlayableScps.Scp079.Scp079TierManager manager, int exp)
		{
			manager.TotalExp = exp;
		}
	}
}
