using System;

namespace PlayerRoles.PlayableScps.Subroutines
{
	public abstract class ScpKeySubroutine<T> : ScpStandardSubroutine<T> where T : PlayerRoleBase
	{
		private bool _held;

		protected abstract ActionName TargetKey { get; }

        protected virtual bool IsKeyHeld
        {
            get
            {
                return _held;
            }
            set
            {
                if (value != _held)
                {
                    _held = value;
                    if (value)
                    {
                        OnKeyDown();
                    }
                    else
                    {
                        OnKeyUp();
                    }
                }
            }
        }

        protected virtual bool KeyPressable
        {
            get
            {
                if (base.Owner == null || !base.Owner.isLocalPlayer)
                {
                    return false;
                }
                return !global::UnityEngine.Cursor.visible;
            }
        }

        protected virtual bool KeyReleasable => true;

        protected virtual void Update()
        {
            if (base.Owner == null)
                return;

            if (KeyPressable && global::UnityEngine.Input.GetKey(NewInput.GetKey(TargetKey)))
            {
                IsKeyHeld = true;
            }
            else if (IsKeyHeld && KeyReleasable)
            {
                IsKeyHeld = false;
            }
        }

        protected virtual void OnKeyDown()
		{
			
		}

		protected virtual void OnKeyUp()
		{
			
		}

        public override void ResetObject()
        {
            base.ResetObject();
            _held = false;
        }
	}
}