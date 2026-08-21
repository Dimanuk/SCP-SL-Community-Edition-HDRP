namespace VoiceChat
{
    public static class VoiceChatSettings
    {
        public const int SampleRate = 48000;

        public const int Channels = 1;

        public const int PacketSizePerChannel = 480;

        public const int BufferLength = 24000;

        public const int NoiseReductionBuffer = 480;

        public const int MaxEncodedSize = 512;

        public const int MaxBitrate = 120000;

        public static readonly global::VoiceChat.CaressNoiseReduction.NoiseReducerConfig NoiseReductionSettings = new global::VoiceChat.CaressNoiseReduction.NoiseReducerConfig
        {
            Model = global::VoiceChat.CaressNoiseReduction.RnNoiseModel.Speech,
            NumChannels = 1,
            SampleRate = 48000,
            Attenuation = 10.0
        };
    }
}
