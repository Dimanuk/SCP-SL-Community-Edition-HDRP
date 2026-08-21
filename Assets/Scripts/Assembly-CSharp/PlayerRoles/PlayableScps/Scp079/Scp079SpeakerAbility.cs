using PlayerRoles.PlayableScps.Scp079.Cameras;
using PlayerRoles.PlayableScps.Scp079.Map;
using UnityEngine;
using VoiceChat;

namespace PlayerRoles.PlayableScps.Scp079
{
    public class Scp079SpeakerAbility : Scp079KeyAbilityBase
    {
        private string _abilityName;
        private Scp079VoiceModule _voiceModule;

        public override float AuxRegenMultiplier
        {
            get
            {
                if (_voiceModule == null || !_voiceModule.ServerIsSending || _voiceModule.CurrentChannel != VoiceChatChannel.Proximity)
                    return 1f;

                VcMuteFlags flags = VoiceChatMutes.GetFlags(base.Owner);
                if ((flags & (VcMuteFlags.LocalRegular | VcMuteFlags.GlobalRegular)) == 0)
                    return 0f;

                return 1f;
            }
        }

        public bool CanTransmit => !base.LostSignalHandler.Lost;

        public override ActionName ActivationKey => ActionName.AltVoiceChat;

        public override bool IsReady => true;

        public override bool IsVisible
        {
            get
            {
                if (Scp079ToggleMapAbility.MapVisible)
                    return false;

                return CanTransmit;
            }
        }

        public override string AbilityName => _abilityName;

        public override string FailMessage => null;

        protected override void Trigger()
        {
        }

        protected override void Awake()
        {
            base.Awake();
            _abilityName = Translations.Get(Scp079HudTranslation.UseSpeaker);
            base.CurrentCamSync.OnCameraChanged += RefreshNearestSpeaker;
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            _voiceModule = base.ScpRole.VoiceModule as Scp079VoiceModule;
            RefreshNearestSpeaker();
        }

        private void RefreshNearestSpeaker()
        {
            if (!base.CurrentCamSync.TryGetCurrentCamera(out var cam))
                return;

            if (!Scp079Speaker.TryGetSpeaker(cam, out var best))
                return;

            if (_voiceModule?.ProximityPlayback?.transform == null)
                return;

            _voiceModule.ProximityPlayback.transform.position = best.Position;
        }
    }
}
