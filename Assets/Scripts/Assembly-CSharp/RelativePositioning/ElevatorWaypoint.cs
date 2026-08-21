namespace RelativePositioning
{
    public class ElevatorWaypoint : global::RelativePositioning.WaypointBase
    {
        [global::UnityEngine.SerializeField]
        private global::Interactables.Interobjects.ElevatorChamber _elevator;

        private global::UnityEngine.Transform _transform;

        private bool _transformSet;

        private global::UnityEngine.Transform ElevatorTransform
        {
            get
            {
                if (!_transformSet)
                {
                    _transform = _elevator.transform;
                    _transformSet = true;
                }
                return _transform;
            }
        }

        protected override void Start()
        {
            base.Start();
            SetId((byte)(_elevator.AssignedGroup + 1));
        }

        protected override float SqrDistanceTo(global::UnityEngine.Vector3 pos)
        {
            if (!_elevator.WorldspaceBounds.Contains(pos))
            {
                return float.MaxValue;
            }
            return -1f;
        }

        public override global::UnityEngine.Vector3 GetWorldspacePosition(global::UnityEngine.Vector3 relPosition)
        {
            return ElevatorTransform.TransformPoint(relPosition);
        }

        public override global::UnityEngine.Vector3 GetRelativePosition(global::UnityEngine.Vector3 worldPoint)
        {
            return ElevatorTransform.InverseTransformPoint(worldPoint);
        }

        public override global::UnityEngine.Quaternion GetWorldspaceRotation(global::UnityEngine.Quaternion relRotation)
        {
            return ElevatorTransform.rotation * relRotation;
        }

        public override global::UnityEngine.Quaternion GetRelativeRotation(global::UnityEngine.Quaternion worldRot)
        {
            return global::UnityEngine.Quaternion.Inverse(ElevatorTransform.rotation) * worldRot;
        }
    }
}
