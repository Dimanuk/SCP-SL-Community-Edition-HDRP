namespace InventorySystem.Items.Test
{
    public class TestItem : global::InventorySystem.Items.Autosync.AutosyncItem
    {
        public override float Weight => 1f;

        public static void Log(string msg)
        {
            global::GameCore.Console.AddLog("TEST ITEM: " + msg, new global::UnityEngine.Color(0.3765f, 0.7882f, 0.9019f));
        }

        public override void EquipUpdate()
        {
            if (!base.IsLocalPlayer)
            {
                return;
            }
            if (global::UnityEngine.Input.GetKeyDown(global::UnityEngine.KeyCode.G))
            {
                Log("Sending empty message");
                new global::InventorySystem.Items.Autosync.AutosyncCmd(this).Send();
            }
            if (global::UnityEngine.Input.GetKeyDown(global::UnityEngine.KeyCode.H))
            {
                Log("Sending 'KNOCK KNOCK'");
                global::Mirror.NetworkWriter writer;
                using (new global::InventorySystem.Items.Autosync.AutosyncCmd(this, out writer))
                {
                    global::Mirror.NetworkWriterExtensions.WriteString(writer, "KNOCK KNOCK");
                }
            }
            if (!global::UnityEngine.Input.GetKeyDown(global::UnityEngine.KeyCode.J))
            {
                return;
            }
            Log("Sending a sequence:");
            for (int i = 1; i <= 10; i++)
            {
                global::Mirror.NetworkWriter writer2;
                using (new global::InventorySystem.Items.Autosync.AutosyncCmd(this, out writer2))
                {
                    global::Mirror.NetworkWriterExtensions.WriteString(writer2, "Sequence: " + i);
                }
            }
        }

        public override void ServerConfirmAcqusition()
        {
            base.ServerConfirmAcqusition();
            Log("Received acquisition confirmation of " + base.ItemSerial);
        }

        internal override void ClientProcessRpcLocally(global::Mirror.NetworkReader reader)
        {
            base.ClientProcessRpcLocally(reader);

            if (this.ViewModel is TestItemViewmodel testViewModel)
            {
                string text = global::Mirror.NetworkReaderExtensions.ReadString(reader);
                testViewModel.UpdateText(text);
            }
        }

        internal override void ServerProcessCmd(global::Mirror.NetworkReader reader)
        {
            base.ServerProcessCmd(reader);
            string text;
            if (reader.Position <= 0)
            {
                text = "Empty";
                Log("Received an empty message.");
            }
            else
            {
                string text2 = global::Mirror.NetworkReaderExtensions.ReadString(reader);
                Log("Received a message - " + text2);
                text = ((!(text2 == "KNOCK KNOCK")) ? ("Unknown - " + text2) : "Who's there?");
            }
            Log("Sending response - " + text);
            global::Mirror.NetworkWriter writer;
            using (new global::InventorySystem.Items.Autosync.AutosyncRpc(this, toAll: false, out writer))
            {
                global::Mirror.NetworkWriterExtensions.WriteString(writer, text);
            }
        }
    }
}
