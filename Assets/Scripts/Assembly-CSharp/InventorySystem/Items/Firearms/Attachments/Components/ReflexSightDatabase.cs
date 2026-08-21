namespace InventorySystem.Items.Firearms.Attachments.Components
{
    public static class ReflexSightDatabase
    {
        public static global::System.Collections.Generic.Dictionary<ushort, global::System.Collections.Generic.Dictionary<int, global::InventorySystem.Items.Firearms.Attachments.Components.ReflexSightSyncMessage>> Database = new global::System.Collections.Generic.Dictionary<ushort, global::System.Collections.Generic.Dictionary<int, global::InventorySystem.Items.Firearms.Attachments.Components.ReflexSightSyncMessage>>();

        private static readonly global::System.Collections.Generic.HashSet<(ushort WeaponSerial, int AttachmentId)> DirtyValues = new global::System.Collections.Generic.HashSet<(ushort, int)>();

        [global::UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            StaticUnityMethods.OnUpdate += ServerUpdate;
            global::InventorySystem.Items.Firearms.BasicMessages.FirearmBasicMessagesHandler.OnStatusRequested += HandleNewClient;
            RegisterServerHandler();
            CustomNetworkManager.OnServerStarted += RegisterServerHandler;
            CustomNetworkManager.OnClientReady += delegate
            {
                Database.Clear();
                global::Mirror.NetworkClient.ReplaceHandler<global::InventorySystem.Items.Firearms.Attachments.Components.ReflexSightSyncMessage>(ClientReceiveMessage);
            };
        }

        private static void RegisterServerHandler()
        {
            global::Mirror.NetworkServer.RegisterHandler<global::InventorySystem.Items.Firearms.Attachments.Components.ReflexSightSyncMessage>(ServerReceiveMessage);
        }

        private static void HandleNewClient(ReferenceHub hub)
        {
            foreach (global::System.Collections.Generic.KeyValuePair<ushort, global::System.Collections.Generic.Dictionary<int, global::InventorySystem.Items.Firearms.Attachments.Components.ReflexSightSyncMessage>> item in Database)
            {
                foreach (global::System.Collections.Generic.KeyValuePair<int, global::InventorySystem.Items.Firearms.Attachments.Components.ReflexSightSyncMessage> item2 in item.Value)
                {
                    hub.networkIdentity.connectionToClient.Send(item2.Value);
                }
            }
        }

        private static void ClientReceiveMessage(global::InventorySystem.Items.Firearms.Attachments.Components.ReflexSightSyncMessage msg)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                AddMessage(msg);
            }
        }

        private static void ServerReceiveMessage(global::Mirror.NetworkConnection conn, global::InventorySystem.Items.Firearms.Attachments.Components.ReflexSightSyncMessage msg)
        {
            if (!ReferenceHub.TryGetHubNetID(conn.identity.netId, out var hub)
                || !hub.inventory.UserInventory.Items.TryGetValue(msg.WeaponSerial, out var value)
                || value is not global::InventorySystem.Items.Firearms.Firearm firearm)
            {
                return;
            }

            int index = msg.AttachmentId;
            if (index < 0 || index >= firearm.Attachments.Length)
                return;

            if (firearm.Attachments[index] is not global::InventorySystem.Items.Firearms.Attachments.Components.ReflexSightAttachment)
                return;

            AddMessage(msg);
        }

        private static void AddMessage(global::InventorySystem.Items.Firearms.Attachments.Components.ReflexSightSyncMessage msg)
        {
            if (Database.TryGetValue(msg.WeaponSerial, out var value))
            {
                value[msg.AttachmentId] = msg;
            }
            else
            {
                Database.Add(msg.WeaponSerial, new global::System.Collections.Generic.Dictionary<int, global::InventorySystem.Items.Firearms.Attachments.Components.ReflexSightSyncMessage> { [msg.AttachmentId] = msg });
            }
            if (global::Mirror.NetworkServer.active)
            {
                DirtyValues.Add((msg.WeaponSerial, msg.AttachmentId));
            }
        }

        private static void ServerUpdate()
        {
            if (!StaticUnityMethods.IsPlaying)
            {
                return;
            }
            foreach (var (weaponSerial, attachmentId) in DirtyValues)
            {
                if (Database.TryGetValue(weaponSerial, out var value) && value.TryGetValue(attachmentId, out var value2))
                {
                    global::Mirror.NetworkServer.SendToAll(value2);
                }
            }
            DirtyValues.Clear();
        }
    }
}