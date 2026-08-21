using System.Collections.Generic;
using MapGeneration;
using Mirror;
using UnityEngine;

namespace PlayerRoles.FirstPersonControl.Spawnpoints
{
    public static class RoleSpawnpointManager
    {
        private struct SpawnpointDefinition
        {
            public RoleTypeId[] Roles;

            public ISpawnpointHandler[] CompatibleSpawnpoints;

            public SpawnpointDefinition(params RoleTypeId[] roles)
            {
                Roles = roles;
                CompatibleSpawnpoints = null;
            }

            public SpawnpointDefinition SetSpawnpoints(params ISpawnpointHandler[] spawnpoints)
            {
                CompatibleSpawnpoints = spawnpoints;
                return this;
            }
        }

        private static readonly SpawnpointDefinition[] DefinedSpawnpoints = new SpawnpointDefinition[1] { new SpawnpointDefinition(RoleTypeId.ClassD).SetSpawnpoints(new RoomRoleSpawnpoint(new Vector3(-6.18f, 0.91f, -4.23f), 5f, 0f, 26.26f, 0.73f, 7, 1, RoomName.LczClassDSpawn), new RoomRoleSpawnpoint(new Vector3(-6.18f, 0.91f, 4.23f), 175f, 0f, 26.26f, 0.73f, 7, 1, RoomName.LczClassDSpawn)) };

        public static bool TryGetSpawnpointForRole(RoleTypeId role, out ISpawnpointHandler spawnpoint)
        {
            bool flag = false;
            List<ISpawnpointHandler> list = new List<ISpawnpointHandler>();
            SpawnpointDefinition[] definedSpawnpoints = DefinedSpawnpoints;
            for (int i = 0; i < definedSpawnpoints.Length; i++)
            {
                SpawnpointDefinition spawnpointDefinition = definedSpawnpoints[i];
                if (spawnpointDefinition.Roles.Contains(role))
                {
                    flag = true;
                    list.AddRange(spawnpointDefinition.CompatibleSpawnpoints);
                }
            }
            spawnpoint = (flag ? list.RandomItem() : null);
            return flag;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            PlayerRoleManager.OnRoleChanged += delegate (ReferenceHub hub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
            {
                if (NetworkServer.active && newRole is IFpcRole { SpawnpointHandler: not null } fpcRole && fpcRole.SpawnpointHandler.TryGetSpawnpoint(out var position, out var horizontalRot) && newRole.ServerSpawnFlags.HasFlag(RoleSpawnFlags.UseSpawnpoint))
                {
                    hub.transform.position = position;
                    fpcRole.FpcModule.MouseLook.CurrentHorizontal = horizontalRot;
                }
            };
        }
    }
}
