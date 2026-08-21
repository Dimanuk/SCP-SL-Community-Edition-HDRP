using System;
using System.Diagnostics;
using AudioPooling;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerRoles.PlayableScps.HUDs;
using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939
{
	public class Scp939FpsAnimator : ScpViewmodelBase
	{
		private static readonly int WalkCycleHash = Animator.StringToHash("WalkCycle");
		private static readonly int WalkBlendHash = Animator.StringToHash("WalkBlend");
		private static readonly int ClawAttackHash = Animator.StringToHash("ClawAttack");
		private static readonly int FocusActiveHash = Animator.StringToHash("Focus");
		private static readonly int JumpingHash = Animator.StringToHash("IsJumping");
		private static readonly int LungeStateHash = Animator.StringToHash("LungeState");
		private static readonly int LungeTriggerHash = Animator.StringToHash("LungeTrigger");
		private static readonly int CloudHash = Animator.StringToHash("ChargingCloud");
		private const int ModelLayerWalk = 5;
		private const int JumpLayer = 2;
		private const float MinFocusStateToDisplay = 0.4f;
		private const float JumpLayerAdjustmentSpeed = 4.5f;
		private const float JumpOverLifetime = 0.4f;
		private const int CloudLayer = 4;
		private const float CloudTransitionSpeed = 1.5f;
		private const float CloudMaxWeight = 2.5f;

		[SerializeField]
		private float _dampTimeBlend;

		[SerializeField]
		private AudioClip _attackSound;

		[SerializeField]
		private Vector2 _pitchRandomization;

		private Scp939Model _model;
		private Scp939MovementModule _fpc;
		private Scp939FocusAbility _focusAbility;
		private Scp939LungeAbility _lungeAbility;
		private Scp939ClawAbility _clawAbility;
		private Scp939AmnesticCloudAbility _cloudAbility;

		public override float CamFOV => 36f;
		protected override void Start()
		{
			base.Start();

			Scp939Role ScpRole = Role as Scp939Role;
			SubroutineManagerModule subroutineModule = ScpRole.SubroutineModule;
			subroutineModule.TryGetSubroutine(out _focusAbility);
			subroutineModule.TryGetSubroutine(out _lungeAbility);
			subroutineModule.TryGetSubroutine(out _clawAbility);
			subroutineModule.TryGetSubroutine(out _cloudAbility);
			
			IFpcRole fpcRole = Role as IFpcRole;
			_fpc = fpcRole.FpcModule as Scp939MovementModule;
			if (_fpc != null && _fpc.CharacterModelInstance is Scp939Model model)
				_model = model;

			if (_lungeAbility != null)
				_lungeAbility.OnStateChanged += OnLungeStateChanged;
			if (_clawAbility != null)
				_clawAbility.OnTriggered += OnAttackTriggered;

			if (_lungeAbility != null && _lungeAbility.State != Scp939LungeState.None)
			{
				Anim.SetInteger(LungeStateHash, (int)_lungeAbility.State);
				Anim.SetTrigger(LungeTriggerHash);
			}

			if (!Owner.isLocalPlayer)
			{
				UpdateAnimations();
				SkipAnimations(1f, 3);
			}
		}

		protected override void PrimeAnimatorForAlignment()
		{
			if (Anim == null)
				return;

			Anim.Update(0f);
			Anim.Update(2f);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (_lungeAbility != null)
				_lungeAbility.OnStateChanged -= OnLungeStateChanged;
			if (_clawAbility != null)
				_clawAbility.OnTriggered -= OnAttackTriggered;
		}
		private void OnLungeStateChanged(Scp939LungeState state)
		{
			if (state == Scp939LungeState.None)
				return;

			Anim.SetInteger(LungeStateHash, (int)state);
			Anim.SetTrigger(LungeTriggerHash);
		}

		private void OnAttackTriggered()
		{
			Anim.SetTrigger(ClawAttackHash);

			AudioSource source = AudioSourcePoolManager.PlaySound(_attackSound, base.transform, 15f);
			if (source != null)
				source.pitch = UnityEngine.Random.Range(_pitchRandomization.x, _pitchRandomization.y);
		}

		private void SetCloudLayer(float weight, bool charging)
		{
			Anim.SetBool(CloudHash, charging);

			weight += (charging ? 1f : -1f) * Time.deltaTime * CloudTransitionSpeed;
			Anim.SetLayerWeight(CloudLayer, Mathf.Clamp(weight, 0f, CloudMaxWeight));
		}

		protected override void UpdateAnimations()
		{
			if (Anim == null)
				return;

			float cloudWeight = Anim.GetLayerWeight(CloudLayer);
			bool charging = _cloudAbility != null && _cloudAbility._targetState;
			Anim.SetBool(CloudHash, charging);

			cloudWeight += (charging ? 1f : -1f) * Time.deltaTime * CloudTransitionSpeed;
			Anim.SetLayerWeight(CloudLayer, Mathf.Clamp(cloudWeight, 0f, CloudMaxWeight));

			if (_model != null && _model.Animator != null)
			{
				AnimatorStateInfo stateInfo = _model.Animator.GetCurrentAnimatorStateInfo(ModelLayerWalk);
				float walkCycle = stateInfo.normalizedTime;
				if (float.IsNaN(walkCycle))
					walkCycle = 0f;

				Anim.SetFloat(WalkCycleHash, walkCycle);
			}

			float speed = 0f;
			if (_fpc != null && _fpc.IsGrounded)
			{
				speed = Misc.MagnitudeIgnoreY(_fpc.Motor.Velocity);
			}
			if (float.IsNaN(speed))
				speed = 0f;

			Anim.SetFloat(WalkBlendHash, speed, _dampTimeBlend, Time.deltaTime);

			Anim.SetBool(JumpingHash, _fpc != null && !_fpc.IsGrounded);

			bool focusActive = false;
			if (_focusAbility != null)
			{

				bool focusStateActive = _focusAbility._targetState;
				bool lungeCharging = _lungeAbility != null && _lungeAbility.State == Scp939LungeState.Triggered;
				bool focusProgress = _focusAbility._state > MinFocusStateToDisplay;
				focusActive = focusStateActive || lungeCharging || focusProgress;
			}
			Anim.SetBool(FocusActiveHash, focusActive);

			float lungeLayerWeight = Anim.GetLayerWeight(JumpLayer);
			float targetWeight = 0f;

			if (_clawAbility != null && _clawAbility.Cooldown.IsReady
				&& _focusAbility != null && !_focusAbility._targetState)
				targetWeight = 1f;
			if (Role != null && Role._activeTime != null)
			{
				float elapsed = (float)Role._activeTime.Elapsed.TotalSeconds;
				targetWeight = Mathf.Min(targetWeight, Mathf.Max(elapsed - JumpOverLifetime, 0f));
			}

			lungeLayerWeight = Mathf.MoveTowards(
				lungeLayerWeight,
				targetWeight,
				Time.deltaTime * JumpLayerAdjustmentSpeed
			);
			Anim.SetLayerWeight(JumpLayer, lungeLayerWeight);
		}
	}
}
