using System;
using Mirror;
using UnityEngine;

namespace PlayerRoles.Spectating
{
    public class SpectatorTargetTracker : MonoBehaviour
    {
        private static bool _trackedTransformSet;

        private static Transform _trackedTransform;

        private static Vector3 _trackedTransformPositionOffset;

        private static Quaternion _trackedTransformRotationOffset;

        private static SpectatableModuleBase _curTracked;

        private static Offset _offsetCache;

        [field: SerializeField]
        public GameObject AttachmentsMenu { get; private set; }

        [field: SerializeField]
        public GameObject VoiceChatMenu { get; private set; }

        private static Transform TrackedTransform
        {
            get
            {
                int num;
                if (_trackedTransformSet)
                {
                    num = ((_trackedTransform == null) ? 1 : 0);
                    if (num == 0)
                    {
                        goto IL_001e;
                    }
                }
                else
                {
                    num = 1;
                }
                _trackedTransformSet = false;
                goto IL_001e;
            IL_001e:
                if (num == 0)
                {
                    return _trackedTransform;
                }
                return MainCameraController.CurrentCamera;
            }
        }

        public static bool TrackerSet { get; private set; }

        public static SpectatorTargetTracker Singleton { get; private set; }

        public static ReferenceHub LastTrackedPlayer { get; private set; }

        public static SpectatableModuleBase CurrentTarget
        {
            get
            {
                return _curTracked;
            }
            set
            {
                if (!(_curTracked == value))
                {
                    if (_curTracked != null)
                    {
                        _curTracked.OnStoppedSpectating();
                    }
                    _curTracked = value;
                    MainCameraController.ForceUpdatePosition();
                    if (value != null)
                    {
                        LastTrackedPlayer = (value.MainRole.TryGetOwner(out var hub) ? hub : null);
                        value.OnBeganSpectating();
                    }
                    OnTargetChanged?.Invoke();
                }
            }
        }

        public static Offset CurrentOffset
        {
            get
            {
                if (_trackedTransformSet || !TrackerSet)
                {
                    Transform trackedTransform = TrackedTransform;
                    _offsetCache.position = trackedTransform.TransformPoint(_trackedTransformPositionOffset);
                    _offsetCache.rotation = (trackedTransform.rotation * _trackedTransformRotationOffset).eulerAngles;
                }
                else if (CurrentTarget != null)
                {
                    // No Pooled guard on purpose: when the spectated target's role has been
                    // returned to the pool (e.g. nobody left to spectate), reading its camera
                    // throws a NullReferenceException, which MainCameraController.GetPositionAndRotation
                    // catches and falls back to the default surface camera position.
                    _offsetCache.position = CurrentTarget.CameraPosition;
                    _offsetCache.rotation = CurrentTarget.CameraRotation;
                }
                return _offsetCache;
            }
        }

        public static Action OnTargetChanged;

        private void OnEnable()
        {
            Singleton = this;
            TrackerSet = true;
        }

        private void OnDisable()
        {
            CurrentTarget = null;
            if (Singleton == this)
            {
                TrackerSet = false;
            }
        }

        private void OnDestroy()
        {
            Singleton = null;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            PlayerRoleManager.OnRoleChanged += OnRoleChanged;
        }

        private static void OnRoleChanged(ReferenceHub hub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            if (TrackerSet && prevRole is ISpectatableRole spectatableRole && CurrentTarget == spectatableRole.SpectatorModule)
            {
                CurrentTarget = ((newRole is ISpectatableRole spectatableRole2) ? spectatableRole2.SpectatorModule : null);
            }

            if (!hub.isLocalPlayer)
                return;

            if (newRole is SpectatorRole spectatorRole)
            {
                LastTrackedPlayer = hub;

                if (Singleton != null)
                {
                    Singleton.gameObject.SetActive(true);
                }
                else
                {
                    Singleton = UnityEngine.Object.Instantiate(spectatorRole.TrackerPrefab, Vector3.zero, Quaternion.identity, null);
                }

                if (newRole is OverwatchRole)
                {
                    if (Singleton.AttachmentsMenu != null) Singleton.AttachmentsMenu.SetActive(false);
                    if (Singleton.VoiceChatMenu != null) Singleton.VoiceChatMenu.SetActive(true);
                }
                else
                {
                    if (Singleton.VoiceChatMenu != null) Singleton.VoiceChatMenu.SetActive(false);
                    if (Singleton.AttachmentsMenu != null) Singleton.AttachmentsMenu.SetActive(true);
                }
            }
            else if (TrackerSet && Singleton != null)
            {
                Singleton.gameObject.SetActive(false);
            }
        }

        public static void SetTrackedTransform(Transform trackedTransform, Vector3 localPosOffset, Quaternion localRotOffset)
        {
            _trackedTransform = trackedTransform;
            _trackedTransformSet = trackedTransform != null;
            _trackedTransformPositionOffset = localPosOffset;
            _trackedTransformRotationOffset = localRotOffset;
        }

        public static void SetTrackedTransform(Transform trackedTransform)
        {
            SetTrackedTransform(trackedTransform, Vector3.zero, Quaternion.identity);
        }

        public static bool TryGetTrackedPlayer(out ReferenceHub hub)
        {
            if (TrackerSet && CurrentTarget != null && CurrentTarget.MainRole.TryGetOwner(out hub))
            {
                return true;
            }
            hub = null;
            return false;
        }
    }
}
