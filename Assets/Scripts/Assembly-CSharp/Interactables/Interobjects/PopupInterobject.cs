
using Interactables.Verification;
using UnityEngine;

namespace Interactables.Interobjects
{
	public abstract class PopupInterobject : MonoBehaviour, IClientInteractable, IInteractable
	{
		protected enum PopupState : byte
		{
			Off = 0,
			Enabling = 1,
			On = 2,
			Disabling = 3
		}

		public float FadeSpeedSeconds = 0.2f;

		private static PopupInterobject _masterInstance;

		public float CloseAutomaticallyDistance = 5f;

        protected static Vector4 TrackedPosition;

		private static PopupInterobject _currentInstance;

		private static float _timer;

		private static PopupState _currentState;

        protected static PopupState CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                if (_currentInstance != null)
                    _currentInstance.OnClientStateChange();
            }
        }

        public IVerificationRule VerificationRule => StandardDistanceVerification.Default;

        public void ClientInteract(InteractableCollider colliderId)
        {
            if (CurrentState == PopupState.Off)
            {
                _currentInstance = this;
                CurrentState = PopupState.Enabling;
                _timer = FadeSpeedSeconds;
            }
            else if (CurrentState == PopupState.On)
            {
                CurrentState = PopupState.Disabling;
                _timer = FadeSpeedSeconds;
            }
        }

        private void Update()
        {
            if (CurrentState == PopupState.Enabling || CurrentState == PopupState.Disabling)
            {
                _timer -= Time.deltaTime;
                float t = Mathf.Clamp01(_timer / FadeSpeedSeconds);
                float enableRatio = (CurrentState == PopupState.Enabling) ? (1f - t) : t;
                OnClientUpdate(enableRatio);

                if (_timer <= 0f)
                {
                    CurrentState = CurrentState == PopupState.Enabling ? PopupState.On : PopupState.Off;
                }
            }
            else if (CurrentState == PopupState.On)
            {
                if (!IsInRange())
                {
                    CurrentState = PopupState.Disabling;
                    _timer = FadeSpeedSeconds;
                }
            }
        }

        private static bool IsInRange()
        {
            if (ReferenceHub.LocalHub == null)
                return false;

            Vector3 playerPos = ReferenceHub.LocalHub.transform.position;
            float distance = Vector3.Distance(playerPos, TrackedPosition);

            return distance <= _currentInstance.CloseAutomaticallyDistance;
        }

        protected abstract void OnClientStateChange();

        protected abstract void OnClientUpdate(float enableRatio);
    }
}
