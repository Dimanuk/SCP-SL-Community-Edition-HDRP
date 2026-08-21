using VoiceChat;
using VoiceChat.Codec;
using VoiceChat.Playbacks;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939
{
	public class Scp939VoiceModule : StandardScpVoiceModule
	{
		[SerializeField]
		private SingleBufferPlayback _proximityChat;

		private OpusDecoder _mimicryDecoder;

		private bool _mimicryDecoderSet;

		public const VoiceChatChannel MimicryChannel = VoiceChatChannel.Mimicry;

		public override OpusDecoder Decoder
		{
			get
			{
				if (base.CurrentChannel != VoiceChatChannel.Mimicry)
				{
					return base.Decoder;
				}
				if (!_mimicryDecoderSet)
				{
					_mimicryDecoder = new OpusDecoder();
					_mimicryDecoderSet = true;
				}
				return _mimicryDecoder;
			}
		}

		public override void ProcessSamples(float[] data, int len)
		{
			if (base.CurrentChannel == VoiceChatChannel.Mimicry)
			{
				_proximityChat.Buffer.Write(data, len);
			}
			else
			{
				base.ProcessSamples(data, len);
			}
		}

		public override VoiceChatChannel ValidateReceive(ReferenceHub speaker, VoiceChatChannel channel)
		{
			if (channel != VoiceChatChannel.Mimicry)
			{
				return base.ValidateReceive(speaker, channel);
			}
			return channel;
		}

		public override VoiceChatChannel ValidateSend(VoiceChatChannel channel)
		{
			if (channel != VoiceChatChannel.Mimicry)
			{
				return base.ValidateSend(channel);
			}
			return channel;
		}

		public override void ResetObject()
		{
			base.ResetObject();
			if (_mimicryDecoderSet)
			{
				_mimicryDecoder?.Dispose();
				_mimicryDecoderSet = false;
			}
		}
	}
}
