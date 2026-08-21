using TMPro;
using UnityEngine;

namespace PlayerRoles.RoleHelp
{
    public class RoleHelpNameLabel : MonoBehaviour
    {
        // IL2CPP offset: [this+0x18]
        [SerializeField]
        private TMP_Text _text;

        private const string NicknameFile = "Class_Nicknames";

        private void Awake()
        {
            if (!ReferenceHub.TryGetLocalHub(out ReferenceHub hub))
                return;

            PlayerRoleBase currentRole = hub.roleManager.CurrentRole;
            if (currentRole == null)
                return;

            _text.text = string.Format(
                _text.text,
                currentRole.RoleName,
                TranslationReader.Get(NicknameFile, (int)currentRole.RoleTypeId));
        }
    }
}