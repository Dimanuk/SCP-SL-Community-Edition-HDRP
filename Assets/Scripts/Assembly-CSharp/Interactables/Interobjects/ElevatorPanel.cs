
using Interactables.Verification;
using Mirror;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Interactables.Interobjects
{
	public class ElevatorPanel : InteractableCollider, IClientInteractable, IInteractable
	{
		[SerializeField]
		private Material[] _levelMats;

		[SerializeField]
		private Material _movingUpMat;

		[SerializeField]
		private Material _movingDownMat;

		[SerializeField]
		private Material _disabledMat;

		[SerializeField]
		private Renderer _targetRenderer;

        public ElevatorChamber AssignedChamber { get; private set; }

        public IVerificationRule VerificationRule => StandardDistanceVerification.Default;

        public void ClientInteract(InteractableCollider _)
        {
            if (AssignedChamber == null)
                return;

            if (ElevatorDoor.AllElevatorDoors.TryGetValue(AssignedChamber.AssignedGroup, out var doors))
            {
                int nextLevel = AssignedChamber.CurrentLevel + 1;
                if (nextLevel >= doors.Count)
                    nextLevel = 0;

                NetworkClient.Send(new ElevatorManager.ElevatorSyncMsg(AssignedChamber.AssignedGroup, nextLevel));
            }
        }

        public void SetChamber(ElevatorChamber chamber)
        {
            AssignedChamber = chamber;
        }

        public void SetLevel(int level)
        {
            if (_targetRenderer != null && _levelMats != null && level >= 0 && level < _levelMats.Length)
            {
                _targetRenderer.material = _levelMats[level];
            }
        }

        public void SetLocked()
        {
            if (_targetRenderer != null && _disabledMat != null)
            {
                _targetRenderer.material = _disabledMat;
            }
        }

        public void SetMoving(bool up)
        {
            if (_targetRenderer != null)
            {
                _targetRenderer.material = up ? _movingUpMat : _movingDownMat;
            }
        }
    }
}
