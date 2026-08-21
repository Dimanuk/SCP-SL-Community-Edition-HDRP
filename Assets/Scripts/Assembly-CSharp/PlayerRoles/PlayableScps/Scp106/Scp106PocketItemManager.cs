using System;
using System.Collections.Generic;
using AudioPooling;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using MapGeneration;
using Mirror;
using PlayerRoles.FirstPersonControl;
using RelativePositioning;
using UnityEngine;
using Utils.NonAllocLINQ;

using Random = UnityEngine.Random;

namespace PlayerRoles.PlayableScps.Scp106
{
    public static class Scp106PocketItemManager
    {
        private class PocketItem
        {
            public double TriggerTime;
            public bool Remove;
            public bool WarningSent;
            public RelativePosition DropPosition;
        }

        public struct WarningMessage : NetworkMessage
        {
            public RelativePosition Position;
        }

        private const float WarningTime = 3f;
        private const float RaycastRange = 30f;
        private const float SoundRange = 12f;
        private const float SpawnOffset = 0.3f;
        private const float RandomEscapeVelocity = 0.2f;
        private const int MaxValidPositions = 64;

        private static readonly Vector3[] ValidPositionsNonAlloc = new Vector3[MaxValidPositions];
        private static readonly HashSet<ItemPickupBase> ToRemove = new HashSet<ItemPickupBase>();
        private static readonly Dictionary<ItemPickupBase, PocketItem> TrackedItems = new Dictionary<ItemPickupBase, PocketItem>();

        private static readonly Vector2 HeightLimit = new Vector2(-1990f, -2002f);
        private static readonly Vector2 TimerRage = new Vector2(90f, 240f);
        private static readonly float[] RecycleChances = new float[] { 0.5f, 0.7f, 1f };

        private static float RandomVel => Random.Range(-RandomEscapeVelocity, RandomEscapeVelocity);

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            ItemPickupBase.OnPickupAdded += OnAdded;
            ItemPickupBase.OnPickupDestroyed += OnRemoved;
            StaticUnityMethods.OnUpdate += Update;

            CustomNetworkManager.OnClientReady += () =>
            {
                TrackedItems.Clear();

                NetworkClient.ReplaceHandler<WarningMessage>(msg =>
                {
                    if (PlayerRoleLoader.TryGetRoleTemplate<Scp106Role>(RoleTypeId.Scp106, out var role))
                    {
                        AudioSourcePoolManager.PlaySound(role.ItemSpawnSound, msg.Position, SoundRange);
                    }
                });
            };
        }

        private static void Update()
        {
            if (!NetworkServer.active)
                return;

            bool changed = false;

            foreach (var kvp in TrackedItems)
            {
                ItemPickupBase pickup = kvp.Key;
                PocketItem data = kvp.Value;

                if (pickup == null || !ValidateHeight(pickup))
                {
                    changed |= ToRemove.Add(pickup);
                    continue;
                }

                double timeLeft = data.TriggerTime - NetworkTime.time;

                if (timeLeft > WarningTime)
                    continue;

                if (!data.Remove && !data.WarningSent)
                {
                    NetworkServer.SendToAll(new WarningMessage { Position = data.DropPosition });
                    data.WarningSent = true;
                }

                if (timeLeft > 0.0)
                    continue;

                if (data.Remove)
                {
                    pickup.DestroySelf();
                }
                else if (pickup.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.linearVelocity = new Vector3(RandomVel, Physics.gravity.y, RandomVel);
                    pickup.transform.position = data.DropPosition.Position;
                    pickup.RefreshPositionAndRotation();
                }

                changed |= ToRemove.Add(pickup);
            }

            if (changed)
            {
                HashsetExtensions.ForEach(ToRemove, x => TrackedItems.Remove(x));
                ToRemove.Clear();
            }
        }

        private static void OnAdded(ItemPickupBase ipb)
        {
            if (!NetworkServer.active || !ValidateHeight(ipb))
                return;

            if (!InventoryItemLoader.TryGetItem<ItemBase>(ipb.Info.ItemId, out var itemBase))
                return;

            TrackedItems.Add(ipb, new PocketItem
            {
                Remove = Random.value > RecycleChances[GetRarity(itemBase)],
                TriggerTime = NetworkTime.time + Random.Range(TimerRage.x, TimerRage.y),
                DropPosition = GetRandomValidSpawnPosition(),
                WarningSent = false
            });
        }

        private static void OnRemoved(ItemPickupBase ipb)
        {
            if (NetworkServer.active)
            {
                TrackedItems.Remove(ipb);
            }
        }

        private static bool ValidateHeight(ItemPickupBase ipb)
        {
            float y = ipb.transform.position.y;
            return y >= HeightLimit.y && y <= HeightLimit.x;
        }

        private static int GetRarity(ItemBase ib)
        {
            int rarity = 0;

            if (HasFlagFast(ib, ItemTierFlags.Rare))
                rarity++;
            if (HasFlagFast(ib, ItemTierFlags.MilitaryGrade))
                rarity++;
            if (HasFlagFast(ib, ItemTierFlags.ExtraRare))
                rarity += 2;

            return Mathf.Min(rarity, RecycleChances.Length - 1);
        }

        private static bool HasFlagFast(ItemBase ib, ItemTierFlags flag)
        {
            return (ib.TierFlags & flag) == flag;
        }
        private static RelativePosition GetRandomValidSpawnPosition()
        {
            int count = 0;

            foreach (ReferenceHub hub in ReferenceHub.AllHubs)
            {
                if (!(hub.roleManager.CurrentRole is IFpcRole fpcRole))
                    continue;

                Vector3 pos = fpcRole.FpcModule.Position;
                if (!(pos.y < HeightLimit.x) && TryGetRoofPosition(pos, out var roofPos))
                {
                    ValidPositionsNonAlloc[count] = roofPos;
                    if (++count >= MaxValidPositions)
                        break;
                }
            }

            if (count > 0)
            {
                return new RelativePosition(ValidPositionsNonAlloc[Random.Range(0, count)]);
            }

            foreach (RoomIdentifier room in RoomIdentifier.AllRoomIdentifiers)
            {
                if ((room.Zone == FacilityZone.HeavyContainment || room.Zone == FacilityZone.Entrance) 
                    && TryGetRoofPosition(room.transform.position, out var roofPos2))
                {
                    ValidPositionsNonAlloc[count] = roofPos2;
                    if (++count >= MaxValidPositions)
                        break;
                }
            }

            if (count == 0)
            {
                throw new InvalidOperationException("GetRandomValidSpawnPosition found no valid spawn positions.");
            }

            int index = Random.Range(0, count);
            return new RelativePosition(ValidPositionsNonAlloc[index]);
        }

        private static bool TryGetRoofPosition(Vector3 point, out Vector3 result)
        {
            if (Physics.Raycast(point, Vector3.up, out var hit, RaycastRange, FpcStateProcessor.Mask))
            {
                result = hit.point + Vector3.down * SpawnOffset;
                return true;
            }

            result = Vector3.zero;
            return false;
        }
    }
}