using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin
{
    public class ColorSlider : CustomSlider
    {
        public enum ModifiedColorValue
        {
            R = 0,
            G = 1,
            B = 2,
            A = 3
        }

        public RawImage[] Images;
        public ModifiedColorValue ValueToModify;

        protected override void OnValueChanged(float newValue)
        {
            float normalized = newValue * 0.01f;

            if (Images == null)
                return;

            foreach (var img in Images)
            {
                if (img == null)
                    continue;

                Color c = img.color;

                switch (ValueToModify)
                {
                    case ModifiedColorValue.R:
                        img.color = new Color(normalized, c.g, c.b, c.a);
                        break;
                    case ModifiedColorValue.G:
                        img.color = new Color(c.r, normalized, c.b, c.a);
                        break;
                    case ModifiedColorValue.B:
                        img.color = new Color(c.r, c.g, normalized, c.a);
                        break;
                    case ModifiedColorValue.A:
                        img.color = new Color(c.r, c.g, c.b, normalized);
                        break;
                }
            }
        }
    }
}