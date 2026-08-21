using AudioPooling;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.Items.MicroHID
{
    public static class MicroHIDHandler
    {
        public static readonly Dictionary<ushort, float> SyncEnergy = new Dictionary<ushort, float>();
        public static readonly Dictionary<ushort, HidStatusMessage> SyncStates = new Dictionary<ushort, HidStatusMessage>();

        private static readonly Dictionary<int, AudioSource> Sources = new Dictionary<int, AudioSource>();

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            Inventory.OnServerStarted += RegisterServerHandlers;
            CustomNetworkManager.OnClientReady += RegisterClientHandlers;
        }

        private static void RegisterServerHandlers()
        {
            NetworkServer.ReplaceHandler<HidStatusMessage>(ServerReceiveKey, true);
        }

        private static void ServerReceiveKey(NetworkConnection conn, HidStatusMessage msg)
        {
            if (!ReferenceHub.TryGetHub(conn.identity.gameObject, out ReferenceHub hub))
                return;

            if (hub.inventory.CurItem.SerialNumber != msg.Serial)
                return;

            if (!hub.inventory.UserInventory.Items.TryGetValue(msg.Serial, out ItemBase itemBase) ||
                itemBase is not MicroHIDItem microHIDItem)
                return;

            if (Enum.IsDefined(typeof(HidUserInput), msg.MessageCode))
            {
                microHIDItem.UserInput = (HidUserInput)msg.MessageCode;
            }
        }

        private static void RegisterClientHandlers()
        {
            NetworkClient.ReplaceHandler<HidStatusMessage>(ClientReceiveStatus, true);
            Sources.Clear();
        }

        private static void ClientReceiveStatus(HidStatusMessage msg)
        {
            // The template is used as the sound/clip reference: remote players have no ItemBase
            // instance on this client (Inventory.CurInstance is only populated for the local
            // player and on the server), so matching on CurItem.SerialNumber is all we can do.
            if (!InventoryItemLoader.TryGetItem(ItemType.MicroHID, out MicroHIDItem template))
                return;

            if (msg.MessageType == HidStatusMessageType.EnergySync)
            {
                ClientProcessEnergySync(msg.Serial, msg.MessageCode);
                return;
            }

            foreach (ReferenceHub hub in ReferenceHub.AllHubs)
            {
                if (hub.inventory.CurItem.SerialNumber != msg.Serial)
                    continue;

                ClientProcessStateSync(hub, hub.inventory.CurInstance as MicroHIDItem ?? template, msg);
                break;
            }
        }

        private static void ClientProcessStateSync(ReferenceHub targetPlayer, MicroHIDItem hidReference, HidStatusMessage msg)
        {   
            if (!Enum.IsDefined(typeof(HidState), msg.MessageCode) || targetPlayer == null)
            {
                return;
            }

            SyncStates[msg.Serial] = msg;
            HidState state = (HidState)msg.MessageCode;

            if (targetPlayer.inventory.CurInstance == hidReference)
            {
                hidReference.State = state;
            }

            bool hasSource = Sources.TryGetValue(msg.Serial, out AudioSource source) && source != null;

            if (state == HidState.StopSound)
            {
                if (hasSource)
                {
                    source.Stop();
                }
                return;
            }

            if (!hasSource)
            {
                source = AudioSourcePoolManager.PlaySound(null, targetPlayer.transform, MicroHIDItem.SoundMaxDistance,
                    1f, FalloffType.Exponential, AudioMixerChannelType.DefaultSfx, 1f, reserved: true);
                Sources[msg.Serial] = source;
            }

            if (source == null)
            {
                return;
            }

            source.transform.localPosition = Vector3.zero;

            AudioClip clip;
            switch (state)
            {
                case HidState.PoweringUp:
                    clip = hidReference.PowerUpClip;
                    break;
                case HidState.PoweringDown:
                    clip = (source.clip == hidReference.FireClip) ? hidReference.FireToPowerDownClip : hidReference.PowerDownClip;
                    break;
                case HidState.Primed:
                    clip = (source.clip == hidReference.FireClip) ? hidReference.FireToPrimedClip : hidReference.PrimedClip;
                    break;
                case HidState.Firing:
                    clip = hidReference.FireClip;
                    break;
                default:
                    return;
            }

            source.clip = clip;
            source.Stop();
            source.Play();
        }

        private static void ClientProcessEnergySync(ushort msgSerial, byte rawEnergy)
        {
            float normalizedEnergy = rawEnergy / 255f;

            SyncEnergy[msgSerial] = normalizedEnergy;

            // On the host the item instance is server-authoritative — don't overwrite its energy
            // with the quantized (byte) value we just echoed to ourselves.
            if (!NetworkServer.active &&
                ReferenceHub.LocalHub != null &&
                ReferenceHub.LocalHub.inventory.CurItem.SerialNumber == msgSerial &&
                ReferenceHub.LocalHub.inventory.CurInstance is MicroHIDItem microHIDItem)
            {
                microHIDItem.RemainingEnergy = normalizedEnergy;
            }
        }
    }
}