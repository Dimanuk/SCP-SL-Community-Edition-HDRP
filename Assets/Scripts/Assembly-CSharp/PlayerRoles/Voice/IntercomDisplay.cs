using System.Runtime.InteropServices;

using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoles.Voice
{
	public class IntercomDisplay : NetworkBehaviour
	{
		private enum IcomText
		{
			Ready = 0,
			Transmitting = 1,
			TrasmittingBypass = 2,
			Restarting = 3,
			AdminUsing = 4,
			Muted = 5,
			Unknown = 6,
			Wait = 7
		}

		private static IntercomDisplay _singleton;

		[SerializeField]
		private Text _text;

		[SyncVar]
		private string _overrideText;

		private Intercom _icom;

		private string[] _translations;

		private bool[] _translationsSet;

        private void Awake()
        {
            _icom = GetComponent<global::PlayerRoles.Voice.Intercom>();
            _singleton = this;
            int num = 0;
            foreach (int value in global::System.Enum.GetValues(typeof(global::PlayerRoles.Voice.IntercomDisplay.IcomText)))
            {
                num = global::UnityEngine.Mathf.Max(num, value + 1);
            }
            _translations = new string[num];
            _translationsSet = new bool[num];
        }

        private void Update()
        {
            if (!string.IsNullOrEmpty(_overrideText))
            {
                _text.text = _overrideText;
                return;
            }
            switch (global::PlayerRoles.Voice.Intercom.State)
            {
                case global::PlayerRoles.Voice.IntercomState.Ready:
                    {
                        bool flag = global::VoiceChat.VoiceChatMuteIndicator.ReceivedFlags != global::VoiceChat.VcMuteFlags.None;
                        _text.text = GetTranslation(flag ? global::PlayerRoles.Voice.IntercomDisplay.IcomText.Muted : global::PlayerRoles.Voice.IntercomDisplay.IcomText.Ready);
                        break;
                    }
                case global::PlayerRoles.Voice.IntercomState.Starting:
                    _text.text = GetTranslation(global::PlayerRoles.Voice.IntercomDisplay.IcomText.Wait);
                    break;
                case global::PlayerRoles.Voice.IntercomState.InUse:
                    _text.text = (_icom.BypassMode ? GetTranslation(global::PlayerRoles.Voice.IntercomDisplay.IcomText.TrasmittingBypass) : string.Format(GetTranslation(global::PlayerRoles.Voice.IntercomDisplay.IcomText.Transmitting), global::UnityEngine.Mathf.Round(_icom.RemainingTime)));
                    break;
                case global::PlayerRoles.Voice.IntercomState.Cooldown:
                    _text.text = string.Format(GetTranslation(global::PlayerRoles.Voice.IntercomDisplay.IcomText.Restarting), global::UnityEngine.Mathf.Round(_icom.RemainingTime));
                    break;
                case global::PlayerRoles.Voice.IntercomState.NotFound:
                    _text.text = GetTranslation(global::PlayerRoles.Voice.IntercomDisplay.IcomText.Unknown);
                    break;
            }
        }

        private string GetTranslation(global::PlayerRoles.Voice.IntercomDisplay.IcomText val)
        {
            int num = (int)val;
            if (!_translationsSet[num])
            {
                _translations[num] = TranslationReader.Get("Intercom", num, val.ToString());
                _translationsSet[num] = true;
            }
            return _translations[num];
        }

        public static bool TrySetDisplay(string str)
        {
            if (_singleton == null)
            {
                return false;
            }
            _singleton._overrideText = str;
            return true;
        }
    }
}
