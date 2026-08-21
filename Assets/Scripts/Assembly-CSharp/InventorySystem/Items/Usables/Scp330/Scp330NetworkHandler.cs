using AudioPooling;
using CustomPlayerEffects;
using UnityEngine;

namespace InventorySystem.Items.Usables.Scp330
{
	public static class Scp330NetworkHandler
	{
		public static readonly global::System.Collections.Generic.Dictionary<ushort, global::InventorySystem.Items.Usables.Scp330.CandyKindID> ReceivedSelectedCandies = new global::System.Collections.Generic.Dictionary<ushort, global::InventorySystem.Items.Usables.Scp330.CandyKindID>();

		public static event global::System.Action<global::InventorySystem.Items.Usables.Scp330.SelectScp330Message> OnClientSelectMessageReceived;

		[global::UnityEngine.RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			CustomNetworkManager.OnClientReady += RegisterHandlers;
		}

		private static void RegisterHandlers()
		{
			global::Mirror.NetworkServer.ReplaceHandler<global::InventorySystem.Items.Usables.Scp330.SelectScp330Message>(ServerSelectMessageReceived);
			global::Mirror.NetworkClient.ReplaceHandler<global::InventorySystem.Items.Usables.Scp330.SelectScp330Message>(ClientSelectMessageReceived);
			global::Mirror.NetworkClient.ReplaceHandler<global::InventorySystem.Items.Usables.Scp330.SyncScp330Message>(ClientSyncMessageReceived);
			ReceivedSelectedCandies.Clear();
		}

		private static void ServerSelectMessageReceived(global::Mirror.NetworkConnection conn, global::InventorySystem.Items.Usables.Scp330.SelectScp330Message msg)
		{
			if (!ReferenceHub.TryGetHubNetID(conn.identity.netId, out var hub) || !(hub.inventory.CurInstance is global::InventorySystem.Items.Usables.Scp330.Scp330Bag scp330Bag) || scp330Bag == null || scp330Bag.ItemSerial != msg.Serial || msg.CandyID >= scp330Bag.Candies.Count)
			{
				return;
			}
			if (msg.Drop)
			{
				if (global::InventorySystem.InventoryExtensions.ServerCreatePickup(psi: new global::InventorySystem.Items.Pickups.PickupSyncInfo(scp330Bag.ItemTypeId, hub.PlayerCameraReference.position, global::UnityEngine.Quaternion.identity, scp330Bag.Weight, 0), inv: hub.inventory, item: scp330Bag) is global::InventorySystem.Items.Usables.Scp330.Scp330Pickup scp330Pickup)
				{
					scp330Pickup.PreviousOwner = new global::Footprinting.Footprint(hub);
					global::InventorySystem.Items.Usables.Scp330.CandyKindID candyKindID = scp330Bag.TryRemove(msg.CandyID);
					if (candyKindID != global::InventorySystem.Items.Usables.Scp330.CandyKindID.None)
					{
						scp330Pickup.ExposedCandy = candyKindID;
						scp330Pickup.StoredCandies.Add(candyKindID);
					}
				}
			}
			else if (msg.CandyID >= 0 && msg.CandyID < scp330Bag.Candies.Count)
			{
				scp330Bag.SelectedCandyId = msg.CandyID;
				msg.CandyID = (int)scp330Bag.Candies[msg.CandyID];
				global::InventorySystem.Items.Usables.PlayerHandler handler = global::InventorySystem.Items.Usables.UsableItemsController.GetHandler(hub);
				handler.CurrentUsable = new global::InventorySystem.Items.Usables.CurrentlyUsedItem(scp330Bag, msg.Serial, global::UnityEngine.Time.timeSinceLevelLoad);
				handler.CurrentUsable.Item.OnUsingStarted();
				global::Utils.Networking.NetworkUtils.SendToAuthenticated(msg);
			}
		}

		private static void ClientSyncMessageReceived(global::InventorySystem.Items.Usables.Scp330.SyncScp330Message msg)
		{
			if (ReferenceHub.TryGetLocalHub(out var hub) && hub.inventory.UserInventory.Items.TryGetValue(msg.Serial, out var value) && value is global::InventorySystem.Items.Usables.Scp330.Scp330Bag scp330Bag)
			{
				scp330Bag.Candies = msg.Candies;
			}
		}

		private static void ClientSelectMessageReceived(SelectScp330Message msg)
        {
            // Remembered per-serial so a spectator viewmodel created after the selection
            // (InitSpectator) can still resolve which candy is currently held.
            ReceivedSelectedCandies[msg.Serial] = (CandyKindID)msg.CandyID;

            OnClientSelectMessageReceived?.Invoke(msg);

            if (!InventoryExtensions.TryGetHubHoldingSerial(msg.Serial, out ReferenceHub hub)) return;

            if (!(hub.inventory.CurInstance is Scp330Bag scp330Bag) || scp330Bag == null) return;

            // The local player begins the eating animation/timing locally (mirrors the server path).
            if (hub.isLocalPlayer)
                scp330Bag.OnUsingStarted();

            // The server rewrote CandyID from a list index into the CandyKindID value before broadcasting.
            AudioClip clip = Scp330Viewmodel.GetClipForCandy((CandyKindID)msg.CandyID);
            if (clip == null || hub.transform == null) return;

            AudioSource audioSource = AudioSourcePoolManager.PlaySound(
                clip,
                hub.transform,
                1f,
                1f,
                default,
                default,
                0f,
                true
            );

            if (audioSource == null) return;

            audioSource.pitch = UsableItemModifierEffectExtensions.GetSpeedMultiplier(ItemType.SCP330, hub);
            UsableItemsController.CurrentlyPlayingSources[msg.Serial] = audioSource;
            UsableItemsController.StartTimes[msg.Serial] = Time.timeSinceLevelLoad;
        }

		public static void SerializeSyncMessage(this global::Mirror.NetworkWriter writer, global::InventorySystem.Items.Usables.Scp330.SyncScp330Message value)
		{
			global::Mirror.NetworkWriterExtensions.WriteUShort(writer, value.Serial);
			writer.WriteByte((byte)value.Candies.Count);
			foreach (global::InventorySystem.Items.Usables.Scp330.CandyKindID candy in value.Candies)
			{
				writer.WriteByte((byte)candy);
			}
		}

		public static global::InventorySystem.Items.Usables.Scp330.SyncScp330Message DeserializeSyncMessage(this global::Mirror.NetworkReader reader)
		{
			ushort serial = global::Mirror.NetworkReaderExtensions.ReadUShort(reader);
			byte b = reader.ReadByte();
			global::System.Collections.Generic.List<global::InventorySystem.Items.Usables.Scp330.CandyKindID> list = new global::System.Collections.Generic.List<global::InventorySystem.Items.Usables.Scp330.CandyKindID>();
			for (int i = 0; i < b; i++)
			{
				list.Add((global::InventorySystem.Items.Usables.Scp330.CandyKindID)reader.ReadByte());
			}
			return new global::InventorySystem.Items.Usables.Scp330.SyncScp330Message
			{
				Candies = list,
				Serial = serial
			};
		}

		public static void SerializeSelectMessage(this global::Mirror.NetworkWriter writer, global::InventorySystem.Items.Usables.Scp330.SelectScp330Message value)
		{
			int num = value.CandyID + 1;
			global::Mirror.NetworkWriterExtensions.WriteUShort(writer, value.Serial);
			global::Mirror.NetworkWriterExtensions.WriteSByte(writer, (sbyte)(value.Drop ? (-num) : num));
		}

		public static global::InventorySystem.Items.Usables.Scp330.SelectScp330Message DeserializeSelectMessage(this global::Mirror.NetworkReader reader)
		{
			ushort serial = global::Mirror.NetworkReaderExtensions.ReadUShort(reader);
			int num = global::Mirror.NetworkReaderExtensions.ReadSByte(reader);
			return new global::InventorySystem.Items.Usables.Scp330.SelectScp330Message
			{
				CandyID = global::UnityEngine.Mathf.Abs(num) - 1,
				Serial = serial,
				Drop = (num < 0)
			};
		}
	}
}
