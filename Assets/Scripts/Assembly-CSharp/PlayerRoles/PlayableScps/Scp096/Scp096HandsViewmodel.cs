using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.HUDs;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp096
{
	public class Scp096HandsViewmodel : ScpViewmodelBase
	{
		[SerializeField]
		private float _fieldOfView;

		[SerializeField]
		private float _dampTime;

		[SerializeField]
		private float _weightAdjustSpeed;

		private bool _useEnragedLayer;

		private FirstPersonMovementModule _fpc;

		private Scp096AttackAbility _attackAbility;

		private Scp096StateController _stateController;

		private const int EnrageLayer = 1;

		private static readonly int HashWalk = Animator.StringToHash("Walk");
		private static readonly int HashExitRage = Animator.StringToHash("RageExit");
		private static readonly int HashEnterRage = Animator.StringToHash("RageEnter");
		private static readonly int HashPryGate = Animator.StringToHash("PryGate");
		private static readonly int HashLeftAttack = Animator.StringToHash("LeftAttack");
		private static readonly int HashAttackTrigger = Animator.StringToHash("Attack");
		private static readonly int HashTryNotToCry = Animator.StringToHash("TryNotToCry");

		public override float CamFOV => _fieldOfView;

		protected override void Start()
		{
			base.Start();

			if (Role is Scp096Role scp096Role)
			{
				_fpc = scp096Role.FpcModule;
				_stateController = scp096Role.StateController;
				scp096Role.SubroutineModule.TryGetSubroutine(out _attackAbility);
			}
			if (_stateController != null)
			{
				_stateController.OnRageUpdate += OnRageUpdate;
				_stateController.OnAbilityUpdate += OnAbilityUpdate;
			}

			if (_attackAbility != null)
			{
				_attackAbility.OnHitReceived += OnHitReceived;
				_attackAbility.OnAttackTriggered += OnAttackTriggered;
			}

			if (_stateController != null)
			{
				switch (_stateController.RageState)
				{
					case Scp096RageState.Distressed:
						Anim.SetTrigger(HashEnterRage);
						_useEnragedLayer = true;
						break;

					case Scp096RageState.Enraged:
						_useEnragedLayer = true;
						break;

					case Scp096RageState.Calming:
						Anim.SetTrigger(HashExitRage);
						_useEnragedLayer = false;
						break;

					default:
						_useEnragedLayer = false;
						break;
				}
			}

			UpdateLayerWeight(1f);
			LateUpdate();

			// Only non-local (spectated) viewmodels fast-forward their animations to
			// the current state's elapsed time so they don't replay from the beginning.
			if (Owner.isLocalPlayer || _stateController == null)
				return;

			if (_stateController.RageState == Scp096RageState.Enraged)
			{
				SkipAnimations(_stateController.LastRageUpdate, 5);
			}
			else
			{
				SkipAnimations(_stateController.LastRageUpdate, 3);
			}

			UpdateWalk(instant: true);

			if (_stateController.AbilityState == Scp096AbilityState.None)
				return;

			if (_stateController.AbilityState == Scp096AbilityState.PryingGate)
			{
				Anim.SetTrigger(HashPryGate);
			}

			SkipAnimations(_stateController.LastAbilityUpdate, 3);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (_stateController != null)
			{
				_stateController.OnRageUpdate -= OnRageUpdate;
				_stateController.OnAbilityUpdate -= OnAbilityUpdate;
			}

			if (_attackAbility != null)
			{
				_attackAbility.OnHitReceived -= OnHitReceived;
				_attackAbility.OnAttackTriggered -= OnAttackTriggered;
			}
		}

		private void OnAbilityUpdate(Scp096AbilityState newState)
		{
			if ((int)newState == 4)
			{
				Anim.SetTrigger(HashPryGate);
			}
		}

		private void OnRageUpdate(Scp096RageState newState)
		{
			switch ((int)newState)
			{
				case 0:
					_useEnragedLayer = false;
					return;

				case 1: 
					Anim.SetTrigger(HashEnterRage);
					_useEnragedLayer = true;
					return;

				case 2:
					_useEnragedLayer = true;
					return;

				case 3: 
					Anim.SetTrigger(HashExitRage);
					_useEnragedLayer = false;
					return;

				default:
					_useEnragedLayer = false;
					return;
			}
		}

		private void OnAttackTriggered()
		{
			if (!Owner.isLocalPlayer)
				return;

			Anim.SetBool(HashLeftAttack, !_attackAbility.LeftAttack);
			Anim.SetTrigger(HashAttackTrigger);
		}

		private void OnHitReceived(Scp096HitResult hit)
		{
			if (Owner.isLocalPlayer)
				return;

			Anim.SetBool(HashLeftAttack, _attackAbility.LeftAttack);
			Anim.SetTrigger(HashAttackTrigger);
		}

		private void UpdateLayerWeight(float maxDelta)
		{
			if (Anim == null)
				return;

			float currentWeight = Anim.GetLayerWeight(EnrageLayer);
			float targetWeight = _useEnragedLayer ? 1f : 0f;
			float newWeight = Mathf.MoveTowards(currentWeight, targetWeight, maxDelta);

			Anim.SetLayerWeight(EnrageLayer, newWeight);
		}

		private void UpdateWalk(bool instant)
		{
			if (Anim == null || _stateController == null)
				return;

			float value;

			if ((int)_stateController.RageState != 2)
			{
				Vector3 velocity = FpcExtensionMethods.GetVelocity(Owner);
				value = velocity.magnitude / _fpc.SprintSpeed;
			}
			else
			{
				value = (int)_stateController.AbilityState == 3 ? 1f : 0f;
			}

			if (instant)
			{
				Anim.SetFloat(HashWalk, value);
			}
			else
			{
				Anim.SetFloat(HashWalk, value, _dampTime, Time.deltaTime);
			}
		}

		protected override void UpdateAnimations()
		{
			UpdateLayerWeight(Time.deltaTime * _weightAdjustSpeed);

			if (_stateController != null)
			{
				Anim.SetBool(HashTryNotToCry, (int)_stateController.AbilityState == 1);
			}

			UpdateWalk(false);
		}
	}
}