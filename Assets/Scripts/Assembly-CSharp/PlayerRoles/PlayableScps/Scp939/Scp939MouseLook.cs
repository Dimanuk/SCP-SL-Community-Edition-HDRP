using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939
{
	public class Scp939MouseLook : FpcMouseLook
	{
		private float _angleBank;

		private readonly Scp939FocusAbility _focus;

		private readonly Scp939LungeAbility _lunge;

		private readonly Scp939MovementModule _fpc939;

		private readonly bool _isSet;

		private const float FocusAngleLimit = 70f;

		private const float SlowdownAngle = 30f;

		private const float LungeAnglePerSecond = 85f;

		private const float LungeStartAngle = 5f;

		private const float LungeAngleMax = 70f;

		private bool Lunging => _lunge.State == Scp939LungeState.Triggered;

		public Scp939MouseLook(ReferenceHub hub, Scp939MovementModule mm)
			: base(hub, mm)
		{
			SubroutineManagerModule subroutineModule = (hub.roleManager.CurrentRole as Scp939Role).SubroutineModule;
			_isSet = subroutineModule.TryGetSubroutine<Scp939FocusAbility>(out _focus) && subroutineModule.TryGetSubroutine<Scp939LungeAbility>(out _lunge);
			_lunge.OnStateChanged += delegate
			{
				_angleBank = LungeStartAngle;
			};
			_fpc939 = mm;
		}

		protected override float ProcessHorizontalInput(float f)
		{
			if (_fpc939.Noclip.IsActive)
			{
				return base.ProcessHorizontalInput(f);
			}
			if (Lunging)
			{
				_angleBank = Mathf.Min(LungeAngleMax, _angleBank + Time.deltaTime * LungeAnglePerSecond);
				f = Mathf.Sign(f) * Mathf.Min(Mathf.Abs(f), _angleBank);
				_angleBank -= Mathf.Abs(f);
				return f;
			}
			if (_focus.TargetState)
			{
				float num = Mathf.DeltaAngle(CurrentHorizontal, _focus.FrozenRotation);
				float num2 = Mathf.Max(0f, Mathf.Abs(num) - SlowdownAngle);
				if (Mathf.Sign(num) != Mathf.Sign(f))
				{
					f *= 1f - num2 / (FocusAngleLimit - SlowdownAngle);
				}
			}
			return f;
		}

		protected override float ClampHorizontal(float f)
		{
			f = base.ClampHorizontal(f);
			if (!_isSet)
			{
				return f;
			}
			if (_focus.TargetState && !Lunging)
			{
				float num = Mathf.DeltaAngle(f, _focus.FrozenRotation);
				f += Mathf.Sign(num) * Mathf.Max(0f, Mathf.Abs(num) - FocusAngleLimit);
			}
			return f;
		}
	}
}
