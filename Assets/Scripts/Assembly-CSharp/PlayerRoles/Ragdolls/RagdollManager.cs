using System;
using System.Collections.Generic;
using Mirror;
using PlayerStatsSystem;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace PlayerRoles.Ragdolls
{
    public static class RagdollManager
    {
        private static readonly HashSet<NetworkIdentity> CachedRagdollPrefabs = new HashSet<NetworkIdentity>();
        public static int CleanupTime { get; set; }

        private static HashSet<NetworkIdentity> AllRagdollPrefabs
        {
            get
            {
                CachedRagdollPrefabs.Clear();
                foreach (KeyValuePair<RoleTypeId, PlayerRoleBase> allRole in PlayerRoleLoader.AllRoles)
                {
                    if (allRole.Value is IRagdollRole ragdollRole)
                        CachedRagdollPrefabs.Add(ragdollRole.Ragdoll.netIdentity);
                }

                return CachedRagdollPrefabs;
            }
        }

        public static event Action<BasicRagdoll> OnRagdollSpawned;
        public static event Action<BasicRagdoll> OnRagdollRemoved;

        internal static void OnSpawnedRagdoll(BasicRagdoll ragdoll)
        {
            OnRagdollSpawned?.Invoke(ragdoll);
        }

        internal static void OnRemovedRagdoll(BasicRagdoll ragdoll)
        {
            OnRagdollRemoved?.Invoke(ragdoll);
        }

        public static BasicRagdoll ServerSpawnRagdoll(ReferenceHub owner, DamageHandlerBase handler)
        {
            if (!NetworkServer.active || owner == null)
                return null;

            if (!(owner.roleManager.CurrentRole is IRagdollRole ragdollRole))
                return null;

            GameObject gameObject = GameObject.Instantiate(ragdollRole.Ragdoll.gameObject);

            if (gameObject.TryGetComponent<BasicRagdoll>(out BasicRagdoll component))
            {
                Transform transform = ragdollRole.Ragdoll.transform;
                component.Info = new RagdollData(owner, handler, transform.localPosition, transform.localRotation);
            }
            else
            {
                component = null;
            }

            NetworkServer.Spawn(gameObject);
            return component;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += delegate
            {
                HashsetExtensions.ForEach(AllRagdollPrefabs, delegate(NetworkIdentity x)
                {
                    if (x == null) return;
                    NetworkClient.prefabs[x.assetId] = x.gameObject;
                });
            };

            InventorySystem.Inventory.OnLocalClientStarted += delegate
            {
                CleanupTime = PlayerPrefsSl.Get("ragdoll_cleanup", 0);
            };
        }
    }
}