using System;
using System.Runtime.CompilerServices;
using RemoteAdmin.Settings;
using TMPro;
using UnityEngine;

namespace RemoteAdmin.Menus
{
    public class RaSettings : MonoBehaviour
    {
        private static RaSettings _singleton;

        [SerializeField] private ToggleMovementSetting _toggleMovement;
        [SerializeField] private ToggleItemOrderSetting _toggleItemOrder;
        [SerializeField] private ToggleListOrderSetting _toggleListOrder;
        [SerializeField] private ToggleSuggestionsSetting _toggleSuggestions;
        [SerializeField] private ToggleTimestampsSetting _toggleTimestamps;
        [SerializeField] private BandwidthCooldownSetting _bandwidthCooldown;
        [SerializeField] private ToggleSpawnpointSetting _toggleSpawnpoint;
        [SerializeField] private ToggleResetInventorySetting _toggleResetInventory;
        [SerializeField] private ToggleTooltipSetting _toggleTooltip;
        [SerializeField] private TooltipHideDelaySetting _hideTooltipDelay;
        [SerializeField] private TooltipShowDelaySetting _showTooltipDelay;
        [SerializeField] private RedWindowSetting _windowRed;
        [SerializeField] private GreenWindowSetting _windowGreen;
        [SerializeField] private BlueWindowSetting _windowBlue;
        [SerializeField] private AlphaWindowSetting _windowAlpha;
        [SerializeField] private TMP_Text _display;

        public static RaSettings Singleton
        {
            get => _singleton;
            private set => _singleton = value;
        }

        public ToggleMovementSetting ToggleMovement
        {
            get => _toggleMovement;
            set => _toggleMovement = value;
        }

        public ToggleItemOrderSetting ToggleItemOrder
        {
            get => _toggleItemOrder;
            set => _toggleItemOrder = value;
        }

        public ToggleListOrderSetting ToggleListOrder
        {
            get => _toggleListOrder;
            set => _toggleListOrder = value;
        }

        public ToggleSuggestionsSetting ToggleSuggestions
        {
            get => _toggleSuggestions;
            set => _toggleSuggestions = value;
        }

        public ToggleTimestampsSetting ToggleTimestamps
        {
            get => _toggleTimestamps;
            set => _toggleTimestamps = value;
        }

        public BandwidthCooldownSetting BandwidthCooldown
        {
            get => _bandwidthCooldown;
            set => _bandwidthCooldown = value;
        }

        public ToggleSpawnpointSetting ToggleSpawnpoint
        {
            get => _toggleSpawnpoint;
            set => _toggleSpawnpoint = value;
        }

        public ToggleResetInventorySetting ToggleResetInventory
        {
            get => _toggleResetInventory;
            set => _toggleResetInventory = value;
        }

        public ToggleTooltipSetting ToggleTooltip
        {
            get => _toggleTooltip;
            set => _toggleTooltip = value;
        }

        public TooltipHideDelaySetting HideTooltipDelay
        {
            get => _hideTooltipDelay;
            set => _hideTooltipDelay = value;
        }

        public TooltipShowDelaySetting ShowTooltipDelay
        {
            get => _showTooltipDelay;
            set => _showTooltipDelay = value;
        }

        public RedWindowSetting WindowRed
        {
            get => _windowRed;
            set => _windowRed = value;
        }

        public GreenWindowSetting WindowGreen
        {
            get => _windowGreen;
            set => _windowGreen = value;
        }

        public BlueWindowSetting WindowBlue
        {
            get => _windowBlue;
            set => _windowBlue = value;
        }

        public AlphaWindowSetting WindowAlpha
        {
            get => _windowAlpha;
            set => _windowAlpha = value;
        }

        public static event Action OnLoad;

        public static event Action OnReset;

        public static event Action OnSave;

        public void Save(bool updateDisplay = false)
        {
            ServerConsole.ColorDebugLog("[SETTINGS] Saving RA settings...", ConsoleColor.Gray);
            OnSave?.Invoke();

            if (updateDisplay && _display != null)
                _display.text = "<color=green>Changes have been saved successfully!</color>";

            ServerConsole.ColorDebugLog("[SETTINGS] Saving process has finished successfully!", ConsoleColor.Gray);
        }

        public void Load(bool updateDisplay = false)
        {
            ServerConsole.ColorDebugLog("[SETTINGS] Loading RA settings...", ConsoleColor.Gray);
            OnLoad?.Invoke();

            if (updateDisplay && _display != null)
                _display.text = "<color=green>Changes have been loaded successfully!</color>";

            ServerConsole.ColorDebugLog("[SETTINGS] Preferences finished loading successfully!", ConsoleColor.Gray);
        }

        public void ForceReset(bool updateDisplay = false)
        {
            ServerConsole.ColorDebugLog("[SETTINGS] Loading RA settings...", ConsoleColor.Gray);
            OnReset?.Invoke();

            if (updateDisplay && _display != null)
            {
                _display.text = "<color=green>All settings have been reset to default!\n" +
                               "Make sure to press <b>Save</b> to apply changes.</color>";
            }

            ServerConsole.ColorDebugLog("[SETTINGS] Default settings finished loading successfully!", ConsoleColor.Gray);
        }

        public void ToggleListOrdering()
        {
            ToggleListOrder?.Toggle();
        }

        public void ToggleItemOrdering()
        {
            ToggleItemOrder?.Toggle();
        }

        public void RefreshTooltipShowDelay()
        {
            if (ShowTooltipDelay?.RepresentingSlider != null && !ShowTooltipDelay.RepresentingSlider.IsDestroyed())
            {
                ShowTooltipDelay.Refresh();
            }
        }

        public void RefreshTooltipHideDelay()
        {
            if (HideTooltipDelay?.RepresentingSlider != null && !HideTooltipDelay.RepresentingSlider.IsDestroyed())
            {
                HideTooltipDelay.Refresh();
            }
        }

        public void RefreshTimestampsToggle()
        {
            ToggleTimestamps?.Toggle();
        }

        public void RefreshBandwithCooldown()
        {
            if (BandwidthCooldown?.RepresentingSlider != null && !BandwidthCooldown.RepresentingSlider.IsDestroyed())
            {
                BandwidthCooldown.Refresh();
            }
        }

        public void RefreshMovementToggle()
        {
            ToggleMovement?.Toggle();
        }

        public void RefreshTooltipsToggle()
        {
            ToggleTooltip?.Toggle();
        }

        public void RefreshSuggestionsToggle()
        {
            ToggleSuggestions?.Toggle();
        }

        public void RefreshSpawnpointToggle()
        {
            ToggleSpawnpoint?.Toggle();
        }

        public void RefreshResetInventoryToggle()
        {
            ToggleResetInventory?.Toggle();
        }

        private void Awake()
        {
            Singleton = this;
        }

        private void Start()
        {
            ServerConsole.ColorDebugLog("[SETTINGS] Loading RA settings...", ConsoleColor.Gray);
            OnLoad?.Invoke();
            ServerConsole.ColorDebugLog("[SETTINGS] Preferences finished loading successfully!", ConsoleColor.Gray);
        }

        public RaSettings()
        {
            ToggleMovement = new ToggleMovementSetting();
            ToggleItemOrder = new ToggleItemOrderSetting();
            ToggleListOrder = new ToggleListOrderSetting();
            ToggleSuggestions = new ToggleSuggestionsSetting();
            ToggleTimestamps = new ToggleTimestampsSetting();
            BandwidthCooldown = new BandwidthCooldownSetting();
            ToggleSpawnpoint = new ToggleSpawnpointSetting();
            ToggleResetInventory = new ToggleResetInventorySetting();
            ToggleTooltip = new ToggleTooltipSetting();
            HideTooltipDelay = new TooltipHideDelaySetting();
            ShowTooltipDelay = new TooltipShowDelaySetting();
            WindowRed = new RedWindowSetting();
            WindowGreen = new GreenWindowSetting();
            WindowBlue = new BlueWindowSetting();
            WindowAlpha = new AlphaWindowSetting();
        }
    }
}