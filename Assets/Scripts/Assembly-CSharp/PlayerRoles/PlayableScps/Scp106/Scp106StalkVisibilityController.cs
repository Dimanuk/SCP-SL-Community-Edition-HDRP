using System.Collections.Generic;
using System.Diagnostics;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Spectating;
using PlayerStatsSystem;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace PlayerRoles.PlayableScps.Scp106
{
    public class Scp106StalkVisibilityController : ScpStandardSubroutine<Scp106Role>
    {
        private const float AbsoluteDistance = 4f;
        private const float HealthToDistance = 0.3f;
        private const float InvisibleHeight = 8000f;
        private const float TransitionSpeed = 11.5f;
        private const float ServerTolerance = 5f;
        private const float SendCooldown = 0.08f;
        private const float SubmergeTolerance = 0.8f;

        private Scp106StalkAbility _stalk;
        private bool _anyFaded;
        private readonly Stopwatch _sendStopwatch = Stopwatch.StartNew();
        private readonly HashSet<CharacterModel> _affectedModels = new HashSet<CharacterModel>();
        public readonly Dictionary<int, byte> SyncDamage= new Dictionary<int, byte>();

        protected override void Awake()
        {
            base.Awake();
            GetSubroutine(out _stalk);
        }

        private void UpdateAll()
        {
            if (base.Owner.isLocalPlayer)
            {
                UpdateClient();
            }
            else if (SpectatorNetworking.IsLocallySpectated(base.Owner))
            {
                UpdateSpectator();
            }
            else if (_anyFaded)
            {
                CleanupFade();
            }

            if (NetworkServer.active)
            {
                UpdateServer();
            }
        }

        private void UpdateClient()
        {
            foreach (ReferenceHub hub in ReferenceHub.AllHubs)
            {
                if (!(hub.roleManager.CurrentRole is HumanRole humanRole))
                    continue;

                float direction = GetVisibilityForPlayer(hub, humanRole) ? TransitionSpeed : -TransitionSpeed;
                FirstPersonMovementModule fpcModule = humanRole.FpcModule;
                CharacterModel model = fpcModule.CharacterModelInstance;

                bool wasFullyVisible = model.Fade == 0f;
                model.Fade += direction * Time.deltaTime;

                if (model.Fade < 1f)
                    _affectedModels.Add(model);
                else
                    _affectedModels.Remove(model);

                _anyFaded = true;

                if (!NetworkServer.active)
                {
                    if (model.Fade == 0f)
                    {
                        fpcModule.Position = Vector3.up * InvisibleHeight;
                    }
                    else if (wasFullyVisible || fpcModule.Motor.IsInvisible)
                    {
                        fpcModule.Position = fpcModule.Motor.ReceivedPosition.Position;
                    }
                }
            }
        }

        private void UpdateSpectator()
        {
            RefreshDamageDictionary();
            UpdateClient();
        }

        private void CleanupFade()
        {
            HashsetExtensions.ForEach(_affectedModels, delegate(CharacterModel x)
            {
                x.Fade = 1f;
            });
            _affectedModels.Clear();
            _anyFaded = false;
        }

        private void UpdateServer()
        {
            if (_stalk.IsActive)
            {
                RefreshDamageDictionary();
                if (!(_sendStopwatch.Elapsed.TotalSeconds < SendCooldown))
                {
                    _sendStopwatch.Restart();
                    ServerSendRpc(toAll: false);
                }
            }
        }

        private bool GetVisibilityForPlayer(ReferenceHub hub, HumanRole role)
        {
            FpcMotor motor = role.FpcModule.Motor;
            if (motor.IsInvisible)
                return false;

            if (!_stalk.IsActive || base.ScpRole.Sinkhole.NormalizedState < SubmergeTolerance)
                return true;

            if (hub.playerEffectsController.GetEffect<CustomPlayerEffects.Invigorated>().IsEnabled)
                return false;

            if (!SyncDamage.TryGetValue(hub.PlayerId, out byte damage))
                damage = 0;

            return Vector3.Distance(
                base.ScpRole.FpcModule.Position,
                motor.ReceivedPosition.Position)
                < damage * HealthToDistance + AbsoluteDistance;
        }

        private void RefreshDamageDictionary()
        {
            SyncDamage.Clear();
            foreach (ReferenceHub hub in ReferenceHub.AllHubs)
            {
                if (!(hub.roleManager.CurrentRole is HumanRole humanRole))
                    continue;

                if (hub.playerEffectsController.GetEffect<CustomPlayerEffects.Traumatized>().IsEnabled)
                {
                    SyncDamage[hub.PlayerId] = 0;
                    continue;
                }

                HealthStat health = hub.playerStats.GetModule<HealthStat>();
                int damage = Mathf.FloorToInt(health.MaxValue - health.CurValue);

                if (damage != 0 && !(Vector3.Distance(
                    humanRole.FpcModule.Position,
                    base.ScpRole.FpcModule.Position) - ServerTolerance
                    > damage * HealthToDistance + AbsoluteDistance))
                {
                    SyncDamage[hub.PlayerId] = (byte)Mathf.Clamp(damage, 0, 255);
                }
            }
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            writer.WriteByte((byte)SyncDamage.Count);
            foreach (var pair in SyncDamage)
            {
                writer.WriteRecyclablePlayerId(new RecyclablePlayerId(pair.Key));
                writer.WriteByte(pair.Value);
            }
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            SyncDamage.Clear();
            byte count = reader.ReadByte();
            for (int i = 0; i < count; i++)
            {
                int playerId = reader.ReadRecyclablePlayerId().Value;
                SyncDamage[playerId] = reader.ReadByte();
            }
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            FirstPersonMovementModule.OnPositionUpdated += UpdateAll;
        }

        public override void ResetObject()
        {
            base.ResetObject();
            CleanupFade();
            FirstPersonMovementModule.OnPositionUpdated -= UpdateAll;
        }
    }
}