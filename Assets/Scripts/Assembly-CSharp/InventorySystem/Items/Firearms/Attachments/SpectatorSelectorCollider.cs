
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Items.Firearms.Attachments
{
    public class SpectatorSelectorCollider : MonoBehaviour, IAttachmentSelectorButton
    {
        public const float DefaultColor = 0.7f;

        public const float CurrentColor = 1f;

        public const float LerpSpeed = 12f;

        [SerializeField]
        public RawImage _image;

        [SerializeField]
        public SpectatorAttachmentSelector _selector;

        public Firearm _firearm;

        public AttachmentSlot _mySlot;

        public RectTransform RectTransform => _image.rectTransform;

        public byte ButtonId { get; set; }

        public bool IsHovered { get; private set; }

        public void Click()
        {
            _selector.ProcessCollider(ButtonId);
        }

        public void Hover(bool isHovering)
        {
            IsHovered = isHovering;

            if (isHovering)
            {
                _selector.ShowStats(ButtonId);
            }
            else
            {
                _selector.ShowStats(-1);
            }
        }

        public void UpdateColors(AttachmentSlot slot)
        {
            // A button that is disabled while the cursor is on it never receives the pointer-exit
            // event, so its hover state is dropped here instead.
            if (!base.gameObject.activeInHierarchy)
            {
                IsHovered = false;
            }

            if (base.gameObject.activeSelf)
            {
                bool flag = slot == _mySlot || (_firearm != null && ButtonId < _firearm.Attachments.Length && _firearm.Attachments[ButtonId].IsEnabled);
                _image.color = Color.Lerp(_image.color, Color.Lerp(Color.black, Color.white, flag ? 1f : 0.7f), 12f * Time.deltaTime);
            }
        }

        public void Setup(Texture icon, AttachmentSlot slot, Vector2 pos, Firearm fa)
        {
            if (icon != null)
            {
                Vector2 sizeDelta = new(icon.width, icon.height);
                _firearm = fa;
                _mySlot = slot;
                _image.texture = icon;
                _image.rectTransform.sizeDelta = sizeDelta;
                _image.rectTransform.localPosition = pos;
            }
            else
            {
                _image.rectTransform.sizeDelta = Vector2.zero;
            }
        }
    }
}
