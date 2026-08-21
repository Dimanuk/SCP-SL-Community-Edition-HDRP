using System;
using System.Collections.Generic;
using Interactables.Interobjects.DoorUtils;
using UnityEngine;

namespace Interactables.Interobjects
{
	public class ElevatorDoor : BasicDoor, INonInteractableDoor
	{
        public static readonly global::System.Collections.Generic.Dictionary<global::Interactables.Interobjects.ElevatorManager.ElevatorGroup, global::System.Collections.Generic.List<global::Interactables.Interobjects.ElevatorDoor>> AllElevatorDoors = new global::System.Collections.Generic.Dictionary<global::Interactables.Interobjects.ElevatorManager.ElevatorGroup, global::System.Collections.Generic.List<global::Interactables.Interobjects.ElevatorDoor>>();

        public static Action<ElevatorManager.ElevatorGroup, ElevatorDoor> OnPairsChanged;

		public static Action<ElevatorManager.ElevatorGroup, ElevatorDoor> OnLocksChanged;

		public ElevatorPanel TargetPanel;

		[SerializeField]
		private ElevatorManager.ElevatorGroup _group;

		[SerializeField]
		private Vector3 _targetPosition;

		[SerializeField]
		private Vector3 _topPosition;

		[SerializeField]
		private Vector3 _bottomPosition;

        public global::UnityEngine.Vector3 TargetPosition => base.transform.TransformPoint(_targetPosition);

        public global::UnityEngine.Vector3 TopPosition => base.transform.TransformPoint(_topPosition);

        public global::UnityEngine.Vector3 BottomPosition => base.transform.TransformPoint(_bottomPosition);

        public ElevatorManager.ElevatorGroup Group => _group;

		public bool IgnoreLockdowns => true;

		public bool IgnoreRemoteAdmin => true;

		public override bool AllowInteracting(ReferenceHub ply, byte colliderId)
		{
			return 0 != 0;
		}

        protected override void Start()
        {
            base.Start();
            if (!AllElevatorDoors.TryGetValue(_group, out var value) || value == null)
            {
                AllElevatorDoors[_group] = new global::System.Collections.Generic.List<global::Interactables.Interobjects.ElevatorDoor> { this };
                return;
            }
            bool flag = false;
            float y = TargetPosition.y;
            for (int i = 0; i < value.Count; i++)
            {
                if (!(y >= value[i].TargetPosition.y))
                {
                    value.Insert(i, this);
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                value.Add(this);
            }
            OnPairsChanged?.Invoke(_group, this);
        }

        protected override void LockChanged(ushort prevValue)
		{
			base.LockChanged(prevValue);
			Action<ElevatorManager.ElevatorGroup, ElevatorDoor> onLocksChanged = OnLocksChanged;
			if (onLocksChanged != null)
			{
				ElevatorManager.ElevatorGroup arg = _group;
				onLocksChanged(arg, this);
			}
		}

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (AllElevatorDoors.TryGetValue(_group, out var value))
            {
                value.Remove(this);
            }
        }
	}
}
