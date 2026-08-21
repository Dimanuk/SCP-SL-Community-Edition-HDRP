using UnityEngine;
using UnityEngine.Rendering;

namespace Knife.DeferredDecals
{
    public class DecalExclusionCamera
    {
        public static int ExclusionMaskID;
        public static Material ExclusionDepthMaterial;
        public static Shader ExclusionDepthShader;
        public static CommandBuffer ExclusionRenderBuffer;

        internal Camera _camera;
        internal RenderTexture _exclusionRenderTexture;
        internal Camera _exclusionCamera;

        static DecalExclusionCamera()
        {
            ExclusionMaskID = Shader.PropertyToID("_ExclusionMask");
            ExclusionRenderBuffer = new CommandBuffer();
            ExclusionRenderBuffer.name = "[Decal Master] Exclusion Render Buffer";
        }

        public DecalExclusionCamera(Camera camera)
        {
            if (ExclusionDepthMaterial == null)
            {
                ExclusionDepthShader = Resources.Load<Shader>("Knife/Deferred Decals/DepthToTarget");
                if (ExclusionDepthShader != null)
                    ExclusionDepthMaterial = new Material(ExclusionDepthShader);
            }

            if (camera == null)
                return;

            int width = camera.pixelWidth;
            int height = camera.pixelHeight;

            _exclusionRenderTexture = new RenderTexture(width, height, 16, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
            _exclusionRenderTexture.filterMode = FilterMode.Point;
            _exclusionRenderTexture.Create();
            _exclusionRenderTexture.name = "Exclusion Mask for camera " + camera.name;

            _camera = camera;
        }

        internal void UpdateExclusionCamera()
        {
            DeferredDecalsSystem system;
            if (_camera != null && _camera.cameraType == CameraType.SceneView)
                system = Object.FindFirstObjectByType<DeferredDecalsSystem>();
            else
                system = DeferredDecalsSystem.Singleton;

            if (system == null || !system.UseExclusionMask)
                return;

            if (_exclusionCamera == null)
            {
                GameObject go = new GameObject("Decal Exclusion Camera", typeof(Camera));
                _exclusionCamera = go.GetComponent<Camera>();
                _exclusionCamera.targetTexture = _exclusionRenderTexture;
                _exclusionCamera.enabled = false;
                _exclusionCamera.gameObject.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
                _exclusionCamera.renderingPath = RenderingPath.Forward;
                _exclusionCamera.allowHDR = false;
                _exclusionCamera.allowMSAA = false;
                _exclusionCamera.clearFlags = CameraClearFlags.SolidColor;
                _exclusionCamera.backgroundColor = Color.clear;

                if (ExclusionDepthShader != null)
                    _exclusionCamera.SetReplacementShader(ExclusionDepthShader, "RenderType");

                _exclusionCamera.transform.SetParent(_camera.transform);
                _exclusionCamera.transform.localPosition = Vector3.zero;
                _exclusionCamera.transform.localRotation = Quaternion.identity;
                _exclusionCamera.AddCommandBuffer(CameraEvent.AfterEverything, ExclusionRenderBuffer);
            }

            _exclusionCamera.cullingMask = system.ExclusionMask;
            _exclusionCamera.depth = _camera.depth;
            _exclusionCamera.fieldOfView = _camera.fieldOfView;
            _exclusionCamera.nearClipPlane = _camera.nearClipPlane;
            _exclusionCamera.farClipPlane = _camera.farClipPlane;

            _exclusionCamera.Render();

            Shader.SetGlobalTexture(ExclusionMaskID, _exclusionRenderTexture);
        }

        internal void DestroyCamera()
        {
            if (_exclusionRenderTexture != null)
                _exclusionRenderTexture.Release();

            if (_exclusionCamera != null && _exclusionCamera.cameraType == CameraType.Game)
                Object.Destroy(_exclusionCamera.gameObject);
        }
    }
}
