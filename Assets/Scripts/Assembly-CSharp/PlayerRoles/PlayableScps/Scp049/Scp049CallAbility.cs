using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp049
{
    public class Scp049CallAbility : ScpKeySubroutine<Scp049Role>
    {
        private const float BaseCooldown = 60f;
        private const float EffectDuration = 20f;

        public readonly AbilityCooldown Cooldown = new AbilityCooldown();
        public readonly AbilityCooldown Duration = new AbilityCooldown();

        private bool _serverTriggered;

        [SerializeField]
        private Scp049AudioPlayer _audio;

        public bool IsMarkerShown
        {
            get
            {
                if (!Duration.IsReady)
                {
                    if (NetworkServer.active)
                        return _serverTriggered;
                    return true;
                }
                return false;
            }
        }

        protected override ActionName TargetKey => ActionName.Reload;

        private void ServerRefreshDuration()
        {
            if (_serverTriggered && Duration.IsReady)
            {
                Cooldown.Trigger(BaseCooldown);
                _serverTriggered = false;
                ServerSendRpc(true);
            }
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            if (!_serverTriggered && Cooldown.IsReady)
            {
                Duration.Trigger(EffectDuration);
                _serverTriggered = true;
                ServerSendRpc(true);
            }
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            Cooldown.WriteCooldown(writer);
            Duration.WriteCooldown(writer);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            Cooldown.ReadCooldown(reader);
            Duration.ReadCooldown(reader);
        }

        protected override void Update()
        {
            base.Update();

            if (NetworkServer.active)
                ServerRefreshDuration();
        }

        protected override void OnKeyDown()
        {
            base.OnKeyDown();
            ClientSendCmd();
        }

        public override void ResetObject()
        {
            base.ResetObject();
            Cooldown.Clear();
            Duration.Clear();
            _serverTriggered = false;
        }
    }
}