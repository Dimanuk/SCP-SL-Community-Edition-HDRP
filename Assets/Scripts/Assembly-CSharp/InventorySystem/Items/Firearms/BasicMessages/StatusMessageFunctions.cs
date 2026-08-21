namespace InventorySystem.Items.Firearms.BasicMessages
{
    public static class StatusMessageFunctions
    {
        public static void Serialize(this global::Mirror.NetworkWriter writer, global::InventorySystem.Items.Firearms.BasicMessages.StatusMessage value)
        {
            value.Serialize(writer);
        }

        public static global::InventorySystem.Items.Firearms.BasicMessages.StatusMessage Deserialize(this global::Mirror.NetworkReader reader)
        {
            global::InventorySystem.Items.Firearms.BasicMessages.StatusMessage result = default(global::InventorySystem.Items.Firearms.BasicMessages.StatusMessage);
            result.Deserialize(reader);
            return result;
        }
    }
}
