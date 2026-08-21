
using PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace CustomPlayerEffects
{
	public class VignetteRefractionPulse : PostProcessEffectPulse
	{
		private VignetteRefraction _effect;

        protected override float EffectValue
        {
            get
            {
                if (_effect == null)
                    throw new System.NullReferenceException(nameof(_effect));

                return _effect.RefractionPower.value;
            }
            set
            {
                if (_effect == null)
                    throw new System.NullReferenceException(nameof(_effect));

                _effect.RefractionPower.value = value;
            }
        }

        protected override void SetEffectType(PostProcessProfile profile)
        {
            VignetteRefraction setting = profile.GetSetting<VignetteRefraction>();

            if (setting == null)
                throw new System.NullReferenceException(
                    $"Не удалось получить настройку VignetteRefraction из профиля {profile.name}");

            _effect = setting;
        }
    }
}
