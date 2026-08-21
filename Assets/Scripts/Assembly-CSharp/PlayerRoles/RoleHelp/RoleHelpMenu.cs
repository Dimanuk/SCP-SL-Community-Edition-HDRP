using CursorManagement;
using PlayerRoles;
using ToggleableMenus;
using UnityEngine;

namespace PlayerRoles.RoleHelp
{
    public class RoleHelpMenu : ToggleableMenuBase
    {
        private bool _instanceSet;
        private GameObject _instance;
        private bool _fading;

        [SerializeField]
        private Transform _parent;

        [SerializeField]
        private GameObject _root;

        [SerializeField]
        private CanvasGroup _fader;

        [SerializeField]
        private float _fadeSpeed;

        public override bool CanToggle => _instanceSet;

        public override CursorOverrideMode CursorOverride => CursorOverrideMode.NoOverride;

        protected override void OnToggled()
        {
            _fading = true;
            _root.SetActive(true);
        }

        protected override void Awake()
        {
            base.Awake();
            PlayerRoleManager.OnRoleChanged += OnRoleChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            PlayerRoleManager.OnRoleChanged -= OnRoleChanged;
        }

        private void Update()
        {
            if (!_fading)
                return;

            float speed = IsEnabled ? _fadeSpeed : -_fadeSpeed;

            if (UpdateAlpha(speed))
            {
                _fading = false;
                _root.SetActive(IsEnabled);
            }
        }

        private bool UpdateAlpha(float speed)
        {
            float alpha = _fader.alpha;
            alpha = Mathf.Clamp01(alpha + Time.deltaTime * speed);
            _fader.alpha = alpha;

            bool reachedOne = alpha == 1f;   
            bool reachedZero = alpha == 0f;  

            return speed > 0f ? reachedOne : reachedZero;
        }

        private void OnRoleChanged(ReferenceHub hub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            if (!hub.isLocalPlayer)
                return;

            IsEnabled = false;

            if (_instanceSet)
            {
                Destroy(_instance);
                _instanceSet = false;
            }

            GameObject roleHelpInfo = newRole.RoleHelpInfo;
            if (roleHelpInfo != null)
            {
                _instance = Instantiate(roleHelpInfo, _parent);
                _instanceSet = true;
            }
        }
    }
}
