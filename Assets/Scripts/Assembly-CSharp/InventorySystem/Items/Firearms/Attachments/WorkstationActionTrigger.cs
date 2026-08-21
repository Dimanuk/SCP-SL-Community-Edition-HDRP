using System;
using Interactables;
using Interactables.Verification;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Attachments
{
    public class WorkstationActionTrigger : InteractableCollider, IClientInteractable, IInteractable
    {
        public const float BaselineWidth = 0.61f;

        public const float ParentWidth = 0.25f;

        public RectTransform _rt;

        public BoxCollider _col;

        public float _depth;

        public Vector2 _halfSize;

        public Action<Vector2> TargetAction { get; set; }

        public IVerificationRule VerificationRule => StandardDistanceVerification.Default;

        protected override void Awake()
        {
            Target = this;
            _rt = GetComponent<RectTransform>();
            _col = base.gameObject.AddComponent<BoxCollider>();
            _depth = 0.61f;
            Transform parent = _rt.parent;
            while (parent != null)
            {
                if (parent.TryGetComponent<WorkstationActionTrigger>(out var _))
                {
                    _depth += 0.25f;
                }

                parent = parent.parent;
            }

            UpdateSize();
        }

        public void Update()
        {
            UpdateSize();
        }

        public void UpdateSize()
        {
            _col.size = new Vector3(_rt.rect.size.x, _rt.rect.size.y, _depth);
            _halfSize = new Vector2(_col.size.x, _col.size.y) * 0.5f;
        }

        public void ClientInteract(InteractableCollider _)
        {
            Vector3 point = CenterScreenRaycast.LastRaycastHit.point;
            Vector3 vector = _rt.InverseTransformPoint(point);
            TargetAction?.Invoke(vector / _halfSize);
        }
    }
}