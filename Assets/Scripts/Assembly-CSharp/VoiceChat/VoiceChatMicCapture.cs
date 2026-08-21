namespace VoiceChat
{
    public class VoiceChatMicCapture : global::UnityEngine.MonoBehaviour
    {
        private static global::VoiceChat.VoiceChatMicCapture _singleton;

        private static bool _singletonSet;

        private static string _selectedMic;

        private global::VoiceChat.CaressNoiseReduction.NoiseReducer _noiseReducer;

        private global::VoiceChat.Networking.PlaybackBuffer _recordBuffer;

        private global::VoiceChat.Networking.PlaybackBuffer _sendBuffer;

        private global::UnityEngine.AudioSource _micSource;

        private global::VoiceChat.VoiceChatChannel _channel;

        private float[] _noiseReductionBuffer;

        private float[] _samples;

        private int _lastSample;

        private int _samplesCount;

        private bool _micStarted;

        private bool _noiseReductionFailed;

        [global::UnityEngine.SerializeField]
        [global::UnityEngine.Tooltip("EDITOR/DEBUG: echo your own voice back to you through the full network round-trip (client -> server -> client). Wear headphones! Lets you HEAR yourself exactly as others do and verify the whole voice path (capture -> Opus encode -> network -> server route -> network -> decode -> playback) is alive. Leave OFF for normal play.")]
        private bool _hearYourself;

        private static bool MicCaptureDenied => !HearYourself && (global::VoiceChat.VoiceChatPrivacySettings.PrivacyFlags & global::VoiceChat.VcPrivacyFlags.AllowMicCapture) == 0;

        public static bool HearYourself => _singletonSet && _singleton._hearYourself;

        private void Awake()
        {
            if (_singleton != null)
            {
                throw new global::System.InvalidOperationException("More than one 'VoiceChatMicCapture'' detected on scene!");
            }
            _singleton = this;
            _singletonSet = true;
            try
            {
                _noiseReducer = new global::VoiceChat.CaressNoiseReduction.NoiseReducer(global::VoiceChat.VoiceChatSettings.NoiseReductionSettings);
            }
            catch (global::System.Exception ex)
            {
                _noiseReductionFailed = true;
                global::UnityEngine.Debug.LogError("Noise reducer exception: " + ex.Message);
            }
            _noiseReductionBuffer = new float[480];
            _recordBuffer = new global::VoiceChat.Networking.PlaybackBuffer();
            _sendBuffer = new global::VoiceChat.Networking.PlaybackBuffer();
            _micSource = base.gameObject.AddComponent<global::UnityEngine.AudioSource>();
            _micSource.volume = 0f;
            _micSource.loop = true;
            _micSource.bypassEffects = true;
            _micSource.bypassListenerEffects = true;
            _micSource.bypassReverbZones = true;
            global::VoiceChat.VoiceChatPrivacySettings.OnUserFlagsChanged += OnPrivacySettingsUpdated;
        }

        private void OnDestroy()
        {
            global::VoiceChat.VoiceChatPrivacySettings.OnUserFlagsChanged -= OnPrivacySettingsUpdated;
            StopAllMicrophones();
            _singletonSet = false;
        }

        private void Update()
        {
            if (HearYourself && !_micStarted)
            {
                StartRecording();
            }

            if (UpdateRecording(out var loudness, out var isSpeaking))
            {
                global::VoiceChat.VoiceChatMicrophoneIndicator.ShowIndicator(isSpeaking, loudness);
            }
        }

        private void OnPrivacySettingsUpdated(ReferenceHub hub)
        {
            if (hub.isLocalPlayer && (!_micStarted || MicCaptureDenied))
            {
                RestartRecording();
            }
        }

        private bool UpdateRecording(out float loudness, out bool isSpeaking)
        {
            loudness = 0f;
            isSpeaking = false;
            if (!ReferenceHub.TryGetLocalHub(out var hub))
            {
                return true;
            }
            if (!(hub.roleManager.CurrentRole is global::PlayerRoles.Voice.IVoiceRole voiceRole))
            {
                return true;
            }
            _channel = voiceRole.VoiceModule.GetUserInput();
            bool flag = _channel == global::VoiceChat.VoiceChatChannel.None;
            isSpeaking = !flag && MicCaptureDenied;
            if (!_micStarted)
            {
                return true;
            }
            int position = global::UnityEngine.Microphone.GetPosition(_selectedMic);
            _micSource.clip.GetData(_samples, 0);
            int num = position - _lastSample;
            if (num < 0)
            {
                num += _samplesCount;
            }
            if (flag)
            {
                _lastSample = position;
                return true;
            }
            for (int i = 0; i < num; i++)
            {
                float num2 = _samples[(_lastSample + i) % _samplesCount];
                if (num2 > loudness)
                {
                    loudness = num2;
                }
                _recordBuffer.Write(num2);
            }
            _lastSample = position;
            int num3 = 480;
            while (_recordBuffer.Length >= num3)
            {
                _recordBuffer.ReadTo(_noiseReductionBuffer, num3, 0L);
                if (global::VoiceChat.VoiceChatNoiseReducerToggle.Enabled && !_noiseReductionFailed)
                {
                    _noiseReducer.ReduceNoise(_noiseReductionBuffer);
                }
                _sendBuffer.Write(_noiseReductionBuffer, num3);
                global::VoiceChat.Networking.VoiceTransceiver.ClientSendData(_sendBuffer, _channel);
            }
            isSpeaking = true;
            return num > 0;
        }

        public static void StopAllMicrophones()
        {
            if (!_singletonSet || !_singleton._micStarted)
            {
                return;
            }
            string[] devices = global::UnityEngine.Microphone.devices;
            foreach (string deviceName in devices)
            {
                if (global::UnityEngine.Microphone.IsRecording(deviceName))
                {
                    global::UnityEngine.Microphone.End(deviceName);
                }
            }
            _singleton._micStarted = false;
        }

        public static void StartRecording()
        {
            if (_singletonSet && !_singleton._micStarted)
            {
                RestartRecording();
            }
        }

        public static void RestartRecording()
        {
            if (!_singletonSet || !global::VoiceChat.VoiceChatMicrophoneSelector.TryGetPreferredMicrophone(out var mic))
            {
                return;
            }
            if (MicCaptureDenied)
            {
                StopAllMicrophones();
                return;
            }
            if (_singleton._micStarted && _selectedMic != mic)
            {
                StopAllMicrophones();
                _selectedMic = mic;
            }
            global::UnityEngine.AudioSource micSource = _singleton._micSource;
            micSource.clip = global::UnityEngine.Microphone.Start(_selectedMic, loop: true, 1, 48000);
            if (micSource.clip == null)
            {
                _singleton._micStarted = false;
                global::UnityEngine.Debug.LogError("Microphone '" + _selectedMic + "' failed to start recording.");
                return;
            }
            _singleton._recordBuffer.Clear();
            _singleton._sendBuffer.Clear();
            _singleton._micStarted = true;
            int samples = micSource.clip.samples;
            _singleton._lastSample = 0;
            if (samples != _singleton._samplesCount)
            {
                _singleton._samplesCount = samples;
                _singleton._samples = new float[samples];
            }
            micSource.Play();
        }

        public static global::VoiceChat.VoiceChatChannel GetCurrentChannel()
        {
            if (!_singletonSet)
            {
                return global::VoiceChat.VoiceChatChannel.None;
            }
            return _singleton._channel;
        }
    }
}
