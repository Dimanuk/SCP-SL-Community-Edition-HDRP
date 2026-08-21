using MapGeneration;
using PlayerRoles.PlayableScps.Scp079.GUI;
using PlayerRoles.PlayableScps.Scp079.Overcons;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Cameras
{
    public class Scp079DirectionalCameraSelector : Scp079KeyAbilityBase
    {
        private static string _translationNoCamera;
        private static string _translationSwitch;
        private static readonly Vector3Int[] WorldDirections = new Vector3Int[4]
        {
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 0, -1),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 0, 1)
        };

        [SerializeField]
        private ActionName _key;

        [SerializeField]
        private Vector3 _direction;

        private Scp079Camera _lastCamera;
        private bool _lastValid;
        private float _lastSwitchCost;
        private float _failMessageSwitchCost;

        public override bool IsReady
        {
            get
            {
                _lastValid = TryGetCamera(out _lastCamera);
                if (!_lastValid)
                    return false;
                
                _lastSwitchCost = base.CurrentCamSync.GetSwitchCost(_lastCamera);
                return _lastSwitchCost <= base.AuxManager.CurrentAux;
            }
        }

        public override ActionName ActivationKey => _key;

        public override bool IsVisible => !Scp079CursorManager.LockCameras;

        public override string AbilityName
        {
            get
            {
                if (!_lastValid)
                    return _translationNoCamera;
                return string.Format(_translationSwitch, _lastCamera.Label, _lastSwitchCost);
            }
        }

        public override string FailMessage
        {
            get
            {
                if (base.AuxManager.CurrentAux >= _failMessageSwitchCost)
                    return null;
                return GetNoAuxMessage(_failMessageSwitchCost);
            }
        }

        protected virtual bool TryGetCamera(out Scp079Camera targetCamera)
        {
            targetCamera = null;
            bool result = false;
            
            Transform currentCamera = MainCameraController.CurrentCamera;
            Vector3 normalized = currentCamera.TransformDirection(_direction).normalized;
            
            Vector3Int bestDir = Vector3Int.zero;
            float bestDot = -1f;
            
            foreach (Vector3Int dir in WorldDirections)
            {
                float dot = Vector3.Dot(dir, normalized);
                if (dot < bestDot)
                    continue;
                
                bestDir = dir;
                bestDot = dot;
            }

            if (bestDot <= 0f)
                return false;

            Vector3Int targetCoords = RoomIdUtils.PositionToCoords(currentCamera.position) + bestDir;
            
            foreach (CameraOvercon visibleOvercon in CameraOverconRenderer.VisibleOvercons)
            {
                if (visibleOvercon.IsElevator)
                    continue;
                
                Scp079Camera target = visibleOvercon.Target;
                if (RoomIdUtils.PositionToCoords(target.Position) != targetCoords)
                    continue;
                
                targetCamera = target;
                result = true;
                if (targetCamera.IsMain)
                    return true;
            }
            
            return result;
        }

        protected override void Trigger()
        {
            base.CurrentCamSync.ClientSwitchTo(_lastCamera);
        }

        protected override void Start()
        {
            base.Start();
            _translationNoCamera = Translations.Get(Scp079HudTranslation.NoCamera);
            _translationSwitch = Translations.Get(Scp079HudTranslation.GoToCamera);
            
            base.CurrentCamSync.OnCameraChanged += delegate
            {
                _failMessageSwitchCost = 0f;
            };
        }

        public override void OnFailMessageAssigned()
        {
            _failMessageSwitchCost = _lastValid ? _lastSwitchCost : 0f;
        }

        static Scp079DirectionalCameraSelector()
        {
            WorldDirections = new Vector3Int[4]
            {
                new Vector3Int(-1, 0, 0),
                new Vector3Int(0, 0, -1),
                new Vector3Int(1, 0, 0),
                new Vector3Int(0, 0, 1)
            };
        }
    }
}