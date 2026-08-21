using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.Hotkeys
{
    public class HotkeyIconPositioner : MonoBehaviour
    {
        private readonly List<RectTransform> _targetTransforms = new List<RectTransform>();

        private float _lastWidth;

        private int _lastActiveNumber;

        private void Start()
        {
            Transform t = transform;
            int childCount = t.childCount;

            for (int i = childCount - 1; i >= 0; i--)
            {
                Transform child = t.GetChild(i);
                if (child.TryGetComponent(out RectTransform rt))
                {
                    _targetTransforms.Add(rt);
                }
            }
        }

        private void Update()
        {
            int activeCount = 0;
            float totalWidth = 0f;

            foreach (RectTransform rt in _targetTransforms)
            {
                if (rt == null)
                    continue;

                if (rt.gameObject.activeInHierarchy)
                {
                    activeCount++;
                    totalWidth += rt.sizeDelta.x;
                }
            }

            if (activeCount != _lastActiveNumber || totalWidth != _lastWidth)
            {
                Refresh();
                _lastActiveNumber = activeCount;
                _lastWidth = totalWidth;
            }
        }

        private void Refresh()
        {
            float currentX = 0f;

            for (int i = 0; i < _targetTransforms.Count; i++)
            {
                RectTransform rt = _targetTransforms[i];
                if (rt == null)
                    continue;

                if (!rt.gameObject.activeInHierarchy)
                    continue;

                // Vector3.left * currentX → Vector2 → anchoredPosition
                rt.anchoredPosition = (Vector2)(Vector3.left * currentX);
                currentX += rt.sizeDelta.x;
            }
        }
    }
}
