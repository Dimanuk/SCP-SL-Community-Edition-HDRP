using System.Collections.Generic;

using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp106
{
	public class Scp106MovementModule : FirstPersonMovementModule
	{
		[SerializeField]
		private float _stalkSpeed;

		private const float SubmergingLerp = 1.5f;

		private const int MaxDetections = 8;

		private const float MinDistanceSqr = 0.006f;

		private const float ClientsideDetectionRange = 0.3f;

		private const int GlassLayer = 14;

		private const int DoorLayer = 27;

		private const float SlowdownTransitionSpeed = 4f;

		private int _numberOfDetections;

		private float _ccRadius;

		private Vector3 _ccOffset;

		private float _slowndownTarget;

		private float _slowndownSpeed;

		private float _normalSpeed;

		private Scp106SinkholeController _sinkhole;

		private readonly Collider[] _collidersNonAlloc = new Collider[8];

		private readonly RaycastHit[] _hitsNonAlloc = new RaycastHit[8];

		private static readonly int DetectionMask = (1 << GlassLayer) | (1 << DoorLayer);

        private static readonly global::System.Collections.Generic.HashSet<global::UnityEngine.Collider> EnabledColliders = new global::System.Collections.Generic.HashSet<global::UnityEngine.Collider>();

        private float MovementSpeed
        {
            get
            {
                return WalkSpeed;
            }
            set
            {
                SneakSpeed = value;
                WalkSpeed = value;
                SprintSpeed = value;
            }
        }

        public float CurSlowdown { get; private set; }

        public override void SpawnObject()
        {
            base.SpawnObject();
            _sinkhole = (base.Hub.roleManager.CurrentRole as global::PlayerRoles.PlayableScps.Scp106.Scp106Role).Sinkhole;
        }

        private void RefreshInstance()
        {
            _slowndownTarget = (_sinkhole.State ? 1 : 0);
            global::UnityEngine.Vector3 vector;
            if (base.Hub.isLocalPlayer)
            {
                global::UnityEngine.Vector3 moveDirection = base.Motor.MoveDirection;
                if (base.IsGrounded)
                {
                    moveDirection.y = 0f;
                }
                vector = base.Position + moveDirection.normalized * 0.3f;
            }
            else
            {
                if (!global::Mirror.NetworkServer.active && !global::PlayerRoles.Spectating.SpectatorNetworking.IsLocallySpectated(base.Hub))
                {
                    return;
                }
                vector = base.Motor.ReceivedPosition.Position;
            }
            RefreshCollidersOverlap(vector);
            if ((vector - base.Position).sqrMagnitude > 0.006f)
            {
                RefreshCollidersCapsuleCast(base.Position, vector);
            }
            CurSlowdown = global::UnityEngine.Mathf.MoveTowards(CurSlowdown, _slowndownTarget, global::UnityEngine.Time.deltaTime * 4f);
            if (_sinkhole.IsDuringAnimation)
            {
                MovementSpeed = global::UnityEngine.Mathf.Lerp(MovementSpeed, 0f, 1.5f * global::UnityEngine.Time.deltaTime);
                return;
            }
            float num = global::UnityEngine.Mathf.Lerp(_normalSpeed, _slowndownSpeed, CurSlowdown);
            MovementSpeed = (_sinkhole.State ? _stalkSpeed : num);
        }

        private void RefreshCollidersCapsuleCast(global::UnityEngine.Vector3 curPoint, global::UnityEngine.Vector3 targetPoint)
        {
            float num = global::UnityEngine.Vector3.Distance(targetPoint, curPoint);
            global::UnityEngine.Vector3 direction = (targetPoint - curPoint) / num;
            _numberOfDetections = global::UnityEngine.Physics.CapsuleCastNonAlloc(curPoint + _ccOffset, curPoint - _ccOffset, _ccRadius, direction, _hitsNonAlloc, num, DetectionMask);
            for (int i = 0; i < _numberOfDetections; i++)
            {
                ProcessCollider(_hitsNonAlloc[i].collider);
            }
        }

        private void RefreshCollidersOverlap(global::UnityEngine.Vector3 point)
        {
            _numberOfDetections = global::UnityEngine.Physics.OverlapCapsuleNonAlloc(point + _ccOffset, point - _ccOffset, _ccRadius, _collidersNonAlloc, DetectionMask);
            for (int i = 0; i < _numberOfDetections; i++)
            {
                ProcessCollider(_collidersNonAlloc[i]);
            }
        }

        private void ProcessCollider(global::UnityEngine.Collider col)
        {
            bool flag = false;
            if (col.gameObject.layer == 14 && col is global::UnityEngine.BoxCollider)
            {
                col.isTrigger = true;
                EnabledColliders.Add(col);
                flag = true;
            }
            global::UnityEngine.Transform parent = col.transform;
            global::Interactables.Interobjects.DoorUtils.DoorVariant component;
            while (!parent.TryGetComponent<global::Interactables.Interobjects.DoorUtils.DoorVariant>(out component))
            {
                parent = parent.parent;
                if (!(parent != null))
                {
                    if (flag)
                    {
                        _slowndownTarget = 1f;
                    }
                    return;
                }
            }
            if (component is global::Interactables.Interobjects.DoorUtils.IScp106PassableDoor scp106PassableDoor && scp106PassableDoor.IsScp106Passable)
            {
                // Unity forbids isTrigger on concave (non-convex) MeshColliders and logs a spammy error.
                // SCP-106-passable doors normally use convex colliders; guard here so a stray concave
                // MeshCollider on a passable door doesn't throw the "Triggers on concave MeshColliders
                // are not supported" error every physics tick.
                if (!(col is global::UnityEngine.MeshCollider meshCollider) || meshCollider.convex)
                {
                    col.isTrigger = true;
                    EnabledColliders.Add(col);
                }
                _slowndownTarget = global::UnityEngine.Mathf.Max(_slowndownTarget, global::UnityEngine.Mathf.Clamp01(1f - component.GetExactState()));
            }
        }

        private void Awake()
        {
            global::PlayerRoles.FirstPersonControl.CharacterControllerSettingsPreset characterControllerSettings = CharacterControllerSettings;
            _ccRadius = characterControllerSettings.Radius + characterControllerSettings.SkinWidth;
            _ccOffset = global::UnityEngine.Vector3.up * characterControllerSettings.Height / 2f;
            _slowndownSpeed = SneakSpeed;
            _normalSpeed = WalkSpeed;
            MovementSpeed = _normalSpeed;
        }

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            StaticUnityMethods.OnUpdate += delegate
            {
                global::Utils.NonAllocLINQ.HashsetExtensions.ForEach(EnabledColliders, delegate (global::UnityEngine.Collider x)
                {
                    if (!(x == null))
                    {
                        x.isTrigger = false;
                    }
                });
                EnabledColliders.Clear();
                global::Utils.NonAllocLINQ.HashsetExtensions.ForEach(global::PlayerRoles.PlayableScps.Scp106.Scp106Role.AllInstances, delegate (global::PlayerRoles.PlayableScps.Scp106.Scp106Role x)
                {
                    (x.FpcModule as global::PlayerRoles.PlayableScps.Scp106.Scp106MovementModule).RefreshInstance();
                });
            };
        }
    }
}
