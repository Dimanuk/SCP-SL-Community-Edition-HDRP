using System.Diagnostics;
using AudioPooling;
using CustomPlayerEffects;
using PlayerRoles;
using UnityEngine;

namespace InventorySystem.Items.Usables.Scp244.Hypothermia
{
    public class AudioSubEffect : HypothermiaSubEffectBase, ISoundtrackMutingEffect
    {
        [SerializeField] private TemperatureSubEffect _temperature;
        [SerializeField] private AudioSource _fogSoundtrack;
        [SerializeField] private float _soundtrackFadeSpeed;
        [SerializeField] private AudioClip _enterFogSound;
        [SerializeField] private AnimationCurve _shakingOverTemperature;
        [SerializeField] private AudioSource _shakingSoundSource;
        [SerializeField] private float _thirdpersonShakeVolume;

        private bool _prevExposed;
        private readonly Stopwatch _enterSfxCooldown;

        private const float SfxDistance = 10f;
        private const float SfxCooldown = 1.5f;

        public bool MuteSoundtrack { get; private set; }


        public override bool IsActive => false;

        private void UpdateShake(float curTemp)
        {
            if (!PlayerRolesUtils.IsHuman(Hub))
                curTemp = 0f;

            float volume = _shakingOverTemperature.Evaluate(curTemp);

            volume *= IsLocalPlayer ? 1f : _thirdpersonShakeVolume;

            _shakingSoundSource.volume = volume;
        }

        private void UpdateExposure(bool isExposed)
        {
            bool shouldMute = isExposed;

            if (!IsLocalPlayer || !PlayerRolesUtils.IsAlive(Hub))
                shouldMute = false;

            MuteSoundtrack = shouldMute;


            float targetVolume = shouldMute ? 1f : 0f;
            float currentVolume = _fogSoundtrack.volume;

            _fogSoundtrack.volume = Mathf.Lerp(currentVolume, targetVolume, _soundtrackFadeSpeed * Time.deltaTime);

            if (shouldMute == _prevExposed)
                return;

            _prevExposed = shouldMute;
            if (shouldMute)
            {
                if (_enterSfxCooldown.Elapsed.TotalSeconds > SfxCooldown)
                {
                    AudioSourcePoolManager.PlaySound(_enterFogSound, transform.position, SfxDistance);
                }
            }
            else
            {
                _enterSfxCooldown.Restart();
            }
        }

        internal override void UpdateEffect(float curExposure)
        {

            UpdateExposure(curExposure > 0f);
            UpdateShake(_temperature.CurTemperature);
        }

        public AudioSubEffect()
        {
            _enterSfxCooldown = Stopwatch.StartNew();
        }
    }
}