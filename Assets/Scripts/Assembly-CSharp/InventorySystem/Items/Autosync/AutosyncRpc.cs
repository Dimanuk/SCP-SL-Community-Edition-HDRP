using System;

using Mirror;

namespace InventorySystem.Items.Autosync
{
	public class AutosyncRpc : AutosyncWriterBase
	{
		private enum Mode
		{
			Local = 0,
			AllClients = 1,
			Conditional = 2
		}

        private readonly global::InventorySystem.Items.Autosync.AutosyncRpc.Mode _mode;

        private readonly global::System.Func<ReferenceHub, bool> _predicate;

        private readonly global::Mirror.NetworkConnection _ownerConnection;

        public AutosyncRpc(global::InventorySystem.Items.Autosync.AutosyncItem item, bool toAll, out global::Mirror.NetworkWriter writer)
            : base(item, out writer)
        {
            if (toAll)
            {
                _mode = global::InventorySystem.Items.Autosync.AutosyncRpc.Mode.AllClients;
                return;
            }
            _mode = global::InventorySystem.Items.Autosync.AutosyncRpc.Mode.Local;
            _ownerConnection = item.Owner.connectionToClient;
        }

        public AutosyncRpc(global::InventorySystem.Items.Autosync.AutosyncItem item, global::System.Func<ReferenceHub, bool> predicate, out global::Mirror.NetworkWriter writer)
            : base(item, out writer)
        {
            _mode = global::InventorySystem.Items.Autosync.AutosyncRpc.Mode.Conditional;
            _predicate = predicate;
        }


        public AutosyncRpc(global::InventorySystem.Items.Autosync.AutosyncItem item, bool toAll)
            : this(item, toAll, out var _)
        {
        }

        public AutosyncRpc(global::InventorySystem.Items.Autosync.AutosyncItem item, global::System.Func<ReferenceHub, bool> predicate)
            : this(item, predicate, out var _)
        {
        }

        protected override void HandleSending(global::InventorySystem.Items.Autosync.AutosyncMessage msg)
        {
            if (global::Mirror.NetworkServer.active)
            {
                switch (_mode)
                {
                    case global::InventorySystem.Items.Autosync.AutosyncRpc.Mode.Local:
                        _ownerConnection.Send(msg);
                        break;
                    case global::InventorySystem.Items.Autosync.AutosyncRpc.Mode.AllClients:
                        global::Mirror.NetworkServer.SendToReady(msg);
                        break;
                    case global::InventorySystem.Items.Autosync.AutosyncRpc.Mode.Conditional:
                        global::Utils.Networking.NetworkUtils.SendToHubsConditionally(msg, _predicate);
                        break;
                }
            }
        }
    }
}
