namespace VoiceChat
{
    public class VoiceChatMicrophoneIndicator : global::UnityEngine.MonoBehaviour
    {
        [global::UnityEngine.SerializeField]
        private global::UnityEngine.UI.Image _outline;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.UI.Image _loudnessIndicator;

        [global::UnityEngine.SerializeField]
        private float _minValue;

        [global::UnityEngine.SerializeField]
        private float _maxValue;

        [global::UnityEngine.SerializeField]
        private float _dropSpeed;

        [global::UnityEngine.SerializeField]
        private float _curvePower;

        private static global::VoiceChat.VoiceChatMicrophoneIndicator _singleton;

        private static bool _singletonSet;

        private float FillAmount
        {
            get
            {
                return _loudnessIndicator.fillAmount;
            }
            set
            {
                _loudnessIndicator.fillAmount = global::UnityEngine.Mathf.Clamp01(value);
            }
        }

        private void Awake()
        {
            _singleton = this;
            _singletonSet = true;
            base.gameObject.SetActive(value: false);
            global::PlayerRoles.PlayerRoleManager.OnRoleChanged += UpdateColor;
        }

        private void OnDestroy()
        {
            _singletonSet = false;
            global::PlayerRoles.PlayerRoleManager.OnRoleChanged -= UpdateColor;
        }

        private void Update()
        {
            FillAmount -= global::UnityEngine.Time.deltaTime * _dropSpeed;
        }

        private void UpdateColor(ReferenceHub userHub, global::PlayerRoles.PlayerRoleBase prevRole, global::PlayerRoles.PlayerRoleBase newRole)
        {
            if (userHub.isLocalPlayer)
            {
                global::UnityEngine.Color roleColor = newRole.RoleColor;
                UpdateColor(_outline, roleColor);
                UpdateColor(_loudnessIndicator, roleColor);
            }
        }

        private void UpdateColor(global::UnityEngine.UI.Graphic target, global::UnityEngine.Color c)
        {
            target.color = new global::UnityEngine.Color(c.r, c.g, c.b, target.color.a);
        }

        private void RefreshIndicator(bool isSpeaking, float loudness)
        {
            base.gameObject.SetActive(isSpeaking);
            float num = global::UnityEngine.Mathf.Pow(global::UnityEngine.Mathf.InverseLerp(_minValue, _maxValue, loudness), _curvePower);
            FillAmount = (isSpeaking ? global::UnityEngine.Mathf.Max(FillAmount, num) : num);
        }

        public static void ShowIndicator(bool isSpeaking, float loudness)
        {
            if (_singletonSet)
            {
                _singleton.RefreshIndicator(isSpeaking, loudness);
            }
        }
    }
}
