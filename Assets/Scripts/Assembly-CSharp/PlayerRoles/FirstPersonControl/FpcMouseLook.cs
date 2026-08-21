using CursorManagement;
using InventorySystem.Items;
using Mirror;
using RelativePositioning;
using UnityEngine;

namespace PlayerRoles.FirstPersonControl
{
    public class FpcMouseLook
    {
        private static FpcMouseLook _localInstance;

        public const float MinimumVer = -88f;
        public const float MaximumVer = 88f;
        public const float OverallMultiplier = 2f;

        private const float FullAngle = 360f;
        private const float SmoothTime = 10f;
        private const float ThirdpersonSmooth = 22f;

        private readonly ReferenceHub _hub;
        private readonly FirstPersonMovementModule _fpmm;

        private float _curHorizontal;
        private float _curVertical;

        private float _syncHorizontal;
        private float _syncVertical;

        private float _inputHorizontal;
        private float _inputVertical;

        private ushort _prevSyncH;
        private ushort _prevSyncV;

        protected virtual float BiaxialSensitivity
        {
            get
            {
                float sens = SensitivitySettings.SensMultiplier * OverallMultiplier;

                if (_hub.inventory.CurInstance is IZoomModifyingItem zoomItem)
                {
                    sens *= zoomItem.SensitivityScale;
                }

                return sens;
            }
        }

        public float CurrentHorizontal
        {
            get => _curHorizontal;
            set
            {
                _curHorizontal = ClampHorizontal(value);
                _inputHorizontal = _curHorizontal;
            }
        }

        public float CurrentVertical
        {
            get => _curVertical;
            set
            {
                _curVertical = ClampVertical(value);
                _inputVertical = _curVertical;
            }
        }

        private Quaternion TargetHubRotation => Quaternion.Euler(0f, _curHorizontal, 0f);
        private Quaternion TargetCamRotation => Quaternion.Euler(-_curVertical, 0f, 0f);

        public FpcMouseLook(ReferenceHub hub, FirstPersonMovementModule fpmm)
        {
            _hub = hub;
            _fpmm = fpmm;

            float initialHorizontal = hub.transform.eulerAngles.y;

            CurrentHorizontal = ClampHorizontal(initialHorizontal);
            CurrentVertical = ClampVertical(0f);

            if (hub.isLocalPlayer)
            {
                _localInstance = this;
            }
        }

        public static bool TryGetLocalMouseLook(out FpcMouseLook ml)
        {
            ml = _localInstance;
            return ml != null;
        }

        public void UpdateRotation()
        {
            Quaternion hubRotation;
            Quaternion camRotation;

            if (_hub.isLocalPlayer)
            {
                float mouseX;
                float mouseY;

                if (SensitivitySettings.RawInput)
                {
                    mouseX = Input.GetAxisRaw("Mouse X");
                    mouseY = Input.GetAxisRaw("Mouse Y");
                }
                else
                {
                    mouseX = Input.GetAxis("Mouse X");
                    mouseY = Input.GetAxis("Mouse Y");
                }

                float sensitivity = BiaxialSensitivity;

                mouseX = ProcessHorizontalInput(mouseX * sensitivity);
                mouseY = ProcessVerticalInput(mouseY * sensitivity);

                if (Cursor.visible || CursorManager.MovementLocked)
                {
                    mouseX = 0f;
                    mouseY = 0f;
                }

                _inputVertical = ClampVertical(_inputVertical + mouseY);
                _inputHorizontal = ClampHorizontal(_inputHorizontal + mouseX);

                float lerpT = SensitivitySettings.SmoothInput ? Time.deltaTime * SmoothTime : 1f;

                _curVertical = ClampVertical(Mathf.LerpAngle(_curVertical, _inputVertical, lerpT));
                _curHorizontal = ClampHorizontal(Mathf.LerpAngle(_curHorizontal, _inputHorizontal, lerpT));

                hubRotation = TargetHubRotation;
                camRotation = TargetCamRotation;
            }
            else
            {
                byte waypointId = _fpmm.Motor.ReceivedPosition.WaypointId;

                CurrentHorizontal = WaypointBase.GetWorldRotation(waypointId,
                    Quaternion.Euler(Vector3.up * _syncHorizontal)).eulerAngles.y;

                CurrentVertical = _syncVertical;

                float lerpT = NetworkServer.active ? 1f : (Time.deltaTime * ThirdpersonSmooth);

                hubRotation = Quaternion.Lerp(_hub.transform.rotation, TargetHubRotation, lerpT);
                camRotation = Quaternion.Lerp(_hub.PlayerCameraReference.localRotation, TargetCamRotation, lerpT);
            }

            _hub.transform.rotation = hubRotation;
            _hub.PlayerCameraReference.localRotation = camRotation;
        }

        public void GetMouseInput(out float hRot, out float vRot)
        {
            float mouseX;
            float mouseY;

            if (SensitivitySettings.RawInput)
            {
                mouseX = Input.GetAxisRaw("Mouse X");
                mouseY = Input.GetAxisRaw("Mouse Y");
            }
            else
            {
                mouseX = Input.GetAxis("Mouse X");
                mouseY = Input.GetAxis("Mouse Y");
            }

            float sensitivity = BiaxialSensitivity;

            hRot = ProcessHorizontalInput(mouseX * sensitivity);
            vRot = ProcessVerticalInput(mouseY * sensitivity);
        }

        public void ApplySyncValues(ushort horizontal, ushort vertical)
        {
            if (_prevSyncH == horizontal && _prevSyncV == vertical)
            {
                _fpmm.Motor.RotationDetected = false;
                return;
            }

            _prevSyncH = horizontal;
            _prevSyncV = vertical;

            _syncHorizontal = Mathf.Lerp(0f, FullAngle, (float)horizontal / 65535f);
            _syncVertical = Mathf.Lerp(MinimumVer, MaximumVer, (float)vertical / 65535f);

            if (_hub.isLocalPlayer)
            {
                CurrentHorizontal = _syncHorizontal;
                CurrentVertical = _syncVertical;
            }

            _fpmm.Motor.RotationDetected = true;
        }

        public void GetSyncValues(byte waypointId, out ushort syncH, out ushort syncV)
        {
            Quaternion hubRot = Quaternion.Euler(0f, CurrentHorizontal, 0f);
            Quaternion relativeRot = WaypointBase.GetRelativeRotation(waypointId, hubRot);
            float horizontalAngle = relativeRot.eulerAngles.y;

            syncH = (ushort)Mathf.RoundToInt(Mathf.InverseLerp(0f, FullAngle, horizontalAngle) * 65535f);
            syncV = (ushort)Mathf.RoundToInt(Mathf.InverseLerp(MinimumVer, MaximumVer, CurrentVertical) * 65535f);
        }

        protected virtual float ClampHorizontal(float f)
        {
            while (f < 0f) f += FullAngle;
            while (f >= FullAngle) f -= FullAngle;
            return f;
        }

        protected virtual float ClampVertical(float f)
        {
            return Mathf.Clamp(f, MinimumVer, MaximumVer);
        }

        protected virtual float ProcessVerticalInput(float f)
        {
            if (SensitivitySettings.Invert)
                f = -f;

            return f;
        }

        protected virtual float ProcessHorizontalInput(float f)
        {
            return f;
        }
    }
}
