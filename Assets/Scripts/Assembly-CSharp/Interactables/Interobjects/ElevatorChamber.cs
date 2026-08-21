using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Interactables.Interobjects.DoorUtils;
using UnityEngine;

namespace Interactables.Interobjects
{
	public class ElevatorChamber : MonoBehaviour
	{
		public delegate void ElevatorMoved(Bounds elevatorBounds, ElevatorChamber chamber, Vector3 deltaPos, Quaternion deltaRot);

		private enum ElevatorSequence
		{
			DoorClosing = 0,
			MovingAway = 1,
			Arriving = 2,
			DoorOpening = 3,
			Ready = 4
		}


		public List<ElevatorPanel> AllPanels = new List<ElevatorPanel>();

		private static readonly int DoorAnimHash = global::UnityEngine.Animator.StringToHash("isOpen");

        private const float RotationGrowth = 1.8f;

		private ElevatorDoor _lastDestination;

		private bool _goingUp;

		private float _percentOfRotation;

        private global::Interactables.Interobjects.ElevatorChamber.ElevatorSequence _curSequence = global::Interactables.Interobjects.ElevatorChamber.ElevatorSequence.Ready;

        private global::Interactables.Interobjects.ElevatorManager.ElevatorGroup _assignedGroup;

        private global::UnityEngine.Bounds _cachedBounds;

        private bool _cachedBoundsUpToDate;

		private bool _eventAssigned;

		private readonly Stopwatch _seqTimer = Stopwatch.StartNew();

		[SerializeField]
		private Animator _doorAnimator;

		[SerializeField]
		private Vector3 _boundsCenter;

		[SerializeField]
		private float _boundsSize;

		[SerializeField]
		private float _boundsHeight;

		[SerializeField]
		private float _doorOpenTime;

		[SerializeField]
		private float _doorCloseTime;

		[SerializeField]
		private float _animationTime;

		[SerializeField]
		private float _rotationTime;

		[SerializeField]
		private AnimationCurve _translationCurve;

		[SerializeField]
		private AnimationCurve _rotationCurve;

        [global::UnityEngine.SerializeField]
        private global::System.Collections.Generic.List<global::UnityEngine.AudioSource> _travelSounds;

        public global::Interactables.Interobjects.ElevatorDoor CurrentDestination { get; private set; }

        public int CurrentLevel { get; private set; }

        public global::Interactables.Interobjects.ElevatorManager.ElevatorGroup AssignedGroup
        {
            get
            {
                return _assignedGroup;
            }
            set
            {
                _assignedGroup = value;
                foreach (global::Interactables.Interobjects.ElevatorPanel allPanel in AllPanels)
                {
                    allPanel.SetChamber(this);
                }
                global::Interactables.Interobjects.ElevatorDoor.OnPairsChanged = (global::System.Action<global::Interactables.Interobjects.ElevatorManager.ElevatorGroup, global::Interactables.Interobjects.ElevatorDoor>)global::System.Delegate.Combine(global::Interactables.Interobjects.ElevatorDoor.OnPairsChanged, new global::System.Action<global::Interactables.Interobjects.ElevatorManager.ElevatorGroup, global::Interactables.Interobjects.ElevatorDoor>(AddNewPanel));
                _eventAssigned = true;
                if (!global::Interactables.Interobjects.ElevatorDoor.AllElevatorDoors.TryGetValue(value, out var value2))
                {
                    return;
                }
                foreach (global::Interactables.Interobjects.ElevatorDoor item in value2)
                {
                    AddNewPanel(value, item);
                }
            }
        }

        public DoorLockReason ActiveLocks { get; private set; }

		public bool IsReady => _curSequence == ElevatorSequence.Ready;

        public global::UnityEngine.Bounds WorldspaceBounds
        {
            get
            {
                if (!_cachedBoundsUpToDate)
                {
                    float num = _boundsSize * (_percentOfRotation * 1.8f + 1f);
                    _cachedBounds = new global::UnityEngine.Bounds(base.transform.TransformPoint(_boundsCenter), new global::UnityEngine.Vector3(num, _boundsHeight, num));
                    _cachedBoundsUpToDate = true;
                }
                return _cachedBounds;
            }
        }

		public static event ElevatorMoved OnElevatorMoved;

        public bool TrySetDestination(int targetLevel, bool force = false)
        {
            if (!global::Interactables.Interobjects.ElevatorDoor.AllElevatorDoors.TryGetValue(AssignedGroup, out var value))
            {
                return false;
            }
            if (targetLevel < 0 || targetLevel >= value.Count)
            {
                return false;
            }
            global::Interactables.Interobjects.ElevatorDoor elevatorDoor = value[targetLevel];
            if (!force)
            {
                if (!IsReady)
                {
                    return false;
                }
                if (elevatorDoor == _lastDestination)
                {
                    return false;
                }
            }
            if (_lastDestination != null)
            {
                if (global::Mirror.NetworkServer.active)
                {
                    _lastDestination.TargetState = false;
                }
                _goingUp = _lastDestination.TargetPosition.y < elevatorDoor.TargetPosition.y;
                _travelSounds.ForEach(delegate (global::UnityEngine.AudioSource x)
                {
                    x.Play();
                });
                _curSequence = global::Interactables.Interobjects.ElevatorChamber.ElevatorSequence.DoorClosing;
                _seqTimer.Restart();
                AllPanels.ForEach(delegate (global::Interactables.Interobjects.ElevatorPanel x)
                {
                    x.SetMoving(_goingUp);
                });
                SetInnerDoor(state: false);
            }
            else
            {
                if (global::Mirror.NetworkServer.active)
                {
                    elevatorDoor.TargetState = true;
                }
                base.transform.position = elevatorDoor.TargetPosition;
                base.transform.rotation = elevatorDoor.transform.rotation;
                base.transform.SetParent(elevatorDoor.transform.parent);
                _curSequence = global::Interactables.Interobjects.ElevatorChamber.ElevatorSequence.Ready;
                _lastDestination = elevatorDoor;
                if (ActiveLocks == global::Interactables.Interobjects.DoorUtils.DoorLockReason.None)
                {
                    AllPanels.ForEach(delegate (global::Interactables.Interobjects.ElevatorPanel x)
                    {
                        x.SetLevel(targetLevel);
                    });
                }
                else
                {
                    AllPanels.ForEach(delegate (global::Interactables.Interobjects.ElevatorPanel x)
                    {
                        x.SetLocked();
                    });
                }
                SetInnerDoor(state: true);
            }
            CurrentLevel = targetLevel;
            CurrentDestination = elevatorDoor;
            return true;
        }

        private void SetInnerDoor(bool state)
        {
            _doorAnimator.SetBool(DoorAnimHash, state);
            _cachedBoundsUpToDate = false;
        }

        private void AddNewPanel(global::Interactables.Interobjects.ElevatorManager.ElevatorGroup group, global::Interactables.Interobjects.ElevatorDoor door)
        {
            global::Interactables.Interobjects.ElevatorPanel targetPanel = door.TargetPanel;
            if (!(targetPanel == null) && !AllPanels.Contains(targetPanel))
            {
                targetPanel.SetChamber(this);
                AllPanels.Add(targetPanel);
            }
        }

        private void Update()
        {
            switch (_curSequence)
            {
                case global::Interactables.Interobjects.ElevatorChamber.ElevatorSequence.MovingAway:
                case global::Interactables.Interobjects.ElevatorChamber.ElevatorSequence.Arriving:
                    {
                        global::UnityEngine.Transform transform = base.transform;
                        global::UnityEngine.Bounds worldspaceBounds = WorldspaceBounds;
                        global::UnityEngine.Vector3 position = transform.position;
                        global::UnityEngine.Quaternion rotation = transform.rotation;
                        UpdateMovement(transform, _curSequence == global::Interactables.Interobjects.ElevatorChamber.ElevatorSequence.Arriving);
                        _cachedBoundsUpToDate = false;
                        global::UnityEngine.Vector3 deltaPos = transform.position - position;
                        global::UnityEngine.Quaternion deltaRot = transform.rotation * global::UnityEngine.Quaternion.Inverse(rotation);
                        global::Interactables.Interobjects.ElevatorChamber.OnElevatorMoved?.Invoke(worldspaceBounds, this, deltaPos, deltaRot);
                        break;
                    }
                case global::Interactables.Interobjects.ElevatorChamber.ElevatorSequence.DoorClosing:
                    if (!(_seqTimer.Elapsed.TotalSeconds < (double)_doorCloseTime))
                    {
                        _curSequence = global::Interactables.Interobjects.ElevatorChamber.ElevatorSequence.MovingAway;
                        _seqTimer.Restart();
                    }
                    break;
                case global::Interactables.Interobjects.ElevatorChamber.ElevatorSequence.DoorOpening:
                    if (!(_seqTimer.Elapsed.TotalSeconds < (double)_doorOpenTime))
                    {
                        _curSequence = global::Interactables.Interobjects.ElevatorChamber.ElevatorSequence.Ready;
                    }
                    break;
            }
        }

        private void Awake()
        {
            global::Interactables.Interobjects.ElevatorDoor.OnLocksChanged = (global::System.Action<global::Interactables.Interobjects.ElevatorManager.ElevatorGroup, global::Interactables.Interobjects.ElevatorDoor>)global::System.Delegate.Combine(global::Interactables.Interobjects.ElevatorDoor.OnLocksChanged, new global::System.Action<global::Interactables.Interobjects.ElevatorManager.ElevatorGroup, global::Interactables.Interobjects.ElevatorDoor>(RefreshLocks));
        }

        private void OnDestroy()
        {
            if (_eventAssigned)
            {
                global::Interactables.Interobjects.ElevatorDoor.OnPairsChanged = (global::System.Action<global::Interactables.Interobjects.ElevatorManager.ElevatorGroup, global::Interactables.Interobjects.ElevatorDoor>)global::System.Delegate.Remove(global::Interactables.Interobjects.ElevatorDoor.OnPairsChanged, new global::System.Action<global::Interactables.Interobjects.ElevatorManager.ElevatorGroup, global::Interactables.Interobjects.ElevatorDoor>(AddNewPanel));
                global::Interactables.Interobjects.ElevatorDoor.OnLocksChanged = (global::System.Action<global::Interactables.Interobjects.ElevatorManager.ElevatorGroup, global::Interactables.Interobjects.ElevatorDoor>)global::System.Delegate.Remove(global::Interactables.Interobjects.ElevatorDoor.OnLocksChanged, new global::System.Action<global::Interactables.Interobjects.ElevatorManager.ElevatorGroup, global::Interactables.Interobjects.ElevatorDoor>(RefreshLocks));
            }
        }


        private void RefreshLocks(global::Interactables.Interobjects.ElevatorManager.ElevatorGroup group, global::Interactables.Interobjects.ElevatorDoor elevDoor)
        {
            if (group != AssignedGroup)
            {
                return;
            }
            ActiveLocks = global::Interactables.Interobjects.DoorUtils.DoorLockReason.None;
            if (global::Interactables.Interobjects.ElevatorDoor.AllElevatorDoors.TryGetValue(group, out var value))
            {
                value.ForEach(delegate (global::Interactables.Interobjects.ElevatorDoor x)
                {
                    ActiveLocks |= (global::Interactables.Interobjects.DoorUtils.DoorLockReason)x.ActiveLocks;
                });
            }
            bool flag = IsReady || _curSequence == global::Interactables.Interobjects.ElevatorChamber.ElevatorSequence.DoorOpening;
            if (ActiveLocks == global::Interactables.Interobjects.DoorUtils.DoorLockReason.None)
            {
                if (flag)
                {
                    AllPanels.ForEach(delegate (global::Interactables.Interobjects.ElevatorPanel x)
                    {
                        x.SetLevel(CurrentLevel);
                    });
                }
                else
                {
                    AllPanels.ForEach(delegate (global::Interactables.Interobjects.ElevatorPanel x)
                    {
                        x.SetMoving(_goingUp);
                    });
                }
            }
            else if (flag)
            {
                AllPanels.ForEach(delegate (global::Interactables.Interobjects.ElevatorPanel x)
                {
                    x.SetLocked();
                });
            }
        }
        private void UpdateMovement(global::UnityEngine.Transform t, bool arriving)
        {
            global::Interactables.Interobjects.ElevatorDoor currentDestination = CurrentDestination;
            float num = global::UnityEngine.Mathf.Clamp01((float)_seqTimer.Elapsed.TotalSeconds / _animationTime);
            UpdateRotation(t, currentDestination, num, arriving);
            if (arriving)
            {
                global::UnityEngine.Vector3 a = (_goingUp ? currentDestination.BottomPosition : currentDestination.TopPosition);
                t.position = global::UnityEngine.Vector3.Lerp(a, currentDestination.TargetPosition, _translationCurve.Evaluate(num));
                if (num < 1f)
                {
                    return;
                }
                if (global::Mirror.NetworkServer.active)
                {
                    currentDestination.TargetState = true;
                }
                SetInnerDoor(state: true);
                _lastDestination = currentDestination;
                if (ActiveLocks == global::Interactables.Interobjects.DoorUtils.DoorLockReason.None)
                {
                    AllPanels.ForEach(delegate (global::Interactables.Interobjects.ElevatorPanel x)
                    {
                        x.SetLevel(CurrentLevel);
                    });
                }
                else
                {
                    AllPanels.ForEach(delegate (global::Interactables.Interobjects.ElevatorPanel x)
                    {
                        x.SetLocked();
                    });
                }
                _curSequence = global::Interactables.Interobjects.ElevatorChamber.ElevatorSequence.DoorOpening;
                _seqTimer.Restart();
            }
            else
            {
                global::UnityEngine.Vector3 b = (_goingUp ? _lastDestination.TopPosition : _lastDestination.BottomPosition);
                t.position = global::UnityEngine.Vector3.Lerp(_lastDestination.TargetPosition, b, 1f - _translationCurve.Evaluate(1f - num));
                if (!(num < 1f))
                {
                    t.SetParent(currentDestination.transform.parent);
                    _curSequence = global::Interactables.Interobjects.ElevatorChamber.ElevatorSequence.Arriving;
                    _seqTimer.Restart();
                }
            }
        }

        private void UpdateRotation(global::UnityEngine.Transform t, global::Interactables.Interobjects.ElevatorDoor dest, float f, bool arriving)
        {
            if (arriving)
            {
                f += 1f;
            }
            f = global::UnityEngine.Mathf.InverseLerp(_rotationTime, 2f - _rotationTime, f);
            _percentOfRotation = (arriving ? (1f - f) : f);
            global::UnityEngine.Quaternion rotation = _lastDestination.transform.rotation;
            global::UnityEngine.Quaternion rotation2 = dest.transform.rotation;
            t.rotation = global::UnityEngine.Quaternion.Lerp(rotation, rotation2, _rotationCurve.Evaluate(f));
        }

        private void OnDrawGizmosSelected()
        {
            global::UnityEngine.Gizmos.color = global::UnityEngine.Color.green;
            global::UnityEngine.Gizmos.DrawWireCube(WorldspaceBounds.center, WorldspaceBounds.size);
            global::UnityEngine.Gizmos.color = new global::UnityEngine.Color(0f, 1f, 0f, 0.1f);
            global::UnityEngine.Gizmos.DrawCube(WorldspaceBounds.center, WorldspaceBounds.size);
        }
	}
}
