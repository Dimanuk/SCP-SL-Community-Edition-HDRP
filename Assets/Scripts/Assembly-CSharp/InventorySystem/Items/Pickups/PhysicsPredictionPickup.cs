using System.Collections.Generic;
using System.Diagnostics;

using Interactables.Interobjects;
using RelativePositioning;
using UnityEngine;

namespace InventorySystem.Items.Pickups
{
	public class PhysicsPredictionPickup : IPickupPhysicsModule
	{
		private const byte VelocitySmoothing = 6;

		private const byte PositionSmoothing = 3;

		private const byte KinematicSmoothing = 5;

		private const byte AngularSmoothing = 4;

		private const float SupersmoothVelocitySquared = 16f;

		private const byte SmoothingCutoffValue = 5;

		private const float MinimalUpdateTime = 0.03f;

		private const float MaximumUpdateTime = 0.8f;

		private const byte RefreshStepSizeFactor = 15;

        private static readonly List<ItemPickupBase> AllPickups = new List<ItemPickupBase>();

        private static ItemPickupBase _masterModule;

		private static int _refreshTimer;

		private readonly Transform _t;

		private readonly Rigidbody _rb;

		private readonly Stopwatch _lastUpdateStopwatch;

		private readonly ItemPickupBase _targetPickup;

		private RelativePosition _lastReceivedPos;

		private Vector3 _estimatedVelocity;

		private bool _firstEstimation;

		private ElevatorChamber _trackedChamber;

		private bool _inElevator;

        public PhysicsPredictionPickup(global::InventorySystem.Items.Pickups.ItemPickupBase targetPickup, global::UnityEngine.Rigidbody rb)
        {
            _targetPickup = targetPickup;
            _firstEstimation = true;
            _rb = rb;
            _t = rb.transform;
            if (global::Mirror.NetworkServer.active)
            {
                AllPickups.Add(_targetPickup);
            }
            else
            {
                _lastUpdateStopwatch = new global::System.Diagnostics.Stopwatch();
                _lastUpdateStopwatch.Start();
            }
            _rb.collisionDetectionMode = global::UnityEngine.CollisionDetectionMode.ContinuousSpeculative;
            global::Interactables.Interobjects.ElevatorChamber.OnElevatorMoved += OnElevatorMoved;
        }

        public void DestroyModule()
        {
            if (global::Mirror.NetworkServer.active)
            {
                AllPickups.Remove(_targetPickup);
            }
            global::Interactables.Interobjects.ElevatorChamber.OnElevatorMoved -= OnElevatorMoved;
        }


        public void UpdateInfo(global::InventorySystem.Items.Pickups.PickupSyncInfo info)
        {
            if (global::Mirror.NetworkServer.active || info.ItemId == ItemType.None)
            {
                return;
            }
            float num = (float)_lastUpdateStopwatch.Elapsed.TotalSeconds;
            if (!(num > 0.03f))
            {
                return;
            }
            global::UnityEngine.Vector3 position = _lastReceivedPos.Position;
            if (position != global::UnityEngine.Vector3.zero)
            {
                global::UnityEngine.Vector3 vector = info.Position - position;
                if (global::UnityEngine.Mathf.Abs(vector.x) + global::UnityEngine.Mathf.Abs(vector.y) + global::UnityEngine.Mathf.Abs(vector.z) < 5f)
                {
                    _estimatedVelocity = vector / num;
                }
            }
            _lastUpdateStopwatch.Restart();
            _lastReceivedPos = info.RelativePosition;
        }

        public void UpdatePhysics()
        {
            if (global::Mirror.NetworkServer.active)
            {
                if (_rb.linearVelocity.sqrMagnitude > 16f)
                {
                    _targetPickup.RefreshPositionAndRotation();
                }
                if (_masterModule == null)
                {
                    _masterModule = _targetPickup;
                }
                if (_masterModule != _targetPickup)
                {
                    return;
                }
                int count = AllPickups.Count;
                int num = global::UnityEngine.Mathf.FloorToInt((float)count / 15f) + 1;
                if (_refreshTimer >= count)
                {
                    _refreshTimer = 0;
                }
                int num2 = -1;
                for (int i = _refreshTimer; i < _refreshTimer + num; i++)
                {
                    if (i < count)
                    {
                        if (AllPickups[i] != null)
                        {
                            AllPickups[i].RefreshPositionAndRotation();
                        }
                        else
                        {
                            num2 = i;
                        }
                    }
                }
                if (num2 >= 0)
                {
                    AllPickups.RemoveAt(num2);
                }
                _refreshTimer += num;
                return;
            }
            float num3 = global::UnityEngine.Vector3.Distance(_targetPickup.Info.Position, _rb.position);
            _rb.useGravity = _rb.position.y > _targetPickup.Info.Position.y;
            if (num3 < 5f)
            {
                global::System.Diagnostics.Stopwatch lastUpdateStopwatch = _lastUpdateStopwatch;
                bool flag = lastUpdateStopwatch != null && lastUpdateStopwatch.Elapsed.TotalSeconds > 0.800000011920929;
                _rb.isKinematic = flag;
                if (_firstEstimation && _estimatedVelocity != global::UnityEngine.Vector3.zero)
                {
                    if (!flag)
                    {
                        _rb.linearVelocity = _estimatedVelocity;
                    }
                    _rb.position = _targetPickup.Info.Position;
                    _rb.rotation = _targetPickup.Info.Rotation;
                    _firstEstimation = false;
                }
                else
                {
                    if (!flag)
                    {
                        _rb.linearVelocity = global::UnityEngine.Vector3.Lerp(_rb.linearVelocity, _estimatedVelocity, 6f * global::UnityEngine.Time.fixedDeltaTime);
                    }
                    _rb.position = global::UnityEngine.Vector3.Lerp(_rb.position, _targetPickup.Info.Position, (float)(_rb.isKinematic ? 5 : 3) * global::UnityEngine.Time.fixedDeltaTime);
                    _rb.rotation = global::UnityEngine.Quaternion.Lerp(_rb.rotation, _targetPickup.Info.Rotation, 4f * global::UnityEngine.Time.fixedDeltaTime);
                }
            }
            else
            {
                if (!_rb.isKinematic)
                {
                    _rb.linearVelocity = _estimatedVelocity;
                    _rb.angularVelocity = global::UnityEngine.Vector3.zero;
                }
                _rb.rotation = _targetPickup.Info.Rotation;
                _rb.position = _targetPickup.Info.Position;
            }
        }

        private void OnElevatorMoved(global::UnityEngine.Bounds elevatorBounds, global::Interactables.Interobjects.ElevatorChamber chamber, global::UnityEngine.Vector3 deltaPos, global::UnityEngine.Quaternion deltaRot)
        {
            bool flag = _inElevator && chamber == _trackedChamber;
            if (!chamber.WorldspaceBounds.Contains(_t.position))
            {
                if (flag)
                {
                    _inElevator = false;
                    _t.position -= deltaPos;
                    _t.SetParent(null);
                }
            }
            else if (!flag)
            {
                _t.SetParent(chamber.transform);
                _t.position += deltaPos;
                _trackedChamber = chamber;
                _inElevator = true;
            }
        }
    }
}
