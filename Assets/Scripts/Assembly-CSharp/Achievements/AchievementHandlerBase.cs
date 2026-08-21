using Mirror;
using System;

namespace Achievements
{
    public abstract class AchievementHandlerBase
    {
        internal virtual void OnInitialize() { }

        internal virtual void OnRoundStarted() { }

        public static void ClientAchieve(AchievementName targetAchievement)
        {
            if (AchievementManager.Achievements.TryGetValue(targetAchievement, out var achievement))
            {
                achievement.Achieve();
            }
        }

        public static void ServerAchieve(NetworkConnection conn, AchievementName targetAchievement)
        {
            if (!NetworkServer.active)
            {
                throw new InvalidOperationException("Method ServerAchieve can only be executed on the server.");
            }

            if (!conn.identity.isLocalPlayer)
            {
                conn.Send(new AchievementManager.AchievementMessage
                {
                    AchievementId = (byte)targetAchievement
                });
            }
            else
            {
                // Listen-server host: a self-send is pointless, so grant the achievement
                // directly on the local client instead (remote clients get it via the
                // AchievementMessage -> AchievementManager.ClientMessageReceived path).
                ClientAchieve(targetAchievement);
            }
        }

        protected AchievementHandlerBase()
        {
            
        }
    }
}