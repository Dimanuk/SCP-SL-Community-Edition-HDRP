using System.Collections.Generic;
using Interactables.Interobjects.DoorUtils;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Rewards
{
    public static class TeammateProtectionRewards
    {
        /*===============================================================
          Вложенный тип — отслеживаемый союзник
        ===============================================================*/
        private class TrackedTeammate
        {
            public readonly ReferenceHub Hub;
            public readonly FpcStandardRoleBase Role;

            private readonly Dictionary<uint, double> _attackers;

            private const float MinDamage      = 100f;
            private const float TimeTolerance  = 6f;
            private const int   AttackersLimit = 5;

            private double _lastDamageTime;
            private float  _damageReceived;

            private static readonly Vector3[] AttackersNonAlloc = new Vector3[5];

            public TrackedTeammate(ReferenceHub ply)
            {
                Hub  = ply;
                Role = ply.roleManager.CurrentRole as FpcStandardRoleBase;

                _attackers = new Dictionary<uint, double>();
                Hub.playerStats.OnThisPlayerDamaged += OnDamaged;
            }

            public void Unsubscribe()
            {
                if (Hub != null)
                    Hub.playerStats.OnThisPlayerDamaged -= OnDamaged;
            }

            public int GetAttackersNonAlloc(out Vector3[] attackersPositions)
            {
                attackersPositions = AttackersNonAlloc;

                if (NetworkTime.time > _lastDamageTime || _damageReceived < MinDamage)
                    return 0;

                int count = 0;
                foreach (var attacker in _attackers)
                {
                    if (attacker.Value > _lastDamageTime)
                        continue;

                    if (!ReferenceHub.TryGetHubNetID(attacker.Key, out var hub))
                        continue;

                    if (hub.roleManager.CurrentRole is not IFpcRole fpcRole)
                        continue;

                    AttackersNonAlloc[count] = fpcRole.FpcModule.Position;
                    if (++count >= AttackersLimit)
                        break;
                }

                _attackers.Clear();
                _damageReceived = 0f;
                return count;
            }

            private void OnDamaged(DamageHandlerBase dhb)
            {
                if (dhb is not AttackerDamageHandler adh)
                    return;
                if (dhb is Scp018DamageHandler or ExplosionDamageHandler)
                    return;

                double now = NetworkTime.time;

                if (now > _lastDamageTime)
                    _damageReceived = 0f;

                _damageReceived += adh.DealtHealthDamage;
                _lastDamageTime  = now + TimeTolerance;

                _attackers[adh.Attacker.NetId] = _lastDamageTime;
            }
        }

        /*===============================================================
          Статические поля и инициализация
        ===============================================================*/
        private const float Cooldown = 10f;

        private static readonly int[] Rewards = new int[6] { 0, 10, 15, 25, 40, 60 };

        private static readonly HashSet<TrackedTeammate> Teammates = new HashSet<TrackedTeammate>();
        private static double _grantTargetCooldown;

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            Scp079DoorAbility.OnServerAnyDoorInteraction += CheckBlock;

            PlayerRoleManager.OnRoleChanged += (hub, prev, cur) =>
            {
                if (!NetworkServer.active)
                    return;

                if (ValidateRole(prev))
                    Teammates.RemoveWhere(x => x.Hub == hub);

                if (ValidateRole(cur))
                    Teammates.Add(new TrackedTeammate(hub));
            };

            ReferenceHub.OnPlayerRemoved += hub =>
            {
                if (!NetworkServer.active)
                    return;

                if (ValidateRole(hub.roleManager.CurrentRole))
                    Teammates.RemoveWhere(x => x.Hub == hub);
            };
        }

        /*===============================================================
          Вспомогательные методы
        ===============================================================*/
        private static bool ValidateRole(PlayerRoleBase prb)
        {
            if (prb is FpcStandardRoleBase)
                return prb.Team == Team.SCPs;

            return false;
        }

        /*===============================================================
          Основная логика проверки «блокировки» двери
        ===============================================================*/
        private static void CheckBlock(Scp079Role scp079, DoorVariant dv)
        {
            if (dv.TargetState)
                return;

            DoorLockReason activeLocks = (DoorLockReason)dv.ActiveLocks;

            if ((!DoorLockUtils.HasFlagFast(activeLocks, DoorLockReason.Lockdown079) &&
                 !DoorLockUtils.HasFlagFast(activeLocks, DoorLockReason.Regular079)) ||
                _grantTargetCooldown > NetworkTime.time)
            {
                return;
            }

            int separatedPairs = 0;
            Transform tr = dv.transform;

            foreach (var teammate in Teammates)
            {
                Vector3[] attackerPositions;
                int attackerCount = teammate.GetAttackersNonAlloc(out attackerPositions);

                if (attackerCount == 0)
                    continue;

                bool teammateSide = tr.InverseTransformPoint(teammate.Role.FpcModule.Position).z > 0f;

                for (int i = 0; i < attackerCount; i++)
                {
                    bool attackerSide = tr.InverseTransformPoint(attackerPositions[i]).z > 0f;

                    if (teammateSide != attackerSide)
                        separatedPairs++;
                }
            }

            int rewardIndex = Mathf.Min(separatedPairs, Rewards.Length - 1);
            Scp079RewardManager.GrantExp(scp079,
                                         Rewards[rewardIndex],
                                         Scp079HudTranslation.ExpGainTeammateProtection);

            _grantTargetCooldown = NetworkTime.time + Cooldown;
        }
    }
}
