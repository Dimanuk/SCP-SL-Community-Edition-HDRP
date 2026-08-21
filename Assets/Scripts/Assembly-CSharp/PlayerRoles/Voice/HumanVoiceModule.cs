using System.Diagnostics;
using System.Runtime.CompilerServices;

using PlayerRoles.Visibility;
using UnityEngine;
using VoiceChat;
using VoiceChat.Playbacks;

namespace PlayerRoles.Voice
{
	public class HumanVoiceModule : StandardVoiceModule, IRadioVoiceModule
	{
		private const float RadioProximityRatio = 0.35f;

		[SerializeField]
		private AudioClip[] _radioOnSounds;

		[SerializeField]
		private AudioClip[] _radioOffSounds;

		[SerializeField]
		private float _toggleSoundsVolume;

		private VisibilityController _vctrl;

		private bool _wasTransmitting;

        private bool Transmitting
        {
            get
            {
                return _wasTransmitting;
            }
            set
            {
                if (Transmitting != value)
                {
                    global::AudioPooling.AudioSourcePoolManager.PlaySound((value ? _radioOnSounds : _radioOffSounds).RandomItem(), null, 0f, _toggleSoundsVolume, FalloffType.Exponential, global::AudioPooling.AudioMixerChannelType.VoiceChat, 0f);
                    _wasTransmitting = value;
                }
            }
        }

        [field: global::UnityEngine.SerializeField]
        public global::VoiceChat.Playbacks.SingleBufferPlayback ProximityPlayback { get; private set; }

        [field: global::UnityEngine.SerializeField]
        public global::VoiceChat.Playbacks.SingleBufferPlayback Scp1576Playback { get; private set; }

        [field: global::UnityEngine.SerializeField]
        public global::VoiceChat.Playbacks.PersonalRadioPlayback RadioPlayback { get; private set; }

        public override bool IsSpeaking => ProximityPlayback.MaxSamples > 0;

        private bool CheckProximity(ReferenceHub hub)
        {
            if (hub != base.Owner)
            {
                return _vctrl.ValidateVisibility(hub);
            }
            return false;
        }

        public override global::VoiceChat.VoiceChatChannel ProcessInputs(bool primary, bool alt)
        {
            if ((primary || alt) && global::InventorySystem.Items.Usables.Scp1576.Scp1576Item.LocallyUsed)
            {
                Transmitting = false;
                return global::VoiceChat.VoiceChatChannel.Scp1576;
            }
            if (alt && RadioPlayback.RadioUsable)
            {
                Transmitting = true;
                return global::VoiceChat.VoiceChatChannel.Radio;
            }
            Transmitting = false;
            if (!primary)
            {
                return global::VoiceChat.VoiceChatChannel.None;
            }
            return global::VoiceChat.VoiceChatChannel.Proximity;
        }

        public override void ProcessSamples(float[] data, int len)
        {
            base.ProcessSamples(data, len);
            switch (base.CurrentChannel)
            {
                case global::VoiceChat.VoiceChatChannel.RoundSummary:
                    return;
                case global::VoiceChat.VoiceChatChannel.Scp1576:
                    Scp1576Playback.Buffer.Write(data, len);
                    return;
                case global::VoiceChat.VoiceChatChannel.Radio:
                    RadioPlayback.DistributeSamples(data, len);
                    break;
            }
            ProximityPlayback.Buffer.Write(data, len);
        }

        public override void Update()
		{
            base.Update();
            ReferenceHub hub;
            bool flag = IsSpeaking && base.CurrentChannel == global::VoiceChat.VoiceChatChannel.Radio && ReferenceHub.TryGetLocalHub(out hub) && hub.roleManager.CurrentRole is global::PlayerRoles.Voice.IVoiceRole voiceRole && voiceRole.VoiceModule is global::PlayerRoles.Voice.IRadioVoiceModule radioVoiceModule && radioVoiceModule.RadioPlayback.RadioUsable;
            ProximityPlayback.Source.volume = (flag ? 0.35f : 1f);
        }

		public override void Awake()
		{
            base.Awake();
            _vctrl = (base.Role as global::PlayerRoles.Visibility.ICustomVisibilityRole).VisibilityController;
        }

        public override global::VoiceChat.VoiceChatChannel ValidateSend(global::VoiceChat.VoiceChatChannel channel)
        {
            if (channel != global::VoiceChat.VoiceChatChannel.Proximity)
            {
                if (channel != global::VoiceChat.VoiceChatChannel.Radio)
                {
                    if (channel == global::VoiceChat.VoiceChatChannel.Scp1576)
                    {
                        if (!global::InventorySystem.Items.Usables.Scp1576.Scp1576Item.ValidatedReceivers.Contains(base.Owner))
                        {
                            return global::VoiceChat.VoiceChatChannel.Proximity;
                        }
                        return global::VoiceChat.VoiceChatChannel.Scp1576;
                    }
                }
                else if (RadioPlayback.RadioUsable)
                {
                    goto IL_0033;
                }
                return global::VoiceChat.VoiceChatChannel.None;
            }
            goto IL_0033;
        IL_0033:
            return channel;
        }

        public override global::VoiceChat.VoiceChatChannel ValidateReceive(ReferenceHub speaker, global::VoiceChat.VoiceChatChannel channel)
        {
            global::VoiceChat.VoiceChatChannel voiceChatChannel = base.ValidateReceive(speaker, channel);
            if (voiceChatChannel == global::VoiceChat.VoiceChatChannel.Intercom || voiceChatChannel == global::VoiceChat.VoiceChatChannel.RoundSummary)
            {
                return voiceChatChannel;
            }
            switch ((int)channel)
            {
                case 4:
                    if (global::InventorySystem.Items.Usables.Scp1576.Scp1576Item.ValidatedReceivers.Contains(base.Owner))
                    {
                        return global::VoiceChat.VoiceChatChannel.Scp1576;
                    }
                    break;
                case 1:
                    if (!CheckProximity(speaker))
                    {
                        break;
                    }
                    goto case 2;
                case 2:
                case 7:
                    return channel;
                case 8:
                    return global::VoiceChat.VoiceChatChannel.Proximity;
            }
            return global::VoiceChat.VoiceChatChannel.None;
        }


        public override void SpawnObject()
        {
            base.SpawnObject();
            RadioPlayback.Setup(base.Owner, ProximityPlayback);
            ProximityPlayback.Source.mute = base.Owner.isLocalPlayer;
        }

        public override void ResetObject()
        {
            base.ResetObject();
            _wasTransmitting = false;
        }
    }
}
