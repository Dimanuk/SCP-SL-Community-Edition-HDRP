using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;
#if !PPS
using UnityEngine.Rendering.PostProcessing;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using ColorParameter = UnityEngine.Rendering.PostProcessing.ColorParameter;
using Vector2Parameter = UnityEngine.Rendering.PostProcessing.Vector2Parameter;
using MinAttribute = UnityEngine.Rendering.PostProcessing.MinAttribute;
#endif

namespace SCPE
{
#if PPS
    [PostProcess(typeof(CausticsRenderer), PostProcessEvent.BeforeStack, "SC Post Effects/Environment/Caustics")]
#endif
    [Serializable]
    public sealed class Caustics : PostProcessEffectSettings
    {
#if PPS
        public UnityEngine.Rendering.PostProcessing.TextureParameter causticsTexture = new UnityEngine.Rendering.PostProcessing.TextureParameter { value = null };
        [Range(0f, 5f)]
        public UnityEngine.Rendering.PostProcessing.FloatParameter intensity = new UnityEngine.Rendering.PostProcessing.FloatParameter { value = 0f };
        
        [Tooltip("Draws the caustics on pixels brighter than this threshold, useful to hide the caustics in shadows")]
        [Range(0f, 2f)]
        public UnityEngine.Rendering.PostProcessing.FloatParameter luminanceThreshold = new UnityEngine.Rendering.PostProcessing.FloatParameter { value = 0f };
        public UnityEngine.Rendering.PostProcessing.BoolParameter projectFromSun = new UnityEngine.Rendering.PostProcessing.BoolParameter { value = false};
        
        [Space]
        
        public UnityEngine.Rendering.PostProcessing.FloatParameter minHeight = new UnityEngine.Rendering.PostProcessing.FloatParameter { value = -5f };
        [Range(0f, 1f)]
        public UnityEngine.Rendering.PostProcessing.FloatParameter minHeightFalloff = new UnityEngine.Rendering.PostProcessing.FloatParameter { value = 1f };

        public UnityEngine.Rendering.PostProcessing.FloatParameter maxHeight = new UnityEngine.Rendering.PostProcessing.FloatParameter { value = 0f };
        [Range(0f, 1f)]
        public UnityEngine.Rendering.PostProcessing.FloatParameter maxHeightFalloff = new UnityEngine.Rendering.PostProcessing.FloatParameter { value = 1f };
        
        [Space]

        [Range(0.1f, 3f)]
        public UnityEngine.Rendering.PostProcessing.FloatParameter size = new UnityEngine.Rendering.PostProcessing.FloatParameter { value = 0.5f };
        [Range(0f, 1f)]
        public UnityEngine.Rendering.PostProcessing.FloatParameter speed = new UnityEngine.Rendering.PostProcessing.FloatParameter { value = 0.2f };
        
        [Space]

        public UnityEngine.Rendering.PostProcessing.BoolParameter distanceFade = new UnityEngine.Rendering.PostProcessing.BoolParameter { value = false };
        public UnityEngine.Rendering.PostProcessing.FloatParameter startFadeDistance = new UnityEngine.Rendering.PostProcessing.FloatParameter { value = 0f };
        public UnityEngine.Rendering.PostProcessing.FloatParameter endFadeDistance = new UnityEngine.Rendering.PostProcessing.FloatParameter { value = 200f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return (enabled.value && intensity > 0 && causticsTexture.value != null);
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (causticsTexture.overrideState && causticsTexture.value == null)
            {
                //Auto assign default texture
                causticsTexture.value = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(UnityEditor.AssetDatabase.GUIDToAssetPath("f76f6be48fafde34b818e658b93e7850"));
            }
        }
        #endif
#endif
    }
}