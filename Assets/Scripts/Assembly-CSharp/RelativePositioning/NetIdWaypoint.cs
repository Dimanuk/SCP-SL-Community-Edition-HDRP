using MapGeneration;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RelativePositioning
{
    public class NetIdWaypoint : WaypointBase
    {
        [SerializeField]
        private NetworkIdentity _targetNetId;

        private Vector3 _pos;

        private const byte Offset = 32;

        private static readonly HashSet<NetIdWaypoint> AllNetWaypoints = new HashSet<NetIdWaypoint>();
        private static bool _refreshNextFrame;

        private static bool _eventSet;

        protected override void Start()
        {
            base.Start();
            SetPosition();
            AllNetWaypoints.Add(this);
            _refreshNextFrame = true;
            if (!_eventSet)
            {
                SeedSynchronizer.OnMapGenerated += () =>
                {
                    _refreshNextFrame = true;
                };
                _eventSet = true;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            AllNetWaypoints.Remove(this);
        }

        protected override float SqrDistanceTo(Vector3 pos)
        {
            return (pos - _pos).sqrMagnitude;
        }

        public override Vector3 GetWorldspacePosition(Vector3 relPosition)
        {
            return relPosition + _pos;
        }

        public override Vector3 GetRelativePosition(Vector3 worldPoint)
        {
            return worldPoint - _pos;
        }

        private void Update()
        {
            if (!_refreshNextFrame)
            {
                base.enabled = false;
                return;
            }
            byte b = Offset;
            foreach (NetIdWaypoint item in Enumerable.OrderBy(AllNetWaypoints, (NetIdWaypoint x) => x._targetNetId.netId))
            {
                item.SetPosition();
                item.SetId(b);
                b++;
            }
            _refreshNextFrame = false;
        }

        private void Reset()
        {
            _targetNetId = GetComponent<NetworkIdentity>();
        }

        private void SetPosition()
        {
            _pos = _targetNetId.transform.position;
        }
    }
}
