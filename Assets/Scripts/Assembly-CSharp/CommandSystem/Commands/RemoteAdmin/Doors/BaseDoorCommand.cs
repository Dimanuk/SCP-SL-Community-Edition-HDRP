namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	public abstract class BaseDoorCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public abstract string Command { get; }

		public abstract string[] Aliases { get; }

		public abstract string Description { get; }

		public virtual string[] Usage { get; } = new string[1] { "%door%" };

		public virtual bool AllowNonDamageableTargets => true;

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			if (arguments.Count == 0)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			if (string.IsNullOrEmpty(arguments.At(0)))
			{
				response = "Please specify a door first.";
				return false;
			}
			bool flag = false;
			string text = arguments.At(0).ToUpper();
			string[] source = text.Split('.');
			global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
			foreach (global::Interactables.Interobjects.DoorUtils.DoorVariant allDoor in global::Interactables.Interobjects.DoorUtils.DoorVariant.AllDoors)
			{
				if (allDoor is global::Interactables.Interobjects.DoorUtils.INonInteractableDoor nonInteractableDoor && nonInteractableDoor.IgnoreRemoteAdmin)
				{
					continue;
				}
				string empty = string.Empty;
				global::Interactables.Interobjects.DoorUtils.DoorNametagExtension nt;
				bool flag2 = allDoor.TryGetComponent<global::Interactables.Interobjects.DoorUtils.DoorNametagExtension>(out nt);
				switch (text)
				{
				case "*":
					if (!flag2)
					{
						continue;
					}
					break;
				case "!*":
				{
					if (flag2)
					{
						continue;
					}
					global::UnityEngine.Transform parent = allDoor.transform.parent;
					if (parent != null && parent.TryGetComponent<global::Interactables.Interobjects.DoorUtils.DoorNametagExtension>(out var _))
					{
						continue;
					}
					break;
				}
				default:
					if (!flag2 || !global::System.Linq.Enumerable.Any(source, (string i) => string.Equals(nt.GetName, i, global::System.StringComparison.OrdinalIgnoreCase)))
					{
						continue;
					}
					break;
				case "**":
					break;
				}
				if (allDoor is global::Interactables.Interobjects.DoorUtils.IDamageableDoor damageableDoor)
				{
					if (damageableDoor.IsDestroyed)
					{
						continue;
					}
				}
				else if (!AllowNonDamageableTargets)
				{
					continue;
				}
				OnTargetFound(allDoor);
				if (!string.IsNullOrEmpty(empty))
				{
					stringBuilder.Append("\n- " + empty);
				}
				flag = true;
			}
			string text2 = global::NorthwoodLib.Pools.StringBuilderPool.Shared.ToStringReturn(stringBuilder);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " ran " + Command + " on the following door(s):" + text2, ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			response = (flag ? ("Affected the following door(s):" + text2) : ("Can't find any door(s) using \"" + text.Replace(".", ", ") + "\"."));
			return flag;
		}

		protected abstract void OnTargetFound(global::Interactables.Interobjects.DoorUtils.DoorVariant door);
	}
}
