using TMPro;
using UnityEngine;
using System;

namespace PlayerRoles.Spectating
{
    public class OverwatchRole : SpectatorRole, IObfuscatedRole
    {
        [SerializeField]
        private TextMeshProUGUI _hudTemplate;

        private bool _hudSetup;

        private TextMeshProUGUI _hudInstance;

        public override RoleTypeId RoleTypeId => RoleTypeId.Overwatch;
        public override UnityEngine.Color RoleColor => UnityEngine.Color.cyan;

        public override bool ReadyToRespawn => false;

        public RoleTypeId GetRoleForUser(ReferenceHub receiver)
        {
            if (receiver != null &&
                receiver.serverRoles != null &&
                PermissionsHandler.IsPermitted(receiver.serverRoles.Permissions, PlayerPermissions.GameplayData))
            {
                return RoleTypeId;
            }

            return RoleTypeId.Spectator;
        }

        public override void DisableRole(RoleTypeId newRole)
        {
            base.DisableRole(newRole);

            if (_hudSetup && _hudInstance != null)
            {
                _hudSetup = false;
                Destroy(_hudInstance.gameObject);
            }
        }

        private void Update()
        {
            if (!SetupHud())
                return;

            if (SpectatorTargetTracker.TryGetTrackedPlayer(out ReferenceHub targetHub) && targetHub != null)
            {
                _hudInstance.text = string.Format(
                    "PlayerID: {0}{1}Nickname: {2}",
                    targetHub.PlayerId,
                    Environment.NewLine,
                    targetHub.nicknameSync.MyNick);
            }
            else if (_hudInstance != null)
            {
                _hudInstance.text = string.Empty;
            }
        }

        private bool SetupHud()
        {
            if (!IsLocalPlayer)
                return false;

            if (_hudSetup)
                return true;

            GameObject spectatorCanvas = GameObject.Find("Spectator Canvas");
            if (spectatorCanvas == null || _hudTemplate == null)
                return false;

            _hudInstance = Instantiate(_hudTemplate, spectatorCanvas.transform);

            RectTransform rt = _hudInstance.rectTransform;
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;

            _hudSetup = true;
            return true;
        }
    }
}