using Interactables.Interobjects.DoorUtils;
using Mirror;
using PlayerRoles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables.Interobjects
{
	public class ElevatorManager : MonoBehaviour
	{
		public enum ElevatorGroup
		{
			GateA = 0,
			GateB = 1,
			LczA01 = 2,
			LczA02 = 3,
			LczB01 = 4,
			LczB02 = 5,
			Nuke = 6,
			Scp049 = 7
		}

        [global::System.Serializable]
        private struct ChamberTypePair
        {
            [global::UnityEngine.SerializeField]
            private global::Interactables.Interobjects.ElevatorManager.ElevatorGroup _group;

            [global::UnityEngine.SerializeField]
            private global::Interactables.Interobjects.ElevatorChamber _prefab;

            public bool TryGet(global::Interactables.Interobjects.ElevatorManager.ElevatorGroup group, out global::Interactables.Interobjects.ElevatorChamber chamber)
            {
                chamber = _prefab;
                return _group == group;
            }
        }


        public struct ElevatorSyncMsg : global::Mirror.NetworkMessage
        {
            public byte Data;

            public ElevatorSyncMsg(global::Interactables.Interobjects.ElevatorManager.ElevatorGroup group, int targetLvl)
            {
                Misc.ByteToBools((byte)group, out var @bool, out var bool2, out var bool3, out var bool4, out var bool5, out var bool6, out var bool7, out var bool8);
                Misc.ByteToBools((byte)targetLvl, out var bool9, out var bool10, out var bool11, out bool8, out bool7, out bool6, out var _, out var _);
                Data = Misc.BoolsToByte(@bool, bool2, bool3, bool4, bool5, bool9, bool10, bool11);
            }

            public void Unpack(out global::Interactables.Interobjects.ElevatorManager.ElevatorGroup group, out int targetLvl)
            {
                Misc.ByteToBools(Data, out var @bool, out var bool2, out var bool3, out var bool4, out var bool5, out var bool6, out var bool7, out var bool8);
                group = (global::Interactables.Interobjects.ElevatorManager.ElevatorGroup)Misc.BoolsToByte(@bool, bool2, bool3, bool4, bool5);
                targetLvl = Misc.BoolsToByte(bool6, bool7, bool8);
            }
        }

        [SerializeField]
		private ChamberTypePair[] _customChambers;

		[SerializeField]
		private ElevatorChamber _defaultChamber;

		private static bool _refreshNextFrame;

        internal static readonly Dictionary<ElevatorGroup, ElevatorChamber> SpawnedChambers = new Dictionary<ElevatorGroup, ElevatorChamber>();

        private static readonly Dictionary<ElevatorGroup, int> SyncedDestinations = new Dictionary<ElevatorGroup, int>();

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += delegate
            {
                NetworkClient.ReplaceHandler<ElevatorSyncMsg>(ClientReceiveMessage);
                NetworkServer.ReplaceHandler<ElevatorSyncMsg>(ServerReceiveMessage);
            };
            ElevatorDoor.OnPairsChanged = (Action<ElevatorGroup, ElevatorDoor>)Delegate.Combine(ElevatorDoor.OnPairsChanged, (Action<ElevatorGroup, ElevatorDoor>)delegate
            {
                _refreshNextFrame = true;
            });
            ReferenceHub.OnPlayerAdded = (Action<ReferenceHub>)Delegate.Combine(ReferenceHub.OnPlayerAdded, new Action<ReferenceHub>(ServerSendAllToPlayer));
        }

        private void Update()
        {
            if (_refreshNextFrame)
            {
                RefreshChambers();
                _refreshNextFrame = false;
            }
        }

        private static void ServerSendAllToPlayer(ReferenceHub ply)
        {
            if (!NetworkServer.active || ply.isLocalPlayer)
            {
                return;
            }
            foreach (KeyValuePair<ElevatorGroup, int> syncedDestination in SyncedDestinations)
            {
                ply.connectionToClient.Send(new ElevatorSyncMsg(syncedDestination.Key, syncedDestination.Value));
            }
        }

        private ElevatorChamber GetChamberForGroup(ElevatorGroup group)
        {
            ChamberTypePair[] customChambers = _customChambers;
            foreach (ChamberTypePair chamberTypePair in customChambers)
            {
                if (chamberTypePair.TryGet(group, out var chamber))
                {
                    return chamber;
                }
            }
            return _defaultChamber;
        }

        public static bool TrySetDestination(ElevatorGroup group, int lvl, bool force = false)
        {
            if (!SpawnedChambers.TryGetValue(group, out var value) || value == null)
            {
                return false;
            }
            if (!value.TrySetDestination(lvl, force))
            {
                return false;
            }
            if (NetworkServer.active)
            {
                NetworkServer.SendToReady(new ElevatorSyncMsg(group, lvl));
                SyncedDestinations[group] = lvl;
            }
            return true;
        }

        private static void ClientReceiveMessage(ElevatorSyncMsg msg)
        {
            if (!NetworkServer.active)
            {
                msg.Unpack(out var group, out var targetLvl);
                SyncedDestinations[group] = targetLvl;
                TrySetDestination(group, targetLvl, force: true);
            }
        }

        private static void ServerReceiveMessage(NetworkConnection conn, ElevatorSyncMsg msg)
        {
            if (!ReferenceHub.TryGetHubNetID(conn.identity.netId, out var hub) || !hub.IsAlive())
            {
                return;
            }
            msg.Unpack(out var group, out var targetLvl);
            if (!SpawnedChambers.TryGetValue(group, out var value) || value == null || !value.IsReady)
            {
                return;
            }
            foreach (ElevatorPanel allPanel in value.AllPanels)
            {
                if (allPanel.AssignedChamber.AssignedGroup == group && (allPanel.AssignedChamber.ActiveLocks == DoorLockReason.None || hub.serverRoles.BypassMode) && allPanel.VerificationRule.ServerCanInteract(hub, allPanel))
                {
                    TrySetDestination(group, targetLvl);
                    break;
                }
            }
        }

        private void RefreshChambers()
        {
            foreach (KeyValuePair<ElevatorGroup, List<ElevatorDoor>> allElevatorDoor in ElevatorDoor.AllElevatorDoors)
            {
                ElevatorGroup key = allElevatorDoor.Key;
                List<ElevatorDoor> value = allElevatorDoor.Value;
                if (value == null || value.Count == 0 || (SpawnedChambers.TryGetValue(key, out var value2) && value2 != null))
                {
                    continue;
                }
                ElevatorChamber elevatorChamber = UnityEngine.Object.Instantiate(GetChamberForGroup(key));
                elevatorChamber.AssignedGroup = key;
                SpawnedChambers[key] = elevatorChamber;
                Transform obj = elevatorChamber.transform;
                obj.position = value[0].TargetPosition;
                obj.SetParent(value[0].transform.parent);
                if (!SyncedDestinations.TryGetValue(key, out var value3))
                {
                    if (!NetworkServer.active)
                    {
                        continue;
                    }
                    value3 = UnityEngine.Random.Range(0, value.Count);
                }
                TrySetDestination(key, value3, force: true);
            }
        }
    }
}
