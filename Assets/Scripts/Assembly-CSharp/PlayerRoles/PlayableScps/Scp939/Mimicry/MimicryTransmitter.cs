using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;
using VoiceChat;
using VoiceChat.Networking;

namespace PlayerRoles.PlayableScps.Scp939.Mimicry
{

    public class MimicryTransmitter : ScpStandardSubroutine<Scp939Role>
    {
        private PlaybackBuffer _copierPlayback;
        private PlaybackBuffer _senderPlayback;

        private int _playbackSize;
        private int _allowedSamples;
        private int _samplesPerSecond;

        private const int HeadSamples = 1920;

        protected override void Awake()
        {
            base.Awake();
            _samplesPerSecond = 48000;
        }

        public void SendVoice(PlaybackBuffer pb)
        {
            pb.Reorganize();
            int bufferLength = pb.Buffer.Length;

            if (_playbackSize < bufferLength)
            {
                _copierPlayback = new PlaybackBuffer(bufferLength);
                _senderPlayback = new PlaybackBuffer(bufferLength);
                _playbackSize = bufferLength;
            }
            else
            {
                _copierPlayback.Clear();
                _senderPlayback.Clear();
            }

            _copierPlayback.Write(pb.Buffer, pb.Length);
            _allowedSamples = HeadSamples;
        }

        public override void ResetObject()
        {
            base.ResetObject();
            _copierPlayback?.Clear();
            _senderPlayback?.Clear();
        }


        private void Update()
        {
            if (!base.Owner.isLocalPlayer || _playbackSize == 0)
                return;

            _allowedSamples += Mathf.CeilToInt(Time.deltaTime * _samplesPerSecond);

            int samplesToSend = Mathf.Min(_allowedSamples, _copierPlayback.Length);

            if (samplesToSend > 0)
            {
                _copierPlayback.ReadTo(_senderPlayback.Buffer, samplesToSend, _senderPlayback.WriteHead);
                _senderPlayback.WriteHead += samplesToSend;
            }

            _allowedSamples = 0;
			
            VoiceTransceiver.ClientSendData(_senderPlayback, VoiceChatChannel.Mimicry, 1);
        }
    }
}