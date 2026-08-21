using System.Collections.Generic;
using UnityEngine;

namespace ToggleableMenus
{
    public static class ToggleableMenuController
    {
        private static ToggleableMenuBase _currentMenu;
        private static bool _anyEnabled;

        public static readonly HashSet<IRegisterableMenu> RegisteredMenus = new HashSet<IRegisterableMenu>();

        public static bool AnyEnabled => _anyEnabled && !IsNull(_currentMenu);

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            StaticUnityMethods.OnUpdate += Update;
        }

        public static bool IsNull(IRegisterableMenu menu)
        {
            if (menu == null) return true;
            if (menu is Object obj && obj == null) return true;
            return false;
        }

        private static void Update()
        {
            if (AnyEnabled && (Input.GetKeyDown(KeyCode.Escape) || !_currentMenu.IsEnabled))
            {
                _currentMenu.IsEnabled = false;
                _currentMenu = null;
            }

            foreach (var menu in RegisteredMenus)
            {
                if (menu is ToggleableMenuBase baseMenu && !IsNull(baseMenu))
                    HandleMenu(baseMenu);
            }

            _anyEnabled = !IsNull(_currentMenu);
        }

        private static void HandleMenu(ToggleableMenuBase menu)
        {
            if (!Input.GetKeyDown(NewInput.GetKey(menu.MenuActionKey)))
                return;

            if (!menu.CanToggle)
                return;

            if (!_anyEnabled)
            {
                menu.IsEnabled = true;
                _anyEnabled = true;
                _currentMenu = menu;
            }
            else if (_currentMenu == menu && AnyEnabled)
            {
                _currentMenu.IsEnabled = false;
                _currentMenu = null;
                _anyEnabled = false;
            }
        }

        public static void ToggleMenu(IRegisterableMenu menu)
        {
            if (IsNull(menu) || !(menu is ToggleableMenuBase baseMenu))
                return;

            if (_currentMenu == baseMenu)
            {
                HideCurrent();
            }
            else
            {
                ForceCurrent(baseMenu);
            }
        }

        public static void ForceCurrent(IRegisterableMenu menu)
        {
            HideCurrent();

            if (IsNull(menu) || !(menu is ToggleableMenuBase baseMenu))
                return;

            _currentMenu = baseMenu;
            _currentMenu.IsEnabled = true;
            _anyEnabled = true;
        }

        public static void HideCurrent()
        {
            if (!IsNull(_currentMenu))
            {
                _currentMenu.IsEnabled = false;
            }
            _currentMenu = null;
            _anyEnabled = false;
        }
    }
}
