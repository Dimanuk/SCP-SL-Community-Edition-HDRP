using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoles.PlayableScps.Scp079.GUI
{
    [RequireComponent(typeof(RectTransform))]
    public class Scp079CanvasScaler : CanvasScaler
    {
        private RectTransform _rt;
        private float _prevRatio;
        private float _defaultRatio;
        private Vector2 _defaultSize;

        protected override void Awake()
        {
            base.Awake();
            _rt = GetComponent<RectTransform>();
            _defaultSize = _rt.sizeDelta;
            _defaultRatio = _defaultSize.x / _defaultSize.y;
        }

        private void Update()
        {
            float num = (float)Screen.width / (float)Screen.height;
            if (_prevRatio != num)
            {
                _prevRatio = num;
                float num2 = num / _defaultRatio;
                _rt.sizeDelta = new Vector2(_defaultSize.x * num2, _defaultSize.y);
            }
        }
    }
}