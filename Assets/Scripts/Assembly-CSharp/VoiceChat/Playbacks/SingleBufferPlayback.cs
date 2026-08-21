
using VoiceChat.Networking;

namespace VoiceChat.Playbacks
{
    public class SingleBufferPlayback : VoiceChatPlaybackBase
    {
        public PlaybackBuffer _buffer;

        public bool _bufferSet;

        public PlaybackBuffer Buffer
        {
            get
            {
                if (!_bufferSet)
                {
                    _buffer = new PlaybackBuffer();
                    _bufferSet = true;
                }

                return _buffer;
            }
        }

        public override int MaxSamples
        {
            get
            {
                if (!_bufferSet)
                {
                    return 0;
                }

                return Buffer.Length;
            }
        }

        public void OnDestroy()
        {
            if (_bufferSet)
            {
                Buffer.Dispose();
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (_bufferSet)
            {
                Buffer.Clear();
            }
        }

        public override float ReadSample()
        {
            return Buffer.Read();
        }

        public void WriteBuffer(float[] samples, int len, int sourceIndex = 0)
        {
            RegisterInput();
            Buffer.Write(samples, len, sourceIndex);
        }

        public void ClearBuffer()
        {
            Buffer.Clear();
        }
    }
}
