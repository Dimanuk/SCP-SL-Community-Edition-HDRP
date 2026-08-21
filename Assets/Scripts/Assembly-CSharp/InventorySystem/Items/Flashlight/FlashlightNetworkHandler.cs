using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using InventorySystem.Items.Flashlight;

namespace InventorySystem.Items.Flashlight
{
    public static class FlashlightNetworkHandler
    {
        public readonly struct FlashlightMessage : NetworkMessage
        {
            public readonly ushort Serial;
            public readonly bool NewState;

            public FlashlightMessage(ushort flashlightSerial, bool newState)
            {
                Serial = flashlightSerial;
                NewState = newState;
            }
        }

        private static readonly HashSet<uint> AlreadyRequestedFirstimeSync = new HashSet<uint>();

        public static readonly Dictionary<ushort, bool> ReceivedStatuses = new Dictionary<ushort, bool>();

        public static event Action<FlashlightMessage> OnStatusReceived;

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += RegisterHandlers;
            Inventory.OnLocalClientStarted += () =>
            {
                NetworkClient.Send(new FlashlightMessage(0, false));
            };
        }

        private static void RegisterHandlers()
        {
            if (NetworkServer.active)
            {
                NetworkServer.RegisterHandler<FlashlightMessage>(ServerProcessMessage, true);
            }
            if (NetworkClient.active)
            {
                NetworkClient.RegisterHandler<FlashlightMessage>(ClientProcessMessage, true);
            }
        }

        private static void ClientProcessMessage(FlashlightMessage msg)
        {
            ReceivedStatuses[msg.Serial] = msg.NewState;
            OnStatusReceived?.Invoke(msg);

            // Apply the server-authoritative state to the locally held flashlight so the
            // local prediction can't drift (drift was causing the toggle to need two presses).
            if (ReferenceHub.TryGetLocalHub(out ReferenceHub hub)
                && hub.inventory.CurInstance is FlashlightItem flashlight
                && flashlight.ItemSerial == msg.Serial)
            {
                flashlight.IsEmittingLight = msg.NewState;
            }
        }

        private static void ServerProcessMessage(NetworkConnection conn, FlashlightMessage msg)
        {
            if (msg.Serial == 0)
            {
                ServerProcessFirstTimeRequest(conn);
                return;
            }

            if (ReferenceHub.TryGetHub(conn.identity.gameObject, out ReferenceHub hub))
            {
                if (hub.inventory.CurInstance is FlashlightItem flashlight && flashlight.ItemSerial == msg.Serial)
                {
                    flashlight.IsEmittingLight = msg.NewState;
                    global::Utils.Networking.NetworkUtils.SendToAuthenticated(new FlashlightMessage(msg.Serial, msg.NewState));
                }
            }
        }

        private static void ServerProcessFirstTimeRequest(NetworkConnection conn)
        {
            if (!AlreadyRequestedFirstimeSync.Add(conn.identity.netId))
                return;

            foreach (ReferenceHub hub in ReferenceHub.AllHubs)
            {
                if (hub.inventory.CurInstance is FlashlightItem flashlight && !flashlight.IsEmittingLight)
                {
                    conn.Send(new FlashlightMessage(flashlight.ItemSerial, false));
                }
            }
        }

        public static void Serialize(this NetworkWriter writer, FlashlightMessage value)
        {
            writer.WriteUShort(value.Serial);
            writer.WriteBool(value.NewState);
        }

        public static FlashlightMessage Deserialize(this NetworkReader reader)
        {
            return new FlashlightMessage(reader.ReadUShort(), reader.ReadBool());
        }
    }
}