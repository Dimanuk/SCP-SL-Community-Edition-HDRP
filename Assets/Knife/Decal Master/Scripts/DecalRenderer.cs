using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Knife.DeferredDecals
{
    public class DecalRenderer : MonoBehaviour
    {
        private Camera _camera;
        private DecalExclusionCamera _decalExclusionCamera;

        private void OnEnable()
        {
            MapGeneration.SeedSynchronizer.OnMapGenerated += Initialize;

            _camera = GetComponent<Camera>();
            if (_camera == null)
                return;

            DeferredDecalsSystem system = DeferredDecalsSystem.Singleton;
            if (system == null || system.DefaultBuffer == null)
                return;

            CommandBuffer cmdBuffer = system.DefaultBuffer.DefaultCommandBuffer;
            if (cmdBuffer != null)
                _camera.AddCommandBuffer(CameraEvent.BeforeReflections, cmdBuffer);

            if (_decalExclusionCamera == null)
                _decalExclusionCamera = new DecalExclusionCamera(_camera);
        }

        private void OnDisable()
        {
            MapGeneration.SeedSynchronizer.OnMapGenerated -= Initialize;

            if (_camera == null)
                return;

            DeferredDecalsSystem system = DeferredDecalsSystem.Singleton;
            if (system == null || system.DefaultBuffer == null)
                return;

            CommandBuffer cmdBuffer = system.DefaultBuffer.DefaultCommandBuffer;
            if (cmdBuffer != null)
                _camera.RemoveCommandBuffer(CameraEvent.BeforeReflections, cmdBuffer);
        }

        private void Initialize()
        {
            _camera = GetComponent<Camera>();
            if (_camera == null)
                return;

            DeferredDecalsSystem system = DeferredDecalsSystem.Singleton;
            if (system == null || system.DefaultBuffer == null)
                return;

            CommandBuffer cmdBuffer = system.DefaultBuffer.DefaultCommandBuffer;
            if (cmdBuffer != null)
                _camera.AddCommandBuffer(CameraEvent.BeforeReflections, cmdBuffer);

            if (_decalExclusionCamera == null)
                _decalExclusionCamera = new DecalExclusionCamera(_camera);
        }

        private void OnDestroy()
        {
            if (_decalExclusionCamera == null)
                return;

            if (_decalExclusionCamera._exclusionRenderTexture != null)
                _decalExclusionCamera._exclusionRenderTexture.Release();

            if (_decalExclusionCamera._exclusionCamera != null
                && _decalExclusionCamera._exclusionCamera.cameraType == CameraType.Game)
            {
                Destroy(_decalExclusionCamera._exclusionCamera.gameObject);
            }
        }

        private void OnPreRender()
        {
            DeferredDecalsSystem system = DeferredDecalsSystem.Singleton;
            if (system == null || !system.EnableDecals)
                return;

            if (_decalExclusionCamera != null)
                _decalExclusionCamera.UpdateExclusionCamera();

            if (system.DefaultBuffer != null)
                system.DefaultBuffer.Render(_camera);
        }
    }
}
