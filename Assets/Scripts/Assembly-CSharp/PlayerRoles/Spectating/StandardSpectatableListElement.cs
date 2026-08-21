using PlayerStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoles.Spectating
{
    public class StandardSpectatableListElement : FullSizeSpectatableListElement
    {
        [SerializeField]
        private Image _healthCircle;

        [SerializeField]
        private Image _avatarBackground;

        [SerializeField]
        private RawImage _avatarIcon;

        [SerializeField]
        private TextMeshProUGUI _roleText;

        [SerializeField]
        private Image _healthIcon;

        [SerializeField]
        private TextMeshProUGUI _healthText;

        [SerializeField]
        private Image _shieldIcon;

        [SerializeField]
        private TextMeshProUGUI _shieldText;

        private HumeShieldStat _hsStat;

        private HealthStat _hpStat;

        private AhpStat _ahpStat;

        private Color _prevColor;

        private const float FillAmountLerpSpeed = 15.5f;

        protected override void Update()
        {
            base.Update();
            if (Target == null)
                return;

            PlayerRoleBase mainRole = Target.MainRole;
            if (mainRole == null)
                return;

            UpdateStats(Time.deltaTime * FillAmountLerpSpeed);

            UpdateColor(mainRole.RoleColor);

            if (_roleText != null)
                _roleText.text = mainRole.RoleName;

            if (_avatarIcon != null && mainRole is IAvatarRole avatarRole)
            {
                _avatarIcon.texture = avatarRole.RoleAvatar;
            }
        }

        private void UpdateStats(float lerpT)
        {
            if (_ahpStat == null || _hsStat == null || _hpStat == null)
                return;

            float totalShield = _hsStat.CurValue + _ahpStat.CurValue;

            if (_shieldText != null)
                _shieldText.text = Mathf.RoundToInt(totalShield).ToString();

            if (_healthText != null)
                _healthText.text = Mathf.RoundToInt(_hpStat.CurValue).ToString();

            bool hasShield = totalShield > 0f;
            if (_shieldIcon != null)  
                _shieldIcon.enabled = hasShield;
            if (_shieldText != null)
                _shieldText.enabled = hasShield;

            if (_healthCircle != null && _hpStat != null)
            {
                float targetFill = _hpStat.CurValue / _hpStat.MaxValue;
                _healthCircle.fillAmount = Mathf.Lerp(_healthCircle.fillAmount, targetFill, lerpT);
            }
        }

        private void UpdateColor(Color c)
        {
            if (c == _prevColor)
                return;

            _prevColor = c;

            if (_roleText != null)       
                _roleText.color = c;

            if (_healthCircle != null)   
                _healthCircle.color = c;

            if (_healthIcon != null)     
                _healthIcon.color = c;

            if (_healthText != null)      
                _healthText.color = c;

            if (_avatarBackground != null)
            {
                _avatarBackground.color = SpectatableListColors.MixAvatarColor(c);
            }
        }

        protected override void OnTargetChanged(SpectatableModuleBase prevTarget, SpectatableModuleBase newTarget)
        {
            if (newTarget == null || newTarget.TargetHub == null)
                return;

            PlayerStats playerStats = newTarget.TargetHub.playerStats;

            _ahpStat = playerStats.GetModule<AhpStat>();   
            _hpStat = playerStats.GetModule<HealthStat>();   
            _hsStat = playerStats.GetModule<HumeShieldStat>(); 

            UpdateStats(1f);
        }
    }
}