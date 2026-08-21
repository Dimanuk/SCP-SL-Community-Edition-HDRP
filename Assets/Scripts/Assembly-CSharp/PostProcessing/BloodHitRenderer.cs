using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    public sealed class BloodHitRenderer : PostProcessEffectRenderer<BloodHit>
    {
        private Shader shader;
        private Texture2D Texture2;

        private readonly int _timeXId;
        private readonly int _lightReflectId;
        private readonly int _hitLeftId;
        private readonly int _hitUpId;
        private readonly int _hitRightId;
        private readonly int _hitDownId;
        private readonly int _bloodHitLeftId;
        private readonly int _bloodHitUpId;
        private readonly int _bloodHitRightId;
        private readonly int _bloodHitDownId;
        private readonly int _hitFullId;
        private readonly int _bloodHitFull1Id;
        private readonly int _bloodHitFull2Id;
        private readonly int _bloodHitFull3Id;
        private readonly int _mainTex2Id;

        public BloodHitRenderer()
        {
            _timeXId = Shader.PropertyToID("_TimeX");
            _lightReflectId = Shader.PropertyToID("_LightReflect");
            _hitLeftId = Shader.PropertyToID("_HitLeft");
            _hitUpId = Shader.PropertyToID("_HitUp");
            _hitRightId = Shader.PropertyToID("_HitRight");
            _hitDownId = Shader.PropertyToID("_HitDown");
            _bloodHitLeftId = Shader.PropertyToID("_BloodHitLeft");
            _bloodHitUpId = Shader.PropertyToID("_BloodHitUp");
            _bloodHitRightId = Shader.PropertyToID("_BloodHitRight");
            _bloodHitDownId = Shader.PropertyToID("_BloodHitDown");
            _hitFullId = Shader.PropertyToID("_HitFull");
            _bloodHitFull1Id = Shader.PropertyToID("_BloodHitFull1");
            _bloodHitFull2Id = Shader.PropertyToID("_BloodHitFull2");
            _bloodHitFull3Id = Shader.PropertyToID("_BloodHitFull3");
            _mainTex2Id = Shader.PropertyToID("_MainTex2");
        }

        public override void Init()
        {
            shader = Shader.Find("Hidden/Custom Effects/BloodHit");
            Texture2 = Resources.Load("BloodHitTexture") as Texture2D;
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetFloat(_lightReflectId, settings.LightReflect.value);
            sheet.properties.SetFloat(_hitLeftId, settings.Hit_Left.value);
            sheet.properties.SetFloat(_hitUpId, settings.Hit_Up.value);
            sheet.properties.SetFloat(_hitRightId, settings.Hit_Right.value);
            sheet.properties.SetFloat(_hitDownId, settings.Hit_Down.value);
            sheet.properties.SetFloat(_bloodHitLeftId, settings.Blood_Hit_Left.value);
            sheet.properties.SetFloat(_bloodHitUpId, settings.Blood_Hit_Up.value);
            sheet.properties.SetFloat(_bloodHitRightId, settings.Blood_Hit_Right.value);
            sheet.properties.SetFloat(_bloodHitDownId, settings.Blood_Hit_Down.value);
            sheet.properties.SetFloat(_hitFullId, settings.Hit_Full.value);
            sheet.properties.SetFloat(_bloodHitFull1Id, settings.Blood_Hit_Full_1.value);
            sheet.properties.SetFloat(_bloodHitFull2Id, settings.Blood_Hit_Full_2.value);
            sheet.properties.SetFloat(_bloodHitFull3Id, settings.Blood_Hit_Full_3.value);

            sheet.properties.SetTexture(_mainTex2Id, Texture2);

            RuntimeUtilities.BlitFullscreenTriangle(
                context.command,
                context.source,
                context.destination,
                sheet,
                0,
                false
            );
        }
    }
}