
using Interactables.Interobjects.DoorUtils;
using Mirror;
using UnityEngine;

namespace InventorySystem.Items.ThrowableProjectiles
{
	public class Scp018Projectile : ExplosionGrenade
    {
        [global::UnityEngine.Header("SCP-018 settings")]
        [SerializeField]
		private float _activationVelocity;

		[SerializeField]
		private float _minimalHeightVelocity;

		[SerializeField]
		private float _cutoffVelocity;

		[SerializeField]
		private float _velocityMultiplier;

		[SerializeField]
		private float _reactivationTime;

		[SerializeField]
		private AnimationCurve _damageOverSpeed;

		[SerializeField]
		private float _damageCooldown;

		[SerializeField]
		private float _friendlyFireTime;

		private const int MinDamageValue = 10;

		private const int HumanDamageMultiplier = 75;

		private const int ScpDamageMultiplier = 800;

		private const int DoorDamageMultiplier = 300;

		private const float DamageVariation = 0.1f;

		private const int HitboxLayer = 13;

		private const int DoorLayer = 27;

		private float _activationSqrt;

		private float _cutoffSqrt;

		private float _activatedTime;

		private float _subspeedTimer;

		private float _cooldownTimer;

		private Vector3 _prevPosition;

        private float CurrentDamage => _damageOverSpeed.Evaluate(global::UnityEngine.Mathf.InverseLerp(_activationSqrt, _cutoffSqrt, Rb.linearVelocity.sqrMagnitude));

        private bool IgnoreFriendlyFire
        {
            get
            {
                if (PreviousOwner.IsSet)
                {
                    return global::UnityEngine.Time.timeSinceLevelLoad > _activatedTime + _friendlyFireTime;
                }
                return true;
            }
        }

        protected override void Start()
        {
            base.Start();
            _activationSqrt = _activationVelocity * _activationVelocity;
            _cutoffSqrt = _cutoffVelocity * _cutoffVelocity;
        }

        protected override void Update()
        {
            base.Update();
            if (_activatedTime == 0f || !global::Mirror.NetworkServer.active)
            {
                return;
            }
            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= global::UnityEngine.Time.deltaTime;
            }
            else
            {
                DetectPlayers();
            }
            if (global::UnityEngine.Mathf.Abs(Rb.linearVelocity.y) >= _minimalHeightVelocity)
            {
                _subspeedTimer = 0f;
                return;
            }
            _subspeedTimer += global::UnityEngine.Time.deltaTime;
            if (_subspeedTimer > _reactivationTime)
            {
                Rb.linearVelocity += ((Rb.linearVelocity.y < 0f) ? global::UnityEngine.Vector3.down : global::UnityEngine.Vector3.up) * _activationVelocity * _velocityMultiplier;
                _subspeedTimer = 0f;
            }
        }

        private void DetectPlayers()
        {
            global::UnityEngine.Vector3 prevPosition = _prevPosition;
            _prevPosition = Rb.position;

            ReferenceHub.TryGetHostHub(out ReferenceHub hostHub);
            bool restoreHostHitboxes = HitboxIdentity.SetOwnHitboxes(hostHub, true);

            bool hitDetected = global::UnityEngine.Physics.Linecast(prevPosition, Rb.position, out var hitInfo, 13);

            if (restoreHostHitboxes)
            {
                HitboxIdentity.SetOwnHitboxes(hostHub, false);
            }

            if (hitDetected && ReferenceHub.TryGetHub(hitInfo.transform.root.gameObject, out var hub))
            {
                float num = CurrentDamage * global::UnityEngine.Random.Range(0.9f, 1.1f) * (float)(global::PlayerRoles.PlayerRolesUtils.IsHuman(hub) ? 75 : 800);
                if (!(num < 10f))
                {
                    Rb.AddForce(global::UnityEngine.Vector3.left + global::UnityEngine.Vector3.forward, global::UnityEngine.ForceMode.Force);
                    hub.playerStats.DealDamage(new global::PlayerStatsSystem.Scp018DamageHandler(this, num, IgnoreFriendlyFire));
                    _cooldownTimer = _damageCooldown;
                }
            }
        }

        private bool TryGetDoor(global::UnityEngine.Collision col, out global::Interactables.Interobjects.DoorUtils.IDamageableDoor door)
        {
            door = null;
            if (col.collider.gameObject.layer != 27)
            {
                return false;
            }
            if (!col.collider.TryGetComponent<global::Interactables.InteractableCollider>(out var component))
            {
                return false;
            }
            if (component.Target is not global::Interactables.Interobjects.DoorUtils.IDamageableDoor damageableDoor)
            {
                return false;
            }
            door = damageableDoor;
            return true;
        }

        protected override void ProcessCollision(global::UnityEngine.Collision collision)
        {
            if (global::Mirror.NetworkServer.active)
            {
                base.ProcessCollision(collision);
                RpcMakeSound(collision.relativeVelocity.sqrMagnitude);
            }
            if (_activatedTime == 0f)
            {
                if (collision.relativeVelocity.sqrMagnitude < _activationSqrt)
                {
                    return;
                }
                if (global::Mirror.NetworkServer.active)
                {
                    ServerActivate();
                }
                _activatedTime = global::UnityEngine.Time.timeSinceLevelLoad;
            }
            if (global::Mirror.NetworkServer.active && TryGetDoor(collision, out var door))
            {
                float num = CurrentDamage * 300f;
                if (num >= 10f)
                {
                    door.ServerDamage(num, global::Interactables.Interobjects.DoorUtils.DoorDamageType.Grenade);
                }
            }
            if (Rb.linearVelocity.sqrMagnitude < _cutoffSqrt)
            {
                Rb.linearVelocity *= _velocityMultiplier;
            }
        }

        [global::Mirror.ClientRpc]
        private void RpcMakeSound(float sqrtSpeed)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                MakeCollisionSound(sqrtSpeed);
            }
        }

        public override void ServerActivate()
        {
            base.ServerActivate();
            global::InventorySystem.Items.Pickups.PickupSyncInfo info = Info;
            info.Locked = true;
            base.Info = info;
            _activatedTime = global::UnityEngine.Time.timeSinceLevelLoad;
            _prevPosition = Rb.position;
        }
	}
}
