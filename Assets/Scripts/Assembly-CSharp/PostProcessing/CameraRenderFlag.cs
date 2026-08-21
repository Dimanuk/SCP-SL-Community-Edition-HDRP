using UnityEngine;

namespace PostProcessing
{
    public class CameraRenderFlag : MonoBehaviour
    {
        public DepthTextureMode DefaultFlags;
        public bool rendersMotionBlur;

        private void TestMotionBlur()
        {
            if (!rendersMotionBlur)
                return;

            if (PostProcessingVolumes.MotionBlurEnabled)
            {
                DefaultFlags |= DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
            }
        }

        private void Start()
        {
            TestMotionBlur();

            var camera = GetComponent <Camera>();
            if (camera != null)
            {
                camera.depthTextureMode = DefaultFlags;
            }
        }
    }
}