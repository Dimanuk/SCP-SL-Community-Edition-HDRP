using CustomPlayerEffects;
using InventorySystem.Disarming;
using InventorySystem.Items;
using GameCore;
using Mirror;
using PlayerRoles;

namespace Achievements.Handlers
{
    public class EscapeHandler : AchievementHandlerBase
    {
        private const int EscapeArtistTime = 180;

        internal override void OnInitialize()
        {
            Escape.OnServerPlayerEscape += OnEscaped;
            PlayerRoleManager.OnServerRoleSet += OnRoleSet;
        }

        private static void OnRoleSet(ReferenceHub userHub, RoleTypeId newId, RoleChangeReason reason)
        {
            if (NetworkServer.active && reason == RoleChangeReason.Escaped && RoundStart.RoundStartTimer.Elapsed.TotalSeconds <= EscapeArtistTime)
            {
                AchievementHandlerBase.ServerAchieve(userHub.networkIdentity.connectionToClient, AchievementName.EscapeArtist);
            }
        }

        private static void OnEscaped(ReferenceHub userHub)
        {
            if (userHub == null || userHub.inventory.IsDisarmed())
            {
                return;
            }

            NetworkConnectionToClient connectionToClient = userHub.networkIdentity.connectionToClient;
            PlayerRoleBase currentRole = userHub.roleManager.CurrentRole;
            if (userHub.playerEffectsController.GetEffect<Scp207>().IsEnabled)
            {
                AchievementHandlerBase.ServerAchieve(connectionToClient, AchievementName.Escape207);
            }
            if (currentRole.RoleTypeId == RoleTypeId.Scientist)
            {
                AchievementHandlerBase.ServerAchieve(connectionToClient, AchievementName.ForScience);
            }
            else
            {
                if (currentRole.RoleTypeId != RoleTypeId.ClassD)
                {
                    return;
                }
                AchievementHandlerBase.ServerAchieve(connectionToClient, AchievementName.ItsAlwaysLeft);
                int num = 0;
                foreach (ItemBase value in userHub.inventory.UserInventory.Items.Values)
                {
                    if (value.Category == ItemCategory.SCPItem)
                    {
                        num++;
                    }
                }
                if (num >= 2)
                {
                    AchievementHandlerBase.ServerAchieve(connectionToClient, AchievementName.PropertyOfChaos);
                }
            }
        }
    }
}
