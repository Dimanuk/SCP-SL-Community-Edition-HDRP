namespace VoiceChat
{
    public class VoiceChatMuteIndicator : global::UnityEngine.MonoBehaviour
    {
        public struct SyncMuteMessage : global::Mirror.NetworkMessage
        {
            public byte Flags;
        }

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.GameObject _root;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.GameObject _locally;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.GameObject _globally;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.GameObject _privacy;

        [global::UnityEngine.SerializeField]
        private global::TMPro.TextMeshProUGUI _privacyText;

        private static global::VoiceChat.VoiceChatMuteIndicator _singleton;

        private static readonly global::VoiceChat.VcMuteFlags Filter = global::VoiceChat.VcMuteFlags.LocalRegular | global::VoiceChat.VcMuteFlags.GlobalRegular;

        public static global::VoiceChat.VcMuteFlags ReceivedFlags { get; private set; }

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += delegate
            {
                ReceivedFlags = global::VoiceChat.VcMuteFlags.None;
                global::Mirror.NetworkClient.ReplaceHandler<global::VoiceChat.VoiceChatMuteIndicator.SyncMuteMessage>(ReceiveMessage);
            };
            global::VoiceChat.VoiceChatMutes.OnFlagsSet += delegate (ReferenceHub hub, global::VoiceChat.VcMuteFlags flags)
            {
                if (global::Mirror.NetworkServer.active)
                {
                    hub.connectionToClient.Send(new global::VoiceChat.VoiceChatMuteIndicator.SyncMuteMessage
                    {
                        Flags = (byte)flags
                    });
                }
            };
            global::VoiceChat.VoiceChatPrivacySettings.OnUserFlagsChanged += delegate (ReferenceHub hub)
            {
                if (hub.isLocalPlayer && !(_singleton == null))
                {
                    _singleton.RefreshIndicator();
                }
            };
        }

        private static void ReceiveMessage(global::VoiceChat.VoiceChatMuteIndicator.SyncMuteMessage smm)
        {
            ReceivedFlags = (global::VoiceChat.VcMuteFlags)((int)Filter & (int)smm.Flags);
            if (_singleton != null)
            {
                _singleton.RefreshIndicator();
            }
        }

        private void Start()
        {
            _singleton = this;
            RefreshIndicator();
        }

        private void RefreshIndicator()
        {
            if ((global::VoiceChat.VoiceChatPrivacySettings.PrivacyFlags & global::VoiceChat.VcPrivacyFlags.AllowMicCapture) == 0)
            {
                _locally.SetActive(value: false);
                _globally.SetActive(value: false);
                _root.SetActive(value: true);
                _privacy.SetActive(value: true);
                _privacyText.text = TranslationReader.GetFormatted("Voicechat", 18, "Press \"{0}\" and navigate to \"Settings\" to adjust.", new ReadableKeyCode(ActionName.PauseMenu));
                return;
            }
            _privacy.SetActive(value: false);
            switch (ReceivedFlags)
            {
                case global::VoiceChat.VcMuteFlags.None:
                    _root.SetActive(value: false);
                    break;
                case global::VoiceChat.VcMuteFlags.GlobalRegular:
                    _root.SetActive(value: true);
                    _locally.SetActive(value: false);
                    _globally.SetActive(value: true);
                    break;
                case global::VoiceChat.VcMuteFlags.LocalRegular:
                    _root.SetActive(value: true);
                    _locally.SetActive(value: true);
                    _globally.SetActive(value: false);
                    break;
                case global::VoiceChat.VcMuteFlags.LocalRegular | global::VoiceChat.VcMuteFlags.GlobalRegular:
                    // v12 left this reachable combination (server-mute + local-mute) without a case,
                    // so the indicator kept a stale state. Show both sub-indicators.
                    _root.SetActive(value: true);
                    _locally.SetActive(value: true);
                    _globally.SetActive(value: true);
                    break;
                case global::VoiceChat.VcMuteFlags.LocalIntercom:
                case global::VoiceChat.VcMuteFlags.LocalRegular | global::VoiceChat.VcMuteFlags.LocalIntercom:
                    break;
            }
        }
    }
}
