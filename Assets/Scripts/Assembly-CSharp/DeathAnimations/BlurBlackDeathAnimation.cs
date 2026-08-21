using PostProcessing;
using SCPE;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace DeathAnimations
{
    public class BlurBlackDeathAnimation : FirstpersonDeathAnimation
    {
        public const float InstantDarknessSpeed = 0.25f;

        [SerializeField]
        public float delayTillDark = 3.3f;

        [NonSerialized]
        private bool _instantDarkness;

        [NonSerialized]
        private PostProcessVolume _tempVolume;

        [NonSerialized]
        private Blur _blur;

        [NonSerialized]
        private Darken _darken;

        [NonSerialized]
        private float _fadeTime;

        private void Update()
        {
            _fadeTime += Time.deltaTime;

            if (_instantDarkness)
            {
                if (_darken != null)
                {
                    _darken.intensity.value = Mathf.Lerp(0f, 1f, _fadeTime * InstantDarknessSpeed);
                }
            }
            else
            {
                if (_darken != null && _fadeTime >= delayTillDark)
                {
                    _darken.intensity.value = Mathf.Lerp(0f, 1f, _fadeTime - delayTillDark);
                }
            }
        }

        protected override void OnAnimationStarted()
        {
            base.OnAnimationStarted();

            if (IsFirstperson)
            {
                base.EventAssigned = true;

                BasicRagdoll ragdoll = TargetRagdoll;
                if (ragdoll?.Info.Handler is PlayerStatsSystem.ScpDamageHandler)
                {
                    _instantDarkness = true;
                }

                enabled = true;

                _darken = ScriptableObject.CreateInstance<Darken>();
                _blur = ScriptableObject.CreateInstance<Blur>();

                // The camera's PostProcessLayer only renders volumes on the dedicated
                // "PostProcessingLayer" (layer 23). Using gameObject.layer (the ragdoll's
                // layer) meant the volume was never picked up, so nothing darkened.
                int ppLayer = LayerMask.NameToLayer("PostProcessingLayer");
                _tempVolume = PostProcessManager.instance.QuickVolume(ppLayer, 2f, _darken, _blur);

                if (_tempVolume != null)
                {
                    if (_tempVolume.profile.TryGetSettings(out Darken darken))
                        _darken = darken;

                    if (_tempVolume.profile.TryGetSettings(out Blur blur))
                        _blur = blur;

                    if (_darken != null)
                    {
                        // Override() sets overrideState=true so the parameter is actually
                        // blended into the stack; plain .value leaves overrideState=false.
                        // enabled must be true or Darken.IsEnabledAndSupported returns false.
                        _darken.enabled.Override(true);
                        _darken.intensity.Override(0f);
                    }

                    if (_blur != null)
                    {
                        _blur.active = true;
                        _blur.enabled.Override(true);
                    }

                    _tempVolume.weight = 1f;
                }

                _fadeTime = 0f;
            }
            else
            {
                Destroy(this);
            }
        }

        protected override void OnAnimationEnded()
        {
            base.EventAssigned = false;

            if (_tempVolume != null)
            {
                RuntimeUtilities.DestroyVolume(_tempVolume, true, true);
                _tempVolume = null;
            }

            Destroy(this);
        }
    }
}