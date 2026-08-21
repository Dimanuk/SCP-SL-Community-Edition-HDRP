using UnityEngine;
using UnityEngine.UI;

namespace RadialMenus
{
    public abstract class RadialMenuBase : MonoBehaviour
    {
        private int _slotsNum;

        private float _slotsAngleStep;

        [SerializeField]
        private RadialMenuSettings _settings;

        [SerializeField]
        private Vector2 _ringWidth = new Vector2(0.41f, 1.1f);

        [SerializeField]
        private Image _slotTemplate;

        [SerializeField]
        protected Image RingImage;

        [SerializeField]
        [Tooltip("Extra rotation (degrees) added to every highlight wedge. Use to align the wedge art with the slot sectors. Positive = clockwise (toward the right).")]
        private float _highlightAngleOffset = 0f;

        [SerializeField]
        [Tooltip("Uniform scale applied to every highlight wedge so it fully covers its sector cell. 1 = native.")]
        private float _highlightScale = 1f;

        private float _appliedAngleOffset = float.NaN;

        private float _appliedScale = float.NaN;

        protected Image[] Highlights;

        public abstract int Slots { get; }

        public int HighlightedSlot { get; private set; }

        private void EnsureHighlightsArray(int requiredSize)
        {
            if (Highlights == null || Highlights.Length < requiredSize)
            {
                Image[] newArray = new Image[requiredSize];
                if (Highlights != null)
                {
                    for (int i = 0; i < Highlights.Length; i++)
                    {
                        newArray[i] = Highlights[i];
                    }
                }
                Highlights = newArray;
            }
        }

        protected Image GetHighlightSafe(int index)
        {
            EnsureHighlightsArray(index + 1);

            Image image = Highlights[index];
            if (image != null)
                return image;

            image = Object.Instantiate(_slotTemplate, RingImage.transform);
            Highlights[index] = image;
            return image;
        }

        protected virtual void OnSlotsNumberChanged(int prev, int cur)
        {
            EnsureHighlightsArray(cur);

            for (int i = 0; i < prev; i++)
            {
                if (Highlights[i] != null)
                    Highlights[i].enabled = false;
            }

            for (int j = 0; j < cur; j++)
            {
                Image highlightSafe = GetHighlightSafe(j);
                highlightSafe.rectTransform.SetAsFirstSibling();
                highlightSafe.rectTransform.localPosition = Vector3.zero;
                highlightSafe.rectTransform.localEulerAngles = (_slotsAngleStep * (float)j + _highlightAngleOffset) * Vector3.back;
                highlightSafe.rectTransform.localScale = Vector3.one * _highlightScale;
                highlightSafe.sprite = _settings.HighlightTemplates[cur];
                highlightSafe.enabled = true;
            }

            if (RingImage != null && _settings != null)
                RingImage.sprite = _settings.MainRings[cur];
        }

        protected bool InRingRange(out float angle)
        {
            float num = (float)Screen.width / (float)Screen.height;
            Vector2 vector = new Vector2(Mathf.Lerp(-1f, 1f, Mathf.Clamp01(Input.mousePosition.x / (float)Screen.width)) * num, Mathf.Lerp(-1f, 1f, Input.mousePosition.y / (float)Screen.height));
            angle = Vector2.Angle(Vector2.up, vector.normalized);
            if (vector.x < 0f)
            {
                angle = 360f - angle;
            }
            float magnitude = vector.magnitude;
            if (magnitude < _ringWidth.y)
            {
                return magnitude > _ringWidth.x;
            }
            return false;
        }

        protected virtual void Update()
        {
            int num = Mathf.Clamp(Slots, 1, _settings.HighlightTemplates.Length + 1);
            if (num != _slotsNum)
            {
                int slotsNum = _slotsNum;
                _slotsNum = num;
                _slotsAngleStep = 360f / (float)num;
                OnSlotsNumberChanged(slotsNum, num);
            }
            // Live re-apply the wedge rotation when the offset changes (so it can be tuned in the
            // Inspector during play). Settles after one apply.
            if ((_highlightAngleOffset != _appliedAngleOffset || _highlightScale != _appliedScale) && Highlights != null && _slotsNum > 0)
            {
                _appliedAngleOffset = _highlightAngleOffset;
                _appliedScale = _highlightScale;
                for (int k = 0; k < _slotsNum && k < Highlights.Length; k++)
                {
                    if (Highlights[k] != null)
                    {
                        Highlights[k].rectTransform.localEulerAngles = (_slotsAngleStep * (float)k + _highlightAngleOffset) * Vector3.back;
                        Highlights[k].rectTransform.localScale = Vector3.one * _highlightScale;
                    }
                }
            }
            if (InRingRange(out var angle))
            {
                HighlightedSlot = 0;
                while (angle > _slotsAngleStep)
                {
                    angle -= _slotsAngleStep;
                    HighlightedSlot++;
                }
            }
            else
            {
                HighlightedSlot = -1;
            }
        }
    }
}
