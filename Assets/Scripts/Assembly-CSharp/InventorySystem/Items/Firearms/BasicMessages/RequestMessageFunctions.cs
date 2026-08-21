namespace InventorySystem.Items.Firearms.BasicMessages
{
    public static class RequestMessageFunctions
    {
        public static void Serialize(this global::Mirror.NetworkWriter writer, global::InventorySystem.Items.Firearms.BasicMessages.RequestMessage value)
        {
            value.Serialize(writer);
        }

        public static global::InventorySystem.Items.Firearms.BasicMessages.RequestMessage Deserialize(this global::Mirror.NetworkReader reader)
        {
            global::InventorySystem.Items.Firearms.BasicMessages.RequestMessage result = default(global::InventorySystem.Items.Firearms.BasicMessages.RequestMessage);
            result.Deserialize(reader);
            return result;
        }
    }
}
