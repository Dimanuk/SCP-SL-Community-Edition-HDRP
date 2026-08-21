using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using NorthwoodLib.Pools;
using RemoteAdmin.Elements;
using RemoteAdmin.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace RemoteAdmin.Menus
{
    public class MapControlMenu : RaCommandMenu
    {
        private const string DefaultOverchargeValue = "3600";

        [SerializeField] internal ColorPreset Closed;
        [SerializeField] internal ColorPreset Opened;
        [SerializeField] internal ColorPreset Unselected;
        [SerializeField] internal ColorPreset Selected;
        [SerializeField] internal ColorPreset LockedUnselected;
        [SerializeField] internal ColorPreset LockedSelected;

        [SerializeField] private RaDoorButton _doorButtonTemplate;
        [SerializeField] private Transform _rootParent;
        [SerializeField] private TMP_Dropdown _zoneDropdown;
        [SerializeField] private TMP_InputField _overchargeDuration;
        [SerializeField] private TMP_InputField _lockdownDuration;
        [SerializeField] private TMP_InputField _cleanupAmount;

        private float _updateTimer;

        private static Func<DoorNametagExtension, string> _cachedNametagSelector;

        public void RefreshButtons()
        {
            foreach (var button in Options)
            {
                if (button is RaDoorButton doorButton)
                {
                    doorButton.UpdateGraphics();
                }
            }
        }

        public void FilterDoors()
        {
            int selectedZone = _zoneDropdown.value;

            foreach (var button in Options)
            {
                if (button is RaDoorButton doorButton)
                {
                    if (doorButton.Door == null || doorButton.Door.TargetDoor == null)
                    {
                        button.gameObject.SetActive(false);
                        continue;
                    }

                    if (selectedZone != 0)
                    {
                        if (doorButton.Door != null &&
                            doorButton.Door.TargetDoor != null &&
                            doorButton.Door.TargetDoor.IsInZone((FacilityZone)selectedZone))
                        {
                            button.gameObject.SetActive(true);
                        }
                        else
                        {
                            button.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        button.gameObject.SetActive(true);
                    }
                }
            }
        }

        protected override string BuildCommand(string command, string format)
        {
            StringBuilder sb = StringBuilderPool.Shared.Rent();

            try
            {
                string[] args = new string[7];
                args[0] = command ?? string.Empty;
                args[1] = GetSelectedPlayers(sb) ?? string.Empty;
                args[2] = GetSelectedOptions(sb) ?? string.Empty;
                args[3] = (InputFieldText != null) ? InputFieldText.text : string.Empty;

                string overchargeText = (_overchargeDuration != null) ? _overchargeDuration.text : string.Empty;
                if (string.IsNullOrEmpty(overchargeText))
                    overchargeText = DefaultOverchargeValue;
                args[4] = overchargeText;

                args[5] = (_lockdownDuration != null) ? _lockdownDuration.text : string.Empty;
                args[6] = (_cleanupAmount != null) ? _cleanupAmount.text : string.Empty;

                return string.Format(format, args);
            }
            finally
            {
                StringBuilderPool.Shared.Return(sb);
            }
        }

        protected override void OnUpdate()
        {
            if (_updateTimer > Time.time)
                return;

            _updateTimer = Time.time + 1f;
            RefreshButtons();
        }

        protected override void OnStart()
        {
            var namedDoors = DoorNametagExtension.NamedDoors.Values
                .OrderBy(x => x.GetName)
                .ToList();

            foreach (var doorData in namedDoors)
            {
                RaDoorButton button = Instantiate(_doorButtonTemplate, _rootParent);
                button.Door = doorData;
                button.CommandMenu = this;
                if (!Options.Contains(button))
                    Options.Add(button);
            }

            RaDoorButton previous = null;

            for (int i = 0; i < Options.Count; i++)
            {
                if (Options[i] is RaDoorButton current)
                {
                    if (previous != null)
                    {
                        current.PreviousButton = previous;
                        previous.NextButton = current;
                    }

                    current.Setup();

                    previous = current;
                }
            }
        }
    }
}
