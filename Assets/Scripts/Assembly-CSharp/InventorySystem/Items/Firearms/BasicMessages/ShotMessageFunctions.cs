namespace InventorySystem.Items.Firearms.BasicMessages
{
    public static class ShotMessageFunctions
    {
        public static void Serialize(this global::Mirror.NetworkWriter writer, global::InventorySystem.Items.Firearms.BasicMessages.ShotMessage value)
        {
            value.Serialize(writer);
        }

        public static global::InventorySystem.Items.Firearms.BasicMessages.ShotMessage Deserialize(this global::Mirror.NetworkReader reader)
        {
            global::InventorySystem.Items.Firearms.BasicMessages.ShotMessage result = default(global::InventorySystem.Items.Firearms.BasicMessages.ShotMessage);
            result.Deserialize(reader);
            return result;
        }
    }
}
