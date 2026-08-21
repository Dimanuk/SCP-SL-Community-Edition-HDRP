using InventorySystem.Items;
using InventorySystem.GUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Hotkeys
{
    public abstract class HotkeyIconBase : MonoBehaviour
    {
        [SerializeField]
        protected Image Outline;

        [SerializeField]
        protected Image HighlightImage;

        [SerializeField]
        protected TextMeshProUGUI KeycodeText;

        private ushort _prevSerial;

        private bool _prevHidden;

        private float _highlightTimer;

        private bool _highlightPlaying;

        private Color _highlightTransparentColor;

        private static readonly AnimationCurve HighlightAnimation;

        static HotkeyIconBase()
        {
            HighlightAnimation = new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(0.5f, 1f),
                new Keyframe(1f, 0f)
            );
        }

        public bool Setup(ushort serial, KeyCode keycode, bool hidden)
        {
            if (_prevSerial == serial && _prevHidden == hidden)
            {
                if (!ForceRefresh(serial))
                    return false;
            }

            ReferenceHub hub = ReferenceHub.LocalHub;
            ItemBase item = null;
            bool hasItem = false;

            if (serial != 0 && hub != null)
            {
                var inventory = hub.inventory;
                if (inventory != null)
                {
                    var userInventory = inventory.UserInventory;
                    if (userInventory != null)
                    {
                        var items = userInventory.Items;
                        if (items != null && items.TryGetValue(serial, out item))
                        {
                            hasItem = true;
                        }
                    }
                }
            }

            if (hasItem)
            {
                gameObject.SetActive(!hidden);

                if (KeycodeText != null)
                    KeycodeText.text = new ReadableKeyCode(keycode).ShortVersion;

                UpdateIcons(serial, item);

                bool appeared = _prevSerial == 0;
                if (appeared)
                {
                    PlayHighlightAnimation();
                    OnRefreshed(serial);
                }

                _prevSerial = serial;
                _prevHidden = hidden;
                return appeared;
            }

            gameObject.SetActive(false);
            _prevSerial = serial;
            _prevHidden = hidden;
            return false;
        }

        public void UpdateAnimations()
        {
            if (!_highlightPlaying)
                return;

            if (!InventoryGuiController.InventoryVisible)
            {
                _highlightTimer += Time.deltaTime;
            }

            if (Outline != null && HighlightImage != null)
            {
                float t = HighlightAnimation.Evaluate(_highlightTimer);
                HighlightImage.color = Color.Lerp(_highlightTransparentColor, Outline.color, t);
            }

            if (_highlightTimer > 1f)
            {
                CancelHighlightAnimation();
            }
        }

        public void PlayHighlightAnimation()
        {
            _highlightTimer = 0f;
            _highlightPlaying = true;

            if (HighlightImage != null)
                HighlightImage.enabled = true;
        }

        public void CancelHighlightAnimation()
        {
            if (!_highlightPlaying)
                return;

            if (HighlightImage != null)
            {
                HighlightImage.color = _highlightTransparentColor;
                HighlightImage.enabled = false;
            }

            _highlightPlaying = false;
        }

        public void SetColors(Color color)
        {
            if (Outline != null)
                Outline.color = color;

            if (KeycodeText != null)
                KeycodeText.color = color;

            _highlightTransparentColor = new Color(color.r, color.g, color.b, 0f);
        }

        protected abstract void UpdateIcons(ushort serial, ItemBase item);

        protected virtual bool ForceRefresh(ushort serial)
        {
            return false;
        }

        protected virtual void OnRefreshed(ushort serial)
        {
        }
    }
}
