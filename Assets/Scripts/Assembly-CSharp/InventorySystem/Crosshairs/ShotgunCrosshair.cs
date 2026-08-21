using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;
using UnityEngine;

namespace InventorySystem.Crosshairs
{
    public class ShotgunCrosshair : FirearmCrosshairBase
    {
        [SerializeField]
        private RectTransform[] _elements;

        [SerializeField]
        private float _displacementRatio;

        [SerializeField]
        private float _radiusRatio;

        [SerializeField]
        private float _lerpSpeed;

        private void SetupElements(float innerAngle, float buckshotRadius)
        {
            if (_elements == null) return;

            float lerpT = Time.deltaTime * _lerpSpeed;

            for (int i = 0; i < _elements.Length; i++)
            {
                RectTransform rect = _elements[i];
                if (rect == null) continue;

                float targetOffset = innerAngle * _displacementRatio;
                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, Vector2.up * targetOffset, lerpT);

                float targetSize = buckshotRadius * _radiusRatio;
                rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, Vector2.one * targetSize, lerpT);
            }
        }

        protected override void UpdateCrosshair(Firearm firearm, float currentInaccuracy, float speed)
        {
            IHitregModule hitregModule = firearm.HitregModule;

            if (hitregModule is BuckshotHitreg buckshotHitreg)
            {
                float innerAngle = currentInaccuracy * 0.4f;

                float buckshotRadius = buckshotHitreg.BuckshotScale;

                SetupElements(innerAngle, buckshotRadius);
            }
        }
    }
}