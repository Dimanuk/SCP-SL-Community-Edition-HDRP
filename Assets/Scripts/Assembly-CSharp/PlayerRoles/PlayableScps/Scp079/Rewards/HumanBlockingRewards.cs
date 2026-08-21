using System.Collections.Generic;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using Mirror;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Rewards
{
    public static class HumanBlockingRewards
    {
        private const float MinDot = 0.5f;
        private const float Cooldown = 5f;
        private const int Reward = 5;
        private const int SqrDistanceCutoff = 400;

        private static double _lastBlockTime;

        private static readonly HashSet<FirstPersonMovementModule> RoomScps = new HashSet<FirstPersonMovementModule>();
        private static readonly HashSet<FirstPersonMovementModule> RoomHumans = new HashSet<FirstPersonMovementModule>();

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            Scp079DoorAbility.OnServerAnyDoorInteraction += ProcessBlockage;
        }

        private static void ProcessBlockage(Scp079Role role, DoorVariant dv)
        {
            if (dv.TargetState)
                return;

            if (_lastBlockTime + Cooldown > NetworkTime.time)
                return;

            Vector3 doorPos = dv.transform.position;
            RoomIdentifier[] rooms = dv.Rooms;

            for (int i = 0; i < rooms.Length; i++)
            {
                if (CheckRoom(rooms[i], doorPos))
                {
                    Scp079RewardManager.GrantExp(role, Reward, Scp079HudTranslation.ExpGainBlockingHuman);
                    _lastBlockTime = NetworkTime.time;
                    break;
                }
            }
        }

        private static bool CheckRoom(RoomIdentifier room, Vector3 doorPos)
        {
            RoomScps.Clear();
            RoomHumans.Clear();

            bool hasScps = false;
            bool hasHumans = false;

            foreach (ReferenceHub hub in ReferenceHub.AllHubs)
            {
                if (hub.roleManager.CurrentRole is not IFpcRole fpcRole)
                    continue;

                FirstPersonMovementModule fpcModule = fpcRole.FpcModule;

                if (RoomIdUtils.RoomAtPosition(fpcModule.Position) != room)
                    continue;

                if (hub.IsSCP())
                {
                    RoomScps.Add(fpcModule);
                    hasScps = true;
                }
                else
                {
                    RoomHumans.Add(fpcModule);
                    hasHumans = true;
                }
            }

            if (!hasHumans || !hasScps)
                return false;

            foreach (FirstPersonMovementModule scp in RoomScps)
            {
                Vector3 scpMoveDir = NormalizeIgnoreY(scp.Motor.MoveDirection.normalized);

                Vector3 toDoor = NormalizeIgnoreY(doorPos - scp.Position);

                if (Vector3.Dot(scpMoveDir, toDoor) < MinDot)
                    continue;

                foreach (FirstPersonMovementModule human in RoomHumans)
                {
                    Vector3 toHuman = human.Position - scp.Position;

                    if (toHuman.sqrMagnitude > SqrDistanceCutoff)
                        continue;

                    if (Vector3.Dot(scpMoveDir, NormalizeIgnoreY(toHuman)) >= MinDot)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static Vector3 NormalizeIgnoreY(Vector3 direction)
        {
            direction.y = 0f;
            return direction.normalized;
        }
    }
}
