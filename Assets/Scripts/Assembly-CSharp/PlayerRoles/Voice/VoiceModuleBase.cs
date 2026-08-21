using GameObjectPools;
using Mirror;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using VoiceChat;
using VoiceChat.Codec;
using VoiceChat.Networking;

namespace PlayerRoles.Voice
{
    public abstract class VoiceModuleBase : MonoBehaviour, IPoolResettable, IPoolSpawnable
    {
        public delegate void SamplesReceived(float[] samples, int len);

        public VoiceChatChannel _lastChannel;

        public OpusDecoder _defaultDecoder;

        public ReferenceHub _owner;

        public int _sentPackets;

        public int _prevSent;

        public const float SilenceTolerance = 0.1f;

        public const float RateLimiterTimeframe = 0.5f;

        public const int RateLimiterTolerance = 128;

        public readonly Stopwatch _rateStopwatch = Stopwatch.StartNew();

        public readonly Stopwatch _silenceStopwatch = Stopwatch.StartNew();

        public static float[] _receiveBuffer;

        public static bool _receiveBufferSet;

        public ReferenceHub Owner => _owner;

        public virtual OpusDecoder Decoder => _defaultDecoder;

        public PlayerRoleBase Role { get; set; }

        public bool ServerIsSending { get; set; }

        public GroupMuteFlags ReceiveFlags { get; set; }

        public VoiceChatChannel CurrentChannel
        {
            get
            {
                return _lastChannel;
            }
            set
            {
                if (_lastChannel != value)
                {
                    _lastChannel = value;
                    OnChannelChanged();
                }
            }
        }

        public abstract bool IsSpeaking { get; }

        public event SamplesReceived OnSamplesReceived;

        public virtual void Awake()
        {
            Role = GetComponent<PlayerRoleBase>();
        }

        public virtual void Update()
        {
            if (_sentPackets > _prevSent)
            {
                ServerIsSending = true;
                _silenceStopwatch.Restart();
            }
            else if (ServerIsSending && _silenceStopwatch.Elapsed.TotalSeconds > 0.10000000149011612)
            {
                ServerIsSending = false;
            }

            if (_rateStopwatch.Elapsed.TotalSeconds >= 0.5)
            {
                _sentPackets = 0;
                _rateStopwatch.Restart();
            }

            _prevSent = _sentPackets;
        }

        public virtual void OnChannelChanged()
        {
        }

        public abstract void ProcessSamples(float[] data, int len);

        public abstract VoiceChatChannel GetUserInput();

        public virtual VoiceChatChannel ValidateSend(VoiceChatChannel channel)
        {
            return channel;
        }

        public virtual VoiceChatChannel ValidateReceive(ReferenceHub speaker, VoiceChatChannel channel)
        {
            return channel;
        }

        public virtual void ResetObject()
        {
            _lastChannel = VoiceChatChannel.None;
            _defaultDecoder?.Dispose();
            ReceiveFlags = GroupMuteFlags.None;
        }

        public virtual void SpawnObject()
        {
            if (Role.TryGetOwner(out _owner))
            {
                _defaultDecoder = new OpusDecoder();
                if (Owner.isLocalPlayer)
                {
                    VoiceChatMicCapture.StartRecording();
                }

                if (NetworkServer.active)
                {
                    ReceiveFlags = VoiceChatReceivePrefs.GetFlagsForUser(Owner);
                }
            }
        }

        public bool CheckRateLimit()
        {
            return _sentPackets++ < 128;
        }

        public void ProcessMessage(VoiceMessage msg)
        {
            CurrentChannel = msg.Channel;
            if (!_receiveBufferSet)
            {
                _receiveBufferSet = true;
                _receiveBuffer = new float[24000];
            }

            int len = Decoder.Decode(msg.Data, msg.DataLength, _receiveBuffer);
            if (Owner.isLocalPlayer || VoiceChatMutes.GetFlags(Owner) == VcMuteFlags.None)
            {
                ProcessSamples(_receiveBuffer, len);
                this.OnSamplesReceived?.Invoke(_receiveBuffer, len);
            }
        }

        public VoiceModuleBase()
        {
        }
    }
}
