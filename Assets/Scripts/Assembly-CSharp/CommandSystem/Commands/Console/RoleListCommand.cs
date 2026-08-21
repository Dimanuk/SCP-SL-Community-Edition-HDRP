namespace CommandSystem.Commands.Console
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class RoleListCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "rolelist";

		public string[] Aliases { get; }

		public string Description { get; } = "Display a list of roles.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
			stringBuilder.AppendLine("<size=25>List of role:</size>");
			foreach (global::System.Collections.Generic.KeyValuePair<global::PlayerRoles.RoleTypeId, global::PlayerRoles.PlayerRoleBase> allRole in global::PlayerRoles.PlayerRoleLoader.AllRoles)
			{
				stringBuilder.AppendLine($"Role #{(int)allRole.Key:000} : {allRole.Key} - \"{allRole.Value.RoleName}\"");
			}
			response = stringBuilder.ToString();
			return true;
		}
	}
}
