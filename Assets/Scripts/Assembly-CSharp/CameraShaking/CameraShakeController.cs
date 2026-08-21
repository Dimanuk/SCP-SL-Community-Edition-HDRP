using System.Collections.Generic;
using InventorySystem.Items;
using NorthwoodLib.Pools;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace CameraShaking
{
    public class CameraShakeController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _viewmodelRoot;

        private Vector3 _startPos;
        private readonly List<IShakeEffect> _effects = new List<IShakeEffect>();

        public static CameraShakeController Singleton;

        public float CamerasFOV
        {
            get => _camera.fieldOfView;
            set => _camera.fieldOfView = value;
        }

        public static void AddEffect(IShakeEffect effect) => Singleton?._effects.Add(effect);

        private void Start()
        {
            _startPos = transform.localPosition;
            Singleton = this;
            if (_viewmodelRoot == null)
                _viewmodelRoot = GetComponentInChildren<SharedHandsController>()?.transform;
        }

        private void OnDisable() => _effects.Clear();

        private void LateUpdate()
        {
            if (!ReferenceHub.TryGetLocalHub(out var hub)) return;

            Quaternion rootRot = Quaternion.identity;
            Quaternion viewmodelRot = Quaternion.identity;
            Vector3 offsetPos = Vector3.zero;
            HashSet<float> fovModifiers = HashSetPool<float>.Shared.Rent();

            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                if (!_effects[i].GetEffect(hub, out ShakeEffectValues values))
                {
                    _effects.RemoveAt(i);
                    continue;
                }

                if (values.FovMultiplier != 1f)
                    fovModifiers.Add(values.FovMultiplier);

                if (hub.roleManager.CurrentRole is IFpcRole fpc && fpc.FpcModule.MouseLook is FpcMouseLook ml)
                {
                    ml.CurrentHorizontal += values.HorizontalMouseLook;
                    ml.CurrentVertical += values.VerticalMouseLook;
                }

                rootRot *= values.RootCameraRotation;
                offsetPos += values.RootCameraPositionOffset;
                viewmodelRot *= values.ViewmodelCameraRotation;
            }

            float totalFovMod = 0f;
            foreach (float mod in fovModifiers) totalFovMod += (mod - 1f);

            CamerasFOV = 70f * (totalFovMod + 1f);
            if (hub.inventory.CurInstance is IZoomModifyingItem zoomItem)
                CamerasFOV /= zoomItem.ZoomAmount;

            HashSetPool<float>.Shared.Return(fovModifiers);

            transform.localRotation = rootRot;
            transform.localPosition = _startPos;
            transform.position += offsetPos;
            if (_viewmodelRoot != null)
                _viewmodelRoot.localRotation = viewmodelRot;
        }
    }
}