
using Mirror;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Modules
{
	public static class PumpMessageHandler
	{
        public static void Serialize(this global::Mirror.NetworkWriter writer, global::InventorySystem.Items.Firearms.Modules.ShotgunResyncMessage value)
        {
            writer.WriteUShort(value.Serial);
            int num = (value.ChamberedRounds << 4) | value.CockedHammers;
            writer.WriteByte((byte)num);
        }

        public static global::InventorySystem.Items.Firearms.Modules.ShotgunResyncMessage Deserialize(this global::Mirror.NetworkReader reader)
        {
            ushort serial = reader.ReadUShort();
            byte num = reader.ReadByte();
            int chamberedBullets = num >> 4;
            int cockedHammers = num & 0xF;
            return new global::InventorySystem.Items.Firearms.Modules.ShotgunResyncMessage(serial, chamberedBullets, cockedHammers);
        }

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += delegate
            {
                global::Mirror.NetworkClient.ReplaceHandler<global::InventorySystem.Items.Firearms.Modules.ShotgunResyncMessage>(ClientMsgReceived);
            };
        }

        private static void ClientMsgReceived(global::InventorySystem.Items.Firearms.Modules.ShotgunResyncMessage msg)
        {
            if (ReferenceHub.TryGetLocalHub(out var hub) && hub.inventory.UserInventory.Items.TryGetValue(msg.Serial, out var value) && value is global::InventorySystem.Items.Firearms.Shotgun shotgun && shotgun.ActionModule is global::InventorySystem.Items.Firearms.Modules.PumpAction pumpAction)
            {
                pumpAction.ChamberedRounds = msg.ChamberedRounds;
                pumpAction.CockedHammers = msg.CockedHammers;
            }
        }
    }
}
