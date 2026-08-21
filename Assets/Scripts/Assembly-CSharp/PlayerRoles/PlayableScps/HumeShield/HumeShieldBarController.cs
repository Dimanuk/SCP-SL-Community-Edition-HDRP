namespace PlayerRoles.PlayableScps.HumeShield
{
    public class HumeShieldBarController : global::UnityEngine.MonoBehaviour
    {
        [global::UnityEngine.SerializeField]
        private StatusBar _targetBar;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.UI.Image _hsWarning;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.Color _hsColor;

        private float _colorTimer;

        private bool _prevVisible;

        private bool _firstFrame;

        private const float FadeSpeed = 8f;

        private const float BlinkSpeed = 35f;

        private void Awake()
        {
            _targetBar.AutohideOption = StatusBar.AutoHideType.AlwaysVisible;
            _firstFrame = true;
        }

        private void Update()
        {
            GetValues(out var barVisible, out var warningColor);
            if (_firstFrame || barVisible != _prevVisible)
            {
                _targetBar.SetAlpha(barVisible ? 1 : 0);
                _firstFrame = true;
                _prevVisible = barVisible;
            }
            if (warningColor.HasValue)
            {
                global::UnityEngine.Color value = warningColor.Value;
                global::UnityEngine.Color hsColor = _hsColor;
                float num = 35f * value.a;
                hsColor.a = (value.a = global::UnityEngine.Mathf.Min(1f, _colorTimer * 8f));
                float t = (global::UnityEngine.Mathf.Sin(_colorTimer * num) + 1f) / 2f;
                _hsWarning.color = global::UnityEngine.Color.Lerp(hsColor, value, t);
                _colorTimer += global::UnityEngine.Time.deltaTime;
            }
            else
            {
                global::UnityEngine.Color color = _hsWarning.color;
                color.a = global::UnityEngine.Mathf.Max(0f, color.a - global::UnityEngine.Time.deltaTime * 8f);
                _hsWarning.color = color;
                _colorTimer = 0f;
            }
        }

        private void GetValues(out bool barVisible, out global::UnityEngine.Color? warningColor)
        {
            barVisible = false;
            warningColor = null;
            if ((global::PlayerRoles.Spectating.SpectatorTargetTracker.TryGetTrackedPlayer(out var hub) || ReferenceHub.TryGetLocalHub(out hub)) && hub.roleManager.CurrentRole is global::PlayerRoles.PlayableScps.HumeShield.IHumeShieldedRole humeShieldedRole)
            {
                barVisible = true;
                warningColor = humeShieldedRole.HumeShieldModule.HsWarningColor;
            }
        }
    }
}
