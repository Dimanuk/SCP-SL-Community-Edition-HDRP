using MapGeneration;
using PlayerRoles;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    public const float DefaultVerticalFOV = 70f;

    public static Transform _currentCamera;

    public static MainCameraController _singleton;

    public static Vector3 _defaultPos;

    [SerializeField]
    public Transform _defaultCamera;

    public static bool InstanceActive { get; set; }

    public static Vector3 LastPosition { get; set; }

    public static Quaternion LastRotation { get; set; }

    public static Ray LastForwardRay => new(LastPosition, LastRotation * Vector3.forward);

    public static Transform CurrentCamera
    {
        get
        {
            return _currentCamera;
        }
        set
        {
            if (!(_currentCamera == value))
            {
                Transform transform = value;
                if (transform == null)
                {
                    transform = _singleton._defaultCamera;
                }

                if (_currentCamera != null)
                {
                    _currentCamera.gameObject.SetActive(value: false);
                }

                _currentCamera = transform;
                if (transform != null)
                {
                    transform.gameObject.SetActive(value: true);
                }
            }
        }
    }

    public static event Action OnUpdated;

    public static event Action OnBeforeUpdated;

    public static bool TryGetCurrentRoom(out RoomIdentifier rid)
    {
        if (CurrentCamera == null)
        {
            rid = null;
            return false;
        }
        Vector3Int key = RoomIdUtils.PositionToCoords(CurrentCamera.position);
        return RoomIdentifier.RoomsByCoordinates.TryGetValue(key, out rid);
    }

    public void Awake()
    {
        InstanceActive = true;
        _singleton = this;
        _defaultPos = _defaultCamera.position;
        CurrentCamera = _defaultCamera;

        PostProcessing.PostProcessChainUnity6Fix.Apply();
    }

    public void OnDestroy()
    {
        InstanceActive = false;
    }

    public void LateUpdate()
    {
        ForceUpdatePosition();
    }

    public static void GetPositionAndRotation(out Vector3 pos, out Quaternion rot)
    {
        pos = _defaultPos;
        rot = Quaternion.identity;

        if (ReferenceHub.TryGetLocalHub(out var hub) && hub.roleManager.CurrentRole is ICameraController cameraController)
        {
            try
            {
                pos = cameraController.CameraPosition;
                float z = (cameraController is IAdvancedCameraController advancedCameraController)
                          ? advancedCameraController.RollRotation : 0f;
                rot = Quaternion.Euler(cameraController.VerticalRotation, cameraController.HorizontalRotation, z);
            }
            catch (System.NullReferenceException)
            {
                pos = _defaultPos;
                rot = Quaternion.identity;
            }
        }
    }

    public static void ForceUpdatePosition()
    {
        if (InstanceActive)
        {
            MainCameraController.OnBeforeUpdated?.Invoke();
            GetPositionAndRotation(out var pos, out var rot);
            CurrentCamera.SetPositionAndRotation(pos, rot);
            LastPosition = pos;
            LastRotation = rot;
            MainCameraController.OnUpdated?.Invoke();
        }
    }
}
