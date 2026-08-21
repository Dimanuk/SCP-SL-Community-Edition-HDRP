using UnityEngine;
using VoiceChat;
using VoiceChat.Playbacks;
using VoiceChat.Networking;

namespace PlayerRoles.PlayableScps.Scp079
{
    public class Scp079VoiceModule : StandardScpVoiceModule
    {

        private Scp079SpeakerAbility _speakerAbility;

        public const VoiceChatChannel SpeakerChannel = VoiceChatChannel.Proximity;

        [field: SerializeField]
        public SingleBufferPlayback ProximityPlayback { get; private set; }

        public override void Awake()
        {
            base.Awake();

            (Role as Scp079Role)?.SubroutineModule.TryGetSubroutine(out _speakerAbility);
        }

        public override VoiceChatChannel ProcessInputs(bool primary, bool alt)
        {
            if (alt)
            {
                var lostHandler = _speakerAbility?.LostSignalHandler;
                if (lostHandler != null && !lostHandler.Lost)
                    return VoiceChatChannel.Proximity;
            }

            if (!primary)
                return VoiceChatChannel.None;

            return PrimaryChannel;
        }

        public override VoiceChatChannel ValidateSend(VoiceChatChannel channel)
        {
            if (channel == VoiceChatChannel.Proximity)
            {
                var lostHandler = _speakerAbility?.LostSignalHandler;
                if (lostHandler != null && !lostHandler.Lost)
                    return channel;
            }

            return base.ValidateSend(channel);
        }

        public override void ProcessSamples(float[] data, int len)
        {
            if (CurrentChannel != VoiceChatChannel.Proximity)
            {
                base.ProcessSamples(data, len);
                return;
            }

            var buffer = ProximityPlayback?.Buffer;
            buffer?.Write(data, len);
        }
    }
}
