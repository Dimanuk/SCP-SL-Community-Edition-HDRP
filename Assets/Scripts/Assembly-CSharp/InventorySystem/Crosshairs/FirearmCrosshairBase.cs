using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Modules;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace InventorySystem.Crosshairs
{
    public abstract class FirearmCrosshairBase : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _fadeGroup;

        private void Update()
        {
            if (!ReferenceHub.TryGetLocalHub(out ReferenceHub localHub))
                return;

            ItemBase currentItem = localHub.inventory.CurInstance;
            Firearm firearm = currentItem as Firearm;

            if (firearm == null)
            {
                if (_fadeGroup != null)
                    _fadeGroup.alpha = 0f;
                return;
            }

            Vector3 velocity = localHub.GetVelocity();
            float speed = velocity.MagnitudeIgnoreY();

            bool isGrounded = localHub.IsGrounded();

            bool cursorVisible = Cursor.visible;
            bool hasHideFlag = AttachmentsUtils.HasDownsideFlag(firearm, AttachmentDescriptiveDownsides.Laser);

            float adsAmount = firearm.AdsModule != null ? firearm.AdsModule.ClientAdsAmount : 0f;
            float targetAlpha = (cursorVisible || hasHideFlag) ? 0f : (1f - adsAmount);
            if (_fadeGroup != null)
                _fadeGroup.alpha = targetAlpha;

            if (targetAlpha <= 0f)
                return;

            float currentInaccuracy = firearm.BaseStats.GetInaccuracy(firearm, false, speed, isGrounded);

            if (firearm.ActionModule is AutomaticAction autoAction)
            {
                currentInaccuracy += autoAction.FullautoInaccuracy;
            }

            UpdateCrosshair(firearm, currentInaccuracy, speed);
        }

        protected abstract void UpdateCrosshair(Firearm firearm, float currentInaccuracy, float speed);
    }
}