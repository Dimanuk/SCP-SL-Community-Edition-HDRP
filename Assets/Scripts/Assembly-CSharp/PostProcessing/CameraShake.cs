using System;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    [Serializable]
    [PostProcess(typeof(CameraShakeRenderer), PostProcessEvent.AfterStack, "Custom/CameraShake")]
    public sealed class CameraShake : PostProcessEffectSettings
    {
        public FloatParameter scanLineJitter= new FloatParameter { value = 0f };
        public FloatParameter verticalJump = new FloatParameter { value = 0f };
        public FloatParameter horizontalShake = new FloatParameter { value = 0f };
        public FloatParameter colorDrift = new FloatParameter { value = 0f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (!enabled.value)
                return false;

            return scanLineJitter.value != 0f
                || verticalJump.value != 0f
                || horizontalShake.value != 0f
                || colorDrift.value != 0f;
        }
    }
}