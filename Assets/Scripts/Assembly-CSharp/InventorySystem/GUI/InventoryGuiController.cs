using InventorySystem.Disarming;
using InventorySystem.Items;
using Mirror;
using PlayerRoles;
using System;
using System.Diagnostics;
using ToggleableMenus;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace InventorySystem.GUI
{
    public class InventoryGuiController : ToggleableMenuBase
    {
        public static InventoryGuiController Singleton;

        private static readonly Stopwatch CooldownStopwatch = new Stopwatch();

        private static readonly byte InventoryFadeSpeed = 10;

        [SerializeField] private CanvasGroup _toggleablePart;
        [SerializeField] private RadialInventory _displaySettings;
        [SerializeField] private PostProcessVolume _menuBlur;

        private bool _prevVisible;

        public static bool InventoryVisible
        {
            get => Singleton != null && Singleton.IsEnabled;
            set
            {
                if (Singleton != null)
                    Singleton.IsEnabled = value;
            }
        }

        public static bool ItemsSafeForInteraction
        {
            get
            {
                if (Singleton == null || Singleton._toggleablePart == null)
                    return false;

                if (Singleton._toggleablePart.alpha > 0f)
                    return false;

                if (Cursor.visible)
                {
                    if (!CooldownStopwatch.IsRunning)
                        CooldownStopwatch.Restart();
                    return false;
                }

                if (CooldownStopwatch.IsRunning)
                {
                    if (CooldownStopwatch.Elapsed.TotalSeconds > 0.1d)
                    {
                        CooldownStopwatch.Stop();
                        return true;
                    }
                    return false;
                }

                return true;
            }
        }

        public override bool CanToggle => IsEnabled || CanInventoryBeDisplayed();

        public static IInventoryGuiDisplayType DisplayController => Singleton != null ? Singleton._displaySettings : null;

        private static Inventory UserInventory => ReferenceHub.LocalHub != null ? ReferenceHub.LocalHub.inventory : null;

        private void ItemsModified(ReferenceHub hub)
        {
            if (hub.isLocalPlayer && _displaySettings != null)
            {
                _displaySettings.ItemsModified(hub.inventory);
            }
        }

        private void AmmoModified(ReferenceHub hub)
        {
            if (hub.isLocalPlayer && _displaySettings != null)
            {
                _displaySettings.AmmoModified(hub);
            }
        }

        private void RoleChanged(ReferenceHub hub, PlayerRoleBase oldRole, PlayerRoleBase newRole)
        {
            if (hub.isLocalPlayer)
            {
                if (IsEnabled)
                    IsEnabled = false;

                if (_displaySettings != null)
                {
                    _displaySettings.AmmoModified(hub);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Singleton = this;
            IsEnabled = false;

            Inventory.OnItemsModified += ItemsModified;
            Inventory.OnAmmoModified += AmmoModified;
            PlayerRoleManager.OnRoleChanged += RoleChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Inventory.OnItemsModified -= ItemsModified;
            Inventory.OnAmmoModified -= AmmoModified;
            PlayerRoleManager.OnRoleChanged -= RoleChanged;

            Singleton = null;
        }

        private void Update()
        {
            RefreshAnimations(false);

            if (InventoryVisible)
            {
                if (!CanInventoryBeDisplayed())
                {
                    IsEnabled = false;
                }

                if (_displaySettings != null)
                {
                    ushort itemSerial;
                    InventoryGuiAction action = _displaySettings.DisplayAndSelectItems(UserInventory, out itemSerial);

                    if (action == InventoryGuiAction.Drop)
                    {
                        UserInventory.CmdDropItem(itemSerial, false);
                    }
                    else if (action == InventoryGuiAction.Select)
                    {
                        UserInventory.CmdSelectItem(itemSerial);
                    }
                }
            }

            if (_prevVisible != InventoryVisible)
            {
                if (InventoryVisible && _displaySettings != null)
                {
                    _displaySettings.InventoryToggled(true);
                }
                _prevVisible = InventoryVisible;
            }
        }

        private void RefreshAnimations(bool forceNoAnimations)
        {
            if (_toggleablePart == null) return;

            bool isVisible = InventoryVisible;
            GameObject go = _toggleablePart.gameObject;

            if (!isVisible)
            {
                if (go.activeSelf)
                {
                    if (forceNoAnimations)
                    {
                        _toggleablePart.alpha = 0f;
                        go.SetActive(false);
                    }
                    else
                    {
                        _toggleablePart.alpha -= Time.deltaTime * InventoryFadeSpeed;
                        if (_toggleablePart.alpha <= 0f)
                        {
                            _toggleablePart.alpha = 0f;
                            go.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                if (!go.activeSelf) go.SetActive(true);

                if (forceNoAnimations)
                {
                    _toggleablePart.alpha = 1f;
                }
                else
                {
                    if (_toggleablePart.alpha < 1f)
                    {
                        _toggleablePart.alpha += Time.deltaTime * InventoryFadeSpeed;
                        if (_toggleablePart.alpha > 1f) _toggleablePart.alpha = 1f;
                    }
                }
            }
        }

        public static bool CanInventoryBeDisplayed()
        {
            ReferenceHub localHub;
            if (!ReferenceHub.TryGetLocalHub(out localHub)) return false;

            if (!localHub.IsHuman())
                return false;

            if (localHub.inventory.IsDisarmed()) return false;

            if (UserInventory == null) return false;

            // The current item may forbid holstering (e.g. a jailbird mid-charge).
            if (UserInventory.CurInstance is InventorySystem.Items.IEquipDequipModifier modifier && !modifier.AllowHolster)
                return false;

            // Interaction blockers (e.g. SeveredHands) must be respected even with empty
            // hands — the original checks this unconditionally, not only while holding an item.
            return !localHub.interCoordinator.AnyBlocker(BlockedInteraction.OpenInventory);
        }

        protected override void OnToggled()
        {
            RefreshAnimations(!CanInventoryBeDisplayed());
        }
    }
}