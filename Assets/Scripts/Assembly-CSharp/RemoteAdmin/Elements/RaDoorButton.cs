using Interactables.Interobjects.DoorUtils;
using RemoteAdmin.Menus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin.Elements
{
    public class RaDoorButton : ValueButton
    {
        public DoorNametagExtension Door;

        [SerializeField]
        private Image _image;

        private MapControlMenu _mapMenu;

        private bool _hasTargetDoor;

        private bool _menuInitialized;

        public RaDoorButton PreviousButton { get; set; }
        public RaDoorButton NextButton { get; set; }

        public bool HasDoor
        {
            get => _hasTargetDoor;
            set => _hasTargetDoor = value;
        }

        public bool HasDoorVariant => Door?.TargetDoor != null;

        public DoorVariant DoorVariant => Door?.TargetDoor;

        public string DisplayText
        {
            get => Text?.text;
            set
            {
                if (Text != null)
                    Text.text = value;
            }
        }

        public override void SetState(bool isSelected)
        {
            base.SetState(isSelected);
            UpdateGraphics();
        }

        public void Setup()
        {
            if (CommandMenu is MapControlMenu mapMenu)
                _mapMenu = mapMenu;
            else
                _mapMenu = null;

            _menuInitialized = _mapMenu != null;

            _hasTargetDoor = Door != null;

            if (_hasTargetDoor)
            {
                if (Text != null)
                    Text.text = Door.GetName;

                Value = Door.GetName + ".";
            }

            UpdateGraphics();
        }

        public void UpdateGraphics()
        {
            if (!_menuInitialized)
                return;

            Color bgColor = BackgroundColor();
            if (_image != null)
                _image.color = bgColor;

            Color outlineColor = OutlineColor();
            if (Outline != null)
                Outline.effectColor = outlineColor;
        }

        private Color BackgroundColor()
        {
            if (!_hasTargetDoor)
                return _mapMenu.Closed.Color;

            if (Door.TargetDoor != null)
            {
                var damageable = Door.TargetDoor.GetComponent<IDamageableDoor>();
                if (damageable != null && damageable.IsDestroyed)
                    return _mapMenu.LockedUnselected.Color;

                if (Door.TargetDoor.TargetState) 
                    return _mapMenu.Opened.Color;
            }

            return _mapMenu.Closed.Color;
        }

        private Color OutlineColor()
        {
            // Doorless buttons ("All" / "All listed" / "All not listed") must still
            // reflect selection — only the locked-color branch is door-specific.
            if (_hasTargetDoor && Door.TargetDoor != null && Door.TargetDoor.ActiveLocks != 0)
            {
                return IsSelected
                    ? _mapMenu.LockedSelected.Color
                    : _mapMenu.LockedUnselected.Color;
            }

            return IsSelected
                ? _mapMenu.Selected.Color
                : _mapMenu.Unselected.Color;
        }
    }
}