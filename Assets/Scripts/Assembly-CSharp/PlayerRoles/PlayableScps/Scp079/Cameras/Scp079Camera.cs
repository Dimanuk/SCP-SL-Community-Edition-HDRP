using System;
using Mirror;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace PlayerRoles.PlayableScps.Scp079.Cameras
{
    public class Scp079Camera : Scp079InteractableBase,
                                IAdvancedCameraController,
                                ICameraController
    {
        public bool IsMain;
        public string Label;

        public CameraRotationAxis VerticalAxis;
        public CameraRotationAxis HorizontalAxis;
        public CameraZoomAxis ZoomAxis;

        [SerializeField] private Transform _cameraAnchor;

        [SerializeField] private Renderer[] _targetRenderers;
        [SerializeField] private Material _offlineMat;
        [SerializeField] private Material _onlineMat;

        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (value == _isActive)
                    return;

                _isActive = value;
                foreach (Renderer r in _targetRenderers)
                    r.sharedMaterial = _isActive ? _onlineMat : _offlineMat;

                OnAnyCameraStateChanged?.Invoke(this);
            }
        }

        public bool IsUsedByLocalPlayer
        {
            get
            {
                if (!Scp079Role.LocalInstanceActive)
                    return false;

                return Scp079Role.LocalInstance.CurrentCamera == this;
            }
        }

        public Vector3 CameraPosition { get; private set; }
        public float   VerticalRotation   { get; private set; }
        public float   HorizontalRotation { get; private set; }
        public float   RollRotation       { get; private set; }

        public static event Action<Scp079Camera> OnAnyCameraStateChanged;

        protected override void Awake()
        {
            base.Awake();
            VerticalAxis.Awake(this);
            HorizontalAxis.Awake(this);
            ZoomAxis.Awake(this);
        }

        protected virtual void Update()
        {
            VerticalAxis.Update(this);
            HorizontalAxis.Update(this);
            ZoomAxis.Update(this);

            if (!IsActive)
                return;

            if (Scp079Role.ActiveInstances.All(x => x.CurrentCamera != this, emptyResult: true))
            {
                IsActive = false;
                return;
            }

            Vector3 euler = _cameraAnchor.rotation.eulerAngles;
            VerticalRotation   = euler.x;
            HorizontalRotation = euler.y;
            RollRotation       = euler.z;

            CameraPosition = _cameraAnchor.position;
        }

        internal void WriteAxes(NetworkWriter writer)
        {
            writer.WriteUShort(VerticalAxis.Value16BitCompression);
            writer.WriteUShort(HorizontalAxis.Value16BitCompression);
            writer.WriteByte(ZoomAxis.Value8BitCompression);
        }

        internal void ApplyAxes(NetworkReader reader)
        {
            if (IsUsedByLocalPlayer)
                return;

            VerticalAxis.Value16BitCompression   = reader.ReadUShort();
            HorizontalAxis.Value16BitCompression = reader.ReadUShort();
            ZoomAxis.Value8BitCompression        = reader.ReadByte();
        }
    }
}
