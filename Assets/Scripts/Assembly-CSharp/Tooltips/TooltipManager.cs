using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tooltips
{
    public class TooltipManager : MonoBehaviour
    {
        [SerializeField] private Tooltip _tooltip;

        private float _showTimer;
        private float _hideTimer;

        public Dictionary<GameObject, string> StoredTips { get; } = new Dictionary<GameObject, string>();

        public bool IsEnabled { get; set; } = true;

        public bool IsVisible { get; private set; }

        public Tooltip Tooltip => _tooltip;

        [field: SerializeField]
        public float ShowDelay { get; set; }

        [field: SerializeField]
        public float HideDelay { get; set; }

        private float ShowTimer
        {
            get => _showTimer;
            set => _showTimer = Mathf.Clamp(value, 0f, ShowDelay);
        }

        private float HideTimer
        {
            get => _hideTimer;
            set => _hideTimer = Mathf.Clamp(value, 0f, HideDelay);
        }

        private void Update()
        {
            if (!IsEnabled)
            {
                if (IsVisible)
                    SetState(false);
                return;
            }

            // Follow the cursor: the shipped client repositions the tooltip every frame while it's
            // visible (TooltipManager.Update). The project only set it once in SetState, so it never
            // tracked the mouse.
            if (IsVisible && _tooltip != null)
                _tooltip.SetPosition(Input.mousePosition);

            // The tooltip must stay up only while the cursor is over an element that actually HAS a stored
            // tip — not over any UI. The old code passed raw "over UI" to TickState, so the tooltip never
            // hid as long as the cursor was anywhere on the RA panel. (Matches reference: the hide path is
            // taken whenever no StoredTip is found for the hovered element.)
            bool hoveringTooltipped = false;
            if (IsCursorOverUI(out RaycastResult result) && result.gameObject != null)
            {
                if (StoredTips.TryGetValue(result.gameObject, out string tip) ||
                    (result.gameObject.transform.parent != null &&
                     StoredTips.TryGetValue(result.gameObject.transform.parent.gameObject, out tip)))
                {
                    _tooltip.SetText(tip);
                    hoveringTooltipped = true;
                }
            }

            TickState(hoveringTooltipped);
        }

        private void SetState(bool isVisible)
        {
            IsVisible = isVisible;
            ShowTimer = 0f;
            HideTimer = 0f;

            if (_tooltip != null)
            {
                _tooltip.SetPosition(Input.mousePosition);
                _tooltip.gameObject.SetActive(isVisible);
            }
        }

        private void TickState(bool isHoveringElement)
        {
            if (!isHoveringElement)
            {
                if (IsVisible)
                {
                    HideTimer += Time.deltaTime;
                    if (HideTimer >= HideDelay)
                        SetState(false);
                }
            }
            else
            {
                if (!IsVisible)
                {
                    ShowTimer += Time.deltaTime;
                    if (ShowTimer >= ShowDelay)
                        SetState(true);
                }
                else
                {
                    HideTimer = 0f;
                }
            }
        }

        private bool IsCursorOverUI(out RaycastResult result)
        {
            result = default;

            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                result = results[0];
                return result.gameObject != null && result.gameObject.layer == 5; 
            }

            return false;
        }
    }
}