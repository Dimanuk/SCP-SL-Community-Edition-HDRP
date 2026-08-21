using Mirror;
using PlayerRoles;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InventorySystem.Items.Firearms.Attachments
{
    public class SpectatorAttachmentSelector : AttachmentSelectorBase
    {
        [SerializeField]
        public SpectatorSelectorFirearmButton _firearmButton;

        [SerializeField]
        public GameObject _summaryButton;

        [SerializeField]
        public float _rescaleSpeed = 10f;

        // The spectator has no live weapon, so a hidden clone of the template firearm is
        // instantiated to serve as the edit target. RefreshState assigns it to SelectedFirearm,
        // which is what the whole editor (including AttachmentPresetSelector) operates on.
        private Firearm _instantiatedFirearm;

        public override bool UseLookatMode { get; set; }

        public override void SelectAttachmentId(byte attachmentId)
        {
            for (int i = 0; i < base.SelectedFirearm.Attachments.Length; i++)
            {
                if (base.SelectedFirearm.Attachments[i].Slot == SelectedSlot && base.SelectedFirearm.Attachments[i].IsEnabled)
                {
                    base.SelectedFirearm.Attachments[i].IsEnabled = false;
                    break;
                }
            }

            base.SelectedFirearm.Attachments[attachmentId].IsEnabled = true;
            base.SelectedFirearm.ApplyAttachmentsCode(base.SelectedFirearm.GetCurrentAttachmentsCode(), reValidate: true);
            AttachmentPreferences.SetPreset(base.SelectedFirearm.ItemTypeId, 0);
            ResendPreference();
        }

        public override void LoadPreset(uint loadedCode)
        {
            base.SelectedFirearm.ApplyAttachmentsCode(loadedCode, reValidate: true);
            ResendPreference();
        }

        public override void RegisterAction(RectTransform t, Action<Vector2> action)
        {
            EventTrigger eventTrigger = t.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            entry.callback.AddListener(delegate (BaseEventData x)
            {
                Vector2 vector = (x as PointerEventData).position - (Vector2)t.position;
                Vector2 vector2 = t.sizeDelta / 2f;
                Vector2 obj = vector / vector2;
                action?.Invoke(obj);
            });
            eventTrigger.triggers.Add(entry);
        }

        public void SelectFirearm(Firearm fa)
        {
            bool hadInstance = _instantiatedFirearm != null;
            bool faNull = fa == null;

            // Already showing this firearm type — nothing to rebuild.
            if (hadInstance && !faNull && _instantiatedFirearm.ItemTypeId == fa.ItemTypeId)
                return;

            _summaryButton.SetActive(!faNull);

            if (hadInstance)
                UnityEngine.Object.Destroy(_instantiatedFirearm.gameObject);

            if (faNull)
            {
                RefreshState(null, null);
                return;
            }

            _instantiatedFirearm = UnityEngine.Object.Instantiate(fa);
            _instantiatedFirearm.SimulatedInstanceMode = true;
            _instantiatedFirearm.ApplyAttachmentsCode(fa.GetSavedPreferenceCode(), reValidate: true);
            _instantiatedFirearm.OnAdded(null);
            RefreshState(_instantiatedFirearm, null);
        }

        public void ResendPreference()
        {
            base.SelectedFirearm.SavePreferenceCode();
            AttachmentsSetupPreference message = default(AttachmentsSetupPreference);
            message.Weapon = base.SelectedFirearm.ItemTypeId;
            message.AttachmentsCode = base.SelectedFirearm.GetCurrentAttachmentsCode();
            NetworkClient.Send(message);
        }

        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            Inventory.OnLocalClientStarted += SendPreferences;
            PlayerRoleManager.OnRoleChanged += OnRoleChanged;
        }

        public void Start()
        {
            bool flag = true;
            foreach (KeyValuePair<ItemType, ItemBase> availableItem in InventoryItemLoader.AvailableItems)
            {
                if (availableItem.Value is Firearm firearm && firearm.Attachments.Length > 1)
                {
                    SpectatorSelectorFirearmButton obj = (flag ? _firearmButton : UnityEngine.Object.Instantiate(_firearmButton, _firearmButton.transform.parent));
                    flag = false;
                    obj.Setup(this, firearm);
                }
            }
        }

        public void OnEnable()
        {
            ShowStats(-1);
            SelectFirearm(null);
        }

        public void Update()
        {
            if (SelectedFirearm != null)
            {
                LerpRects(Time.deltaTime * _rescaleSpeed);

                bool anyHovered = false;

                foreach (var slot in SlotsPool)
                {
                    if (slot != null && slot is SpectatorSelectorCollider collider)
                    {
                        collider.UpdateColors(SelectedSlot);
                        anyHovered |= collider.IsHovered;
                    }
                }

                foreach (var selectable in SelectableAttachmentsPool)
                {
                    if (selectable != null && selectable is SpectatorSelectorCollider collider)
                    {
                        collider.UpdateColors(SelectedSlot);
                        anyHovered |= collider.IsHovered;
                    }
                }

                if (_isCorrectAttachment && !anyHovered)
                {
                    ShowStats(-1);
                }

                RefreshState(SelectedFirearm, null);
            }
        }

        public static void OnRoleChanged(ReferenceHub hub, PlayerRoleBase oldRole, PlayerRoleBase newRole)
        {
            if (hub.isLocalPlayer && !hub.IsAlive() && NetworkClient.active)
            {
                SendPreferences();
            }
        }

        public static void SendPreferences()
        {
            foreach (ItemBase value in InventoryItemLoader.AvailableItems.Values)
            {
                if (value is Firearm weapon)
                {
                    AttachmentsSetupPreference message = default;
                    message.Weapon = value.ItemTypeId;
                    message.AttachmentsCode = weapon.GetSavedPreferenceCode();
                    NetworkClient.Send(message);
                }
            }
        }
    }
}
