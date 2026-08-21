using System;
using System.Collections.Generic;

using Mirror;
using UnityEngine;

namespace InventorySystem.Items.Firearms.BasicMessages
{
    public static class FirearmBasicMessagesHandler
    {
        public static readonly Dictionary<ushort, FirearmStatus> ReceivedStatuses = new ();

        private static readonly HashSet<int> AlreadyRequestedStatuses = new();

        public static event Action<StatusMessage> OnStatusMessageReceived;


        public static event Action<ReferenceHub> OnStatusRequested;


        public static event Action<RequestMessage> OnClientConfirmationReceived;

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += RegisterHandlers;
            global::InventorySystem.Inventory.OnLocalClientStarted += delegate
            {
                global::Mirror.NetworkClient.Send(new global::InventorySystem.Items.Firearms.BasicMessages.RequestMessage(0, global::InventorySystem.Items.Firearms.BasicMessages.RequestType.RequestStatuses));
            };
        }

        private static void RegisterHandlers()
        {
            ReceivedStatuses.Clear();
            AlreadyRequestedStatuses.Clear();
            global::Mirror.NetworkClient.ReplaceHandler<global::InventorySystem.Items.Firearms.BasicMessages.StatusMessage>(ClientStatusMessageReceived);
            global::Mirror.NetworkClient.ReplaceHandler<global::InventorySystem.Items.Firearms.BasicMessages.RequestMessage>(ClientConfirmationReceived);
            global::Mirror.NetworkServer.ReplaceHandler<global::InventorySystem.Items.Firearms.BasicMessages.ShotMessage>(ServerShotReceived);
            global::Mirror.NetworkServer.ReplaceHandler<global::InventorySystem.Items.Firearms.BasicMessages.RequestMessage>(ServerRequestReceived);
        }

        private static void ClientConfirmationReceived(RequestMessage msg)
        {
            OnClientConfirmationReceived?.Invoke(msg);

            if (!ReferenceHub.TryGetLocalHub(out ReferenceHub hub))
                return;

            if (hub.inventory == null || !hub.inventory.UserInventory.Items.TryGetValue(msg.Serial, out var itemBase))
                return;

            if (itemBase is not Firearm firearm || firearm.AmmoManagerModule == null)
                return;

            switch (msg.Request)
            {
                case RequestType.Reload:
                    firearm.AmmoManagerModule.ClientReload();
                    break;

                case RequestType.Unload:
                    firearm.AmmoManagerModule.ClientUnload();
                    break;
            }
        }

        private static void ServerShotReceived(global::Mirror.NetworkConnection conn, global::InventorySystem.Items.Firearms.BasicMessages.ShotMessage msg)
        {
            if (ReferenceHub.TryGetHub(conn.identity.gameObject, out var hub) && msg.ShooterWeaponSerial == hub.inventory.CurItem.SerialNumber && hub.inventory.CurInstance is global::InventorySystem.Items.Firearms.Firearm firearm && firearm.ActionModule.ServerAuthorizeShot())
            {
                if (global::CustomPlayerEffects.SpawnProtected.CheckPlayer(hub) && !global::CustomPlayerEffects.SpawnProtected.CanShoot)
                {
                    hub.playerEffectsController.DisableEffect<global::CustomPlayerEffects.SpawnProtected>();
                }
                firearm.HitregModule.ServerProcessShot(msg);
                // Local player already called OnWeaponShot() client-side (predictive) — skip to avoid double Fire trigger on listen server
                if (!hub.isLocalPlayer)
                    firearm.OnWeaponShot();
            }
        }

        private static void ServerRequestReceived(global::Mirror.NetworkConnection conn, global::InventorySystem.Items.Firearms.BasicMessages.RequestMessage msg)
        {
            if (!ReferenceHub.TryGetHub(conn.identity.gameObject, out var hub))
            {
                return;
            }
            if (msg.Request == global::InventorySystem.Items.Firearms.BasicMessages.RequestType.RequestStatuses && AlreadyRequestedStatuses.Add(hub.PlayerId))
            {
                foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
                {
                    global::InventorySystem.Inventory inventory = allHub.inventory;
                    if (inventory.CurInstance is global::InventorySystem.Items.Firearms.Firearm firearm && !(inventory.CurInstance == null))
                    {
                        conn.Send(new global::InventorySystem.Items.Firearms.BasicMessages.StatusMessage(inventory.CurItem.SerialNumber, firearm.Status));
                    }
                }
                global::InventorySystem.Items.Firearms.BasicMessages.FirearmBasicMessagesHandler.OnStatusRequested?.Invoke(hub);
            }
            if (msg.Serial != hub.inventory.CurItem.SerialNumber || !(hub.inventory.CurInstance is global::InventorySystem.Items.Firearms.Firearm firearm2))
            {
                return;
            }
            switch (msg.Request)
            {
                case global::InventorySystem.Items.Firearms.BasicMessages.RequestType.Reload:
                    if (firearm2.AmmoManagerModule.ServerTryReload())
                        global::Utils.Networking.NetworkUtils.SendToAuthenticated(msg);
                    break;
                case global::InventorySystem.Items.Firearms.BasicMessages.RequestType.Unload:
                    if (firearm2.AmmoManagerModule.ServerTryUnload())
                        global::Utils.Networking.NetworkUtils.SendToAuthenticated(msg);
                    break;
                case global::InventorySystem.Items.Firearms.BasicMessages.RequestType.Dryfire:
                    if (firearm2.ActionModule.ServerAuthorizeDryFire())
                    {
                        global::Utils.Networking.NetworkUtils.SendToAuthenticated(msg);
                        firearm2.OnWeaponDryfired();
                    }
                    break;
                case global::InventorySystem.Items.Firearms.BasicMessages.RequestType.AdsIn:
                    firearm2.AdsModule.ServerAds = true;
                    global::Utils.Networking.NetworkUtils.SendToAuthenticated(msg);
                    break;
                case global::InventorySystem.Items.Firearms.BasicMessages.RequestType.AdsOut:
                    firearm2.AdsModule.ServerAds = false;
                    global::Utils.Networking.NetworkUtils.SendToAuthenticated(msg);
                    break;
                case global::InventorySystem.Items.Firearms.BasicMessages.RequestType.ToggleFlashlight:
                    {
                        bool currentFlashlightState = firearm2.Status.Flags.HasFlagFast(global::InventorySystem.Items.Firearms.FirearmStatusFlags.FlashlightEnabled);
                        bool newFlashlightState = !currentFlashlightState;
                        if (global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.HasAdvantageFlag(firearm2, global::InventorySystem.Items.Firearms.Attachments.AttachmentDescriptiveAdvantages.Flashlight))
                        {
                            firearm2.Status = new global::InventorySystem.Items.Firearms.FirearmStatus(firearm2.Status.Ammo, global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.OverrideFlashlightFlags(firearm2, newFlashlightState), firearm2.Status.Attachments);
                        }
                        break;
                    }
                case global::InventorySystem.Items.Firearms.BasicMessages.RequestType.Inspect:
                    global::Utils.Networking.NetworkUtils.SendToHubsConditionally(msg, (ReferenceHub x) => x.roleManager.CurrentRole is global::PlayerRoles.Spectating.SpectatorRole);
                    break;
                case global::InventorySystem.Items.Firearms.BasicMessages.RequestType.ReloadStop:
                case global::InventorySystem.Items.Firearms.BasicMessages.RequestType.RequestStatuses:
                    break;
            }
        }

        private static void ClientStatusMessageReceived(global::InventorySystem.Items.Firearms.BasicMessages.StatusMessage msg)
        {
            ReceivedStatuses[msg.Serial] = msg.Status;
            global::InventorySystem.Items.Firearms.BasicMessages.FirearmBasicMessagesHandler.OnStatusMessageReceived?.Invoke(msg);
        }
    }
}