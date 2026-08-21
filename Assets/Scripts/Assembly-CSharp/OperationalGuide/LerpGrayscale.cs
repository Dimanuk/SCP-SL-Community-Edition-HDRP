using UnityEngine;
using UnityEngine.UI;

namespace OperationalGuide
{
    public class LerpGrayscale : MonoBehaviour
    {
        public Image GrayscaleImage;

        private float _colorModifier;
        public float ColorModifier;

        private const float GrayscaleThreshold = 0.95f;
        private const string ModifierProperty = "_Modifier";

        private void Update()
        {
            if (Mathf.Approximately(ColorModifier, _colorModifier))
                return;

            if (GrayscaleImage == null)
                return;

            Material mat = GrayscaleImage.material;
            if (mat == null)
                return;

            float modifier = ColorModifier > GrayscaleThreshold ? 1f : ColorModifier;

            mat.SetFloat(ModifierProperty, modifier);

            _colorModifier = ColorModifier;
        }
    }
}
