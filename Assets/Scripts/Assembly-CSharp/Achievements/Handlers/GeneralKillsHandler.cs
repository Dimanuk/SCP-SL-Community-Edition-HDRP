using GameCore;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.MicroHID;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp096;
using PlayerStatsSystem;

namespace Achievements.Handlers
{
    public class GeneralKillsHandler : AchievementHandlerBase
    {
        private const int AnomalouslyEfficientTime = 60;

        internal override void OnInitialize()
        {
            PlayerStats.OnAnyPlayerDied += HandleDeath;
        }

        private static void HandleDeath(ReferenceHub deadPlayer, DamageHandlerBase handler)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (RoundStart.RoundStartTimer.Elapsed.TotalSeconds < AnomalouslyEfficientTime && deadPlayer != null)
            {
                Send(deadPlayer, AchievementName.WowReally);
            }

            if (handler is AttackerDamageHandler attackerDamageHandler && attackerDamageHandler.Attacker.Hub != null)
            {
                HandleAttackerKill(deadPlayer, attackerDamageHandler);
            }

            if (deadPlayer.GetRoleId() == RoleTypeId.Scp096 && deadPlayer.roleManager.CurrentRole is Scp096Role scp096Role && scp096Role.IsRageState(Scp096RageState.Distressed))
            {
                Send(deadPlayer, AchievementName.InvoluntaryRageQuit);
                return;
            }

            switch (handler)
            {
                case UniversalDamageHandler universalDamageHandler:
                    if (universalDamageHandler.TranslationId == DeathTranslations.PocketDecay.Id)
                    {
                        Send(deadPlayer, AchievementName.Newb);
                    }
                    if (universalDamageHandler.TranslationId == DeathTranslations.Falldown.Id)
                    {
                        Send(deadPlayer, AchievementName.Gravity);
                    }
                    if (universalDamageHandler.TranslationId == DeathTranslations.Tesla.Id)
                    {
                        if (deadPlayer.inventory.CurInstance is MicroHIDItem)
                        {
                            Send(deadPlayer, AchievementName.Overcurrent);
                        }
                        Send(deadPlayer, AchievementName.DeepFried);
                    }
                    break;

                case ScpDamageHandler scpDamageHandler:
                    if (scpDamageHandler.Attacker.Role == RoleTypeId.Scp173)
                    {
                        Send(deadPlayer, AchievementName.FirstTime);
                    }
                    break;

                case MicroHidDamageHandler microHidDamageHandler:
                    if (deadPlayer.IsSCP() && microHidDamageHandler.Attacker.Hub != null)
                    {
                        Send(microHidDamageHandler.Attacker.Hub, AchievementName.MicrowaveMeal);
                    }
                    break;

                case RecontainmentDamageHandler recontainmentDamageHandler:
                    if (deadPlayer.GetRoleId() == RoleTypeId.Scp106)
                    {
                        Send(recontainmentDamageHandler.Attacker.Hub, AchievementName.SecureContainProtect);
                    }
                    break;

                case ExplosionDamageHandler explosionDamageHandler:
                    if (explosionDamageHandler.Attacker.Hub == deadPlayer)
                    {
                        Send(deadPlayer, AchievementName.Rocket);
                    }
                    break;
            }
        }

        private static void HandleAttackerKill(ReferenceHub deadPlayer, AttackerDamageHandler faDH)
        {
            ReferenceHub hub = faDH.Attacker.Hub;
            RoleTypeId roleId = deadPlayer.GetRoleId();

            if (hub.IsSCP() && deadPlayer.inventory.CurInstance is MicroHIDItem microHid && (microHid.State == HidState.PoweringUp || microHid.State == HidState.Firing))
            {
                Send(hub, AchievementName.IllPassThanks);
            }

            switch (faDH.Attacker.Role)
            {
                case RoleTypeId.ClassD:
                    if (roleId == RoleTypeId.Scientist && deadPlayer.inventory.CurInstance is KeycardItem)
                    {
                        Send(hub, AchievementName.Betrayal);
                    }
                    break;

                case RoleTypeId.Scientist:
                    if (roleId == RoleTypeId.ClassD)
                    {
                        Send(hub, AchievementName.JustResources);
                    }
                    else if (deadPlayer.IsSCP())
                    {
                        Send(hub, AchievementName.SomethingDoneRight);
                    }
                    break;
            }
        }

        private static void Send(ReferenceHub hub, AchievementName name)
        {
            if (hub != null)
            {
                AchievementHandlerBase.ServerAchieve(hub.networkIdentity.connectionToClient, name);
            }
        }
    }
}