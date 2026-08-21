using UnityEngine;
using UnityEngine.UI;

namespace OperationalGuide
{
    public class VarientAlphaLerp : MonoBehaviour
    {

        public static float MinAlpha = 0.1f;
        
        public static float DistanceDivider = 350f;

        public RawImage VarientA;
        
        public RawImage VarientB;

        public bool IsASelected = true;

        private float _lerpA;
        private float _lerpB;

        public VarientAlphaLerp()
        {
            IsASelected = true;
        }

        private void Update()
        {
            Vector3 mousePos = Input.mousePosition;

            if (VarientA != null)
            {
                float distA = Vector2.Distance(mousePos, VarientA.transform.position);
                _lerpA = Mathf.Clamp01(1f - (distA / DistanceDivider));
            }

            if (VarientB != null)
            {
                float distB = Vector2.Distance(mousePos, VarientB.transform.position);
                _lerpB = Mathf.Clamp01(1f - (distB / DistanceDivider));
            }

            if (IsASelected && VarientA != null)
            {
                Color col = VarientA.color;
                VarientA.color = LerpAlpha(col, _lerpA);
            }
            else if (!IsASelected && VarientB != null)
            {
                Color col = VarientB.color;
                VarientB.color = LerpAlpha(col, _lerpB);
            }
        }

        public Color LerpAlpha(Color startingColor, float lerpAmount)
        {

            float alpha = Mathf.Lerp(0f, 1f, lerpAmount);
            
            alpha = Mathf.Max(MinAlpha, alpha);
            
            startingColor.a = alpha;
            return startingColor;
        }
        public void SetVarientStatus(bool isA)
        {
            IsASelected = isA;
        }
    }
}