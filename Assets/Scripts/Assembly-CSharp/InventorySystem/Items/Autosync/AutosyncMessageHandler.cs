
using Mirror;
using UnityEngine;

namespace InventorySystem.Items.Autosync
{
	public static class AutosyncMessageHandler
	{
        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += delegate
            {
                global::Mirror.NetworkServer.ReplaceHandler<global::InventorySystem.Items.Autosync.AutosyncMessage>(ServerHandleMessage);
                global::Mirror.NetworkClient.ReplaceHandler(delegate (global::InventorySystem.Items.Autosync.AutosyncMessage msg)
                {
                    msg.ProcessRpc();
                });
            };
        }

        private static void ServerHandleMessage(global::Mirror.NetworkConnection conn, global::InventorySystem.Items.Autosync.AutosyncMessage msg)
        {
            if (!(conn.identity == null) && ReferenceHub.TryGetHub(conn.identity.gameObject, out var hub))
            {
                msg.ProcessCmd(hub);
            }
        }

        public static global::InventorySystem.Items.Autosync.AutosyncMessage ReadAutosyncMessage(this global::Mirror.NetworkReader reader)
        {
            return new global::InventorySystem.Items.Autosync.AutosyncMessage(reader);
        }

        public static void WriteAutosyncMessage(this global::Mirror.NetworkWriter writer, global::InventorySystem.Items.Autosync.AutosyncMessage msg)
        {
            msg.Serialize(writer);
        }
    }
}
