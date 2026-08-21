
using UnityEngine;

namespace OperationalGuide
{
	internal class SetAnimatorSpeed : MonoBehaviour
	{
		private static readonly int Speed = Animator.StringToHash("Speed");

		public Animator Animator;

		public float CustomSpeed = 1f;

		private void Start()
		{
			Animator animator = Animator;
			float customSpeed = CustomSpeed;
			int speed = Speed;
			animator.SetFloat(speed, customSpeed);
		}

		private void OnEnable()
		{
			Animator animator = Animator;
			float customSpeed = CustomSpeed;
			int speed = Speed;
			animator.SetFloat(speed, customSpeed);
		}
	}
}
