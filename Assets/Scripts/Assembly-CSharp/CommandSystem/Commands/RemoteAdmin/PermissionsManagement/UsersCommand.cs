namespace CommandSystem.Commands.RemoteAdmin.PermissionsManagement
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.Commands.RemoteAdmin.PermissionsManagementCommand))]
	public class UsersCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "users";

		public string[] Aliases { get; }

		public string Description { get; } = "List all the users that are assigned to any group.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PermissionsManagement, out response))
			{
				return false;
			}
			global::System.Collections.Generic.Dictionary<string, string> stringDictionary = ServerStatic.RolesConfig.GetStringDictionary("Members");
			global::System.Collections.Generic.Dictionary<string, string> dictionary = ServerStatic.SharedGroupsMembersConfig?.GetStringDictionary("SharedMembers");
			global::System.Text.StringBuilder stringBuilder = global::NorthwoodLib.Pools.StringBuilderPool.Shared.Rent("Players with assigned groups:");
			foreach (global::System.Collections.Generic.KeyValuePair<string, string> item in stringDictionary)
			{
				stringBuilder.Append("\n" + item.Key + " - " + item.Value);
			}
			if (dictionary != null)
			{
				foreach (global::System.Collections.Generic.KeyValuePair<string, string> item2 in dictionary)
				{
					stringBuilder.Append("\n" + item2.Key + " - " + item2.Value + " <color=#FFD700>[SHARED MEMBERSHIP]</color>");
				}
			}
			response = stringBuilder.ToString();
			global::NorthwoodLib.Pools.StringBuilderPool.Shared.Return(stringBuilder);
			return true;
		}
	}
}
