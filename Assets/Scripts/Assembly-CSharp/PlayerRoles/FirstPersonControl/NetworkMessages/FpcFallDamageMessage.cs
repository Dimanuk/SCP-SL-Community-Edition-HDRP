using AudioPooling;
using CustomPlayerEffects;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl.Thirdperson;
using RelativePositioning;
using UnityEngine;
using Utils.Networking;

namespace PlayerRoles.FirstPersonControl.NetworkMessages
{
    public readonly struct FpcFallDamageMessage : NetworkMessage
    {
        public const float SoundDistance = 14f;
        private const float SoundVolume = 1f;

        private readonly ReferenceHub _hub;
        private readonly Vector3 _prevPos;
        private readonly RoleTypeId _role;

        public FpcFallDamageMessage(ReferenceHub hub, Vector3 prevPos, RoleTypeId role)
        {
            _hub = hub;
            _prevPos = prevPos;
            _role = role;
        }

        public FpcFallDamageMessage(NetworkReader reader)
        {
            var playerId = reader.ReadRecyclablePlayerId();

            if (playerId.Value == 0)
            {
                _hub = null;
                _prevPos = reader.ReadRelativePosition().Position;
                _role = reader.ReadRoleType();
            }
            else
            {
                _hub = ReferenceHub.GetHub(playerId.Value);
                _prevPos = Vector3.zero;
                _role = _hub != null ? _hub.GetRoleId() : RoleTypeId.None;
            }
        }

        public void Write(NetworkWriter writer)
        {
            if (_hub == null || !_hub.IsAlive())
            {
                writer.WriteReferenceHub(null);
                writer.WriteRelativePosition(new RelativePosition(_prevPos));
                writer.WriteRoleType(_role);
                return;
            }
            writer.WriteReferenceHub(_hub);
        }

        public void ProcessMessage()
        {
            if (!NetworkClient.active)
                return;

            if (_role == RoleTypeId.None)
                return;

            if (!ReferenceHub.TryGetLocalHub(out var localHub))
                return;

            Vector3 soundPosition = (_hub != null && _hub.IsAlive())
                ? _hub.transform.position
                : _prevPos;

            float distance = Vector3.Distance(localHub.transform.position, soundPosition);
            if (distance > SoundDistance)
                return;

            if (!TryGetFallSound(out AudioClip clip, out float baseVolume))
                return;

            float distanceFactor = 1f - (distance / SoundDistance);
            float finalVolume = baseVolume * Mathf.Clamp01(distanceFactor * 1.4f);

            if (finalVolume < 0.05f)
                return;

            AudioSourcePoolManager.PlaySound(
                clip,
                soundPosition,
                maxDistance: SoundDistance * 1.6f,
                volume: finalVolume,
                falloffType: FalloffType.Exponential,
                channel: AudioMixerChannelType.DefaultSfx,
                spatial: 1f
            );
        }

        private Vector3 GetSoundPosition()
        {
            if (_hub != null && _hub.IsAlive())
            {
                return _hub.transform.position;
            }

            return _prevPos;
        }

        private bool TryGetFallSound(out AudioClip clip, out float volume)
        {
            clip = null;
            volume = SoundVolume;

            if (!PlayerRoleLoader.TryGetRoleTemplate(_role, out PlayerRoleBase roleTemplate))
                return false;

            if (roleTemplate is not IFpcRole fpcRole)
                return false;

            if (!fpcRole.FpcModule.CharacterModelTemplate.TryGetComponent<AnimatedCharacterModel>(out var model))
                return false;

            clip = model.SharedSettings.FallDamageSound;
            return clip != null;
        }
    }
}