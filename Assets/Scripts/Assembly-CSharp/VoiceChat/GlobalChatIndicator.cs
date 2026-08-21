namespace VoiceChat
{
    public class GlobalChatIndicator : global::UnityEngine.MonoBehaviour
    {
        [global::UnityEngine.SerializeField]
        private global::TMPro.TextMeshProUGUI _nickname;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.UI.RawImage _icon;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.UI.Graphic[] _backgrounds;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.UI.Outline[] _outlines;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.GameObject _iconRoot;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.Texture _radioIcon;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.Texture _intercomIcon;

        private global::VoiceChat.Playbacks.IGlobalPlayback _playback;

        private ReferenceHub _owner;

        private bool _wasSpeaking;

        private float _noSpeakTime;

        private global::UnityEngine.Color _lastColor;

        private global::UnityEngine.Transform _t;

        private const float SustainTime = 0.3f;

        public void Setup(global::VoiceChat.Playbacks.IGlobalPlayback igp, ReferenceHub owner)
        {
            _playback = igp;
            _owner = owner;
            _t = base.transform;
            _noSpeakTime = 0f;
        }

        public void Refresh()
        {
            if (!_playback.GlobalChatActive)
            {
                if (_wasSpeaking)
                {
                    _noSpeakTime += global::UnityEngine.Time.deltaTime;
                    SetColors(0f);
                    if (!(_noSpeakTime < 0.3f))
                    {
                        base.gameObject.SetActive(value: false);
                        _wasSpeaking = false;
                    }
                }
                return;
            }
            if (!_wasSpeaking)
            {
                base.gameObject.SetActive(value: true);
                _t.SetAsLastSibling();
                _wasSpeaking = true;
            }
            _lastColor = _playback.GlobalChatColor;
            SetColors(_playback.GlobalChatLoudness);
            if (TryGetIcon(_playback.GlobalChatIcon, _owner, out var result))
            {
                _icon.texture = result;
                _iconRoot.SetActive(value: true);
            }
            else
            {
                _iconRoot.SetActive(value: false);
            }
            _nickname.text = _playback.GlobalChatName;
        }

        private void SetColors(float loudness)
        {
            global::UnityEngine.UI.Graphic[] backgrounds = _backgrounds;
            for (int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i].color = global::UnityEngine.Color.Lerp(global::UnityEngine.Color.black, _lastColor, loudness);
            }
            global::UnityEngine.UI.Outline[] outlines = _outlines;
            for (int i = 0; i < outlines.Length; i++)
            {
                outlines[i].effectColor = global::UnityEngine.Color.Lerp(_lastColor, global::UnityEngine.Color.white, loudness);
            }
        }

        private bool TryGetIcon(global::VoiceChat.Playbacks.GlobalChatIconType icon, ReferenceHub owner, out global::UnityEngine.Texture result)
        {
            result = null;
            switch (icon)
            {
                case global::VoiceChat.Playbacks.GlobalChatIconType.None:
                    return false;
                case global::VoiceChat.Playbacks.GlobalChatIconType.Radio:
                    result = _radioIcon;
                    return true;
                case global::VoiceChat.Playbacks.GlobalChatIconType.Intercom:
                    result = _intercomIcon;
                    return true;
                case global::VoiceChat.Playbacks.GlobalChatIconType.Avatar:
                    if (owner == null || !(owner.roleManager.CurrentRole is global::PlayerRoles.IAvatarRole avatarRole))
                    {
                        return false;
                    }
                    result = avatarRole.RoleAvatar;
                    return true;
                default:
                    return false;
            }
        }
    }
}
