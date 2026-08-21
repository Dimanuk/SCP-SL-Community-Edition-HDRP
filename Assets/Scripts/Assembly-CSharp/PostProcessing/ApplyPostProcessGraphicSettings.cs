using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using CustomPlayerEffects;

namespace PostProcessing
{
    public class ApplyPostProcessGraphicSettings : MonoBehaviour
    {
        public bool applyMotionBlur = true;
        public bool applyBrightness;
        public bool applyBloom;
        public bool applyAmbientOcclusion;
        public bool applyAntialiasing = true;

        private PostProcessProfile postProcessProfile;
        private float _desiredBrightness;
        private float _maxBrightness = 1f;

        private void Start()
        {
            PostProcessVolume volume = GetComponent <PostProcessVolume>();
            if (volume != null)
                postProcessProfile = volume.sharedProfile;

            if (postProcessProfile == null)
                return;

            if (applyBrightness)
            {
                GammaSlider.OnGammaChanged += OnGammaChanged;
                InsufficientLighting.OnLimitChanged += OnLimitChanged;
            }

            if (applyMotionBlur && postProcessProfile.TryGetSettings(out MotionBlur motionBlur))
            {
                int value = PlayerPrefsSl.Get(((int)SettingsOption.gfxsets_mb).ToString(), 0);
                motionBlur.enabled.Override(value != 0);
            }

            if (applyBrightness)
            {
                _desiredBrightness = PlayerPrefsSl.Get("UVBrightness2", 0f);
                _maxBrightness = 1f;
                UpdateBrightness();
            }

            if (applyBloom && postProcessProfile.TryGetSettings(out Bloom bloom))
            {
                bool enabled = PlayerPrefsSl.Get(((int)SettingsOption.gfxsets_bloom).ToString(), true);
                bloom.enabled.Override(enabled);
            }

            if (applyAmbientOcclusion && postProcessProfile.TryGetSettings(out AmbientOcclusion ao))
            {
                int mode = PlayerPrefsSl.Get(((int)SettingsOption.gfxsets_ao).ToString(), 2);
                if (mode == 1)
                    ao.enabled.Override(true);
                else if (mode == 2)
                    ao.enabled.Override(false);
            }

            PostProcessLayer layer = GetComponent <PostProcessLayer>();
            if (layer != null && applyAntialiasing)
            {
                int mode = PlayerPrefsSl.Get(((int)SettingsOption.gfxsets_aa).ToString(), 1);
                layer.antialiasingMode = (PostProcessLayer.Antialiasing)mode;
            }
        }

        private void UpdateBrightness()
        {
            if (postProcessProfile == null)
                return;

            if (postProcessProfile.TryGetSettings(out ColorGrading colorGrading))
            {
                float brightness = Mathf.Min(_desiredBrightness, _maxBrightness);
                // Override() flips overrideState on as well — assigning .value alone leaves the
                // parameter un-overridden, so the post-process stack ignores it and the slider
                // appears to do nothing.
                colorGrading.postExposure.Override(brightness);
            }
            else
            {
                Debug.LogWarning("[ApplyPP] UpdateBrightness: profile has no ColorGrading settings");
            }
        }

        private void OnGammaChanged(float value)
        {
            _desiredBrightness = value;
            UpdateBrightness();
        }

        private void OnLimitChanged(float value)
        {
            _maxBrightness = value;
            UpdateBrightness();
        }
    }
}