namespace PlayerRoles.Voice
{
    public abstract class GlobalVoiceModuleBase : global::PlayerRoles.Voice.StandardVoiceModule
    {
        public override bool IsSpeaking => base.GlobalChatActive;

        protected abstract global::VoiceChat.VoiceChatChannel PrimaryChannel { get; }

        public override global::VoiceChat.VoiceChatChannel ValidateSend(global::VoiceChat.VoiceChatChannel channel)
        {
            if (channel != PrimaryChannel)
            {
                return global::VoiceChat.VoiceChatChannel.None;
            }
            return channel;
        }

        public override void ProcessSamples(float[] data, int len)
        {
            GlobalPlayback.Buffer.Write(data, len);
        }

        public override global::VoiceChat.VoiceChatChannel ValidateReceive(ReferenceHub speaker, global::VoiceChat.VoiceChatChannel channel)
        {
            channel = base.ValidateReceive(speaker, channel);
            if (channel == PrimaryChannel)
            {
                return channel;
            }
            if ((uint)(channel - 1) <= 1u || (uint)(channel - 5) <= 2u)
            {
                return channel;
            }
            return global::VoiceChat.VoiceChatChannel.None;
        }

        public override global::VoiceChat.VoiceChatChannel ProcessInputs(bool primary, bool alt)
        {
            if (!primary)
            {
                return global::VoiceChat.VoiceChatChannel.None;
            }
            return PrimaryChannel;
        }
    }
}
