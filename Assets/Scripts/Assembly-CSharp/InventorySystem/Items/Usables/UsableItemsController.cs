using System;
using System.Collections.Generic;
using Mirror;
using PlayerRoles;
using UnityEngine;

namespace InventorySystem.Items.Usables
{
    public static class UsableItemsController
    {
        public static readonly Dictionary<ReferenceHub, PlayerHandler> Handlers = new Dictionary<ReferenceHub, PlayerHandler>();
        public static readonly Dictionary<ushort, float> GlobalItemCooldowns = new Dictionary<ushort, float>();
        public static readonly Dictionary<ushort, float> StartTimes = new Dictionary<ushort, float>();
        public static readonly Dictionary<ushort, AudioSource> CurrentlyPlayingSources = new Dictionary<ushort, AudioSource>();

        public static event Action<ReferenceHub, UsableItem> ServerOnUsingCompleted;
        public static event Action<StatusMessage> OnClientStatusReceived;

        [RuntimeInitializeOnLoadMethod]
        private static void InitOnLoad()
        {
            NetworkServer.ReplaceHandler<StatusMessage>(ServerReceivedStatus);
            CustomNetworkManager.OnClientReady += OnClientReady;
            PlayerRoleManager.OnRoleChanged += ResetPlayerOnRoleChange;
            StaticUnityMethods.OnUpdate += Update;

            ReferenceHub.OnPlayerRemoved += hub => Handlers.Remove(hub);
        }

        public static void OnClientReady()
        {
            NetworkClient.ReplaceHandler<StatusMessage>(ClientReceivedStatus);
            NetworkClient.ReplaceHandler<ItemCooldownMessage>(ClientReceivedCooldown);

            GlobalItemCooldowns.Clear();
            StartTimes.Clear();
            CurrentlyPlayingSources.Clear();
            Handlers.Clear();
        }

        public static PlayerHandler GetHandler(ReferenceHub ply)
        {
            if (ply == null)
                return null;

            if (!Handlers.TryGetValue(ply, out var handler))
            {
                handler = new PlayerHandler();
                Handlers[ply] = handler;
            }
            return handler;
        }

        public static float GetCooldown(ushort itemSerial, ItemBase item, PlayerHandler handler)
        {
            float cooldown = 0f;

            if (GlobalItemCooldowns.TryGetValue(itemSerial, out var globalValue))
                cooldown = globalValue;

            if (handler != null && handler.PersonalCooldowns.TryGetValue(item.ItemTypeId, out var personalValue) && personalValue > cooldown)
                cooldown = personalValue;

            return cooldown - Time.timeSinceLevelLoad;
        }

        public static void PlaySoundOnPlayer(ReferenceHub ply, AudioClip clip)
        {
            if (ply == null || clip == null)
                return;

            var curItem = ply.inventory.CurItem;

            AudioSource source = AudioPooling.AudioSourcePoolManager.PlaySound(
                clip, ply.transform, UsableItem.AudibleSfxRange);

            if (source != null)
            {
                source.pitch = CustomPlayerEffects.UsableItemModifierEffectExtensions.GetSpeedMultiplier(
                    curItem.TypeId, ply);

                CurrentlyPlayingSources[curItem.SerialNumber] = source;
            }
        }

        private static void Update()
        {
            if (!StaticUnityMethods.IsPlaying || !NetworkServer.active)
                return;

            foreach (var kvp in Handlers)
            {
                ReferenceHub hub = kvp.Key;
                PlayerHandler handler = kvp.Value;

                handler.DoUpdate(hub);

                var current = handler.CurrentUsable;
                if (current.ItemSerial == 0 || current.Item == null)
                    continue;

                float speedMultiplier = CustomPlayerEffects.UsableItemModifierEffectExtensions.GetSpeedMultiplier(
                    current.Item.ItemTypeId, hub);

                if (current.ItemSerial != hub.inventory.CurItem.SerialNumber)
                {
                    current.Item.OnUsingCancelled();
                    handler.CurrentUsable = CurrentlyUsedItem.None;

                    hub.inventory.connectionToClient?.Send(new StatusMessage(StatusMessage.StatusType.Cancel, current.ItemSerial));
                }
                else if (Time.timeSinceLevelLoad >= current.StartTime + current.Item.UseTime / speedMultiplier)
                {
                    current.Item.ServerOnUsingCompleted();
                    ServerOnUsingCompleted?.Invoke(hub, current.Item);

                    handler.CurrentUsable = CurrentlyUsedItem.None;
                }
            }
        }

        private static void ServerReceivedStatus(NetworkConnection conn, StatusMessage msg)
        {
            if (!ReferenceHub.TryGetHub(conn.identity.gameObject, out var hub) ||
                hub.inventory.CurInstance is not UsableItem usableItem ||
                usableItem.ItemSerial != msg.ItemSerial)
            {
                return;
            }

            PlayerHandler handler = GetHandler(hub);

            switch (msg.Status)
            {
                case StatusMessage.StatusType.Start:
                    if (usableItem.ServerValidateStartRequest(handler) &&
                        handler.CurrentUsable.ItemSerial == 0 &&
                        usableItem.CanStartUsing)
                    {
                        float cooldown = GetCooldown(msg.ItemSerial, usableItem, handler);

                        if (cooldown > 0f)
                        {
                            conn.Send(new ItemCooldownMessage(msg.ItemSerial, cooldown));
                        }
                        else if (CustomPlayerEffects.UsableItemModifierEffectExtensions.GetSpeedMultiplier(usableItem.ItemTypeId, hub) > 0f)
                        {
                            handler.CurrentUsable = new CurrentlyUsedItem(usableItem, msg.ItemSerial, Time.timeSinceLevelLoad);
                            handler.CurrentUsable.Item.OnUsingStarted();

                            Utils.Networking.NetworkUtils.SendToAuthenticated(new StatusMessage(StatusMessage.StatusType.Start, msg.ItemSerial));
                        }
                    }
                    break;

                case StatusMessage.StatusType.Cancel:
                    if (usableItem.ServerValidateCancelRequest(handler) && handler.CurrentUsable.ItemSerial != 0)
                    {
                        float speedMultiplier = CustomPlayerEffects.UsableItemModifierEffectExtensions.GetSpeedMultiplier(
                            handler.CurrentUsable.Item.ItemTypeId, hub);

                        if (handler.CurrentUsable.StartTime + handler.CurrentUsable.Item.MaxCancellableTime / speedMultiplier > Time.timeSinceLevelLoad)
                        {
                            handler.CurrentUsable.Item.OnUsingCancelled();
                            handler.CurrentUsable = CurrentlyUsedItem.None;

                            Utils.Networking.NetworkUtils.SendToAuthenticated(new StatusMessage(StatusMessage.StatusType.Cancel, msg.ItemSerial));
                        }
                    }
                    break;
            }
        }

        private static void ClientReceivedStatus(StatusMessage msg)
        {
            bool isStart = msg.Status == StatusMessage.StatusType.Start;

            if (!isStart && CurrentlyPlayingSources.TryGetValue(msg.ItemSerial, out var playingSource) && playingSource != null)
            {
                playingSource.Stop();
                StartTimes.Remove(msg.ItemSerial);
                CurrentlyPlayingSources.Remove(msg.ItemSerial);
            }

            foreach (ReferenceHub hub in ReferenceHub.AllHubs)
            {
                AudioClip clip;

                if (hub.isLocalPlayer)
                {
                    if (!hub.inventory.UserInventory.Items.TryGetValue(msg.ItemSerial, out var itemBase) ||
                        itemBase is not UsableItem usable)
                        continue;

                    if (isStart)
                        usable.OnUsingStarted();
                    else
                        usable.OnUsingCancelled();

                    if (usable.UsingSfxClip == null)
                        break;

                    clip = usable.UsingSfxClip;
                }
                else
                {
                    if (hub.inventory.CurItem.SerialNumber != msg.ItemSerial)
                        continue;

                    if (!InventoryItemLoader.AvailableItems.TryGetValue(hub.inventory.CurItem.TypeId, out var prefab) ||
                        prefab is not UsableItem usablePrefab ||
                        usablePrefab.UsingSfxClip == null)
                        break;

                    clip = usablePrefab.UsingSfxClip;
                }

                if (isStart)
                {
                    PlaySoundOnPlayer(hub, clip);
                    StartTimes[msg.ItemSerial] = Time.timeSinceLevelLoad;
                }

                break;
            }

            OnClientStatusReceived?.Invoke(msg);
        }

        private static void ClientReceivedCooldown(ItemCooldownMessage msg)
        {
            // Look the item up by serial in the local inventory — the message can arrive
            // before CurInstance switches to the newly equipped item.
            if (ReferenceHub.TryGetLocalHub(out var hub) &&
                hub.inventory.UserInventory.Items.TryGetValue(msg.ItemSerial, out var itemBase) &&
                itemBase is UsableItem usableItem)
            {
                usableItem.RemainingCooldown = msg.RemainingTime;
            }
        }

        private static void ResetPlayerOnRoleChange(ReferenceHub ply, PlayerRoleBase oldRole, PlayerRoleBase newRole)
        {
            if (ply != null)
                GetHandler(ply).ResetAll();
        }

        static UsableItemsController()
        {
            Handlers = new Dictionary<ReferenceHub, PlayerHandler>();
            GlobalItemCooldowns = new Dictionary<ushort, float>();
            StartTimes = new Dictionary<ushort, float>();
            CurrentlyPlayingSources = new Dictionary<ushort, AudioSource>();
        }
    }
}