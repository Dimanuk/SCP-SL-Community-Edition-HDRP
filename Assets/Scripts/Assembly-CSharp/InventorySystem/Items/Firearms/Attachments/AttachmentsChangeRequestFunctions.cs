namespace InventorySystem.Items.Firearms.Attachments
{
    public static class AttachmentsChangeRequestFunctions
    {
        public static void Serialize(this global::Mirror.NetworkWriter writer, global::InventorySystem.Items.Firearms.Attachments.AttachmentsChangeRequest value)
        {
            value.Serialize(writer);
        }

        public static global::InventorySystem.Items.Firearms.Attachments.AttachmentsChangeRequest Deserialize(this global::Mirror.NetworkReader reader)
        {
            global::InventorySystem.Items.Firearms.Attachments.AttachmentsChangeRequest result = default(global::InventorySystem.Items.Firearms.Attachments.AttachmentsChangeRequest);
            result.Deserialize(reader);
            return result;
        }
    }
}
