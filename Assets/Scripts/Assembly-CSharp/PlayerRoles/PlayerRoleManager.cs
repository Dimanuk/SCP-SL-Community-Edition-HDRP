using System;
using System.Collections.Generic;
using CustomPlayerEffects;
using GameObjectPools;
using Mirror;
using PlayerRoles.SpawnData;
using UnityEngine;

namespace PlayerRoles
{
    public class PlayerRoleManager : NetworkBehaviour
    {
        public delegate void ServerRoleSet(ReferenceHub userHub, RoleTypeId newRole, RoleChangeReason reason);

        public delegate void RoleChanged(ReferenceHub userHub, PlayerRoleBase prevRole, PlayerRoleBase newRole);

        public readonly Dictionary<uint, RoleTypeId> PreviouslySentRole = new Dictionary<uint, RoleTypeId>();

        private ReferenceHub _hub;

        private bool _hubSet;

        public bool _anySet;

        private bool _sendNextFrame;

        private PlayerRoleBase _curRole;

        private const RoleTypeId DefaultRole = RoleTypeId.None;

        public PlayerRoleBase CurrentRole
        {
            get
            {
                if (!_anySet)
                {
                    InitializeNewRole(RoleTypeId.None, RoleChangeReason.None);
                }
                return _curRole;
            }
            set
            {
                _curRole = value;
                _anySet = true;
            }
        }

        private ReferenceHub Hub
        {
            get
            {
                if (!_hubSet && ReferenceHub.TryGetHub(base.gameObject, out _hub))
                {
                    _hubSet = true;
                }
                return _hub;
            }
        }

        public static event ServerRoleSet OnServerRoleSet;

        public static event RoleChanged OnRoleChanged;

        private void Update()
        {
            if (!NetworkServer.active || !_sendNextFrame)
            {
                return;
            }
            _sendNextFrame = false;
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.isLocalPlayer)
                {
                    continue;
                }
                RoleTypeId roleTypeId = CurrentRole.RoleTypeId;
                if (CurrentRole is IObfuscatedRole obfuscatedRole)
                {
                    roleTypeId = obfuscatedRole.GetRoleForUser(allHub);
                    if (PreviouslySentRole.TryGetValue(allHub.netId, out var value) && value == roleTypeId)
                    {
                        continue;
                    }
                }
                allHub.connectionToClient.Send(new RoleSyncInfo(Hub, roleTypeId, allHub));
                PreviouslySentRole[allHub.netId] = roleTypeId;
            }
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            InitializeNewRole(DefaultRole, RoleChangeReason.Destroyed);
        }

        private PlayerRoleBase GetRoleBase(RoleTypeId targetId)
        {
            if (!PlayerRoleLoader.TryGetRoleTemplate<PlayerRoleBase>(targetId, out var result))
            {
                Debug.LogError($"Role #{targetId} could not be found. Player with ID {Hub.PlayerId} will receive the default role ({RoleTypeId.None}).");
                if (!PlayerRoleLoader.TryGetRoleTemplate<PlayerRoleBase>(RoleTypeId.None, out result))
                {
                    throw new NotImplementedException("Role change failed. Default role is not correctly implemented.");
                }
            }
            if (!PoolManager.Singleton.TryGetPoolObject(result.gameObject, out var poolObject, autoSetup: false) || !(poolObject is PlayerRoleBase result2))
            {
                throw new InvalidOperationException($"Role {targetId} failed to initialize, pool was not found or dequed object was of incorrect type.");
            }
            return result2;
        }

        public void InitializeNewRole(RoleTypeId targetId, RoleChangeReason reason, RoleSpawnFlags spawnFlags = RoleSpawnFlags.All, NetworkReader data = null)
        {
            PlayerRoleBase playerRoleBase;
            bool flag;
            if (_anySet)
            {
                playerRoleBase = CurrentRole;
                playerRoleBase.DisableRole(targetId);
                flag = true;
            }
            else
            {
                playerRoleBase = null;
                flag = false;
            }
            if (reason == RoleChangeReason.Destroyed && targetId == RoleTypeId.None)
            {
                return;
            }
            PlayerRoleBase roleBase = GetRoleBase(targetId);
            Transform obj = roleBase.transform;
            obj.parent = base.transform;
            obj.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            CurrentRole = roleBase;
            roleBase.Init(Hub, reason, spawnFlags);
            roleBase.SetupPoolObject();
            if (CurrentRole is ISpawnDataReader spawnDataReader && data != null)
            {
                spawnDataReader.ReadSpawnData(data);
            }
            if (flag)
            {
                PlayerRoleManager.OnRoleChanged?.Invoke(Hub, playerRoleBase, CurrentRole);
            }
            SpawnProtected.TryGiveProtection(Hub);
        }

        [Server]
        public void ServerSetRole(RoleTypeId newRole, RoleChangeReason reason, RoleSpawnFlags spawnFlags = RoleSpawnFlags.All)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void PlayerRoles.PlayerRoleManager::ServerSetRole(PlayerRoles.RoleTypeId,PlayerRoles.RoleChangeReason,PlayerRoles.RoleSpawnFlags)' called when server was not active");
                return;
            }
            PlayerRoleManager.OnServerRoleSet?.Invoke(Hub, newRole, reason);
            InitializeNewRole(newRole, reason, spawnFlags);
            _sendNextFrame = true;
        }
    }
}
