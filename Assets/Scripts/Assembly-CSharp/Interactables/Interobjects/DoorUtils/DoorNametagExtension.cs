using UnityEngine;

namespace Interactables.Interobjects.DoorUtils
{
	public class DoorNametagExtension : DoorVariantExtension
	{
        public static readonly global::System.Collections.Generic.Dictionary<string, global::Interactables.Interobjects.DoorUtils.DoorNametagExtension> NamedDoors = new global::System.Collections.Generic.Dictionary<string, global::Interactables.Interobjects.DoorUtils.DoorNametagExtension>();

        [SerializeField]
		private string _nametag;

		private BreakableDoor _breakableDoor;

		private bool _previousState;

		private bool _breakable;

		private bool _previousBreak;

		private ushort _previousLocks;

		public string GetName => _nametag;

        private void Start()
        {
            UpdateName(_nametag);
        }

        private void FixedUpdate()
		{
			DoorVariant targetDoor = TargetDoor;
			bool targetState = targetDoor.TargetState;
			if (targetState == _previousState)
			{
				ushort previousLocks = _previousLocks;
				if (targetDoor.ActiveLocks == previousLocks)
				{
					if (!_breakable)
					{
						return;
					}
					BreakableDoor breakableDoor = _breakableDoor;
					bool previousBreak = _previousBreak;
					if (breakableDoor.IsDestroyed == previousBreak)
					{
						return;
					}
				}
			}
			_previousState = targetState;
			ushort activeLocks = targetDoor.ActiveLocks;
			_previousLocks = activeLocks;
			if (_breakable)
			{
				bool destroyed = _breakableDoor.IsDestroyed;
				_previousBreak = destroyed;
			}
		}

        public void UpdateName(string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                global::UnityEngine.Debug.LogError("Nametag of " + base.transform.parent.name + "/" + base.name + " has not been set");
            }
            else
            {
                _nametag = newName;
                NamedDoors[newName] = this;
            }
        }
    }
}
