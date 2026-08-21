using System.Collections.Generic;
using System.Diagnostics;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;
using MapGeneration;

namespace PlayerRoles.PlayableScps.Scp173
{
    public class Scp173ObserversTracker : ScpStandardSubroutine<Scp173Role>
    {
        public delegate void ObserversChanged(int prev, int current);

        public readonly HashSet<ReferenceHub> Observers = new HashSet<ReferenceHub>();

        private const float WidthMultiplier = 0.2f;

        [SerializeField]
        public float _modelWidth;

        [SerializeField]
        private float _maxViewDistance;

        [SerializeField]
        public Vector2[] _visibilityReferencePoints;

        private int _curObservers;
        private int _simulatedTargets;
        private float _simulatedStareTime;
        private readonly Stopwatch _simulatedStareSw = Stopwatch.StartNew();

        public int CurrentObservers
        {
            get => _curObservers;
            private set
            {
                if (value != _curObservers)
                {
                    int prev = _curObservers;
                    _curObservers = value;
                    OnObserversChanged?.Invoke(prev, value);
                }
            }
        }

        public bool IsObserved => CurrentObservers > 0;

        public float SimulatedStare
        {
            get => Mathf.Max(0f, _simulatedStareTime - (float)_simulatedStareSw.Elapsed.TotalSeconds);
            set
            {
                _simulatedStareTime = value;
                _simulatedStareSw.Restart();
            }
        }

        public event ObserversChanged OnObserversChanged;

        private void Update()
        {
            UpdateObservers();
        }

        private void CheckRemovedPlayer(ReferenceHub ply)
        {
            if (NetworkServer.active && Observers.Remove(ply))
            {
                CurrentObservers--;
            }
        }

        private int UpdateObserver(ReferenceHub targetHub)
        {
            if (targetHub == null)
                return 0;

            if (!targetHub.IsHuman())
            {
                if (!Observers.Remove(targetHub))
                    return 0;
                return -1;
            }

            if (IsObservedBy(targetHub, WidthMultiplier))
            {
                if (Observers.Add(targetHub))
                    return 1;
            }
            else if (Observers.Remove(targetHub))
            {
                return -1;
            }

            return 0;
        }

        protected override void Awake()
        {
            base.Awake();
            ReferenceHub.OnPlayerRemoved += CheckRemovedPlayer;
        }

        public bool IsObservedBy(ReferenceHub target, float widthMultiplier = 1f)
        {
            if (target == null || base.ScpRole == null || base.ScpRole.FpcModule == null)
                return false;

            Vector3 position = base.ScpRole.FpcModule.Position;
            RoomIdentifier roomIdentifier = RoomIdUtils.RoomAtPosition(position);

            float distance = _maxViewDistance;
            if (roomIdentifier != null && roomIdentifier.Zone == FacilityZone.Surface)
                distance *= 2f;

            if (!VisionInformation.GetVisionInformation(target, target.PlayerCameraReference, position, _modelWidth, distance, checkFog: false, checkLineOfSight: false).IsLooking)
                return false;

            Vector3 targetPos = target.PlayerCameraReference.position;
            Vector3 right = target.PlayerCameraReference.TransformDirection(Vector3.right);

            if (_visibilityReferencePoints == null)
                return false;

            for (int i = 0; i < _visibilityReferencePoints.Length; i++)
            {
                Vector2 point = _visibilityReferencePoints[i];
                Vector3 origin = position + point.x * widthMultiplier * right + Vector3.up * point.y;

                if (!Physics.Linecast(origin, targetPos, VisionInformation.VisionLayerMask))
                    return true;
            }

            return false;
        }

        public void UpdateObservers()
        {
            if (!NetworkServer.active)
                return;

            if (base.Owner == null || base.ScpRole == null)
                return;

            int num = CurrentObservers;
            int num2 = (SimulatedStare > 0f) ? 1 : 0;

            if (_simulatedTargets != num2)
            {
                num += num2 - _simulatedTargets;
                _simulatedTargets = num2;
            }

            if (ReferenceHub.AllHubs != null)
            {
                foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
                {
                    if (allHub == null)
                        continue;
                    num += UpdateObserver(allHub);
                }
            }

            CurrentObservers = num;

            if (!base.Owner.isLocalPlayer)
            {
                ServerSendRpc(toAll: true);
            }
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            writer.WriteByte((byte)Mathf.Clamp(CurrentObservers, 0, 255));
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            CurrentObservers = reader.ReadByte();
        }

        public override void ResetObject()
        {
            base.ResetObject();
            _curObservers = 0;
            _simulatedTargets = 0;
            _simulatedStareTime = 0f;
            Observers.Clear();
        }
    }
}
