using System.Linq;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using PlayerRoles.PlayableScps.Scp079.GUI;
using PlayerRoles.PlayableScps.Scp079.Map;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Overcons
{
    public class OverconManager : MonoBehaviour
    {
        [SerializeField]
        private OverconRendererBase[] _renderers;

        private Scp079CurrentCameraSync _curCamSync;

        // "Cam Overcon" renders the world-tracked icon markers (door/tesla/elevator/CCTV) at their
        // real in-world distance, separately from "Cam HUD" which renders the fixed frame/map close
        // to the lens. Neither camera clears when disabled, so while the surveillance map is open the
        // icons would otherwise keep drawing straight through the map overlay; only "Cam HUD" should
        // stay active then.
        private Camera _overconCamera;

        private const string OverconLayer = "Interface079";
        private const float Range = 200f;

        private static int _raycastMask;

        private static int RaycastMask
        {
            get
            {
                if (_raycastMask == 0)
                    _raycastMask = LayerMask.GetMask(OverconLayer);
                return _raycastMask;
            }
        }

        private static Camera RaycastCam => Scp079Hud.RaycastCamera;

        public OverconBase HighlightedOvercon { get; private set; }
        public static OverconManager Singleton { get; private set; }

        private void OnCameraChanged()
        {
            Scp079Camera currentCamera = _curCamSync.CurrentCamera;

            foreach (OverconRendererBase renderer in _renderers)
                renderer.SpawnOvercons(currentCamera);
        }

        private void Start()
        {
            Singleton = this;

            Scp079Hud.Instance.SubroutineModule.TryGetSubroutine(out _curCamSync);
            _curCamSync.OnCameraChanged += OnCameraChanged;
            HideHUDController.ToggleHUD += ToggleHud;

            foreach (Camera cam in transform.root.GetComponentsInChildren<Camera>(true))
            {
                if (cam.gameObject.name != "Cam Overcon")
                    continue;

                _overconCamera = cam;
                break;
            }

            OnCameraChanged();
        }

        private void OnDestroy()
        {
            HideHUDController.ToggleHUD -= ToggleHud;

            OverconBase[] overcons = OverconBase.ActiveInstances.ToArray();
            foreach (OverconBase overcon in overcons)
            {
                if (overcon != null)
                    Destroy(overcon.gameObject);
            }
            OverconBase.ActiveInstances.Clear();

            if (_curCamSync != null)
                _curCamSync.OnCameraChanged -= OnCameraChanged;
        }

        private void Update()
        {
            if (_overconCamera != null)
                _overconCamera.enabled = !Scp079ToggleMapAbility.MapState;

            if (!Scp079Role.LocalInstanceActive)
                return;

            OverconBase overcon = Physics.Raycast(
                RaycastCam.ScreenPointToRay(Input.mousePosition),
                out RaycastHit hit,
                Range,
                RaycastMask)
                ? hit.collider.GetComponent<OverconBase>()
                : null;

            if (overcon != HighlightedOvercon)
            {
                if (HighlightedOvercon != null)
                    HighlightedOvercon.IsHighlighted = false;

                if (overcon != null)
                    overcon.IsHighlighted = true;

                HighlightedOvercon = overcon;
            }
        }

        private void ToggleHud(bool b)
        {
            if (RaycastCam != null)
                RaycastCam.farClipPlane = b ? Range : 1f;
        }
    }
}
