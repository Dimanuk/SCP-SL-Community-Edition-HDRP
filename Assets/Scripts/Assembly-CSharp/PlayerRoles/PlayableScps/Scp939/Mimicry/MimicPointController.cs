using System;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp939;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Spectating;
using RelativePositioning;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939.Mimicry
{

    public class MimicPointController : ScpStandardSubroutine<Scp939Role>
    {

        private enum RpcStateMsg
        {
            None = 0,
            PlacedByUser = 25,
            RemovedByUser = 26,
            DestroyedByDistance = 27
        }

        [SerializeField]
        private SpriteRenderer _mimicPointIcon;

        [SerializeField]
        private Light _haloSource;

        [SerializeField]
        private AnimationCurve _iconOpacityOverDistance;

        [SerializeField]
        private float _maxDistance;

        [SerializeField]
        private float _focusOpacityReduction;

        private bool _active;
        private float _maxSqrDist;
        private RelativePosition _syncPos;
        private RpcStateMsg _syncMessage;
        private Color _haloColor;
        private Scp939FocusAbility _focus;

        private readonly AbilityCooldown _cooldown = new AbilityCooldown();

        private const float CooldownDuration = 0.2f;
        private const int Scp939IconLayer = 22;
        private const int TeammatesIconLayer = 0;

        [field: SerializeField]
        public Transform MimicPointTransform { get; private set; }

        public bool Active
        {
            get => _active;
            private set
            {
                if (value == _active)
                    return;

                _active = value;

                if (value)
                {
                    UpdateMimicPoint();

                    MainCameraController.OnUpdated += UpdateIcon;
                    FirstPersonMovementModule.OnPositionUpdated += UpdateMimicPoint;
                }
                else
                {
                    MimicPointTransform.localPosition = Vector3.zero;
                    _mimicPointIcon.gameObject.SetActive(false);

                    MainCameraController.OnUpdated -= UpdateIcon;
                    FirstPersonMovementModule.OnPositionUpdated -= UpdateMimicPoint;
                }
            }
        }

        public event Action<Scp939HudTranslation> OnMessageReceived;

        private bool TryGetIconLayer(out int layer)
        {
            layer = TeammatesIconLayer;

            if (!SpectatorTargetTracker.TryGetTrackedPlayer(out var hub) && !ReferenceHub.TryGetLocalHub(out hub))
                return false;

            if (!hub.IsSCP())
                return false;

            if (hub.roleManager.CurrentRole is Scp939Role)
                layer = Scp939IconLayer;

            return true;
        }

        private void UpdateMimicPoint()
        {
            Vector3 position = _syncPos.Position;
            MimicPointTransform.position = position;
            UpdateIcon();

            if (NetworkServer.active && (base.ScpRole.FpcModule.Position - position).sqrMagnitude >= _maxSqrDist)
            {
                _syncMessage = RpcStateMsg.DestroyedByDistance;
                Active = false;
                ServerSendRpc(toAll: true);
            }
        }

        private void UpdateIcon()
        {
            if (!TryGetIconLayer(out var layer))
            {
                _mimicPointIcon.gameObject.SetActive(false);
                return;
            }

            Vector3 camPos = MainCameraController.CurrentCamera.position;
            float distance = Vector3.Distance(camPos, _syncPos.Position);
            float alpha = _iconOpacityOverDistance.Evaluate(distance);

            MimicPointTransform.LookAt(camPos);

            alpha -= _focus.State * _focusOpacityReduction;

            _mimicPointIcon.color = Color.Lerp(Color.clear, Color.white, alpha);
            _haloSource.color = Color.Lerp(Color.clear, _haloColor, alpha);
            _mimicPointIcon.gameObject.layer = layer;
            _mimicPointIcon.gameObject.SetActive(true);
        }

        private void OnHubAdded(ReferenceHub hub)
        {
            if (NetworkServer.active)
                ServerSendRpc(hub);
        }

        private void OnDestroy()
        {
            ReferenceHub.OnPlayerAdded = (Action<ReferenceHub>)Delegate.Remove(
                ReferenceHub.OnPlayerAdded,
                new Action<ReferenceHub>(OnHubAdded));
        }

        protected override void Awake()
        {
            base.Awake();

            _maxSqrDist = _maxDistance * _maxDistance;
            _haloColor = _haloSource.color;

            GetSubroutine<Scp939FocusAbility>(out _focus);

            ReferenceHub.OnPlayerAdded = (Action<ReferenceHub>)Delegate.Combine(
                ReferenceHub.OnPlayerAdded,
                new Action<ReferenceHub>(OnHubAdded));
        }

        public override void ResetObject()
        {
            base.ResetObject();
            Active = false;
            _cooldown.Clear();
        }

        public void ClientToggle()
        {
            if (!_cooldown.IsReady)
                return;

            ClientSendCmd();
            _cooldown.Trigger(CooldownDuration);
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);

            if (Active)
            {
                _syncMessage = RpcStateMsg.RemovedByUser;
                Active = false;
            }
            else
            {
                _syncMessage = RpcStateMsg.PlacedByUser;
                _syncPos = new RelativePosition(base.ScpRole.FpcModule.Position);
                Active = true;
            }

            ServerSendRpc(toAll: true);
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);

            writer.WriteByte((byte)_syncMessage);

            if (Active)
                RelativePositionSerialization.WriteRelativePosition(writer, _syncPos);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);

            _syncMessage = (RpcStateMsg)reader.ReadByte();

            switch (_syncMessage)
            {
                case RpcStateMsg.None:
                    return;

                case RpcStateMsg.PlacedByUser:
                    _syncPos = RelativePositionSerialization.ReadRelativePosition(reader);
                    Active = true;
                    break;

                default:
                    Active = false;
                    break;
            }

            OnMessageReceived?.Invoke((Scp939HudTranslation)_syncMessage);
        }
    }
}