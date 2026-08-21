using System;
using System.Collections.Generic;
using AudioPooling;
using CameraShaking;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Spectating;
using PlayerStatsSystem;
using UnityEngine;
using Utils.Networking;

namespace PlayerRoles.PlayableScps.Scp096
{
    public class Scp096TargetsTracker : ScpStandardSubroutine<Scp096Role>
    {
        private const float Vision096InnerAngle = 0.1f;
        private const float VisionTriggerDistance = 60f;
        private const float HeadSize = 0.12f;
        private const float PostRageCooldownDuration = 10f;

        public GameObject TargetMarker;

        public readonly HashSet<ReferenceHub> Targets = new HashSet<ReferenceHub>();

        private readonly AbilityCooldown _postRageCooldown = new AbilityCooldown();
        private readonly Dictionary<ReferenceHub, GameObject> _markers = new Dictionary<ReferenceHub, GameObject>();
        private readonly HashSet<ReferenceHub> _unvalidatedTargets = new HashSet<ReferenceHub>();

        [SerializeField]
        private AudioClip _targetSound;

        private bool _sendTargetsNextFrame;
        private bool _eventsAssigned;

        public bool CanReceiveTargets => base.ScpRole.IsRageState(Scp096RageState.Docile);

        public event Action<ReferenceHub> OnTargetAdded;
        public event Action<ReferenceHub> OnTargetAttacked;
        public event Action<ReferenceHub> OnTargetRemoved;

        public bool AddTarget(ReferenceHub target, bool isForLook)
        {
            if (target == null || Targets.Contains(target))
            {
                return false;
            }

            if (Targets.Count == 0 && base.Owner.isLocalPlayer)
            {
                PlayTargetSound();
            }
            else if (target.isLocalPlayer)
            {
                PlayTargetSound();
                CameraShakeController.AddEffect(new Scp096BecomeTargetShake());
            }

            Targets.Add(target);

            if (NetworkClient.active && !_markers.ContainsKey(target))
            {
                _markers.Add(target, Instantiate(TargetMarker, target.transform));
            }

            _sendTargetsNextFrame = true;
            OnTargetAdded?.Invoke(target);
            return true;
        }

        public bool RemoveTarget(ReferenceHub target)
        {
            if (target == null || !Targets.Remove(target))
            {
                return false;
            }

            if (_markers.TryGetValue(target, out var value))
            {
                _markers.Remove(target);
                Destroy(value);
            }

            _sendTargetsNextFrame = true;
            OnTargetRemoved?.Invoke(target);
            return true;
        }

        public void ClearAllTargets()
        {
            foreach (ReferenceHub target in Targets)
            {
                if (_markers.TryGetValue(target, out var value))
                {
                    _markers.Remove(target);
                    Destroy(value);
                }
                OnTargetRemoved?.Invoke(target);
            }

            _sendTargetsNextFrame = true;
            Targets.Clear();
        }

        public bool IsObservedBy(ReferenceHub target)
        {
            Vector3 position = (base.ScpRole.FpcModule.CharacterModelInstance as Scp096CharacterModel).Head.position;

            if (Vector3.Dot((target.PlayerCameraReference.position - position).normalized, base.Owner.PlayerCameraReference.forward) < Vision096InnerAngle)
            {
                return false;
            }

            return VisionInformation.GetVisionInformation(target, target.PlayerCameraReference, position, HeadSize, VisionTriggerDistance).IsLooking;
        }

        public bool HasTarget(ReferenceHub target)
        {
            return Targets.Contains(target);
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            Utils.NonAllocLINQ.HashsetExtensions.ForEach(Targets, writer.WriteReferenceHub);
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            _unvalidatedTargets.UnionWith(Targets);

            while (reader.Position < reader.Capacity)
            {
                if (Utils.Networking.ReferenceHubReaderWriter.TryReadReferenceHub(reader, out var hub) 
                    && !_unvalidatedTargets.Remove(hub))
                {
                    AddTarget(hub, isForLook: false);
                }
            }

            Utils.NonAllocLINQ.HashsetExtensions.ForEach(_unvalidatedTargets, delegate(ReferenceHub x)
            {
                RemoveTarget(x);
            });

            _unvalidatedTargets.Clear();
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            _eventsAssigned = true;
            base.Owner.playerStats.OnThisPlayerDamaged += AddTargetOnDamage;
        }

        public override void ResetObject()
        {
            base.ResetObject();
            ClearAllTargets();

            if (_eventsAssigned)
            {
                _eventsAssigned = false;
                base.Owner.playerStats.OnThisPlayerDamaged -= AddTargetOnDamage;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            ReferenceHub.OnPlayerRemoved = (Action<ReferenceHub>)Delegate.Combine(
                ReferenceHub.OnPlayerRemoved, 
                new Action<ReferenceHub>(CheckRemovedPlayer));

            base.ScpRole.StateController.OnRageUpdate += delegate(Scp096RageState state)
            {
                if (state == Scp096RageState.Calming)
                {
                    _postRageCooldown.Trigger(PostRageCooldownDuration);
                    ClearAllTargets();
                }
            };
        }

        private void PlayTargetSound()
        {
            AudioSourcePoolManager.PlaySound(_targetSound, (Transform)null, 1f, 1f, FalloffType.Exponential, AudioMixerChannelType.NoDucking, 0f);
        }

        private void AddTargetOnDamage(DamageHandlerBase obj)
        {
            if (obj is AttackerDamageHandler attackerDamageHandler)
            {
                ReferenceHub hub = attackerDamageHandler.Attacker.Hub;
                if (CanReceiveTargets && hub != null)
                {
                    AddTarget(hub, isForLook: false);
                    OnTargetAttacked?.Invoke(hub);
                }
            }
        }

        private void OnDestroy()
        {
            ReferenceHub.OnPlayerRemoved = (Action<ReferenceHub>)Delegate.Remove(
                ReferenceHub.OnPlayerRemoved, 
                new Action<ReferenceHub>(CheckRemovedPlayer));
        }

        private void Update()
        {
            bool visible = base.Owner.isLocalPlayer || SpectatorNetworking.IsLocallySpectated(base.Owner);

            Utils.NonAllocLINQ.DictionaryExtensions.ForEachValue(_markers, delegate(GameObject x)
            {
                x.SetActive(visible);
            });

            if (NetworkServer.active)
            {
                ServerCheckTargets();
            }
        }

        private void CheckRemovedPlayer(ReferenceHub ply)
        {
            RemoveTarget(ply);
        }

        private void ServerCheckTargets()
        {
            if (base.ScpRole.IsRageState(Scp096RageState.Calming) || !_postRageCooldown.IsReady)
            {
                return;
            }

            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                UpdateTarget(allHub);
            }

            if (_sendTargetsNextFrame)
            {
                _sendTargetsNextFrame = false;
                ServerSendRpc(toAll: true);
            }
        }

        private void UpdateTarget(ReferenceHub target)
        {
            if (!target.IsHuman())
            {
                RemoveTarget(target);
            }
            else if (IsObservedBy(target))
            {
                AddTarget(target, isForLook: true);
            }
        }
    }
}