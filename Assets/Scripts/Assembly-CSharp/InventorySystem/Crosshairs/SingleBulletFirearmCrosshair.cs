using InventorySystem.Items.Firearms;
using UnityEngine;

namespace InventorySystem.Crosshairs
{
    public class SingleBulletFirearmCrosshair : FirearmCrosshairBase
    {
        [SerializeField]
        private RectTransform[] _elements;

        [SerializeField]
        private float _sizeRatio;

        [SerializeField]
        private float _width;

        [SerializeField]
        private float _lerpSpeed;

        [SerializeField]
        private AnimationCurve _sizeOverSpeed;

        private void SetupElements(float innerAngle, float speed, bool forceLerp)
        {
            if (_elements == null) return;

            float t = forceLerp ? 1f : (Time.deltaTime * _lerpSpeed);
            if (t > 1f) t = 1f;

            float speedModifier = _sizeOverSpeed.Evaluate(speed);

            for (int i = 0; i < _elements.Length; i++)
            {
                RectTransform rectTransform = _elements[i];
                if (rectTransform == null) continue;

                float targetPosDist = innerAngle * _sizeRatio;
                Vector2 targetPosition = Vector2.left * targetPosDist;
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, t);

                Vector2 targetSize = new Vector2(speedModifier, _width);
                rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, targetSize, t);
            }
        }

        private void OnEnable()
        {
            SetupElements(0f, 20f, true);
        }

        protected override void UpdateCrosshair(Firearm firearm, float currentInaccuracy, float speed)
        {
            SetupElements(currentInaccuracy, speed, false);
        }
    }
}
