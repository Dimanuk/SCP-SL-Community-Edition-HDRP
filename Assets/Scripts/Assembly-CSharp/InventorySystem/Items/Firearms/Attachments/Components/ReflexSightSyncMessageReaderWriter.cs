namespace InventorySystem.Items.Firearms.Attachments.Components
{
    public static class ReflexSightSyncMessageReaderWriter
    {
        public static void WriteReflexSightSyncMessage(this global::Mirror.NetworkWriter writer, global::InventorySystem.Items.Firearms.Attachments.Components.ReflexSightSyncMessage msg)
        {
            msg.Write(writer);
        }

        public static global::InventorySystem.Items.Firearms.Attachments.Components.ReflexSightSyncMessage ReadReflexSightSyncMessage(this global::Mirror.NetworkReader reader)
        {
            return new global::InventorySystem.Items.Firearms.Attachments.Components.ReflexSightSyncMessage(reader);
        }
    }
}
