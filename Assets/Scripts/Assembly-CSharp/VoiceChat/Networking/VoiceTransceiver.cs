namespace VoiceChat.Networking
{
    public static class VoiceTransceiver
    {
        private static readonly global::System.Collections.Generic.List<global::VoiceChat.Codec.OpusEncoder> Encoders = new global::System.Collections.Generic.List<global::VoiceChat.Codec.OpusEncoder>();

        private static int _encodersCount;

        private static int _packageSize;

        private static float[] _sendBuffer;

        private static byte[] _encodedBuffer;

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += delegate
            {
                global::Mirror.NetworkServer.ReplaceHandler<global::VoiceChat.Networking.VoiceMessage>(ServerReceiveMessage);
                global::Mirror.NetworkClient.ReplaceHandler<global::VoiceChat.Networking.VoiceMessage>(ClientReceiveMessage);
            };
            _packageSize = 480;
            _sendBuffer = new float[_packageSize];
            _encodedBuffer = new byte[512];
        }

        private static void ServerReceiveMessage(global::Mirror.NetworkConnection conn, global::VoiceChat.Networking.VoiceMessage msg)
        {
            if (msg.SpeakerNull || msg.Speaker.netId != conn.identity.netId || !(msg.Speaker.roleManager.CurrentRole is global::PlayerRoles.Voice.IVoiceRole voiceRole) || !voiceRole.VoiceModule.CheckRateLimit())
            {
                return;
            }
            global::VoiceChat.VcMuteFlags flags = global::VoiceChat.VoiceChatMutes.GetFlags(msg.Speaker);
            if ((flags & (global::VoiceChat.VcMuteFlags.GlobalRegular | global::VoiceChat.VcMuteFlags.LocalRegular)) != global::VoiceChat.VcMuteFlags.None)
            {
                return;
            }
            global::VoiceChat.VoiceChatChannel voiceChatChannel = voiceRole.VoiceModule.ValidateSend(msg.Channel);
            if (voiceChatChannel == global::VoiceChat.VoiceChatChannel.None)
            {
                return;
            }
            voiceRole.VoiceModule.CurrentChannel = voiceChatChannel;
            bool hearYourself = global::VoiceChat.VoiceChatMicCapture.HearYourself;
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.roleManager.CurrentRole is global::PlayerRoles.Voice.IVoiceRole voiceRole2)
                {
                    global::VoiceChat.VoiceChatChannel voiceChatChannel2 = voiceRole2.VoiceModule.ValidateReceive(msg.Speaker, voiceChatChannel);
                    if (voiceChatChannel2 == global::VoiceChat.VoiceChatChannel.None && hearYourself && allHub == msg.Speaker)
                    {
                        voiceChatChannel2 = voiceChatChannel;
                    }
                    if (voiceChatChannel2 != global::VoiceChat.VoiceChatChannel.None)
                    {
                        msg.Channel = voiceChatChannel2;
                        allHub.connectionToClient.Send(msg);
                    }
                }
            }
        }

        private static void ClientReceiveMessage(global::VoiceChat.Networking.VoiceMessage msg)
        {
            if (!msg.SpeakerNull && msg.Speaker.roleManager.CurrentRole is global::PlayerRoles.Voice.IVoiceRole voiceRole)
            {
                voiceRole.VoiceModule.ProcessMessage(msg);
            }
        }

        public static void ClientSendData(global::VoiceChat.Networking.PlaybackBuffer micBuffer, global::VoiceChat.VoiceChatChannel targetChannel, int encoderId = 0)
        {
            if (ReferenceHub.TryGetLocalHub(out var hub))
            {
                while (_encodersCount <= encoderId)
                {
                    Encoders.Add(new global::VoiceChat.Codec.OpusEncoder(global::VoiceChat.Codec.Enums.OpusApplicationType.Voip));
                    _encodersCount++;
                }
                global::VoiceChat.Codec.OpusEncoder opusEncoder = Encoders[encoderId];
                while (micBuffer.Length >= _packageSize)
                {
                    micBuffer.ReadTo(_sendBuffer, _packageSize, 0L);
                    int dataLen = opusEncoder.Encode(_sendBuffer, _encodedBuffer);
                    global::Mirror.NetworkClient.Send(new global::VoiceChat.Networking.VoiceMessage(hub, targetChannel, _encodedBuffer, dataLen, isNull: false));
                }
            }
        }
    }
}
