using System.Collections.Generic;
using System.Text;
using InventorySystem.Items.Firearms;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InventorySystem.Items.Firearms.Attachments
{
	public static class AttachmentsServerHandler
	{
        public static readonly Dictionary<ReferenceHub, Dictionary<ItemType, uint>> PlayerPreferences = new();

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            global::InventorySystem.Inventory.OnServerStarted += RegisterNetworkHandlers;
            global::UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ResetOnSceneChange;
            global::InventorySystem.InventoryItemProvider.OnItemProvided = (global::System.Action<ReferenceHub, global::InventorySystem.Items.ItemBase>)global::System.Delegate.Combine(global::InventorySystem.InventoryItemProvider.OnItemProvided, new global::System.Action<ReferenceHub, global::InventorySystem.Items.ItemBase>(SetupProvidedWeapon));
        }

        private static void RegisterNetworkHandlers()
        {
            global::Mirror.NetworkServer.ReplaceHandler<global::InventorySystem.Items.Firearms.Attachments.AttachmentsSetupPreference>(ServerReceivePreference);
            global::Mirror.NetworkServer.ReplaceHandler<global::InventorySystem.Items.Firearms.Attachments.AttachmentsChangeRequest>(ServerReceiveChangeRequest);
        }

        private static void ServerReceiveChangeRequest(global::Mirror.NetworkConnection conn, global::InventorySystem.Items.Firearms.Attachments.AttachmentsChangeRequest msg)
        {
            if (!global::Mirror.NetworkServer.active || !ReferenceHub.TryGetHub(conn.identity.gameObject, out var hub) || !(hub.inventory.CurInstance is global::InventorySystem.Items.Firearms.Firearm firearm) || hub.inventory.CurItem.SerialNumber != msg.WeaponSerial)
            {
                return;
            }
            bool flag = hub.roleManager.CurrentRole is global::PlayerRoles.Spectating.SpectatorRole;
            if (!flag)
            {
                foreach (global::InventorySystem.Items.Firearms.Attachments.WorkstationController allWorkstation in global::InventorySystem.Items.Firearms.Attachments.WorkstationController.AllWorkstations)
                {
                    if (!(allWorkstation == null) && allWorkstation.Status == 3 && allWorkstation.IsInRange(hub))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (flag)
            {
                firearm.ApplyAttachmentsCode(msg.AttachmentsCode, reValidate: true);
                if (firearm.Status.Ammo > firearm.AmmoManagerModule.MaxAmmo)
                {
                    hub.inventory.ServerAddAmmo(firearm.AmmoType, firearm.Status.Ammo - firearm.AmmoManagerModule.MaxAmmo);
                }
                firearm.Status = new global::InventorySystem.Items.Firearms.FirearmStatus((byte)global::UnityEngine.Mathf.Min(firearm.Status.Ammo, firearm.AmmoManagerModule.MaxAmmo), firearm.Status.Flags, msg.AttachmentsCode);
            }
        }

        private static void ServerReceivePreference(global::Mirror.NetworkConnection conn, global::InventorySystem.Items.Firearms.Attachments.AttachmentsSetupPreference msg)
        {
            if (global::Mirror.NetworkServer.active && ReferenceHub.TryGetHub(conn.identity.gameObject, out var hub))
            {
                if (PlayerPreferences.TryGetValue(hub, out var value) && value != null)
                {
                    value[msg.Weapon] = msg.AttachmentsCode;
                    return;
                }
                PlayerPreferences[hub] = new global::System.Collections.Generic.Dictionary<ItemType, uint> { [msg.Weapon] = msg.AttachmentsCode };
            }
        }

        private static void SetupProvidedWeapon(ReferenceHub ply, global::InventorySystem.Items.ItemBase item)
        {
            if (global::Mirror.NetworkServer.active && item is global::InventorySystem.Items.Firearms.Firearm firearm)
            {
                uint value2 = 0u;
                bool hasPref = PlayerPreferences.TryGetValue(ply, out var value) && value != null && value.TryGetValue(item.ItemTypeId, out value2);
                if (!hasPref) 
                    value2 = 0u;

                uint rawCode = value2;
                value2 = firearm.ValidateAttachmentsCode(value2);

                var sb = new StringBuilder();
                uint bit = 1u;
                for (int i = 0; i < firearm.Attachments.Length; i++)
                {
                    if ((value2 & bit) == bit)
                        sb.Append($"[{i}:{firearm.Attachments[i].Slot}/{firearm.Attachments[i].Name}] ");
                    bit <<= 1;
                }


                firearm.ApplyAttachmentsCode(value2, reValidate: false);

                bool hasReserve = ply.inventory.UserInventory.ReserveAmmo.TryGetValue(firearm.AmmoType, out var value3);

                byte loadedAmmo = firearm.Status.Ammo;
                var statusFlags = firearm.Status.Flags;
                if (hasReserve)
                {
                    int num = global::UnityEngine.Mathf.Min(value3, firearm.AmmoManagerModule.MaxAmmo);
                    ply.inventory.UserInventory.ReserveAmmo[firearm.AmmoType] = (ushort)(value3 - num);
                    loadedAmmo = (byte)num;
                    statusFlags |= global::InventorySystem.Items.Firearms.FirearmStatusFlags.MagazineInserted;
                    bool flag = firearm.HasAdvantageFlag(global::InventorySystem.Items.Firearms.Attachments.AttachmentDescriptiveAdvantages.Flashlight);
                    if (flag)
                        statusFlags |= global::InventorySystem.Items.Firearms.FirearmStatusFlags.FlashlightEnabled;
                }
                var newStatus = new global::InventorySystem.Items.Firearms.FirearmStatus(loadedAmmo, statusFlags, value2);
                firearm.Status = newStatus;
            }
        }

        private static void ResetOnSceneChange(global::UnityEngine.SceneManagement.Scene arg1, global::UnityEngine.SceneManagement.Scene arg2)
        {
            PlayerPreferences.Clear();
        }
    }
}
