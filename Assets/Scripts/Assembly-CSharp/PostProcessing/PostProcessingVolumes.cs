using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    public class PostProcessingVolumes : MonoBehaviour
    {
        public static PostProcessingVolumes Singleton;
        public static bool MotionBlurEnabled;

        private static List<PostProcessVolume> _quickVolumes;

        public PostProcessVolume DepthPostProcessVolume;
        public PostProcessVolume DefaultPostProcessVolume;
        public PostProcessVolume ClassPostProcessVolume;

        static PostProcessingVolumes()
        {
            MotionBlurEnabled = true;
            _quickVolumes = new List<PostProcessVolume>();
        }

        private void Awake()
        {
            if (Singleton != null)
            {
                Destroy(this);
                return;
            }

            Singleton = this;

            int motionBlurValue = PlayerPrefsSl.Get(((int)SettingsOption.gfxsets_mb).ToString(), 0);
            MotionBlurEnabled = motionBlurValue > 0;
        }

        public static PostProcessVolume SafeGetVolume(int layer, float priority, params PostProcessEffectSettings[] settings)
        {
            PostProcessManager instance = PostProcessManager.instance;
            if (instance == null)
                return null;

            PostProcessVolume volume = instance.QuickVolume(layer, priority, settings);
            _quickVolumes.Add(volume);
            return volume;
        }

        private void OnDestroy()
        {
            if (_quickVolumes != null)
            {
                foreach (PostProcessVolume volume in _quickVolumes)
                {
                    if (volume != null)
                    {
                        RuntimeUtilities.DestroyVolume(volume, true);
                    }
                }
                _quickVolumes.Clear();
            }
        }
    }
}