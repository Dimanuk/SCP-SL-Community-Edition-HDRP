using System;
using System.Collections.Generic;
using Mirror;
using RelativePositioning;
using UnityEngine;

namespace InventorySystem.Items.ThrowableProjectiles
{
    public static class ThrowableNetworkHandler
    {
        public readonly struct ThrowableItemRequestMessage : NetworkMessage
        {
            public readonly ushort Serial;
            public readonly RequestType Request;
            public readonly Quaternion CameraRotation;
            public readonly RelativePosition CameraPosition;
            public readonly Vector3 PlayerVelocity;

            public ThrowableItemRequestMessage(ushort serial, RequestType type, Quaternion rotation, RelativePosition position, Vector3 startVel)
            {
                Serial = serial;
                Request = type;
                CameraRotation = rotation;
                CameraPosition = position;
                PlayerVelocity = startVel;
            }

            public ThrowableItemRequestMessage(ThrowableItem item, RequestType type, Vector3 startVel = default)
            {
                Serial = item.ItemSerial;
                Request = type;
                CameraRotation = item.Owner.PlayerCameraReference.rotation;
                CameraPosition = new RelativePosition(item.Owner.PlayerCameraReference.position);
                PlayerVelocity = startVel;
            }
        }

        public readonly struct ThrowableItemAudioMessage : NetworkMessage
        {
            public readonly ushort Serial;
            public readonly RequestType Request;
            public readonly float Time;

            public ThrowableItemAudioMessage(ushort itemSerial, RequestType rt)
            {
                Serial = itemSerial;
                Request = rt;
                Time = UnityEngine.Time.timeSinceLevelLoad;
            }
        }

        public enum RequestType : byte
        {
            BeginThrow = 0,
            ConfirmThrowWeak = 1,
            ConfirmThrowFullForce = 2,
            CancelThrow = 3
        }

        public static readonly Dictionary<ushort, ThrowableItemAudioMessage> ReceivedRequests = new Dictionary<ushort, ThrowableItemAudioMessage>();

        private const float MaxPlayerSpeed = 10f;

        public static event Action<ThrowableItemAudioMessage> OnAudioMessageReceived;

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            NetworkServer.ReplaceHandler<ThrowableItemRequestMessage>(ServerProcessRequest);
            CustomNetworkManager.OnClientStarted += RegisterProjectiles;
            CustomNetworkManager.OnClientReady += () =>
            {
                NetworkClient.ReplaceHandler<ThrowableItemAudioMessage>(ClientProcessAudio);
                RegisterProjectiles();
            };
        }

        private static void RegisterProjectiles()
        {
            foreach (var kvp in InventorySystem.InventoryItemLoader.AvailableItems)
            {
                if (kvp.Value is ThrowableItem throwableItem && throwableItem.Projectile != null)
                {
                    NetworkIdentity netId = throwableItem.Projectile.netIdentity;
                    if (netId == null)
                        continue;

                    uint assetId = netId.assetId;

                    if (!NetworkClient.prefabs.ContainsKey(assetId))
                    {
                        NetworkClient.prefabs[assetId] = throwableItem.Projectile.gameObject;
                    }
                }
            }
        }

        private static void ServerProcessRequest(NetworkConnection conn, ThrowableItemRequestMessage msg)
        {
            if (ReferenceHub.TryGetHubNetID(conn.identity.netId, out var hub) &&
                hub.inventory.CurItem.SerialNumber == msg.Serial &&
                hub.inventory.CurInstance is ThrowableItem throwableItem)
            {
                switch (msg.Request)
                {
                    case RequestType.BeginThrow:
                        throwableItem.ServerProcessInitiation();
                        break;

                    case RequestType.CancelThrow:
                        throwableItem.ServerProcessCancellation();
                        break;

                    case RequestType.ConfirmThrowFullForce:
                        throwableItem.ServerProcessThrowConfirmation(true, msg.CameraPosition.Position, msg.CameraRotation, msg.PlayerVelocity);
                        break;

                    case RequestType.ConfirmThrowWeak:
                        throwableItem.ServerProcessThrowConfirmation(false, msg.CameraPosition.Position, msg.CameraRotation, msg.PlayerVelocity);
                        break;
                }
            }
        }

        private static void ClientProcessAudio(ThrowableItemAudioMessage msg)
        {
            ReceivedRequests[msg.Serial] = msg;

            OnAudioMessageReceived?.Invoke(msg);

            if (!InventorySystem.InventoryExtensions.TryGetHubHoldingSerial(msg.Serial, out var hub) || hub.isLocalPlayer)
                return;

            if (!InventorySystem.InventoryItemLoader.TryGetItem<ThrowableItem>(hub.inventory.CurItem.TypeId, out var itemPrefab))
                return;

            AudioClip clip;
            switch (msg.Request)
            {
                case RequestType.BeginThrow:
                    clip = itemPrefab.BeginClip;
                    break;
                case RequestType.ConfirmThrowWeak:
                case RequestType.ConfirmThrowFullForce:
                    clip = itemPrefab.ThrowClip;
                    break;
                case RequestType.CancelThrow:
                    clip = itemPrefab.CancelClip;
                    break;
                default:
                    return;
            }

            if (clip == null)
                return;

            AudioSource src = AudioPooling.AudioSourcePoolManager.PlaySound(clip, hub.transform, 10f);
            if (src != null &&
                CustomPlayerEffects.UsableItemModifierEffectExtensions.TryGetSpeedMultiplier(itemPrefab.ItemTypeId, hub, out float pitch))
            {
                src.pitch = pitch;
            }
        }

        public static Vector3 GetLimitedVelocity(Vector3 plyVel)
        {
            float magnitude = plyVel.magnitude;
            if (magnitude > MaxPlayerSpeed)
            {
                plyVel /= magnitude;
                plyVel *= MaxPlayerSpeed;
            }
            return plyVel;
        }

        private static bool RequiresAdditionalData(RequestType rq)
        {
            return rq == RequestType.ConfirmThrowWeak || rq == RequestType.ConfirmThrowFullForce;
        }

        public static void SerializeRequestMsg(this NetworkWriter writer, ThrowableItemRequestMessage value)
        {
            writer.WriteUShort(value.Serial);
            writer.WriteByte((byte)value.Request);

            if (RequiresAdditionalData(value.Request))
            {
                writer.WriteLowPrecisionQuaternion(new LowPrecisionQuaternion(value.CameraRotation));
                RelativePositioning.RelativePositionSerialization.WriteRelativePosition(writer, value.CameraPosition);
                writer.WriteVector3(value.PlayerVelocity);
            }
        }

        public static ThrowableItemRequestMessage DeserializeRequestMsg(this NetworkReader reader)
        {
            ushort serial = reader.ReadUShort();
            RequestType requestType = (RequestType)reader.ReadByte();

            bool needsData = RequiresAdditionalData(requestType);

            Quaternion rotation = needsData ? reader.ReadLowPrecisionQuaternion().Value : default;
            RelativePosition position = needsData ? RelativePositioning.RelativePositionSerialization.ReadRelativePosition(reader) : default;
            Vector3 startVel = needsData ? reader.ReadVector3() : default;

            return new ThrowableItemRequestMessage(serial, requestType, rotation, position, startVel);
        }

        public static void SerializeAudioMsg(this NetworkWriter writer, ThrowableItemAudioMessage value)
        {
            writer.WriteUShort(value.Serial);
            writer.WriteByte((byte)value.Request);
        }

        public static ThrowableItemAudioMessage DeserializeAudioMsg(this NetworkReader reader)
        {
            return new ThrowableItemAudioMessage(reader.ReadUShort(), (RequestType)reader.ReadByte());
        }
    }
}