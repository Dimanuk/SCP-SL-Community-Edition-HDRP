namespace InventorySystem.Items.Autosync
{
    public class AutosyncCmd : global::InventorySystem.Items.Autosync.AutosyncWriterBase
    {
        public AutosyncCmd(global::InventorySystem.Items.Autosync.AutosyncItem item, out global::Mirror.NetworkWriter writer)
            : base(item, out writer)
        {
        }

        public AutosyncCmd(global::InventorySystem.Items.Autosync.AutosyncItem item)
            : base(item, out var _)
        {
        }

        protected override void HandleSending(global::InventorySystem.Items.Autosync.AutosyncMessage msg)
        {
            if (global::Mirror.NetworkClient.ready && global::Mirror.NetworkClient.active)
            {
                global::Mirror.NetworkClient.Send(msg);
            }
        }
    }
}
