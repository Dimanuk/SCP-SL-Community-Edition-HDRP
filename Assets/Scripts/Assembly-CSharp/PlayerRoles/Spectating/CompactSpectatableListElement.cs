using TMPro;
using UnityEngine;

namespace PlayerRoles.Spectating
{
    public class CompactSpectatableListElement : SpectatableListElementBase
    {
        [SerializeField]
        private TextMeshProUGUI _nicknameText;

        private void Update()
        {
            if (Target == null || Target.TargetHub == null)
                return;

            ReferenceHub targetHub = Target.TargetHub;
            if (targetHub.nicknameSync != null)
            {
                string displayName = targetHub.nicknameSync.DisplayName;
                _nicknameText.text = displayName;
            }

            if (Target.MainRole != null)
            {
                Color roleColor = Target.MainRole.RoleColor;
                _nicknameText.color = roleColor;
            }
        }
    }
}