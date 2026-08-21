using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoles.Spectating
{
    public class FullSizeSpectatableListElement : SpectatableListElementBase
    {
        [SerializeField]
        private Image _mainBackground;

        [SerializeField]
        private TextMeshProUGUI _nicknameText;

        [SerializeField]
        private GameObject _currentIndicator;

        protected virtual void Update()
        {
            var target = this.Target;
            if (target == null) return;

            var mainRole = target.MainRole;
            if (mainRole == null) return;

            if (!mainRole.TryGetOwner(out ReferenceHub owner))
                return;

            if (_nicknameText != null)
            {
                _nicknameText.text = owner.nicknameSync.DisplayName;
            }

            bool isCurrent = base.IsCurrent;

            if (_currentIndicator != null)
            {
                _currentIndicator.SetActive(isCurrent);
            }

            if (_mainBackground != null)
            {
                _mainBackground.color = isCurrent
                    ? SpectatableListColors.BgSelected
                    : SpectatableListColors.BgRegular;
            }
        }
    }
}