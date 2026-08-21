using TMPro;
using UnityEngine;

namespace Tooltips
{
    public class Tooltip : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RectTransform _background;
        [SerializeField] private TMP_Text _textComponent;

        [SerializeField] private Vector3 _offset = Vector3.zero;
        [SerializeField] private float _padding = 4f;

        private string _lastText;

        public void SetText(string text)
        {
            if (string.IsNullOrEmpty(text) || _textComponent == null)
                return;

            if (_lastText == text)
                return;

            _lastText = text;
            _textComponent.text = text;
            _textComponent.RecalculateMasking();

            if (_background != null)
            {
                float width = _textComponent.preferredWidth + _padding * 2f;
                float height = _textComponent.preferredHeight + _padding * 2f;

                _background.sizeDelta = new Vector2(width, height);
            }
        }

        public void SetPosition(Vector3 screenPosition)
        {
            if (_rectTransform == null)
                return;

            // The serialized _canvas reference was lost on import (AssetRipper wrote fileID: 0). Without it
            // SetPosition used to early-return, so the tooltip was NEVER positioned and stayed pinned at its
            // prefab default (far off to the side). Resolve it from the hierarchy as a fallback.
            if (_canvas == null)
                _canvas = _rectTransform.GetComponentInParent<Canvas>();
            if (_canvas == null)
                return;

            // The caller (TooltipManager) passes Input.mousePosition — already a SCREEN position. The old
            // code ran it through Camera.main.WorldToScreenPoint, double-transforming it so the tooltip
            // landed at a fixed off-cursor spot. Use the screen position directly (matches the shipped
            // client: position + _offset, then clamp).
            Vector3 screenPos = screenPosition + _offset;

            if (_background != null)
            {
                Rect rect = _background.rect;
                float scaleFactor = _canvas.scaleFactor;

                float halfWidth = rect.width * scaleFactor * 0.5f;
                float halfHeight = rect.height * scaleFactor * 0.5f;

                screenPos.x = Mathf.Clamp(screenPos.x, halfWidth + _padding, Screen.width - halfWidth - _padding);
                screenPos.y = Mathf.Clamp(screenPos.y, halfHeight + _padding, Screen.height - halfHeight - _padding);
            }

            // Assigning a screen-pixel value straight to RectTransform.position only works on a
            // Screen-Space-Overlay canvas; convert through the tooltip's canvas so it lands under the
            // cursor in every render mode.
            Camera uiCam = _canvas.renderMode != RenderMode.ScreenSpaceOverlay ? _canvas.worldCamera : null;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_rectTransform, screenPos, uiCam, out Vector3 world))
                _rectTransform.position = world;
        }
    }
}