namespace InventorySystem.Items.Firearms.Attachments
{
    public static class AttachmentsSetupPreferenceFunctions
    {
        public static void Serialize(this global::Mirror.NetworkWriter writer, global::InventorySystem.Items.Firearms.Attachments.AttachmentsSetupPreference value)
        {
            value.Serialize(writer);
        }

        public static global::InventorySystem.Items.Firearms.Attachments.AttachmentsSetupPreference Deserialize(this global::Mirror.NetworkReader reader)
        {
            global::InventorySystem.Items.Firearms.Attachments.AttachmentsSetupPreference result = default(global::InventorySystem.Items.Firearms.Attachments.AttachmentsSetupPreference);
            result.Deserialize(reader);
            return result;
        }
    }
}
