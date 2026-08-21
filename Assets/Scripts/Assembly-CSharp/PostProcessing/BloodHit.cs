using System;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    [Serializable]
    [PostProcess(typeof(BloodHitRenderer), PostProcessEvent.AfterStack, "Custom/BloodHit")]
    public sealed class BloodHit : PostProcessEffectSettings
    {
        public FloatParameter Hit_Left = new FloatParameter { value = 1f };
        public FloatParameter Hit_Up = new FloatParameter { value = 0f };
        public FloatParameter Hit_Right = new FloatParameter { value = 0f };
        public FloatParameter Hit_Down= new FloatParameter { value = 0f };
        public FloatParameter Blood_Hit_Left= new FloatParameter { value = 0f };
        public FloatParameter Blood_Hit_Up = new FloatParameter { value = 0f };
        public FloatParameter Blood_Hit_Right= new FloatParameter { value = 0f };
        public FloatParameter Blood_Hit_Down= new FloatParameter { value = 0f };
        public FloatParameter Hit_Full = new FloatParameter { value = 0f };
        public FloatParameter Blood_Hit_Full_1 = new FloatParameter { value = 0f };
        public FloatParameter Blood_Hit_Full_2= new FloatParameter { value = 0f };
        public FloatParameter Blood_Hit_Full_3= new FloatParameter { value = 0f };
        public FloatParameter LightReflect = new FloatParameter { value = 0.5f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value;
        }
    }
}