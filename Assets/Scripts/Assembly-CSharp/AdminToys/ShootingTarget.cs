using System;
using System.Collections.Generic;
using Interactables;
using Interactables.Verification;
using Mirror;
using PlayerStatsSystem;
using UnityEngine;

namespace AdminToys
{
    public class ShootingTarget : AdminToyBase, IDestructible, IClientInteractable, IInteractable, IServerInteractable
    {
        private enum TargetButton { IncreaseHP, DecreaseHP, IncreaseResetTime, DecreaseResetTime, ManualReset, Remove, GlobalResults }

        private float _hp = 10f;
        private int _maxHp = 10;
        private int _autoDestroyTime;
        private float _avg;
        private GameObject _prevHit;

        [SyncVar] private bool _syncMode;

        [SerializeField] private float _stepSize = 0.12f;
        [SerializeField] private string _targetName;
        [SerializeField] private AudioSource _source;
        [SerializeField] private AudioClip _hitSound;
        [SerializeField] private AudioClip _killSound;
        [SerializeField] private AudioClip[] _score;
        [SerializeField] private GameObject _hitIndicator;
        [SerializeField] private Transform _bullsEye;
        [SerializeField] private float _bullsEyeRadius;
        [SerializeField] private Vector3[] _bullsEyeBounds;
        [SerializeField] private Material _prevHitMat;

        public uint NetworkId => netId;
        private readonly List<GameObject> _hits = new List<GameObject>();
        public IVerificationRule VerificationRule => StandardDistanceVerification.Default;
        public Vector3 CenterOfMass => _bullsEye.position;
        public override string CommandName => "Target" + _targetName;

        public override void OnSpawned(ReferenceHub admin, ArraySegment<string> arguments)
        {
            if (Physics.Raycast(admin.transform.position - admin.transform.forward, Vector3.down, out var hitInfo, 2f))
            {
                transform.position = hitInfo.point;
                transform.rotation = Quaternion.Euler(Vector3.up * (Mathf.Round((admin.transform.rotation.eulerAngles.y + 90f) / 10f) * 10f));
            }
            base.OnSpawned(admin, arguments);
        }

        public bool Damage(float damage, DamageHandlerBase handler, Vector3 exactHit)
        {
            if (handler is not AttackerDamageHandler attackerHandler || attackerHandler.Attacker.Hub == null)
                return false;

            var hub = attackerHandler.Attacker.Hub;

            float distance = Vector3.Distance(hub.transform.position, _bullsEye.position);

            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (_syncMode || allHub == hub)
                    TargetRpcReceiveData(allHub.connectionToClient, damage, distance, exactHit, handler);
            }
            return true;
        }

        [TargetRpc]
        private void TargetRpcReceiveData(NetworkConnection conn, float damage, float distance, Vector3 pos, DamageHandlerBase handler)
        {
            float distToCenter = float.PositiveInfinity;
            if (_bullsEyeBounds.Length == 0)
            {
                distToCenter = Vector3.Distance(_bullsEye.position, pos);
            }
            else
            {
                foreach (var bound in _bullsEyeBounds)
                {
                    float d = Vector3.Distance(new Bounds(_bullsEye.TransformPoint(new Vector3(0f, bound.y, bound.x)), Vector3.one * bound.z).ClosestPoint(pos), pos);
                    if (d < distToCenter) distToCenter = d;
                }
            }

            distToCenter = Mathf.Max(0f, distToCenter - _bullsEyeRadius);
            int scoreIdx = Mathf.Min(Mathf.CeilToInt(distToCenter / _stepSize), _score.Length - 1);
            _avg += 1f - (float)scoreIdx / (_score.Length - 1f);

            GameObject hit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hit.GetComponent<Collider>().enabled = false;
            hit.GetComponent<MeshRenderer>().sharedMaterial = _hitIndicator.GetComponent<MeshRenderer>().sharedMaterial;
            hit.transform.localScale = _hitIndicator.transform.localScale;
            hit.transform.SetPositionAndRotation(pos, _hitIndicator.transform.rotation);
            hit.transform.parent = _hitIndicator.transform.parent;

            _hp -= damage;
            _source.Stop();
            _source.PlayOneShot(_score[scoreIdx]);
            _source.PlayOneShot((_hp < 0f) ? _killSound : _hitSound);

            if (_prevHit != null && _prevHit.TryGetComponent<MeshRenderer>(out var prevRenderer))
                prevRenderer.sharedMaterial = _prevHitMat;

            _prevHit = hit;

            if (_autoDestroyTime > 0)
                Destroy(hit, _autoDestroyTime);
            else
                _hits.Add(hit);
        }

        public void ServerInteract(ReferenceHub ply, byte colliderId)
        {
            if (!PermissionsHandler.IsPermitted(ply.serverRoles.Permissions, PlayerPermissions.FacilityManagement))
                return;

            TargetButton button = (TargetButton)colliderId;
            if (button == TargetButton.Remove)
            {
                NetworkServer.Destroy(gameObject);
                return;
            }
            if (button == TargetButton.GlobalResults)
            {
                _syncMode = !_syncMode;
                return;
            }

            if (_syncMode && !ply.isLocalPlayer)
            {
                HandleButtonLogic(button);
                RpcSendInfo(_maxHp, _autoDestroyTime);
            }
        }

        [ClientRpc]
        private void RpcSendInfo(int maxHp, int autoReset)
        {
            _maxHp = maxHp;
            _autoDestroyTime = autoReset;
            ClearTarget();
        }

        private void HandleButtonLogic(TargetButton tb)
        {
            switch (tb)
            {
                case TargetButton.ManualReset: ClearTarget(); break;
                case TargetButton.IncreaseHP: _maxHp = Mathf.Clamp(_maxHp * 2, 1, 256); break;
                case TargetButton.DecreaseHP: _maxHp /= 2; break;
                case TargetButton.IncreaseResetTime: _autoDestroyTime = Mathf.Min(_autoDestroyTime + 1, 10); break;
                case TargetButton.DecreaseResetTime: _autoDestroyTime = Mathf.Max(_autoDestroyTime - 1, 0); break;
            }
        }

        private void ClearTarget()
        {
            foreach (var hit in _hits) Destroy(hit);
            _hits.Clear();
            _avg = 0f;
            _hp = _maxHp;
        }

        public void ClientInteract(InteractableCollider collider) => HandleButtonLogic((TargetButton)collider.ColliderId);
    }
}