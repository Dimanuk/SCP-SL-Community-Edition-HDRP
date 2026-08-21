using UnityEngine;

namespace OperationalGuide
{
    public class OperationalPannablePage : OperationalPage
    {
        public PannableObject PannableObject;
        public GameObject PannableImage;

        private Vector3 _defaultScale;
        private Quaternion _defaultRotation;

        /* ==========================================================
           Page lifecycle overrides
           ========================================================== */

        public override void OnPageEnable()
        {
            if (PannableImage != null)
            {
                Transform t = PannableImage.transform;
                if (t != null)
                {
                    _defaultScale = t.localScale;
                }
            }

            if (PannableObject != null)
            {
                Transform t = PannableObject.transform;
                if (t != null)
                {
                    _defaultRotation = t.rotation;
                }

                if (PannableObject.gameObject != null)
                {
                    PannableObject.gameObject.SetActive(true);
                }
            }

            OperationalGuide guide = OperationalGuide.Instance;
            if (guide != null && guide.FullscreenPannableImage != null)
            {
                Transform fullscreenTransform = guide.FullscreenPannableImage.transform;
                if (fullscreenTransform != null)
                {
                    fullscreenTransform.localScale = Vector3.one;
                    fullscreenTransform.localPosition = Vector3.zero;
                }
            }
        }

        public override void OnPageDisable()
        {
            if (PannableImage != null)
            {
                Transform t = PannableImage.transform;
                if (t != null)
                {
                    t.localScale = _defaultScale;
                }
            }

            if (PannableObject != null)
            {
                Transform t = PannableObject.transform;
                if (t != null)
                {
                    t.rotation = _defaultRotation;
                }

                if (PannableObject.gameObject != null)
                {
                    PannableObject.gameObject.SetActive(false);
                }
            }
        }

        /* ==========================================================
           Public API
           ========================================================== */

        public void ResetUserInput()
        {
            if (PannableObject != null)
            {
                PannableObject.UserInput = false;
            }
        }
    }
}
