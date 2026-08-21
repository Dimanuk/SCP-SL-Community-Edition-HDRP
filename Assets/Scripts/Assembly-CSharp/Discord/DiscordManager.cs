using Discord.Modules;
using UnityEngine;

namespace Discord
{
    public class DiscordManager : MonoBehaviour
    {
        private static bool _singletonSet;

        [SerializeField]
        private bool _isEnabled;

        [field: SerializeField]
        public RichPresenceModule RichPresence { get; private set; }

        [field: SerializeField]
        public RequestableJoinModule JoinModule { get; private set; }

        [field: SerializeField]
        public DebugModule DebugModule { get; private set; }

        [field: SerializeField]
        public CustomNetworkManager NetworkManager { get; set; }

        public static DiscordManager Singleton;
        public bool IsEnabled
        {
            get => _isEnabled;
            private set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    if (RichPresence != null)
                        RichPresence.IsEnabled = value;
                    if (DebugModule != null)
                        DebugModule.IsEnabled = value;
                    if (JoinModule != null)
                        JoinModule.IsEnabled = value;
                }
            }
        }

        private void Awake()
        {
            if (!_singletonSet)
            {
                _singletonSet = true;
                Singleton = this;
            }

            IsEnabled = PlayerPrefsSl.Get("RichPresence", true);
        }

        private void OnEnable()
        {
            PlayerPrefsSl.SettingChanged += OnSettingChanged;
            CallbackController.Initialize("420676877766623232", true, "700330");
        }

        private void OnDisable()
        {
            PlayerPrefsSl.SettingChanged -= OnSettingChanged;
            CallbackController.Shutdown();
        }

        private void Update()
        {
            CallbackController.RunCallbacks();
        }

        private void OnSettingChanged(string key, string value)
        {
            if (key.Length >= 2 && key.Substring(2) == "RichPresence")
            {
                IsEnabled = value == "true";
            }
        }

        public DiscordManager()
        {
            _isEnabled = true;
        }
    }
}