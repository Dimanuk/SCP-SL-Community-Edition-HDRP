using GameObjectPools;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp173
{
    public class Scp173BlinkTimer : SubroutineBase, IPoolResettable
    {
        private const float CooldownBaseline = 3f;
        private const float CooldownPerObserver = 0f;
        private const float BreakneckCooldownMultiplier = 0.5f;
        private const float SustainTime = 3f;

        private Scp173ObserversTracker _observers;
        private Scp173MovementModule _fpcModule;
        private Scp173BreakneckSpeedsAbility _breakneckSpeedsAbility;

        private float _totalCooldown;
        private double _initialStopTime;
        private double _endSustainTime;
        private bool _initialized;

        private float TotalCooldown
        {
            get
            {
                if (!NetworkServer.active)
                    return _totalCooldown;

                return TotalCooldownServer;
            }
        }

        private float TotalCooldownServer => _totalCooldown * (_breakneckSpeedsAbility != null && _breakneckSpeedsAbility.IsActive ? BreakneckCooldownMultiplier : 1f);

        private float RemainingSustain => (float)(_endSustainTime - NetworkTime.time);

        public float RemainingBlinkCooldown => Mathf.Max(0f, (float)(_initialStopTime + (double)TotalCooldown - NetworkTime.time));

        public float RemainingSustainPercent
        {
            get
            {
                if (_endSustainTime != -1.0)
                    return Mathf.Clamp(RemainingSustain, 0f, SustainTime) / SustainTime;

                return 1f;
            }
        }

        public bool AbilityReady => RemainingSustainPercent > 0f && RemainingBlinkCooldown <= 0f;

        protected override void Awake()
        {
            base.Awake();
            TryInitialize();
        }

        private void Start()
        {
            TryInitialize();
        }

        private void TryInitialize()
        {
            if (_initialized)
                return;

            if (base.Role is not Scp173Role scp173Role)
                return;

            _fpcModule = scp173Role.FpcModule as Scp173MovementModule;

            if (scp173Role.SubroutineModule == null)
                return;

            if (scp173Role.SubroutineModule.TryGetSubroutine(out _breakneckSpeedsAbility))
            {
                _breakneckSpeedsAbility.OnToggled += OnBreakneckToggled;
            }

            if (scp173Role.SubroutineModule.TryGetSubroutine(out _observers))
            {
                _observers.OnObserversChanged += OnObserversChanged;
                _initialized = true;
            }
        }

        private void OnBreakneckToggled() => ServerSendRpc(toAll: true);

        private void OnDestroy()
        {
            if (_observers != null)
                _observers.OnObserversChanged -= OnObserversChanged;

            if (_breakneckSpeedsAbility != null)
                _breakneckSpeedsAbility.OnToggled -= OnBreakneckToggled;
        }

        private void OnObserversChanged(int prev, int current)
        {
            if (!NetworkServer.active)
                return;

            if (prev == 0 && RemainingSustainPercent == 0f)
            {
                _initialStopTime = NetworkTime.time;
                _totalCooldown = CooldownBaseline;
            }

            _totalCooldown += CooldownPerObserver * (current - prev);

            _endSustainTime = (current > 0) ? -1.0 : (NetworkTime.time + SustainTime);

            ServerSendRpc(toAll: true);
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            NetworkWriterExtensions.WriteDouble(writer, _initialStopTime);
            NetworkWriterExtensions.WriteDouble(writer, _endSustainTime);
            NetworkWriterExtensions.WriteFloat(writer, TotalCooldownServer);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);

            if (!NetworkServer.active)
            {
                _initialStopTime = NetworkReaderExtensions.ReadDouble(reader);
                _endSustainTime = NetworkReaderExtensions.ReadDouble(reader);
                _totalCooldown = NetworkReaderExtensions.ReadFloat(reader);
            }
        }

        public void ResetObject()
        {
            _totalCooldown = 0f;
            _initialStopTime = 0.0;
            _endSustainTime = 0.0;
        }

        public void ServerBlink(Vector3 pos)
        {
            if (_observers == null || _fpcModule == null)
                return;

            int currentObservers = _observers.CurrentObservers;

            _fpcModule.Position = pos;

            if (_fpcModule.CharController != null)
                _fpcModule.CharController.SimpleMove(Vector3.zero);

            _fpcModule.ServerOverridePosition(_fpcModule.transform.position, Vector3.zero);

            _initialStopTime = NetworkTime.time;

            _observers.UpdateObservers();

            if (currentObservers == _observers.CurrentObservers)
                ServerSendRpc(toAll: true);
        }
    }
}
