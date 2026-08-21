namespace CommandSystem.Commands.Console
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class ItemListCommand : global::CommandSystem.ICommand
	{
		public string Command { get; } = "itemlist";

		public string[] Aliases { get; }

		public string Description { get; } = "Displays a list of items.";

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
			stringBuilder.AppendLine("<size=25>Item List:</size>");
			foreach (global::System.Collections.Generic.KeyValuePair<ItemType, global::InventorySystem.Items.ItemBase> availableItem in global::InventorySystem.InventoryItemLoader.AvailableItems)
			{
				stringBuilder.AppendLine($"ITEM#{(int)availableItem.Key:000} : {availableItem.Key}");
			}
			response = stringBuilder.ToString();
			return true;
		}
	}
}
