using System;
using System.Diagnostics;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Spectating;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace PlayerRoles.PlayableScps.Scp173
{
    public class Scp173BreakneckSpeedsAbility : ScpKeySubroutine<Scp173Role>
    {
        private const float RechargeTime = 40f;
        private const float StareLimit   = 10f;
        private const float MinimalTime  = 1f;

        private readonly Stopwatch _duration = new Stopwatch();

        private float _disableTime;
        private Scp173ObserversTracker _observersTracker;

        [SerializeField] private PostProcessVolume _ppVolume;
        [SerializeField] private float _ppLerpSpeed;

        public readonly AbilityCooldown Cooldown = new AbilityCooldown();
        public Action OnToggled;

        private float Elapsed => (float)_duration.Elapsed.TotalSeconds;

        public bool IsActive
        {
            get => _duration.IsRunning;
            private set
            {
                if (value == IsActive)
                    return;

                if (value)
                {
                    _duration.Start();
                    _disableTime = 0f;
                }
                else
                {
                    _duration.Reset();
                }

                if (NetworkServer.active)
                {
                    if (!value)
                        Cooldown.Trigger(RechargeTime);

                    ServerSendRpc(toAll: true);
                    OnToggled?.Invoke();
                }
            }
        }

        protected override ActionName TargetKey => ActionName.Run;

        private void UpdateServerside()
        {
            if (!IsActive)
                return;

            if (_disableTime > 0f)
            {
                if (Elapsed >= _disableTime)
                    IsActive = false;
            }

            else if (_observersTracker != null && _observersTracker.IsObserved)
            {
                _disableTime = Elapsed + StareLimit;
            }
        }

        protected override void OnKeyDown()
        {
            base.OnKeyDown();
            ClientSendCmd();
        }

        protected override void Update()
        {
            base.Update();

            if (Owner == null)
                return;

            if (Owner.isLocalPlayer || SpectatorNetworking.IsLocallySpectated(Owner))
            {
                if (_ppVolume != null)
                {
                    _ppVolume.enabled = true;
                    _ppVolume.weight = Mathf.Lerp(
                        _ppVolume.weight,
                        IsActive ? 1f : 0f,
                        Time.deltaTime * _ppLerpSpeed);
                }
            }
            else
            {
                if (_ppVolume != null)
                    _ppVolume.enabled = false;
            }

            if (NetworkServer.active)
                UpdateServerside();
        }

        protected override void Awake()
        {
            base.Awake();
            GetSubroutine(out _observersTracker);
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            if (IsActive)
            {
                if (Elapsed >= MinimalTime)
                    IsActive = false;
            }
            else if (Cooldown.IsReady)
            {
                IsActive = true;
            }
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            writer.WriteBool(IsActive);
            if (!IsActive)
                Cooldown.WriteCooldown(writer);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            IsActive = reader.ReadBool();
            if (!IsActive)
                Cooldown.ReadCooldown(reader);
        }

        public override void ResetObject()
        {
            base.ResetObject();
            if (_ppVolume != null)
                _ppVolume.weight = 0f;
            IsActive = false;
            Cooldown.Clear();
        }
    }
}