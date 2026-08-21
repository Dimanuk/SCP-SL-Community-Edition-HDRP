using Interactables.Interobjects.DoorUtils;
using UnityEngine;

namespace Interactables.Interobjects
{
	public class BasicNonInteractableDoor : BasicDoor, INonInteractableDoor, IScp106PassableDoor
	{
		[SerializeField]
		private bool _ignoreLockdowns;

		[SerializeField]
		private bool _ignoreRemoteAdmin;

		[SerializeField]
		private bool _blockScp106;

		public bool IgnoreLockdowns => _ignoreLockdowns;

		public bool IgnoreRemoteAdmin => _ignoreRemoteAdmin;

		public bool IsScp106Passable => !_blockScp106;
	}
}
