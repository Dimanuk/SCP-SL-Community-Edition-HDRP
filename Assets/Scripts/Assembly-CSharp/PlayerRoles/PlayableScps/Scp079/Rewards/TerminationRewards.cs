using MapGeneration;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.PlayableScps.Scp106;
using PlayerStatsSystem;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Rewards
{
    public static class TerminationRewards
    {
        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            PlayerStats.OnAnyPlayerDied += (ply, handler) =>
            {
                bool isScpKill = handler is AttackerDamageHandler attackerDamageHandler
                                 && attackerDamageHandler.Attacker.Role.GetTeam() == Team.SCPs;

                bool isTesla = handler is UniversalDamageHandler universalDamageHandler
                                && universalDamageHandler.TranslationId == DeathTranslations.Tesla.Id;

                if (isScpKill || isTesla)
                    OnHumanTerminated(ply);
            };
            Scp106Attack.OnPlayerTeleported += OnHumanTerminated;
        }

        private static void OnHumanTerminated(ReferenceHub ply)
        {
            if (!NetworkServer.active
                || ply.roleManager.CurrentRole is not HumanRole humanRole
                || !TryGetReward(humanRole.RoleTypeId, out var gainReason, out var amount))
            {
                return;
            }

            RoomIdentifier room = RoomIdUtils.RoomAtPositionRaycasts(humanRole.FpcModule.Position);
            if (room == null || Scp079RewardManager.GrantExpForRoom(room, amount, gainReason))
                return;

            foreach (Scp079Role instance in Scp079Role.ActiveInstances)
            {
                if (instance.CurrentCamera.Room != room)
                    continue;

                Scp079RewardManager.GrantExp(instance,
                                               amount / 2,
                                               Scp079HudTranslation.ExpGainWitnessingTermination);
            }
        }

        public static bool TryGetReward(RoleTypeId rt,
                                        out Scp079HudTranslation gainReason,
                                        out int amount)
        {
            switch (rt.GetTeam())
            {
                case Team.ChaosInsurgency:
                    gainReason = Scp079HudTranslation.ExpGainTerminationChaos;
                    amount     = 30;
                    return true;

                case Team.ClassD:
                    gainReason = Scp079HudTranslation.ExpGainTerminationClassD;
                    amount     = 40;
                    return true;

                case Team.Scientists:
                    gainReason = Scp079HudTranslation.ExpGainTerminationScientist;
                    amount     = 50;
                    return true;

                case Team.OtherAlive:
                    gainReason = Scp079HudTranslation.ExpGainTerminationOther;
                    amount     = 30;
                    return true;

                case Team.FoundationForces:
                    if (rt == RoleTypeId.FacilityGuard)
                    {
                        amount     = 25;
                        gainReason = Scp079HudTranslation.ExpGainTerminationGuard;
                    }
                    else
                    {
                        amount     = 30;
                        gainReason = Scp079HudTranslation.ExpGainTerminationNtf;
                    }
                    return true;

                default:
                    amount     = 0;
                    gainReason = Scp079HudTranslation.Zoom;
                    return false;
            }
        }
    }
}