using Mirror;
using PlayerRoles.Spectating;
using PostProcessing;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace CustomPlayerEffects
{
    public class Flashed : StatusEffectBase
    {
        public const float EnableSpeedMultiplier = 2.5f;
        public const float DisableSpeedMultiplier = 0.9f;

        private PostProcessVolume _processVolume;
        private Lighten _lightenEffect;
        private Darken _darkenEffect;
        private float _remainingWeight;

        private float Weight
        {
            get => _remainingWeight;
            set
            {
                _remainingWeight = Mathf.Clamp01(value);

                if (Hub != null && (Hub.isLocalPlayer || SpectatorNetworking.IsLocallySpectated(Hub)))
                {
                    if (_processVolume != null)
                        _processVolume.weight = _remainingWeight;
                }
            }
        }

        protected override void OnAwake()
        {
            _processVolume = GetComponent<PostProcessVolume>();
            if (_processVolume != null)
            {
                var profile = _processVolume.profile;
                _lightenEffect = profile?.GetSetting<Lighten>();
                _darkenEffect = profile?.GetSetting<Darken>();
            }
        }

        protected override void Enabled()
        {
            bool darkMode = PlayerPrefsSl.Get(SettingsOption.DarkMode.ToString(), false);

            if (_lightenEffect != null)
                _lightenEffect.active = !darkMode;

            if (_darkenEffect != null)
                _darkenEffect.active = darkMode;
        }

        protected override void IntensityChanged(byte prevState, byte newState)
        {
            float timeLeft = newState * 0.1f;

            if (NetworkServer.active)
            {
                TimeLeft = timeLeft;
            }

            if (Hub != null && (Hub.isLocalPlayer || SpectatorNetworking.IsLocallySpectated(Hub)))
            {
                if (timeLeft > 0f && _processVolume != null)
                {
                    _processVolume.enabled = true;
                    Weight = 0f;
                }
            }
        }

        public override void OnBeginSpectating()
        {
            if (Hub != null && IsEnabled && _processVolume != null)
            {
                _processVolume.enabled = true;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (NetworkServer.active && Duration == 0f)
            {
                TimeLeft = 1f;
            }

            if (Hub != null && !Hub.isLocalPlayer && !SpectatorNetworking.IsLocallySpectated(Hub))
            {
                if (_processVolume != null)
                    _processVolume.weight = 0f;
                return;
            }

            if (Intensity > 0)
            {
                Weight += Time.deltaTime * EnableSpeedMultiplier;
            }
            else
            {
                if (Weight <= 0f)
                {
                    if (_processVolume != null)
                        _processVolume.enabled = false;
                }
                else
                {
                    Weight -= Time.deltaTime * DisableSpeedMultiplier;
                }
            }
        }
    }
}