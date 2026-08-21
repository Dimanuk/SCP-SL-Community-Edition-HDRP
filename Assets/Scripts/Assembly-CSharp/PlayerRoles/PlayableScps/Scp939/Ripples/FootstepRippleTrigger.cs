using Mirror;
using PlayerRoles.FirstPersonControl.Thirdperson;
using RelativePositioning;
using UnityEngine;
using Utils.Networking;

namespace PlayerRoles.PlayableScps.Scp939.Ripples
{
    public class FootstepRippleTrigger : RippleTriggerBase
    {
        private ReferenceHub _syncPlayer;
        private RelativePosition _syncPos;
        private byte _syncDistance;

        public override void SpawnObject()
        {
            base.SpawnObject();
            AnimatedCharacterModel.OnFootstepPlayed = (System.Action<AnimatedCharacterModel, float>)
                System.Delegate.Combine(AnimatedCharacterModel.OnFootstepPlayed, 
                new System.Action<AnimatedCharacterModel, float>(OnFootstepPlayed));
        }

        public override void ResetObject()
        {
            base.ResetObject();
            AnimatedCharacterModel.OnFootstepPlayed = (System.Action<AnimatedCharacterModel, float>)
                System.Delegate.Remove(AnimatedCharacterModel.OnFootstepPlayed, 
                new System.Action<AnimatedCharacterModel, float>(OnFootstepPlayed));
        }

        private void OnFootstepPlayed(AnimatedCharacterModel model, float dis)
        {
            if (!(model.OwnerHub.roleManager.CurrentRole is HumanRole humanRole))
                return;

            Vector3 myPosition = base.ScpRole.FpcModule.Position;
            Vector3 targetPosition = humanRole.FpcModule.Position;

            if ((myPosition - targetPosition).sqrMagnitude > dis * dis)
                return;

            if (base.IsLocalOrSpectated && !humanRole.FpcModule.Motor.IsInvisible)
            {
                base.Player.Play(humanRole);
            }

            if (!NetworkServer.active)
                return;

            _syncPlayer = model.OwnerHub;
            _syncPos = new RelativePosition(targetPosition);
            _syncDistance = (byte)Mathf.Min(dis, 255f);
            ServerSendRpc(toAll: false);
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            ReferenceHubReaderWriter.WriteReferenceHub(writer, _syncPlayer);
            RelativePositionSerialization.WriteRelativePosition(writer, _syncPos);
            writer.WriteByte(_syncDistance);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);

            if (!ReferenceHubReaderWriter.TryReadReferenceHub(reader, out _syncPlayer))
                return;

            if (!(_syncPlayer.roleManager.CurrentRole is HumanRole humanRole))
                return;

            if (humanRole.FpcModule.Motor == null || !humanRole.FpcModule.Motor.IsInvisible)
                return;

            if (!(humanRole.FpcModule.CharacterModelInstance is AnimatedCharacterModel animatedCharacterModel))
                return;

            _syncPos = RelativePositionSerialization.ReadRelativePosition(reader);
            _syncDistance = reader.ReadByte();

            base.Player.Play(_syncPos.Position, humanRole.RoleColor);

            AudioPooling.AudioSourcePoolManager.PlaySound(
                animatedCharacterModel.RandomFootstep, 
                _syncPos.Position, 
                _syncDistance);
        }
    }
}