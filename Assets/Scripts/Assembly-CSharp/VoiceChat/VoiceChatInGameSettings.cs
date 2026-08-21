namespace VoiceChat
{
    public class VoiceChatInGameSettings : global::UnityEngine.MonoBehaviour
    {
        [global::UnityEngine.SerializeField]
        private global::UnityEngine.GameObject _acceptedRoot;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.GameObject _deniedRoot;

        private void Awake()
        {
            UpdateSettings();
            global::VoiceChat.VoiceChatPrivacySettings.OnUserFlagsChanged += OnUserFlagsChanged;
        }

        private void OnDestroy()
        {
            global::VoiceChat.VoiceChatPrivacySettings.OnUserFlagsChanged -= OnUserFlagsChanged;
        }

        private void OnUserFlagsChanged(ReferenceHub hub)
        {
            if (hub.isLocalPlayer)
            {
                UpdateSettings();
            }
        }

        private void UpdateSettings()
        {
            bool flag = (global::VoiceChat.VoiceChatPrivacySettings.PrivacyFlags & global::VoiceChat.VcPrivacyFlags.AllowMicCapture) == global::VoiceChat.VcPrivacyFlags.AllowMicCapture;
            _acceptedRoot.SetActive(flag);
            _deniedRoot.SetActive(!flag);
        }

        public void ShowPrivacySettings()
        {
            global::VoiceChat.VoiceChatPrivacySettings singleton = global::VoiceChat.VoiceChatPrivacySettings.Singleton;
            if (singleton != null)
            {
                singleton.Open(advanced: true, forceOpen: false);
            }
        }
    }
}
