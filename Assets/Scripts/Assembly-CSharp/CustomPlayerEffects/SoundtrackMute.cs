namespace CustomPlayerEffects
{
    public class SoundtrackMute : global::CustomPlayerEffects.StatusEffectBase, global::CustomPlayerEffects.ISoundtrackMutingEffect, global::RemoteAdmin.Interfaces.ICustomRADisplay
    {
        public string DisplayName { get; }

        public bool CanBeDisplayed { get; }

        public bool MuteSoundtrack => base.IsEnabled;
    }
}
