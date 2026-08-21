using System;
using InventorySystem;
using InventorySystem.Items;

namespace CommandSystem.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class GiveCommand : ICommand
    {
        public string Command { get; } = "give";

        public string[] Aliases { get; }

        public string Description { get; } = "Give items to yourself.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            ReferenceHub localHub = ReferenceHub.LocalHub;
            if (localHub == null)
            {
                response = "You must join a server to execute this command.";
                return false;
            }

            if (!localHub.characterClassManager.isServer)
            {
                response = "You are not the owner of this server!";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "SYNTAX: give <item id>";
                return false;
            }

            if (!int.TryParse(arguments.Array[arguments.Offset], out int itemId))
            {
                response = "Item id must be an integer.";
                return false;
            }

            ItemType itemType = (ItemType)itemId;
            ItemBase itemBase = localHub.inventory.ServerAddItem(itemType);
            if (itemBase == null)
            {
                response = string.Format("Could not add {0}. Inventory is full or the item is not defined.", itemType);
                return false;
            }

            response = string.Format("{0} has been added to your inventory!", itemBase.ItemTypeId);
            return true;
        }
    }
}
