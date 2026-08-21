namespace InventorySystem.Items.Autosync
{
    public abstract class AutosyncItem : global::InventorySystem.Items.ItemBase, global::InventorySystem.Items.IAcquisitionConfirmationTrigger
    {
        public bool AcquisitionAlreadyReceived { get; set; }

        public virtual void ServerConfirmAcqusition()
        {
        }

        internal virtual void ServerProcessCmd(global::Mirror.NetworkReader reader)
        {
        }

        internal virtual void ClientProcessRpcTemplate(global::Mirror.NetworkReader reader, ushort serial)
        {
        }

        internal virtual void ClientProcessRpcLocally(global::Mirror.NetworkReader reader)
        {
        }
    }
}
