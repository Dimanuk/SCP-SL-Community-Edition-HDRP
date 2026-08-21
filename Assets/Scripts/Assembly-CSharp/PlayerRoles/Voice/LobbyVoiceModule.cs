
using PlayerRoles.Spectating;
using VoiceChat;

namespace PlayerRoles.Voice
{
	public class LobbyVoiceModule : SpectatorVoiceModule
	{
        public VoiceChatChannel ValidateAuth(VoiceChatChannel channelToValidate)
        {
            if (base.Owner.characterClassManager.InstanceMode != 0)
            {
                return channelToValidate;
            }

            return VoiceChatChannel.None;
        }

        public override VoiceChatChannel ValidateSend(VoiceChatChannel channel)
        {
            return ValidateAuth(base.ValidateSend(channel));
        }

        public override VoiceChatChannel ValidateReceive(ReferenceHub sp, VoiceChatChannel ch)
        {
            // Server build guards its own (dead) host hub here; the client build has no
            // isLocalPlayer check at all, so a playing listen-host must still receive.
            if (base.Owner.isLocalPlayer && ServerStatic.IsDedicated)
            {
                return VoiceChatChannel.None;
            }

            if ((base.ReceiveFlags & GroupMuteFlags.Lobby) != 0)
            {
                return VoiceChatChannel.None;
            }

            return ValidateAuth(base.ValidateReceive(sp, ch));
        }
    }
}
