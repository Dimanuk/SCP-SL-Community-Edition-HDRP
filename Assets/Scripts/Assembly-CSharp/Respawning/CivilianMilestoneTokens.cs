using System.Collections.Generic;
using MapGeneration;
using PlayerRoles;
using UnityEngine;

namespace Respawning
{
    public static class CivilianMilestoneTokens
    {
        private const float MinAliveTime = 10f;

        private static readonly global::System.Collections.Generic.Dictionary<global::MapGeneration.FacilityZone, float> RewardsPerZone = new global::System.Collections.Generic.Dictionary<global::MapGeneration.FacilityZone, float>
        {
            [global::MapGeneration.FacilityZone.HeavyContainment] = 0.1f,
            [global::MapGeneration.FacilityZone.Entrance] = 0.15f,
            [global::MapGeneration.FacilityZone.Surface] = 0.2f
        };


        private static readonly global::System.Collections.Generic.Dictionary<global::PlayerRoles.HumanRole, global::System.Collections.Generic.HashSet<global::MapGeneration.FacilityZone>> Tracker = new global::System.Collections.Generic.Dictionary<global::PlayerRoles.HumanRole, global::System.Collections.Generic.HashSet<global::MapGeneration.FacilityZone>>();

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            global::PlayerRoles.PlayerRoleManager.OnRoleChanged += OnRoleChanged;
            CustomNetworkManager.OnClientReady += Tracker.Clear;
            StaticUnityMethods.OnUpdate += delegate
            {
                if (global::Mirror.NetworkServer.active)
                {
                    global::Utils.NonAllocLINQ.HashsetExtensions.ForEach(ReferenceHub.AllHubs, UpdatePlayer);
                }
            };
        }


        private static void OnRoleChanged(ReferenceHub hub, global::PlayerRoles.PlayerRoleBase prev, global::PlayerRoles.PlayerRoleBase cur)
        {
            if (global::Mirror.NetworkServer.active && prev is global::PlayerRoles.HumanRole key && Tracker.TryGetValue(key, out var value))
            {
                value.Clear();
            }
        }

        private static void UpdatePlayer(ReferenceHub hub)
        {
            if (!(hub.roleManager.CurrentRole is global::PlayerRoles.HumanRole humanRole))
            {
                return;
            }
            global::Respawning.SpawnableTeamType spawnableTeamType;
            global::Respawning.SpawnableTeamType spawnableTeamType2;
            switch (humanRole.Team)
            {
                default:
                    return;
                case global::PlayerRoles.Team.Scientists:
                    spawnableTeamType = global::Respawning.SpawnableTeamType.NineTailedFox;
                    spawnableTeamType2 = global::Respawning.SpawnableTeamType.ChaosInsurgency;
                    break;
                case global::PlayerRoles.Team.ClassD:
                    spawnableTeamType = global::Respawning.SpawnableTeamType.ChaosInsurgency;
                    spawnableTeamType2 = global::Respawning.SpawnableTeamType.NineTailedFox;
                    break;
            }
            if (!(humanRole.ActiveTime < 10f))
            {
                global::UnityEngine.Vector3Int key = global::MapGeneration.RoomIdUtils.PositionToCoords(humanRole.FpcModule.Position);
                if (global::MapGeneration.RoomIdentifier.RoomsByCoordinates.TryGetValue(key, out var value) && !(value == null) && Tracker.GetOrAdd(humanRole, () => new global::System.Collections.Generic.HashSet<global::MapGeneration.FacilityZone>()).Add(value.Zone) && RewardsPerZone.TryGetValue(value.Zone, out var value2))
                {
                    global::Respawning.RespawnTokensManager.GrantTokens(global::InventorySystem.Disarming.DisarmedPlayers.IsDisarmed(hub.inventory) ? spawnableTeamType2 : spawnableTeamType, value2);
                }
            }
        }
    }
}
