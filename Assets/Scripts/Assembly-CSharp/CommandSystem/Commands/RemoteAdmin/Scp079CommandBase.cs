namespace CommandSystem.Commands.RemoteAdmin
{
	public abstract class Scp079CommandBase : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public abstract string Command { get; }

		public abstract string[] Aliases { get; }

		public abstract string Description { get; }

		public abstract string[] Usage { get; }

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 2)
			{
				response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			if (!uint.TryParse(arguments.At(arguments.Count - 1), out var result))
			{
				response = "Value argument must be a valid number.\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			return OnExecute(arguments, sender, (int)result, out response);
		}

		public virtual bool OnExecute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, int input, out string response)
		{
			string[] newargs;
			global::System.Collections.Generic.List<ReferenceHub> list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
			global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
			int num = 0;
			foreach (ReferenceHub item in list)
			{
				if (item.roleManager.CurrentRole is global::PlayerRoles.PlayableScps.Scp079.Scp079Role scp079Role && scp079Role.SubroutineModule.TryGetSubroutine<global::PlayerRoles.PlayableScps.Scp079.Scp079TierManager>(out var subroutine))
				{
					ApplyChanges(subroutine, input);
					num++;
				}
			}
			if (num > 0)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} used \"{1} ({2})\" command on player{3}{4}.", sender.LogName, Command, input, (num == 1) ? " " : "s ", global::NorthwoodLib.Pools.StringBuilderPool.Shared.ToStringReturn(stringBuilder)), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			}
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}

		public abstract void ApplyChanges(global::PlayerRoles.PlayableScps.Scp079.Scp079TierManager manager, int input);
	}
}
