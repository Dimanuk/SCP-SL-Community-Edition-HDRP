using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Ragdolls;
using PlayerRoles.Spectating;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace PlayerRoles.PlayableScps.Scp049
{
    public class Scp049ResurrectIndicators : ScpStandardSubroutine<Scp049Role>
    {

        private struct PotentialRagdoll
        {
            public BasicRagdoll Ragdoll;
            public Stopwatch Stopwatch;
        }

        private readonly struct Indicator
        {
            public readonly GameObject Instance;
            private readonly CanvasGroup _group;
            private readonly SpriteRenderer _sprite;

            private static readonly Color Transparent = new Color(1f, 1f, 1f, 0f);

            public void SetAlpha(float f)
            {
                f = Mathf.Clamp01(f);
                _group.alpha = f;
                _sprite.color = Color.Lerp(Transparent, Color.white, f);
            }

            public Indicator(GameObject inst)
            {
                Instance = inst;
                _group = inst.GetComponentInChildren<CanvasGroup>();
                _sprite = inst.GetComponentInChildren<SpriteRenderer>();
                SetAlpha(0f);
            }
        }

        private enum ListSyncRpcType : byte
        {
            FullResync = 0,
            Add = 1,
            Remove = 2
        }

        [SerializeField]
        private float _showDelay;

        [SerializeField]
        private float _fullOpacityDistance;

        [SerializeField]
        private float _visibleDistance;

        [SerializeField]
        private GameObject _indicatorTemplate;

        [SerializeField]
        private Vector3 _posOffset;

        private readonly Dictionary<uint, Indicator> _indicatorInstances = new Dictionary<uint, Indicator>();

        private readonly Queue<PotentialRagdoll> _potentialRagdolls = new Queue<PotentialRagdoll>();

        private readonly HashSet<BasicRagdoll> _availableRagdolls = new HashSet<BasicRagdoll>();
        private Scp049ResurrectAbility _resurrectAbility;

        private ListSyncRpcType _rpcType;

        private uint _syncRagdoll;

        private void Update()
        {

            if (!NetworkServer.active)
                return;

            if (!ServerCheckNew())
                ServerRevalidateOld();
        }

        private bool ServerCheckNew()
        {
            if (_potentialRagdolls.Count == 0)
                return false;

            var potential = _potentialRagdolls.Peek();
            if (potential.Stopwatch.Elapsed.TotalSeconds < _showDelay)
                return false;

            var ragdoll = _potentialRagdolls.Dequeue().Ragdoll;

            if (ragdoll == null)
                return false;

            if (!_resurrectAbility.CheckRagdoll(ragdoll))
                return false;

            _availableRagdolls.Add(ragdoll);
            ServerSendRpc(ListSyncRpcType.Add, ragdoll);
            return true;
        }

        private bool ServerRevalidateOld()
        {
            if (!HashsetExtensions.TryGetFirst(_availableRagdolls, x => !_resurrectAbility.CheckRagdoll(x), out var first))
                return false;

            _availableRagdolls.Remove(first);
            ServerSendRpc(ListSyncRpcType.Remove, first);
            return true;
        }

        private void UpdateIndicators()
        {
            if (!Owner.isLocalPlayer && !SpectatorNetworking.IsLocallySpectated(Owner))
                return;

            var camPos = MainCameraController._currentCamera.position;

            foreach (var ragdoll in _availableRagdolls)
                RefreshIndicator(camPos, ragdoll);
        }

        private void OnHudToggled(bool newState)
        {
            foreach (var indicator in _indicatorInstances.Values)
                indicator.Instance.SetActive(newState);
        }

        private void OnSpectatorTargetChanged()
        {
            if (_indicatorInstances.Count == 0)
                return;

            foreach (var indicator in _indicatorInstances.Values)
                Destroy(indicator.Instance);

            _indicatorInstances.Clear();
        }

        private void RefreshIndicator(Vector3 camPos, BasicRagdoll ragdoll)
        {
            var centerPoint = ragdoll.CenterPoint;
            if (centerPoint == null)
                return;

            var ragdollPos = centerPoint.position;
            var diff = camPos - ragdollPos;
            var sqrDist = diff.sqrMagnitude;
            var sqrVisible = _visibleDistance * _visibleDistance;

            if (!_indicatorInstances.TryGetValue(ragdoll.netId, out var indicator))
            {
                if (sqrDist > sqrVisible)
                    return; 

                var go = Instantiate(_indicatorTemplate);
                indicator = new Indicator(go);

                go.SetActive(HideHUDController.IsHUDVisible);

                _indicatorInstances[ragdoll.netId] = indicator;
            }
            else
            {
                if (sqrDist > sqrVisible)
                {
                    indicator.SetAlpha(0f);
                    return;
                }

                var dist = Mathf.Sqrt(sqrDist);
                var alpha = Mathf.InverseLerp(_visibleDistance, _fullOpacityDistance, dist);
                indicator.SetAlpha(alpha);

                var tr = indicator.Instance.transform;
                tr.position = ragdollPos + _posOffset;
                tr.forward = MainCameraController._currentCamera.forward;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            GetSubroutine(out _resurrectAbility);
        }

        public override void SpawnObject()
        {
            base.SpawnObject();

            PlayerRoleManager.OnRoleChanged += OnRoleChanged;
            RagdollManager.OnRagdollRemoved += OnRagdollRemoved;
            RagdollManager.OnRagdollSpawned += OnRagdollSpawned;
            HideHUDController.ToggleHUD += OnHudToggled;
            MainCameraController.OnUpdated += UpdateIndicators;
            SpectatorTargetTracker.OnTargetChanged += OnSpectatorTargetChanged;
        }

        public override void ResetObject()
        {
            base.ResetObject();

            _availableRagdolls.Clear();
            _potentialRagdolls.Clear();

            PlayerRoleManager.OnRoleChanged -= OnRoleChanged;
            RagdollManager.OnRagdollRemoved -= OnRagdollRemoved;
            RagdollManager.OnRagdollSpawned -= OnRagdollSpawned;
            HideHUDController.ToggleHUD -= OnHudToggled;
            MainCameraController.OnUpdated -= UpdateIndicators;
            SpectatorTargetTracker.OnTargetChanged -= OnSpectatorTargetChanged;

            foreach (var indicator in _indicatorInstances.Values)
                Destroy(indicator.Instance);

            _indicatorInstances.Clear();
        }

        public override void ServerWriteRpc(NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);

            writer.WriteByte((byte)_rpcType);

            if (_rpcType == ListSyncRpcType.FullResync)
            {
                foreach (var ragdoll in _availableRagdolls)
                    writer.WriteUInt(ragdoll.netId);
            }
            else
            {
                writer.WriteUInt(_syncRagdoll);
            }
        }

        public override void ClientProcessRpc(NetworkReader reader)
        {
            base.ClientProcessRpc(reader);

            _rpcType = (ListSyncRpcType)reader.ReadByte();

            if (_rpcType == ListSyncRpcType.FullResync)
                return;

            var netId = reader.ReadUInt();
            if (!NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity))
                return;

            if (!identity.TryGetComponent<BasicRagdoll>(out var ragdoll))
                return;

            if (_rpcType == ListSyncRpcType.Add)
            {
                _availableRagdolls.Add(ragdoll);
                return;
            }

            if (_indicatorInstances.TryGetValue(ragdoll.netId, out var indicator))
                Destroy(indicator.Instance);

            _availableRagdolls.Remove(ragdoll);
        }

        private void OnRoleChanged(ReferenceHub hub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            if (!NetworkServer.active)
                return;

            if (newRole is not SpectatorRole)
                return;

            _rpcType = ListSyncRpcType.FullResync;
            base.ServerSendRpc(hub);
        }

        private void OnRagdollSpawned(BasicRagdoll ragdoll)
        {
            if (!NetworkServer.active)
                return;

            _potentialRagdolls.Enqueue(new PotentialRagdoll
            {
                Ragdoll = ragdoll,
                Stopwatch = Stopwatch.StartNew()
            });
        }

        private void ServerSendRpc(ListSyncRpcType rpcType, BasicRagdoll ragdoll)
        {
            _rpcType = rpcType;
            _syncRagdoll = ragdoll?.netId ?? 0;

            ServerSendRpc(x => x == Owner || x.roleManager.CurrentRole is SpectatorRole);
        }

        private void OnRagdollRemoved(BasicRagdoll ragdoll)
        {
            _availableRagdolls.Remove(ragdoll);

            if (_indicatorInstances.TryGetValue(ragdoll.netId, out var indicator))
            {
                Destroy(indicator.Instance);
                _indicatorInstances.Remove(ragdoll.netId);
            }
        }
    }
}