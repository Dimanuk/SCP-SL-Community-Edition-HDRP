using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CustomPlayerEffects
{
    public class LerpAudioMixerVisuals : LerpVisualsBase
    {
        [SerializeField]
        private string _mixerFloatName;

        [SerializeField]
        private float _mixerTargetValue;

        private PlayerEffectsController _playerEffectsController;
        private float _defaultMixerValue;

        private static readonly Dictionary<string, float> DefaultValues = new();

        protected override void Awake()
        {
            base.Awake();

            if (TargetEffect == null)
                return;

            _playerEffectsController = TargetEffect.Hub.playerEffectsController;

            if (string.IsNullOrEmpty(_mixerFloatName))
                return;

            if (DefaultValues.TryGetValue(_mixerFloatName, out float cachedValue))
            {
                _defaultMixerValue = cachedValue;
            }
            else
            {
                AudioMixer mixer = _playerEffectsController?.mixer;
                if (mixer != null && mixer.GetFloat(_mixerFloatName, out float currentValue))
                {
                    _defaultMixerValue = currentValue;
                    DefaultValues[_mixerFloatName] = currentValue;
                }
            }
        }

        protected override void OnWeightChanged(float weight)
        {
            if (_playerEffectsController == null || string.IsNullOrEmpty(_mixerFloatName))
                return;

            AudioMixer mixer = _playerEffectsController.mixer;
            if (mixer != null)
                mixer.SetFloat(_mixerFloatName, Mathf.Lerp(_defaultMixerValue, _mixerTargetValue, weight));
        }

        protected override void OnShutdown()
        {
            if (_playerEffectsController == null || string.IsNullOrEmpty(_mixerFloatName))
                return;

            AudioMixer mixer = _playerEffectsController.mixer;
            if (mixer != null)
                mixer.SetFloat(_mixerFloatName, _defaultMixerValue);
        }
    }
}
