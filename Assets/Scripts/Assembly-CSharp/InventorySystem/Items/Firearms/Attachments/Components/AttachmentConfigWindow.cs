using System;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Attachments.Components
{
    public class AttachmentConfigWindow : MonoBehaviour
    {
        private bool _wasActive;

        [SerializeField]
        private RectTransform _exitTransform;

        private AttachmentSelectorBase _selector;
        private Attachment _attachment;

        public Action OnDestroyed;

        public AttachmentSelectorBase Selector { get => _selector; set => _selector = value; }
        public Attachment Attachment { get => _attachment; set => _attachment = value; }

        protected virtual void SafeUpdate() { }

        protected virtual void SetLayout(RectTransform transformToFit)
        {
            if (transformToFit == null)
                return;

            var rt = GetComponent<RectTransform>();
            Vector2 targetSize = transformToFit.sizeDelta;
            Vector2 ownSize = rt.sizeDelta;

            if (Mathf.Approximately(ownSize.y, 0f))
                return;

            float ratio = targetSize.y / ownSize.y;

            rt.localPosition = transformToFit.localPosition;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one * ratio;
            rt.sizeDelta = targetSize / ratio;
        }

        public virtual void Setup(AttachmentSelectorBase selector, Attachment attachment, RectTransform transformToFit)
        {
            _selector = selector;
            _attachment = attachment;

            _selector.OnSummaryToggled += DestroySelf;

            if (_exitTransform != null)
                _selector.RegisterAction(_exitTransform, _ => DestroySelf());

            SetLayout(transformToFit);
        }

        protected virtual void OnDisable()
        {
            _wasActive = false;
        }

        protected virtual void OnDestroy()
        {
            if (_selector != null)
            {
                _selector.OnSummaryToggled -= DestroySelf;
            }

            if (gameObject.scene.isLoaded)
            {
                OnDestroyed?.Invoke();
            }
        }

        public void DestroySelf()
        {
            if (this != null && gameObject != null)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void Update()
        {
            if (!Validate())
            {
                Destroy(gameObject);
                return;
            }

            _wasActive = _attachment.IsEnabled;
            SafeUpdate();
        }

        protected bool Validate()
        {
            if (_attachment == null)
                return false;

            if (!_wasActive)
                return true;

            if (!_attachment.IsEnabled)
                return false;

            if (!_attachment.TryGetParentFirearm(out var firearm))
                return false;

            if (firearm.SimulatedInstanceMode)
                return true;

            Inventory inventory = (firearm.Owner != null) ? firearm.Owner.inventory : null;
            return inventory != null && inventory.CurInstance == firearm;
        }
    }
}
