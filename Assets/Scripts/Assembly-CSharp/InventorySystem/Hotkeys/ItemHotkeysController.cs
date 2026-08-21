using System;
using System.Diagnostics;
using InventorySystem.GUI;
using InventorySystem.Items;
using Mirror;
using PlayerRoles;
using UnityEngine;

namespace InventorySystem.Hotkeys
{
    public class ItemHotkeysController : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _fadeGroup;

        [SerializeField]
        private HotkeyIconBase[] _hotkeyIcons;

        private static readonly IHotkeyItemSelector[] DefinedHotkeys;

        private int _smartFeaturesConfig;

        private readonly Stopwatch _stopwatch;

        private const float FadeoutTime = 5.5f;
        private const float FadeoutMaxSpeed = 1.2f;
        private const float FadeoutMinSpeed = 0.05f;
        private const float FadeinSpeed = 2f;

        private bool SmartFeaturesEnabled
        {
            get
            {
                if (_smartFeaturesConfig == 0)
                {
                    bool enabled = PlayerPrefsSl.Get("InventorySmartHotkeys", true);
                    _smartFeaturesConfig = enabled ? 2 : 1;
                }

                return _smartFeaturesConfig == 2;
            }
        }

        static ItemHotkeysController()
        {
            DefinedHotkeys = new IHotkeyItemSelector[5];

            // [0] Keycard (subscribes to inventory events in its own constructor)
            DefinedHotkeys[0] = new KeycardHotkey();

            // [1] Primary firearm
            DefinedHotkeys[1] = new FirearmHotkey(true);

            // [2] Secondary firearm
            DefinedHotkeys[2] = new FirearmHotkey(false);

            // [3] Medical items, best to worst
            DefinedHotkeys[3] = new SpecialItemHotkey(ActionName.HotkeyMedical,
                ItemType.SCP500,
                ItemType.Adrenaline,
                ItemType.Medkit,
                ItemType.Painkillers);

            // [4] Grenades, best to worst
            DefinedHotkeys[4] = new SpecialItemHotkey(ActionName.HotkeyGrenade,
                ItemType.GrenadeHE,
                ItemType.GrenadeFlash,
                ItemType.SCP018,
                ItemType.SCP2176);
        }

        private void Start()
        {
            PlayerRoleManager.OnRoleChanged += UpdateColors;
        }

        private void OnDestroy()
        {
            PlayerRoleManager.OnRoleChanged -= UpdateColors;
        }

        private void UpdateColors(ReferenceHub rh, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            if (!rh.isLocalPlayer)
                return;

            Color teamColor = newRole.RoleColor;
            foreach (var icon in _hotkeyIcons)
            {
                if (icon != null)
                    icon.SetColors(teamColor);
            }
        }

        private void RefreshAlpha()
        {
            if (_fadeGroup == null)
                return;

            float alpha = _fadeGroup.alpha;
            float elapsed = (float)_stopwatch.Elapsed.TotalSeconds;

            if (elapsed > FadeoutTime)
            {
                alpha -= Time.deltaTime * (alpha + FadeoutMinSpeed) * FadeoutMaxSpeed;
            }
            else
            {
                alpha += Time.deltaTime * FadeinSpeed;
            }

            alpha = Mathf.Clamp01(alpha);

            if (_fadeGroup.alpha != alpha)
                _fadeGroup.alpha = alpha;
        }

        private void Update()
        {
            if (!ReferenceHub.TryGetLocalHub(out ReferenceHub hub))
                return;

            if (!NetworkClient.active)
                return;

            RefreshAlpha();

            bool hidden = InventoryGuiController.InventoryVisible || !InventoryGuiController.CanInventoryBeDisplayed();

            if (InventoryGuiController.DisplayController is not RadialInventory radial)
                throw new Exception("The hotkeys do not support non-standard RadialInventory.");

            for (int i = 0; i < DefinedHotkeys.Length; i++)
            {
                var selector = DefinedHotkeys[i];
                var icon = _hotkeyIcons[i];

                ushort serial = selector.GetCorrespondingItemSerial(hub, radial.OrganizedContent, SmartFeaturesEnabled);
                KeyCode key = NewInput.GetKey(selector.KeyActionName);

                if (serial == 0)
                {
                    icon.CancelHighlightAnimation();
                }
                else
                {
                    if (Input.GetKeyDown(key) && InventoryGuiController.ItemsSafeForInteraction)
                    {
                        if (!hidden)
                        {
                            icon.PlayHighlightAnimation();
                            _stopwatch.Restart();
                        }

                        if (serial != hub.inventory.CurItem.SerialNumber)
                            hub.inventory.CmdProcessHotkey(selector.KeyActionName, serial);
                    }

                    icon.UpdateAnimations();
                }

                if (icon.Setup(serial, key, hidden))
                {
                    _stopwatch.Restart();
                }
            }
        }

        public ItemHotkeysController()
        {
            _stopwatch = new Stopwatch();
        }
    }
}
