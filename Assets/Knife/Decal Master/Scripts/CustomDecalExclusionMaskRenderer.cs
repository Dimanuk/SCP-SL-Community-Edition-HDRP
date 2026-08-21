using UnityEngine;
using UnityEngine.Rendering;

namespace Knife.DeferredDecals
{
    [ExecuteAlways]
    [RequireComponent(typeof(Renderer))]
    public class CustomDecalExclusionMaskRenderer : MonoBehaviour
    {
        public int[] Submeshes;

        private Renderer attachedRenderer;

        public Renderer AttachedRenderer
        {
            get
            {
                if (attachedRenderer == null)
                    attachedRenderer = GetComponent<Renderer>();

                return attachedRenderer;
            }
        }

        private void OnEnable()
        {
            if (Submeshes == null || Submeshes.Length == 0)
                return;

            AddToExclusionBuffer();
        }

        private void OnDisable()
        {
            RemoveFromExclusionBuffer();
        }

        private void OnValidate()
        {
            RemoveFromExclusionBuffer();

            if (Submeshes == null || Submeshes.Length == 0)
                return;

            AddToExclusionBuffer();
        }

        private void AddToExclusionBuffer()
        {
            CommandBuffer buffer = DecalExclusionCamera.ExclusionRenderBuffer;
            if (buffer == null || AttachedRenderer == null)
                return;

            Material mat = DecalExclusionCamera.ExclusionDepthMaterial;
            if (mat == null)
                return;

            for (int i = 0; i < Submeshes.Length; i++)
            {
                buffer.DrawRenderer(AttachedRenderer, mat, Submeshes[i]);
            }
        }

        private void RemoveFromExclusionBuffer()
        {
            CommandBuffer buffer = DecalExclusionCamera.ExclusionRenderBuffer;
            if (buffer != null)
                buffer.Clear();
        }
    }
}