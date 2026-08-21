
using UnityEngine.Rendering.PostProcessing;

namespace CustomPlayerEffects
{
	public class VignettePulse : PostProcessEffectPulse
	{
		private Vignette _effect;

        protected override float EffectValue
        {
            get
            {
                if (_effect == null)
                    throw new System.NullReferenceException(nameof(_effect));

                return _effect.intensity.value;
            }
            set
            {
                if (_effect == null)
                    throw new System.NullReferenceException(nameof(_effect));

                _effect.intensity.value = value;
            }
        }

        protected override void SetEffectType(PostProcessProfile profile)
        {
            Vignette setting = profile.GetSetting<Vignette>();

            if (setting == null)
                throw new System.NullReferenceException(
                    $"Не удалось получить настройку Vignette из профиля {profile.name}");

            _effect = setting;
        }
    }
}
