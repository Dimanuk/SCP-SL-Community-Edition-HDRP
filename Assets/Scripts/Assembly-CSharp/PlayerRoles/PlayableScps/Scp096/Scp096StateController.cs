using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Mirror;
using PlayerRoles.PlayableScps.Subroutines;

namespace PlayerRoles.PlayableScps.Scp096
{
	 public class Scp096StateController : ScpStandardSubroutine<Scp096Role>
    {
        public event Action<Scp096RageState> OnRageUpdate;

        public event Action<Scp096AbilityState> OnAbilityUpdate;

        private Scp096RageState _rageState;

        private Scp096AbilityState _abilityState;

        private readonly Stopwatch _rageChangeSw;

        private readonly Stopwatch _abilityChangeSw;

        public Scp096RageState RageState
        {
            get => _rageState;
            set
            {
                if (_rageState == value)
                    return;

                OnRageUpdate?.Invoke(value);

                _rageState = value;
                _rageChangeSw.Restart();

                if (NetworkServer.active)
                    ServerSendRpc(toAll: true);
            }
        }

        public Scp096AbilityState AbilityState
        {
            get => _abilityState;
            set
            {
                if (_abilityState == value)
                    return;

                OnAbilityUpdate?.Invoke(value);

                _abilityState = value;
                _abilityChangeSw.Restart();

                if (NetworkServer.active)
                    ServerSendRpc(toAll: true);
            }
        }

        public float LastRageUpdate => (float)_rageChangeSw.Elapsed.TotalSeconds;

        public float LastAbilityUpdate => (float)_abilityChangeSw.Elapsed.TotalSeconds;

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            writer.WriteByte((byte)RageState);
            writer.WriteByte((byte)AbilityState);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);

            if (!NetworkServer.active)
            {
                RageState = (Scp096RageState)reader.ReadByte();
                AbilityState = (Scp096AbilityState)reader.ReadByte();
            }
        }

        public override void ResetObject()
        {
            base.ResetObject();
            RageState = Scp096RageState.Docile;
            AbilityState = Scp096AbilityState.None;
            _rageChangeSw.Stop();
            _abilityChangeSw.Stop();
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            _rageChangeSw.Start();
            _abilityChangeSw.Start();
        }

        public void SetRageState(Scp096RageState state)
        {
            RageState = state;
        }

        public void SetAbilityState(Scp096AbilityState state)
        {
            AbilityState = state;
        }

        public Scp096StateController()
        {
            _rageChangeSw = new Stopwatch();
            _abilityChangeSw = new Stopwatch();
        }

	}
}
