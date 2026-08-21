using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace PlayerRoles
{
    public static class PlayerRolesNetUtils
    {
        public static readonly Dictionary<uint, NetworkReader> QueuedRoles = new();

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += delegate
            {
                QueuedRoles.Clear();

                NetworkClient.ReplaceHandler((Action<RoleSyncInfo>)((msg) =>
                {
                }), true);

                NetworkClient.ReplaceHandler((Action<RoleSyncInfoPack>)((pack) =>
                {
                }), true);
            };

            ReferenceHub.OnPlayerAdded = (Action<ReferenceHub>)Delegate.Combine(
                ReferenceHub.OnPlayerAdded,
                new Action<ReferenceHub>(HandleSpawnedPlayer)
            );
        }

        private static void HandleSpawnedPlayer(ReferenceHub hub)
        {
            if (NetworkServer.active)
            {
                if (!hub.isLocalPlayer)
                {
                    hub.connectionToClient.Send(new RoleSyncInfoPack(hub));
                }
            }
            else
            {
                if (QueuedRoles.TryGetValue(hub.netId, out NetworkReader value))
                {
                    hub.roleManager.InitializeNewRole(value.ReadRoleType(), RoleChangeReason.None, RoleSpawnFlags.All, value);
                    QueuedRoles.Remove(hub.netId);
                }
            }
        }

        public static void WriteRoleSyncInfo(this NetworkWriter writer, RoleSyncInfo info)
        {
            info.Write(writer);
        }

        public static RoleSyncInfo ReadRoleSyncInfo(this NetworkReader reader)
        {
            return new RoleSyncInfo(reader);
        }

        public static void WriteRoleSyncInfoPack(this NetworkWriter writer, RoleSyncInfoPack info)
        {
            info.WritePlayers(writer);
        }

        public static RoleSyncInfoPack ReadRoleSyncInfoPack(this NetworkReader reader)
        {
            return new RoleSyncInfoPack(reader);
        }
    }
}