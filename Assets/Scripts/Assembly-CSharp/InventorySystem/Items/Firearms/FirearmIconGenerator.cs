using System;
using UnityEngine;
using UnityEngine.UI;
using InventorySystem.Items.Firearms.Attachments.Components;

namespace InventorySystem.Items.Firearms
{
    public static class FirearmIconGenerator
    {
        public static Vector2 GenerateIcon(this Firearm firearm, RawImage rootImage, RawImage[] imagePool, Vector2 maxSize, Func<int, Color> colorFunction)
        {
            Bounds bounds = firearm.GenerateIcon(rootImage, imagePool, colorFunction);

            RectTransform rectTransform = rootImage.rectTransform;

            float scale = Mathf.Min(maxSize.y / bounds.size.y, maxSize.x / bounds.size.x);

            rectTransform.localScale = Vector3.one * scale;
            rectTransform.localPosition = -bounds.center * scale;

            return (Vector2)bounds.size * scale;
        }

        public static Bounds GenerateIcon(this Firearm firearm, RawImage rootImage, RawImage[] imagePool, Func<int, Color> colorFunction)
        {
            var attachments = firearm.Attachments;

            rootImage.texture = firearm.BodyIconTexture;
            rootImage.SetNativeSize();

            int poolIndex = 0;
            for (int i = 0; i < attachments.Length; i++)
            {
                var attachment = attachments[i];
                if (attachment == null || !attachment.IsEnabled || !(attachment is IDisplayableAttachment displayable))
                    continue;

                if (poolIndex >= imagePool.Length)
                    break;

                RawImage attachmentImage = imagePool[poolIndex];
                attachmentImage.gameObject.SetActive(true);

                attachmentImage.texture = displayable.Icon;
                attachmentImage.SetNativeSize();
                attachmentImage.color = colorFunction?.Invoke(i) ?? Color.white;

                Vector2 offset = displayable.IconOffset;
                int parentIndex = Mathf.Clamp(displayable.ParentId, 0, attachments.Length - 1);
                if (attachments[parentIndex].IsEnabled)
                    offset += displayable.ParentOffset;

                attachmentImage.rectTransform.localPosition = offset;
                poolIndex++;
            }

            for (int i = poolIndex; i < imagePool.Length; i++)
            {
                imagePool[i].gameObject.SetActive(false);
            }

            RectTransform rootRect = rootImage.rectTransform;
            Bounds bounds = new Bounds(rootRect.localPosition, Vector3.zero);
            EncapsulateRect(ref bounds, rootRect);

            for (int i = 0; i < poolIndex; i++)
            {
                EncapsulateRect(ref bounds, imagePool[i].rectTransform);
            }

            return bounds;
        }

        private static void EncapsulateRect(ref Bounds b, RectTransform rct)
        {
            Vector2 half = rct.sizeDelta / 2f;
            Vector3 pos = rct.localPosition;

            b.Encapsulate(pos + Vector3.up * half.y + Vector3.left * half.x);
            b.Encapsulate(pos + Vector3.down * half.y + Vector3.right * half.x);
        }
    }
}
