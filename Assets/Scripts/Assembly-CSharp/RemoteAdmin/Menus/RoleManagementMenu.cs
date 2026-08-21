using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NorthwoodLib.Pools;
using PlayerRoles;
using RemoteAdmin.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin.Menus
{
    public class RoleManagementMenu : RaCommandMenu
    {
        private const string DefaultName = "None";
        private const string SecretName = "Hubert Moszka";
        private const int SecretMaxTriggers = 173;

        [SerializeField]
        private RoleElement _roleElementPrefab;

        [SerializeField]
        private Transform _rootLayout;

        [SerializeField]
        private RawImage _avatar;

        [SerializeField]
        private Image _background;

        [SerializeField]
        private Image _border;

        [SerializeField]
        private TMP_Text _name;

        [SerializeField]
        private float _backgroundAlpha;

        [SerializeField]
        private Texture _defaultIcon;

        [SerializeField]
        private Texture _secretIcon;

        [SerializeField]
        private Color _secretColor;

        private int _triggers;

        public void UpdateGraphic(Color roleColor, Texture roleIcon, string roleName)
        {
            if (_avatar != null)
                _avatar.texture = roleIcon;

            if (_border != null)
                _border.color = roleColor;

            if (_background != null)
            {
                Color bgColor = GenerateBackground(roleColor);
                _background.color = bgColor;
            }

            if (_name != null)
            {
                _name.color = roleColor;
                _name.text = roleName;
            }
        }

        public void ResetGraphic()
        {
            int currentTriggers = _triggers;
            _triggers = currentTriggers + 1;

            if (currentTriggers < SecretMaxTriggers)
            {
                UpdateGraphic(Color.white, _defaultIcon, DefaultName);
            }
            else
            {
                UpdateGraphic(_secretColor, _secretIcon, SecretName);
            }
        }

        public void ShowSecret()
        {
            UpdateGraphic(_secretColor, _secretIcon, SecretName);
        }

        public override void SendCommand(string command, string format = "")
        {
            // BuildCommand appends the spawn-flag byte after the role argument. With no role
            // selected the role slot is empty, the query splitter drops it, and the flag byte
            // is parsed as the role id instead (flags 3 = RoleTypeId.Scp106).
            if (!AnyRoleSelected())
                return;

            base.SendCommand(command, format);
        }

        protected override string BuildCommand(string command, string format)
        {
            byte flags = 0;

            var raSettings = RaSettings.Singleton;
            if (raSettings != null)
            {
                if (raSettings.ToggleResetInventory != null && raSettings.ToggleResetInventory.Value)
                    flags |= 1;

                if (raSettings.ToggleSpawnpoint != null && raSettings.ToggleSpawnpoint.Value)
                    flags |= 2;
            }

            string baseCommand = base.BuildCommand(command, format);

            return string.Format("{0} {1}", baseCommand, flags);
        }

        protected override void OnStart()
        {
            base.OnStart();

            var allRoles = PlayerRoleLoader.AllRoles;
            var orderedRoles = allRoles.OrderBy(kv => PlayerRolesUtils.GetTeam(kv.Key));

            foreach (var kvp in orderedRoles)
            {
                RoleTypeId roleType = kvp.Key;
                PlayerRoleBase roleBase = kvp.Value;

                if ((byte)roleType == 255)
                    continue;

                if (_roleElementPrefab != null && _rootLayout != null)
                {
                    RoleElement element = Instantiate(_roleElementPrefab, _rootLayout);
                    element.CommandMenu = this;
                    element.SetupInterface(roleBase, roleType);

                    if (element.gameObject != null)
                        element.gameObject.SetActive(true);
                }
            }
        }

        private bool AnyRoleSelected()
        {
            if (Options == null)
                return false;

            foreach (var option in Options)
            {
                if (option is RoleElement roleElement
                    && roleElement.IsSelected
                    && roleElement.gameObject != null
                    && roleElement.gameObject.activeInHierarchy)
                {
                    return true;
                }
            }

            return false;
        }

        private Color GenerateBackground(Color originalColor)
        {
            float alpha = _backgroundAlpha * 0.01f;
            return new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        }
    }
}