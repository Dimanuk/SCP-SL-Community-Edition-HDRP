using System;
using Mirror;
using UnityEngine;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using PlayerRoles.PlayableScps.Scp079.Overcons;
using PlayerRoles.PlayableScps.Scp079.GUI;
using PlayerRoles.PlayableScps.Scp079.Rewards;
using MapGeneration;
using Utils.NonAllocLINQ;

namespace PlayerRoles.PlayableScps.Scp079
{
    public class Scp079TeslaAbility : Scp079KeyAbilityBase
    {
        [SerializeField]
        private int _cost;

        [SerializeField]
        private float _cooldown;

        private string _abilityName;
        private string _cooldownMessage;
        private double _nextUseTime;

        public override bool IsVisible
        {
            get
            {
                if (!Scp079CursorManager.LockCameras)
                {
                    var highlighted = OverconManager.Singleton?.HighlightedOvercon;
                    if (highlighted is TeslaOvercon teslaOvercon)
                        return teslaOvercon != null;
                }
                return false;
            }
        }

        public override bool IsReady
        {
            get
            {
                if (AuxManager.CurrentAux >= _cost)
                    return _nextUseTime < NetworkTime.time;
                return false;
            }
        }

        public override string FailMessage
        {
            get
            {
                if (AuxManager.CurrentAux < _cost)
                    return GetNoAuxMessage(_cost);

                int remaining = Mathf.CeilToInt((float)(_nextUseTime - NetworkTime.time));
                if (remaining > 0)
                    return _cooldownMessage + "\n" + AuxManager.GenerateCustomETA(remaining);

                return null;
            }
        }

        public override ActionName ActivationKey => ActionName.Shoot;

        public override string AbilityName => string.Format(_abilityName, _cost);

        protected override void Start()
        {
            base.Start();
            _abilityName = Translations.Get<Scp079HudTranslation>((Scp079HudTranslation)38);
            _cooldownMessage = Translations.Get<Scp079HudTranslation>((Scp079HudTranslation)39);
        }

        protected override void Trigger()
        {
            ClientSendCmd();
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);

            if (!IsReady)
                return;

            Scp079Camera cam = CurrentCamSync.CurrentCamera;
            if (cam == null)
                return;

            if (ListExtensions.TryGetFirst(
                TeslaGateController.Singleton.TeslaGates,
                (TeslaGate x) => RoomIdUtils.IsTheSameRoom(cam.Position, x.transform.position),
                out var first))
            {
                RewardManager.MarkRoom(cam.Room);
                AuxManager.CurrentAux -= _cost;
                first.RpcInstantBurst();

                _nextUseTime = NetworkTime.time + _cooldown;
                ServerSendRpc(false);
            }
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            NetworkWriterExtensions.WriteDouble(writer, _nextUseTime);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            _nextUseTime = NetworkReaderExtensions.ReadDouble(reader);
        }
    }
}
