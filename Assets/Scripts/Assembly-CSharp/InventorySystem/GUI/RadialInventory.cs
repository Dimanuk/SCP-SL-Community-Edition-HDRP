using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using InventorySystem.GUI.Descriptions;
using InventorySystem.Items;
using PlayerRoles;
using RadialMenus;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.GUI
{
    public class RadialInventory : RadialMenuBase, IInventoryGuiDisplayType
    {

        [Serializable]
        public struct ItemSlot
        {
            [SerializeField] private RawImage _iconSlot;

            public void UpdateVisuals(ItemBase item)
            {
                if (_iconSlot == null)
                    return;

                _iconSlot.enabled = item != null;
                if (item != null)
                {

                    _iconSlot.texture = item.Icon;
                }
            }
        }


        [SerializeField] private ItemSlot[] _slots;
        [SerializeField] private RadialDescriptionBase[] _descriptionTypes;
        [SerializeField] private CanvasGroup _descriptionGroup;
        [SerializeField] private RawImage _dragCursorIcon;
        [SerializeField] private Image _cursorDropIcon;
        [SerializeField] private AmmoElement _ammoElementTemplate;

        [SerializeField] private RoleAccentColor _circleColor;
        [SerializeField] private RoleAccentColor _highlightColor;
        [SerializeField] private RoleAccentColor _heldColor;
        [SerializeField] private RoleAccentColor _blockedColor;
        [SerializeField] private RoleAccentColor _wornColor;


        public readonly ushort[] OrganizedContent = new ushort[8];

        private static readonly Stopwatch DragWatch = new();

        private readonly Dictionary<ItemType, AmmoElement> _organizedAmmo =
            new Dictionary<ItemType, AmmoElement>();

        private int _draggedId = -1;

        private ushort _highlightedSerial;
        private ushort _visibleDescriptionSerial;

        private const byte DescriptionFadeSpeed = 15;
        private const byte TransitionSpeed = 15;

        private Vector2 _originalDragPosition;
        private RoleTypeId _prevRole;

        public override int Slots => _slots.Length;

        private static bool AllowDropping =>
            DragWatch.Elapsed.TotalMilliseconds > 200.0;

        public void InventoryToggled(bool newState)
        {
            _originalDragPosition = Vector3.zero;
            if (newState)
            {
                _descriptionGroup.alpha = 0f;
                return;
            }

            _draggedId = -1;
            _dragCursorIcon.enabled = false;
            _cursorDropIcon.enabled = false;
        }

        public InventoryGuiAction DisplayAndSelectItems(Inventory targetInventory, out ushort itemSerial)
        {
            bool isInventoryNull = targetInventory == null;
            RingImage.color = _circleColor.Color;

            try
            {
                RefreshItemColors(targetInventory, isInventoryNull);
                RefreshDescriptions(targetInventory, isInventoryNull);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error in RadialInventory: {(isInventoryNull ? "null" : targetInventory.isLocalPlayer.ToString())}");
                UnityEngine.Debug.LogException(ex);
            }

            itemSerial = _highlightedSerial;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                HandleDragStart(targetInventory, isInventoryNull);
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                return HandleDragEnd(out itemSerial);
            }

            UpdateDragVisuals();

            if (Input.GetKeyDown(KeyCode.Mouse1))
                return InventoryGuiAction.Drop;

            return InventoryGuiAction.None;
        }

        private void HandleDragStart(Inventory inv, bool isNull)
        {
            _draggedId = HighlightedSlot;
            if (_draggedId >= 0 && !isNull && inv.UserInventory.Items.TryGetValue(OrganizedContent[_draggedId], out var item))
            {
                _originalDragPosition = Input.mousePosition;
                _dragCursorIcon.texture = item.Icon;
                _dragCursorIcon.transform.position = Input.mousePosition;
                _cursorDropIcon.color = RingImage.color;
            }
            else
            {
                _originalDragPosition = Vector2.zero;
            }
        }

        private InventoryGuiAction HandleDragEnd(out ushort itemSerial)
        {
            itemSerial = _highlightedSerial;
            _dragCursorIcon.enabled = false;
            _cursorDropIcon.enabled = false;
            _originalDragPosition = Vector3.zero;

            if (_draggedId >= 0 && OrganizedContent[_draggedId] != 0 && HighlightedSlot < 0 && AllowDropping)
            {
                itemSerial = OrganizedContent[_draggedId];
                return InventoryGuiAction.Drop;
            }

            if (HighlightedSlot == _draggedId || Mathf.Min(HighlightedSlot, _draggedId) < 0 || OrganizedContent[_draggedId] == 0)
            {
                if (_organizedAmmo.Any(x => x.Value.IsHovering()))
                    return InventoryGuiAction.None;

                InventoryGuiController.InventoryVisible = false;
                return InventoryGuiAction.Select;
            }

            ushort temp = OrganizedContent[HighlightedSlot];
            OrganizedContent[HighlightedSlot] = OrganizedContent[_draggedId];
            OrganizedContent[_draggedId] = temp;

            return InventoryGuiAction.None;
        }

        private void UpdateDragVisuals()
        {
            if (_dragCursorIcon.enabled)
            {
                _dragCursorIcon.transform.position = Input.mousePosition;
                _cursorDropIcon.enabled = !InRingRange(out _) && AllowDropping;
            }
            else if (_draggedId >= 0 && _originalDragPosition != Vector2.zero && Vector2.Distance(_originalDragPosition, Input.mousePosition) > 5f && OrganizedContent[_draggedId] > 0)
            {
                DragWatch.Restart();
                _dragCursorIcon.enabled = true;
                _cursorDropIcon.enabled = false;
            }
        }
        public void ItemsModified(Inventory targetInventory)
        {
            for (int i = 0; i < OrganizedContent.Length; i++)
            {
                if (OrganizedContent[i] > 0 && !targetInventory.UserInventory.Items.ContainsKey(OrganizedContent[i]))
                    OrganizedContent[i] = 0;
            }

            foreach (var pair in targetInventory.UserInventory.Items)
            {
                if (!OrganizedContent.Contains(pair.Key))
                {
                    for (int i = 0; i < OrganizedContent.Length; i++)
                    {
                        if (OrganizedContent[i] == 0)
                        {
                            OrganizedContent[i] = pair.Key;
                            break;
                        }
                    }
                }
            }
        }


        public void AmmoModified(ReferenceHub hub)
        {
            var currentRole = hub.roleManager.CurrentRole;
            bool roleChanged = _prevRole != currentRole.RoleTypeId;

            if (roleChanged)
            {
                _prevRole = currentRole.RoleTypeId;
                foreach (var ammo in _organizedAmmo.Values) Destroy(ammo.gameObject);
                _organizedAmmo.Clear();
            }
            else
            {
                foreach (var ammo in _organizedAmmo.Values) ammo.gameObject.SetActive(false);
            }

            foreach (var pair in hub.inventory.UserInventory.ReserveAmmo)
            {
                if (!_organizedAmmo.TryGetValue(pair.Key, out var ammoElement))
                {
                    ammoElement = Instantiate(_ammoElementTemplate, _ammoElementTemplate.transform.parent);
                    ammoElement.transform.localScale = Vector3.one;
                    ammoElement.Setup(pair.Key, currentRole.RoleColor);
                    _organizedAmmo[pair.Key] = ammoElement;
                }
                ammoElement.UpdateAmount(pair.Value);
            }
        }

        private void RefreshItemColors(Inventory inv, bool isNull)
        {
            _highlightedSerial = 0;

            if (_slots == null || RingImage == null || _dragCursorIcon == null)
                return;

            if (_circleColor == null || _highlightColor == null || _heldColor == null ||
                _blockedColor == null || _wornColor == null)
            {
                UnityEngine.Debug.LogWarning("[RadialInventory] Один из RoleAccentColor не назначен в Inspector!");
                return;
            }

            int slotCount = _slots.Length;

            for (int i = 0; i < slotCount; i++)
            {
                bool isHighlighted = (_dragCursorIcon.enabled ? _draggedId : HighlightedSlot) == i;
                Color targetColor;

                if (OrganizedContent[i] > 0 && !isNull &&
                    inv.UserInventory.Items.TryGetValue(OrganizedContent[i], out var item))
                {
                    bool isCurrent = OrganizedContent[i] == inv.CurItem.SerialNumber;

                    Color stateColor;
                    if (item is IWearableItem wearable && wearable.IsWorn)
                        stateColor = _wornColor.Color;
                    else if (item is IEquipDequipModifier modifier && !modifier.AllowEquip)
                        stateColor = _blockedColor.Color;
                    else
                        stateColor = Color.clear;

                    targetColor = (isCurrent || isHighlighted)
                        ? Color.Lerp(_heldColor.Color, _highlightColor.Color, isHighlighted ? (isCurrent ? 0.5f : 1f) : 0f)
                        : stateColor;

                    _slots[i].UpdateVisuals(item);
                    if (isHighlighted)
                        _highlightedSerial = OrganizedContent[i];
                }
                else
                {
                    // ISIL: `test invNull, isHighlighted` — the highlight ring lights up in this
                    // branch only when isNull is TRUE. This branch is reached for empty slots
                    // (OrganizedContent==0) or when no inventory was passed (isNull); real items
                    // are handled in the if-branch above. The isNull path is the SCP-330 candy bag,
                    // which drives its slots via OrganizedContent[i] = i+1 with a null inventory —
                    // so its selected candy must be highlighted here. (A `!isNull` here, copied from
                    // the v13 reference, left the candy bag with no visible selection highlight.)
                    if (isNull && isHighlighted && OrganizedContent[i] > 0)
                    {
                        targetColor = _highlightColor.Color;
                        _highlightedSerial = OrganizedContent[i];
                    }
                    else
                    {
                        targetColor = Color.clear;
                    }

                    if (!isNull)
                        _slots[i].UpdateVisuals(null);
                }

                Image highlight = GetHighlightSafe(i);
                if (highlight != null)
                {
                    highlight.color = Color.Lerp(highlight.color, targetColor, 15 * Time.deltaTime);
                }
            }
        }

        private void RefreshDescriptions(Inventory inv, bool invNull)
        {
            if (invNull)
                return;

            ushort highlighted = _highlightedSerial;

            if (_visibleDescriptionSerial == highlighted &&
                inv.UserInventory.Items.TryGetValue(highlighted, out ItemBase item))
            {
                for (int i = 0; i < _descriptionTypes.Length; i++)
                {
                    RadialDescriptionBase desc = _descriptionTypes[i];
                    bool matches = desc.DescriptionType == item.DescriptionType;
                    desc.gameObject.SetActive(matches);

                    if (matches)
                        desc.UpdateInfo(item, _circleColor.Color);
                }

                _descriptionGroup.alpha = Mathf.Clamp01(
                    _descriptionGroup.alpha + Time.deltaTime * DescriptionFadeSpeed);

                return;
            }

            if (_descriptionGroup.alpha <= 0f)
                _visibleDescriptionSerial = highlighted;

            _descriptionGroup.alpha = Mathf.Clamp01(
                _descriptionGroup.alpha - Time.deltaTime * DescriptionFadeSpeed);
        }
    }
}