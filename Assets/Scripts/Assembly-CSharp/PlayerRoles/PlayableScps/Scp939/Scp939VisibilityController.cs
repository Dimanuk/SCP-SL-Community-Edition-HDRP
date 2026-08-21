using System;
using System.Collections.Generic;
using GameObjectPools;
using InventorySystem.Items;
using InventorySystem.Items.MicroHID;
using InventorySystem.Items.Usables;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerRoles.Spectating;
using PlayerRoles.Voice;
using PlayerStatsSystem;
using RelativePositioning;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939
{
    public class Scp939VisibilityController : FpcVisibilityController, IPoolResettable
    {
        private struct LastSeenInfo
        {
            public double Time;
            public RelativePosition RelPos;
            public Vector3 Velocity;

            public Vector3 WorldPos => RelPos.Position + Velocity * Elapsed;
            public float Elapsed => (float)(NetworkTime.time - Time);
        }

        [SerializeField] private float _pingTolerance;
        [SerializeField] private float _defaultRange;
        [SerializeField] private float _recentFootstepRangeMultiplier;
        [SerializeField] private float _recentFootstepTime;
        [SerializeField] private float _focusMultiplier;
        [SerializeField] private float _exhaustionMultiplier;
        [SerializeField] private float _fadeSpeed;
        [SerializeField] private float _sustain;

        private Scp939Role _scpRole;
        private StaminaStat _stamina;
        private Scp939FocusAbility _focus;
        private bool _wasFaded;

        private static readonly Dictionary<uint, LastSeenInfo> LastSeen = new Dictionary<uint, LastSeenInfo>();

        private readonly Dictionary<uint, double> _lastFootstepSounds = new Dictionary<uint, double>();

        public float CurrentDetectionRange
        {
            get
            {
                float defaultRange = _defaultRange;
                return (defaultRange + defaultRange * _focusMultiplier * _focus.State)
                       * Mathf.Lerp(_exhaustionMultiplier, 1f, _stamina.NormalizedValue);
            }
        }

        private float DetectionRangeForPlayer(ReferenceHub hub)
        {
            float range = CurrentDetectionRange;

            if (_lastFootstepSounds.TryGetValue(hub.netId, out double footstepTime)
                && NetworkTime.time - footstepTime < _recentFootstepTime)
            {
                range *= _recentFootstepRangeMultiplier;
            }

            return range;
        }

        private void OnDestroy()
        {
            if (_wasFaded)
                ResetFade();
        }

        private void LateUpdate()
        {
            if (base.Owner.isLocalPlayer || SpectatorNetworking.IsLocallySpectated(base.Owner))
            {
                PlayerRolesUtils.ForEachRole<HumanRole>(UpdateHuman);
            }
        }

        private void UpdateHuman(ReferenceHub ply, HumanRole human)
        {
            FirstPersonMovementModule fpcModule = human.FpcModule;
            FpcMotor motor = fpcModule.Motor;
            CharacterModel characterModelInstance = fpcModule.CharacterModelInstance;

            bool hasLastSeen = LastSeen.TryGetValue(ply.netId, out LastSeenInfo lastSeen);
            bool isVisible = !motor.IsInvisible;

            bool inRange = isVisible
                && (Vector3.Distance(fpcModule.Position, _scpRole.FpcModule.Position) <= DetectionRangeForPlayer(ply)
                    || (hasLastSeen && lastSeen.Elapsed < _sustain));

            float prevFade = characterModelInstance.Fade;
            characterModelInstance.Fade += Time.deltaTime * (inRange ? _fadeSpeed : -_fadeSpeed);
            _wasFaded = true;

            if (NetworkServer.active || !base.Owner.isLocalPlayer)
                return;

            if (characterModelInstance.Fade == 0f)
            {
                if (prevFade != 0f)
                {
                    characterModelInstance.Hitboxes.ForEach(x => x.SetColliders(false));
                }
            }
            else if (!isVisible)
            {
                fpcModule.Position = hasLastSeen ? lastSeen.WorldPos : motor.ReceivedPosition.Position;
            }
            else
            {
                if (!inRange)
                    return;

                if (prevFade == 0f)
                {
                    fpcModule.Position = motor.ReceivedPosition.Position;
                    (characterModelInstance as AnimatedCharacterModel)?.ForceUpdate();
                    characterModelInstance.Hitboxes.ForEach(x => x.SetColliders(true)); // <>9__19_1
                }

                LastSeen[ply.netId] = new LastSeenInfo
                {
                    RelPos = new RelativePosition(fpcModule.Position),
                    Time = NetworkTime.time,
                    Velocity = motor.Velocity
                };
            }
        }

        private void ResetFade()
        {
            PlayerRolesUtils.ForEachRole<HumanRole>(x =>
            {
                CharacterModel model = x.FpcModule.CharacterModelInstance;
                if (model.Fade != 1f)
                {
                    model.Fade = 1f;
                    model.Hitboxes.ForEach(hitbox => hitbox.SetColliders(true));
                }
            });
        }

        private void OnFootstepPlayed(AnimatedCharacterModel model, float range)
        {
            ReferenceHub ownerHub = model.OwnerHub;

            if (ownerHub.roleManager.CurrentRole is HumanRole humanRole
                && !((humanRole.FpcModule.Position - _scpRole.FpcModule.Position).sqrMagnitude > range * range))
            {
                _lastFootstepSounds[ownerHub.netId] = NetworkTime.time;
            }
        }

        private void OnSpectatorTargetChanged()
        {
            if (_wasFaded)
                ResetFade();
        }

        private void OnRoleChanged(ReferenceHub hub, PlayerRoleBase oldRole, PlayerRoleBase newRole)
        {
            if (_wasFaded && hub.isLocalPlayer && !(newRole is SpectatorRole))
            {
                ResetFade();
            }
        }

        public override void SpawnObject()
        {
            base.SpawnObject();

            _scpRole = base.Role as Scp939Role;
            base.Owner.playerStats.TryGetModule(out _stamina);
            _scpRole.SubroutineModule.TryGetSubroutine(out _focus);

            SpectatorTargetTracker.OnTargetChanged =
                (Action)Delegate.Combine(SpectatorTargetTracker.OnTargetChanged, new Action(OnSpectatorTargetChanged));

            PlayerRoleManager.OnRoleChanged += OnRoleChanged;

            AnimatedCharacterModel.OnFootstepPlayed =
                (Action<AnimatedCharacterModel, float>)Delegate.Combine(
                    AnimatedCharacterModel.OnFootstepPlayed,
                    new Action<AnimatedCharacterModel, float>(OnFootstepPlayed));

            if (base.Owner.isLocalPlayer)
            {
                LastSeen.Clear();
            }
        }

        public override bool ValidateVisibility(ReferenceHub hub)
        {
            if (!base.ValidateVisibility(hub))
                return false;

            if (!(hub.roleManager.CurrentRole is HumanRole humanRole))
                return true;

            FirstPersonMovementModule scpFpc = _scpRole.FpcModule;

            float range = BaseRangeForPlayer(hub, humanRole);
            range = Mathf.Max(range, DetectionRangeForPlayer(hub));
            range += scpFpc.MaxMovementSpeed * _pingTolerance;

            bool inRange = (humanRole.FpcModule.Position - scpFpc.Position).sqrMagnitude <= range * range;

            bool result = inRange
                || (LastSeen.TryGetValue(hub.netId, out LastSeenInfo info) && info.Elapsed < _sustain);

            if (!inRange || _scpRole.IsLocalPlayer)
                return result;

            LastSeen[hub.netId] = new LastSeenInfo { Time = NetworkTime.time };
            return true;
        }

        private float BaseRangeForPlayer(ReferenceHub hub, HumanRole human)
        {
            float range = 0f;
            ItemBase curInstance = hub.inventory.CurInstance;

            if (curInstance is MicroHIDItem microHIDItem)
            {
                if (microHIDItem != null
                    && microHIDItem.State != HidState.Idle
                    && microHIDItem.State != HidState.StopSound)
                {
                    range = 30f;
                }
            }
            else if (curInstance is UsableItem
                && UsableItemsController.Handlers.TryGetValue(hub, out PlayerHandler handler)
                && curInstance != null
                && handler.CurrentUsable.ItemSerial == curInstance.ItemSerial)
            {
                range = 15f;
            }

            if (human.VoiceModule is HumanVoiceModule humanVoiceModule && humanVoiceModule.ServerIsSending)
            {
                range = Mathf.Max(range, humanVoiceModule.ProximityPlayback.Source.maxDistance);
            }

            return range;
        }

        public void ResetObject()
        {
            SpectatorTargetTracker.OnTargetChanged =
                (Action)Delegate.Remove(SpectatorTargetTracker.OnTargetChanged, new Action(OnSpectatorTargetChanged));

            PlayerRoleManager.OnRoleChanged -= OnRoleChanged;

            AnimatedCharacterModel.OnFootstepPlayed =
                (Action<AnimatedCharacterModel, float>)Delegate.Remove(
                    AnimatedCharacterModel.OnFootstepPlayed,
                    new Action<AnimatedCharacterModel, float>(OnFootstepPlayed));

            if (_wasFaded)
            {
                ResetFade();
            }
        }
    }
}
