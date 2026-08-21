using System;
using UnityEngine;

namespace Interactables
{
    public class CenterScreenRaycast : MonoBehaviour
    {
        [SerializeField]
        public float _rayDistance = 300f;

        [SerializeField]
        public LayerMask _centerScreenRayHits;

        public static RaycastHit LastRaycastHit { get; set; }

        public static event Action<RaycastHit> OnCenterRaycastHit;

        public static event Action OnCenterRaycastMissed;

        public void Awake()
        {
            MainCameraController.OnUpdated += PerformRaycast;
        }

        public void OnDestroy()
        {
            MainCameraController.OnUpdated -= PerformRaycast;
        }

        public void PerformRaycast()
        {
            Transform currentCamera = MainCameraController.CurrentCamera;
            if (Physics.Raycast(new Ray(currentCamera.position, currentCamera.forward), out var hitInfo, _rayDistance, _centerScreenRayHits))
            {
                LastRaycastHit = hitInfo;
                CenterScreenRaycast.OnCenterRaycastHit?.Invoke(hitInfo);
            }
            else
            {
                // Nothing under the crosshair - drop the previous hit, otherwise consumers keep
                // treating the last collider as if it were still being looked at.
                LastRaycastHit = default(RaycastHit);
                CenterScreenRaycast.OnCenterRaycastMissed?.Invoke();
            }
        }
    }
}