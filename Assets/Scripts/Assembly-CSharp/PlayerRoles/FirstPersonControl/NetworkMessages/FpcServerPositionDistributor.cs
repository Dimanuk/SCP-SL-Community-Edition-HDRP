using System;
using System.Collections.Generic;
using InventorySystem;
using Mirror;
using PlayerRoles.Visibility;
using RelativePositioning;
using UnityEngine;

namespace PlayerRoles.FirstPersonControl.NetworkMessages
{
    public static class FpcServerPositionDistributor
    {
        private const int MinTickrate = 10;

        private const int MaxTickrate = 60;

        private const int ArrayStartSize = 30;

        private const int ArrayAddAmount = 10;

        private const int ArrayAddThreshold = 5;

        private static readonly Dictionary<uint, Dictionary<uint, FpcSyncData>> PreviouslySent = new Dictionary<uint, Dictionary<uint, FpcSyncData>>();

        private static int[] _bufferPlayerIDs = new int[30];

        private static FpcSyncData[] _bufferSyncData = new FpcSyncData[30];

        private static float _sendCooldown;

        private static float SendRate => 1f / (float)Mathf.Clamp(ServerStatic.ServerTickrate, MinTickrate, MaxTickrate);

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            StaticUnityMethods.OnLateUpdate += LateUpdate;
            PlayerRoleManager.OnRoleChanged += ResetPlayer;
            Inventory.OnServerStarted += PreviouslySent.Clear;
            ReferenceHub.OnPlayerAdded = (Action<ReferenceHub>)Delegate.Combine(ReferenceHub.OnPlayerAdded, (Action<ReferenceHub>)delegate
            {
                EnsureArrayCapacity();
            });
        }

        private static void ResetPlayer(ReferenceHub userHub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            if (!(prevRole is IFpcRole))
            {
                return;
            }
            uint netId = userHub.netId;
            foreach (KeyValuePair<uint, Dictionary<uint, FpcSyncData>> item in PreviouslySent)
            {
                item.Value.Remove(netId);
            }
        }

        private static void LateUpdate()
        {
            if (!NetworkServer.active || !StaticUnityMethods.IsPlaying)
            {
                return;
            }
            _sendCooldown += Time.deltaTime;
            if (_sendCooldown < SendRate)
            {
                return;
            }
            _sendCooldown -= SendRate;
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.Mode != ClientInstanceMode.Unverified && !allHub.isLocalPlayer)
                {
                    allHub.connectionToClient.Send(new FpcPositionMessage(allHub));
                }
            }
        }

        private static void EnsureArrayCapacity()
        {
            int num = Mathf.Min(_bufferPlayerIDs.Length, _bufferSyncData.Length);
            int count = ReferenceHub.AllHubs.Count;
            if (count > num - 5)
            {
                _bufferPlayerIDs = new int[count + 10];
                _bufferSyncData = new FpcSyncData[count + 10];
            }
        }

        public static void WriteAll(ReferenceHub receiver, NetworkWriter writer)
        {
            ushort num = 0;
            bool flag;
            VisibilityController visibilityController;
            if (receiver.roleManager.CurrentRole is ICustomVisibilityRole customVisibilityRole)
            {
                flag = true;
                visibilityController = customVisibilityRole.VisibilityController;
            }
            else
            {
                flag = false;
                visibilityController = null;
            }
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.netId != receiver.netId && allHub.roleManager.CurrentRole is IFpcRole fpcRole)
                {
                    bool flag2 = flag && !visibilityController.ValidateVisibility(allHub);
                    FpcSyncData newSyncData = GetNewSyncData(receiver, allHub, fpcRole.FpcModule, flag2);
                    if (!flag2)
                    {
                        _bufferPlayerIDs[num] = allHub.PlayerId;
                        _bufferSyncData[num] = newSyncData;
                        num++;
                    }
                }
            }
            writer.WriteUShort(num);
            for (int i = 0; i < num; i++)
            {
                writer.WriteRecyclablePlayerId(new RecyclablePlayerId(_bufferPlayerIDs[i]));
                _bufferSyncData[i].Write(writer);
            }
        }

        private static FpcSyncData GetNewSyncData(ReferenceHub receiver, ReferenceHub target, FirstPersonMovementModule fpmm, bool isInvisible)
        {
            FpcSyncData prevSyncData = GetPrevSyncData(receiver, target);
            FpcSyncData fpcSyncData = (isInvisible ? default(FpcSyncData) : new FpcSyncData(prevSyncData, fpmm.SyncMovementState, fpmm.IsGrounded, new RelativePosition(target.transform.position), fpmm.MouseLook));
            PreviouslySent[receiver.netId][target.netId] = fpcSyncData;
            return fpcSyncData;
        }

        private static FpcSyncData GetPrevSyncData(ReferenceHub receiver, ReferenceHub target)
        {
            if (!PreviouslySent.TryGetValue(receiver.netId, out var value))
            {
                PreviouslySent.Add(receiver.netId, new Dictionary<uint, FpcSyncData>());
                return default(FpcSyncData);
            }
            if (!value.TryGetValue(target.netId, out var value2))
            {
                return default(FpcSyncData);
            }
            return value2;
        }
    }
}
