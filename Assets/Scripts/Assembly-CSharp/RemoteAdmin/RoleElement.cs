using PlayerRoles;
using RemoteAdmin.Elements;
using RemoteAdmin.Menus;
using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin
{
    public class RoleElement : ValueButton
    {
        [CompilerGenerated]
        private PlayerRoleBase _role;

        [SerializeField]
        private RawImage _avatar;

        [SerializeField]
        private Image _background;

        [SerializeField]
        private Image _border;

        [SerializeField]
        private float _backgroundAlpha;

        private RoleTypeId _roleTypeId;

        public PlayerRoleBase Role
        {
            get => _role;
            set => _role = value;
        }

        public RoleTypeId RoleTypeId
        {
            get => _roleTypeId;
            set
            {
                _roleTypeId = value;
                Value = value.ToString();
            }
        }

        public void SetupInterface(PlayerRoleBase role, RoleTypeId roleType)
        {
            // The RoleElement prefab is instantiated while inactive (see RoleManagementMenu.OnStart),
            // so Awake/OnInitialize hasn't run yet and the serialized _text reference — lost when the
            // scene was ripped — is still null here. Resolve the label now, otherwise the `Text != null`
            // guard below is skipped and every button keeps its placeholder "SCP-999" text.
            if (Text == null)
                Text = GetComponentInChildren<TMP_Text>(true);

            Role = role;
            RoleTypeId = roleType;

            if (role == null)
                return;

            Color teamColor = role.RoleColor;
            float alpha = _backgroundAlpha * 0.01f;

            Color backgroundColor = GenerateBackground(teamColor);

            if (_avatar != null)
            {
                _avatar.color = Color.white;

                if (role is IAvatarRole avatarRole)
                {
                    _avatar.texture = avatarRole.RoleAvatar;
                }
                else
                {
                    _avatar.texture = null;
                }
            }

            if (_border != null)
                _border.color = teamColor;

            if (_background != null)
                _background.color = backgroundColor;

            if (Text != null)
            {
                string roleName = TranslationReader.Get("RA_RoleManagement", (int)roleType + 1, role.RoleName);
                Text.text = roleName;
                Text.color = teamColor;
            }

            if (CommandMenu != null && CommandMenu.Options != null)
                CommandMenu.Options.Add(this);
        }

        public override void SetState(bool isSelected)
        {
            base.SetState(isSelected);

            RoleManagementMenu roleMenu = CommandMenu as RoleManagementMenu;

            if (isSelected)
            {
                Color teamColor = Role != null ? Role.RoleColor : Color.white;
                float alpha = _backgroundAlpha * 0.01f;

                Color backgroundColor = GenerateBackground(teamColor);

                if (_border != null)
                    _border.color = teamColor;

                if (_background != null)
                    _background.color = backgroundColor;

                if (roleMenu != null && _avatar != null)
                {
                    string fallbackName = Role != null ? Role.RoleName : string.Empty;
                    roleMenu.UpdateGraphic(teamColor, _avatar.mainTexture, TranslationReader.Get("RA_RoleManagement", (int)Role.RoleTypeId + 1, fallbackName));
                }
            }
            else
            {
                if (roleMenu != null)
                    roleMenu.ResetGraphic();
            }
        }

        private Color GenerateBackground(Color originalColor)
        {
            float alpha = _backgroundAlpha * 0.01f;
            return new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        }
    }
}
