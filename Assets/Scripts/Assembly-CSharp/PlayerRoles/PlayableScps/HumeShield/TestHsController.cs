using UnityEngine;

namespace PlayerRoles.PlayableScps.HumeShield
{
    public class TestHsController : HumeShieldModuleBase
    {

        [SerializeField]
        private float _regeneration;

        [SerializeField]
        private float _maxAmount;

        [Space]
        [SerializeField]
        private Color _color;

        [SerializeField]
        private bool _colorActive;

        [Space]
        [SerializeField]
        private float _amountToModify;

        [SerializeField]
        private bool _apply;

        public override float HsMax => _maxAmount;

        public override float HsRegeneration => _regeneration;

        public override Color? HsWarningColor
        {
            get
            {
                if (!_colorActive)
                    return null;
                return _color;
            }
        }

        private void Update()
        {
            if (!_apply)
                return;

            base.HsCurrent += _amountToModify;

            _apply = false;
        }
    }
}