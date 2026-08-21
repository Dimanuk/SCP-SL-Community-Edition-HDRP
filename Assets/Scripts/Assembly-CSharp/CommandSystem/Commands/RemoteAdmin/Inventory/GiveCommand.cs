namespace CommandSystem.Commands.RemoteAdmin.Inventory
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	public class GiveCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "give";

		public string[] Aliases { get; }

		public string Description { get; } = "Give player(s) a specified item.";

		public string[] Usage { get; } = new string[2] { "%player%", "%item%" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
			{
				return false;
			}
			if (arguments.Count >= 2)
			{
				string[] newargs;
				global::System.Collections.Generic.List<ReferenceHub> list = global::Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
				if (newargs == null || newargs.Length == 0)
				{
					response = "You must specify item(s) to give.";
					return false;
				}
				ItemType[] array = global::System.Linq.Enumerable.ToArray(ParseItems(newargs[0]));
				if (array.Length == 0)
				{
					response = "You didn't input any items.";
					return false;
				}
				int num = 0;
				int num2 = 0;
				string arg = string.Empty;
				if (list != null)
				{
					foreach (ReferenceHub item in list)
					{
						try
						{
							ItemType[] array2 = array;
							foreach (ItemType id in array2)
							{
								AddItem(item, sender, id);
							}
						}
						catch (global::System.Exception ex)
						{
							num++;
							arg = ex.Message;
							continue;
						}
						num2++;
					}
				}
				response = ((num == 0) ? string.Format("Done! The request affected {0} player{1}", num2, (num2 == 1) ? "!" : "s!") : $"Failed to execute the command! Failures: {num}\nLast error log:\n{arg}");
				return true;
			}
			response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
			return false;
		}

		private global::System.Collections.Generic.IEnumerable<ItemType> ParseItems(string argument)
		{
			string[] array = argument.Split('.');
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				if (int.TryParse(array2[i], out var result) && global::System.Enum.IsDefined(typeof(ItemType), result))
				{
					yield return (ItemType)result;
				}
			}
		}

		private void AddItem(ReferenceHub ply, global::CommandSystem.ICommandSender sender, ItemType id)
		{
			global::InventorySystem.Items.ItemBase itemBase = global::InventorySystem.InventoryExtensions.ServerAddItem(ply.inventory, id, 0);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, $"{sender.LogName} gave {id} to {ply.LoggedNameFromRefHub()}.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			if (itemBase == null)
			{
				throw new global::System.NullReferenceException($"Could not add {id}. Inventory is full or the item is not defined.");
			}
			if (itemBase is global::InventorySystem.Items.Firearms.Firearm firearm)
			{
				if (global::InventorySystem.Items.Firearms.Attachments.AttachmentsServerHandler.PlayerPreferences.TryGetValue(ply, out var value) && value.TryGetValue(itemBase.ItemTypeId, out var value2))
				{
					global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.ApplyAttachmentsCode(firearm, value2, reValidate: true);
				}
				global::InventorySystem.Items.Firearms.FirearmStatusFlags firearmStatusFlags = global::InventorySystem.Items.Firearms.FirearmStatusFlags.MagazineInserted;
				if (global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.HasAdvantageFlag(firearm, global::InventorySystem.Items.Firearms.Attachments.AttachmentDescriptiveAdvantages.Flashlight))
				{
					firearmStatusFlags |= global::InventorySystem.Items.Firearms.FirearmStatusFlags.FlashlightEnabled;
				}
				firearm.Status = new global::InventorySystem.Items.Firearms.FirearmStatus(firearm.AmmoManagerModule.MaxAmmo, firearmStatusFlags, global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.GetCurrentAttachmentsCode(firearm));
			}
		}
	}
}
