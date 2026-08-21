using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939
{
	public class Scp939MovementModule : FirstPersonMovementModule
	{
		[SerializeField]
		private AnimationCurve _staminaRegenerationCurve;

		private Scp939Motor _motor939;

		private const float StaminaUseRate = 0.1f;

		private const float StaminaRespawnImmunity = 2f;

		protected override FpcMotor NewMotor => _motor939 = new Scp939Motor(Hub, Role as Scp939Role);

		protected override FpcMouseLook NewMouseLook => new Scp939MouseLook(Hub, this);

		protected override FpcStateProcessor NewStateProcessor => new Scp939StateProcessor(Hub, _motor939, this, 0.1f, 2f, _staminaRegenerationCurve);

		protected override PlayerMovementState ValidateMovementState(PlayerMovementState state)
		{
			state = base.ValidateMovementState(state);
			if (state == PlayerMovementState.Sprinting && !_motor939.MovingForwards)
			{
				state = PlayerMovementState.Walking;
			}
			return state;
		}
	}
}
