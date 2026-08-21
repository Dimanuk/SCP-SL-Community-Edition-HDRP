using System.Collections.Generic;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items;
using InventorySystem.Items.Keycards;
using MapGeneration;
using UnityEngine;

namespace InventorySystem.Hotkeys
{
	public class KeycardHotkey : IHotkeyItemSelector
	{
		private static readonly ItemType[] BestToWorstKeycards;
		private static readonly Dictionary<ItemType, int> KeycardValues;
		private static readonly List<int> CurrentlyPossessedKeycards;

		private static bool _holdsKeycard;
		private static ushort _bestKeycard;
		private static ushort _nearestDoorKeycard;
		private static Vector3Int _lastRoom;

		public ActionName KeyActionName => (ActionName)20; // ldc.i4.s 20

		static KeycardHotkey()
		{
			BestToWorstKeycards = new ItemType[]
			{
				(ItemType)11, (ItemType)9,  (ItemType)8, (ItemType)10,
				(ItemType)7,  (ItemType)6,  (ItemType)5, (ItemType)2,
				(ItemType)4,  (ItemType)3,  (ItemType)1, (ItemType)0
			};

			KeycardValues = new Dictionary<ItemType, int>();
			CurrentlyPossessedKeycards = new List<int>();

			for (int i = 0; i < BestToWorstKeycards.Length; i++)
				KeycardValues[BestToWorstKeycards[i]] = i;
		}

		public KeycardHotkey()
		{
			Inventory.OnItemsModified += RefreshBestKeycard;
			Inventory.OnCurrentItemChanged += OnCurrentItemChanged;
		}

		private void OnCurrentItemChanged(ReferenceHub ply, ItemIdentifier oldId, ItemIdentifier newId)
		{
			RefreshBestKeycard(ply);
		}

		public void RefreshBestKeycard(ReferenceHub ply)
		{
			if (ply == null || !ply.isLocalPlayer)
				return;

			_bestKeycard = 0;

			Inventory inventory = ply.inventory;
			if (inventory?.UserInventory?.Items == null)
				return;

			if (_nearestDoorKeycard != 0)
			{
				if (!inventory.UserInventory.Items.ContainsKey(_nearestDoorKeycard))
				{
					// Force the per-room door scan to re-run now that the card is gone.
					_lastRoom = Vector3Int.zero;
					_nearestDoorKeycard = 0;
				}
			}

			int currentPriority = -1;
			_holdsKeycard = false;

			ItemBase curItem = inventory.CurInstance;
			if (curItem != null && KeycardValues.TryGetValue(curItem.ItemTypeId, out currentPriority))
			{
				_holdsKeycard = true;
			}
			else
			{
				currentPriority = -1;
			}

			CurrentlyPossessedKeycards.Clear();
			foreach (var kvp in inventory.UserInventory.Items)
			{
				ItemBase item = kvp.Value;
				if (item == null)
					continue;

				if (KeycardValues.TryGetValue(item.ItemTypeId, out int priority))
				{
					if (!CurrentlyPossessedKeycards.Contains(priority))
						CurrentlyPossessedKeycards.Add(priority);
				}
			}

			if (CurrentlyPossessedKeycards.Count == 0)
				return;

			CurrentlyPossessedKeycards.Sort();

			int targetIndex = 0;
			if (currentPriority != -1)
			{
				int idx = CurrentlyPossessedKeycards.IndexOf(currentPriority);
				if (idx != -1 && idx + 1 < CurrentlyPossessedKeycards.Count)
					targetIndex = idx + 1;
			}

			int targetPriority = CurrentlyPossessedKeycards[targetIndex];
			ItemType targetType = BestToWorstKeycards[targetPriority];

			foreach (var kvp in inventory.UserInventory.Items)
			{
				if (kvp.Value != null && kvp.Value.ItemTypeId == targetType)
				{
					_bestKeycard = kvp.Key;
					break;
				}
			}
		}

		public ushort GetCorrespondingItemSerial(ReferenceHub ply, ushort[] itemsOrder, bool smartFeatureEnabled)
		{
			if (smartFeatureEnabled && !_holdsKeycard)
			{
				if (ply == null || ply.transform == null)
					return 0;

				Vector3 plyPos = ply.transform.position;
				Vector3Int roomCoords = RoomIdUtils.PositionToCoords(plyPos);

				// The door scan only runs when crossing into another room;
				// inside the room the cached result is reused.
				if (_lastRoom != roomCoords)
				{
					_lastRoom = roomCoords;
					_nearestDoorKeycard = 0;

					DoorVariant nearestDoor = GetNearestKeycardDoor(plyPos, roomCoords);
					if (nearestDoor == null || nearestDoor.RequiredPermissions == null)
						return _bestKeycard;

					KeycardPermissions required = nearestDoor.RequiredPermissions.RequiredPermissions;

					foreach (int priority in CurrentlyPossessedKeycards)
					{
						ItemType keycardType = BestToWorstKeycards[priority];

						if (!InventoryItemLoader.AvailableItems.TryGetValue(keycardType, out ItemBase prefab))
							continue;

						if (!(prefab is KeycardItem keycardItem))
							continue;

						if (keycardType == ply.inventory.CurItem.TypeId)
							continue;

						if ((keycardItem.Permissions & required) != required)
							continue;

						foreach (var item in ply.inventory.UserInventory.Items.Values)
						{
							if (item != null && item.ItemTypeId == keycardType)
							{
								_nearestDoorKeycard = item.ItemSerial;
								return _nearestDoorKeycard;
							}
						}
					}
				}
			}

			if (_holdsKeycard)
				return _bestKeycard;

			if (_nearestDoorKeycard != 0)
				return _nearestDoorKeycard;

			return _bestKeycard;
		}

		private DoorVariant GetNearestKeycardDoor(Vector3 plyPos, Vector3Int roomCoords)
		{
			if (!RoomIdentifier.RoomsByCoordinates.TryGetValue(roomCoords, out RoomIdentifier room))
				return null;

			if (!DoorVariant.DoorsByRoom.TryGetValue(room, out HashSet<DoorVariant> doors))
				return null;

			DoorVariant nearest = null;
			float nearestDist = float.MaxValue;

			foreach (DoorVariant door in doors)
			{
				if (door == null)
					continue;

				if (door.RequiredPermissions == null ||
					door.RequiredPermissions.RequiredPermissions == KeycardPermissions.None)
					continue;

				if (door is IDamageableDoor damageable && damageable.IsDestroyed)
					continue;

				float dist = Vector3.Distance(plyPos, door.transform.position);
				if (dist < nearestDist)
				{
					nearestDist = dist;
					nearest = door;
				}
			}

			return nearest;
		}
	}
}
