using System.Diagnostics;

using UnityEngine;
using VoiceChat;
using VoiceChat.Playbacks;

namespace PlayerRoles.Voice
{
    public abstract class StandardVoiceModule : VoiceModuleBase, IGlobalPlayback
    {
        public const float SustainTime = 0.2f;

        public bool _primHeld;

        public bool _altHeld;

        public readonly Stopwatch _primSw = new Stopwatch();

        public readonly Stopwatch _altSw = new Stopwatch();

        [SerializeField]
        public SingleBufferPlayback GlobalPlayback;

        public virtual bool GlobalChatActive => GlobalPlayback.MaxSamples > 0;

        public virtual Color GlobalChatColor => base.Owner.serverRoles.GetVoiceColor();

        public virtual string GlobalChatName => base.Owner.nicknameSync.DisplayName;

        public virtual float GlobalChatLoudness => GlobalPlayback.Loudness;

        public virtual GlobalChatIconType GlobalChatIcon
        {
            get
            {
                if (!IsRoundSummary)
                {
                    return GlobalChatIconType.Avatar;
                }

                return GlobalChatIconType.None;
            }
        }

        public bool IsRoundSummary => RoundSummary.SummaryActive;

        public override VoiceChatChannel GetUserInput()
        {
            KeyCode key = NewInput.GetKey(ActionName.VoiceChat);
            KeyCode key2 = NewInput.GetKey(ActionName.AltVoiceChat);
            return ProcessInputs(ProcessKey(key, ref _primHeld, _primSw), ProcessKey(key2, ref _altHeld, _altSw));
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            GlobalChatIndicatorManager.Subscribe(this, base.Owner);
        }

        public override void ResetObject()
        {
            base.ResetObject();
            _primHeld = false;
            _altHeld = false;
            _primSw.Stop();
            _altSw.Stop();
            GlobalChatIndicatorManager.Unsubscribe(this);
        }

        public override VoiceChatChannel ValidateReceive(ReferenceHub speaker, VoiceChatChannel channel)
        {
            if (speaker == base.Owner)
            {
                return VoiceChatChannel.None;
            }

            if (channel == VoiceChatChannel.Mimicry)
            {
                return channel;
            }

            if (IsRoundSummary && (base.ReceiveFlags & GroupMuteFlags.Summary) == 0)
            {
                return VoiceChatChannel.RoundSummary;
            }

            if (Intercom.CheckPerms(speaker) && channel != VoiceChatChannel.Scp1576)
            {
                return VoiceChatChannel.Intercom;
            }

            return channel;
        }

        public override void ProcessSamples(float[] data, int len)
        {
            switch (base.CurrentChannel)
            {
                case VoiceChatChannel.RoundSummary:
                    GlobalPlayback.WriteBuffer(data, len);
                    break;
                case VoiceChatChannel.Intercom:
                    IntercomPlayback.ProcessSamples(base.Owner, data, len);
                    break;
            }
        }

        public abstract VoiceChatChannel ProcessInputs(bool primary, bool alt);

        public bool ProcessKey(KeyCode kc, ref bool prev, Stopwatch sw)
        {
            if (Input.GetKeyDown(kc))
            {
                prev = true;
            }

            if (!Input.GetKey(kc))
            {
                prev = false;
            }

            if (prev)
            {
                sw.Restart();
            }

            if (sw.IsRunning)
            {
                return sw.Elapsed.TotalSeconds < 0.2;
            }

            return false;
        }
    }
}
