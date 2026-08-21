using System.Collections.Generic;
using Mirror;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939
{
	public class Scp939Motor : FpcMotor
	{
		private const float MinDot = 0.4f;

		private const float LungeRadius = 0.6f;

		private const float FocusToleranceOffset = 0.13f;

		private const float FocusTimeTolerance = 3.5f;

		private const int MaxDetections = 32;

		private const float MinDistanceSqr = 0.4f;

		private readonly Scp939Role _role;

		private readonly Scp939FocusAbility _focus;

		private readonly Scp939LungeAbility _lunge;

		private readonly Scp939AmnesticCloudAbility _cloud;

		private static readonly Collider[] Detections = new Collider[32];

		private static readonly CachedLayerMask Mask = new CachedLayerMask("Hitbox", "Glass");

		private static readonly HashSet<HumanRole> DetectedHumans = new HashSet<HumanRole>();

		private bool IsLocalPlayer => ViewMode == FpcViewMode.LocalPlayer;

		private bool WantsToLunge
		{
			get
			{
				if (_lunge.IsReady)
				{
					if (!_lunge.LungeRequested)
					{
						return base.WantsToJump;
					}
					return true;
				}
				return false;
			}
		}

		private bool IsLunging => _lunge.State == Scp939LungeState.Triggered;

		protected override float Speed
		{
			get
			{
				if (IsLunging)
				{
					return _lunge.LungeForwardSpeed;
				}
				if (IsLocalPlayer && _cloud.TargetState)
				{
					return base.Speed * Mathf.Clamp01(1f - _cloud.HoldDuration);
				}
				float num = _focus.State;
				if (NetworkServer.active && !IsLocalPlayer && _focus.TargetState)
				{
					if (_focus.FrozenTime > 3.5f)
					{
						return 0f;
					}
					num -= 0.13f;
				}
				return base.Speed * Mathf.Clamp01(1f - num);
			}
		}

		protected override Vector3 DesiredMove
		{
			get
			{
				if (!IsLocalPlayer || !IsLunging)
				{
					return base.DesiredMove;
				}
				return CachedTransform.forward;
			}
		}

		public override Vector3 Velocity
		{
			get
			{
				if (!NetworkServer.active || Hub.isLocalPlayer || !IsLunging)
				{
					return base.Velocity;
				}
				return CachedTransform.forward * _lunge.LungeForwardSpeed;
			}
		}

		public bool MovingForwards
		{
			get
			{
				if (IsLocalPlayer)
				{
					return Vector3.Dot(base.MoveDirection.NormalizeIgnoreY(), CachedTransform.forward) > 0.4f;
				}
				return true;
			}
		}

		public Scp939Motor(ReferenceHub hub, Scp939Role role)
			: base(hub, role.FpcModule, enableFallDamage: false)
		{
			_role = role;
			role.SubroutineModule.TryGetSubroutine<Scp939FocusAbility>(out _focus);
			role.SubroutineModule.TryGetSubroutine<Scp939LungeAbility>(out _lunge);
			role.SubroutineModule.TryGetSubroutine<Scp939AmnesticCloudAbility>(out _cloud);
		}

		private void ProcessHitboxCollision(HitboxIdentity hid)
		{
			if (IsLocalPlayer && hid.TargetHub.roleManager.CurrentRole is HumanRole humanRole && !((MainModule.Position - humanRole.FpcModule.Position).SqrMagnitudeIgnoreY() < 0.4f))
			{
				DetectedHumans.Add(humanRole);
			}
		}

		private void ProcessWindowCollision(BreakableWindow window)
		{
			if (NetworkServer.active)
			{
				window.Damage(window.health, new Scp939DamageHandler(_role, Scp939DamageType.LungeTarget), default(Vector3));
				return;
			}
			Collider[] componentsInChildren = window.GetComponentsInChildren<Collider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}

		private void OverlapCapsule(Vector3 point1, Vector3 point2)
		{
			int num = Physics.OverlapCapsuleNonAlloc(point1, point2, 0.6f, Detections, Mask);
			for (int i = 0; i < num; i++)
			{
				if (Detections[i].TryGetComponent<IDestructible>(out var component))
				{
					if (component is HitboxIdentity hid)
					{
						ProcessHitboxCollision(hid);
					}
					else if (component is BreakableWindow window)
					{
						ProcessWindowCollision(window);
					}
				}
			}
		}

		protected override void UpdateFloating()
		{
			if (_focus.State == 1f && _lunge.State != Scp939LungeState.Triggered)
			{
				Vector3 moveDirection = MoveDirection;
				moveDirection.y = Mathf.Min(moveDirection.y, 0f - _lunge.LungeJumpSpeed);
				MoveDirection = moveDirection;
			}
			base.UpdateFloating();
		}

		protected override void UpdateGrounded(ref bool sendJump, float jumpSpeed)
		{
			if (WantsToLunge)
			{
				_lunge.TriggerLunge();
				jumpSpeed = _lunge.LungeJumpSpeed;
				base.WantsToJump = true;
			}
			else if (_focus.State > 0f || _cloud.TargetState)
			{
				jumpSpeed = 0f;
			}
			base.UpdateGrounded(ref sendJump, jumpSpeed);
		}

		protected override Vector3 GetFrameMove()
		{
			Vector3 frameMove = base.GetFrameMove();
			if (!IsLunging || (!NetworkServer.active && !IsLocalPlayer))
			{
				return frameMove;
			}
			DetectedHumans.Clear();
			Vector3 position = MainModule.Position;
			Vector3 vector = position + frameMove;
			Vector3 vector2 = Vector3.down * 0.6f;
			OverlapCapsule(position, vector);
			OverlapCapsule(position + vector2, vector + vector2);
			if (!IsLocalPlayer)
			{
				return frameMove;
			}
			HumanRole human = null;
			float num = float.MaxValue;
			bool flag = false;
			foreach (HumanRole detectedHuman in DetectedHumans)
			{
				float sqrMagnitude = (detectedHuman.FpcModule.Position - vector).sqrMagnitude;
				if (!(sqrMagnitude >= num))
				{
					human = detectedHuman;
					num = sqrMagnitude;
					flag = true;
				}
			}
			if (!flag)
			{
				return frameMove;
			}
			_lunge.ClientSendHit(human);
			return _lunge.LungeJumpSpeed * Time.deltaTime * Vector3.down;
		}
	}
}
