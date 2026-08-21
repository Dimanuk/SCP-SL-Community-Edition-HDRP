namespace VoiceChat
{
    public class VoiceChatNoiseReducerToggle : global::UnityEngine.UI.Toggle
    {
        private const string PrefsKey = "VcNoiseRed";

        private static bool _valueLoaded;

        private static bool _cachedValue;

        public static bool Enabled
        {
            get
            {
                if (!_valueLoaded)
                {
                    _cachedValue = PlayerPrefsSl.Get("VcNoiseRed", defaultValue: true);
                    _valueLoaded = true;
                }
                return _cachedValue;
            }
            private set
            {
                _cachedValue = value;
                PlayerPrefsSl.Set("VcNoiseRed", value);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            base.isOn = Enabled;
            onValueChanged.AddListener(delegate (bool x)
            {
                Enabled = x;
            });
        }
    }
}
