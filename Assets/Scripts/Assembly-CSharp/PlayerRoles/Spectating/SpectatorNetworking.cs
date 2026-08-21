using System;
using Mirror;
using Utils.Networking;

namespace PlayerRoles.Spectating
{
    public static class SpectatorNetworking
    {
        public struct SpectatedNetIdSyncMessage : NetworkMessage
        {
            public uint NetId;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += () =>
            {
                NetworkServer.ReplaceHandler<SpectatedNetIdSyncMessage>(
                    (NetworkConnectionToClient conn, SpectatedNetIdSyncMessage msg) =>
                    {
                        if (ReferenceHub.TryGetHubNetID(conn.identity.netId, out ReferenceHub spectatorHub) &&
                            spectatorHub.roleManager.CurrentRole is SpectatorRole spectatorRole)
                        {
                            ReferenceHub previousTarget = null;
                            ReferenceHub newTarget = null;

                            ReferenceHub.TryGetHubNetID(spectatorRole.SyncedSpectatedNetId, out previousTarget);
                            ReferenceHub.TryGetHubNetID(msg.NetId, out newTarget);
                            spectatorRole.SyncedSpectatedNetId = msg.NetId;
                        }
                    });
            };

            SpectatorTargetTracker.OnTargetChanged += () =>
 {
     uint netIdToSend = 0;

     if (SpectatorTargetTracker.TryGetTrackedPlayer(out ReferenceHub trackedHub))
     {
         netIdToSend = trackedHub.netId;
     }

     NetworkClient.Send(new SpectatedNetIdSyncMessage { NetId = netIdToSend });
 };
        }

        public static void SendToSpectatorsOf<T>(this T msg, ReferenceHub target, bool includeTarget = false)
            where T : struct, NetworkMessage
        {
            NetworkUtils.SendToHubsConditionally(msg, hub =>
                target.IsSpectatedBy(hub) || (includeTarget && hub == target));
        }

        public static void ForeachSpectatorOf(ReferenceHub target, Action<ReferenceHub> action)
        {
            foreach (ReferenceHub hub in ReferenceHub.AllHubs)
            {
                if (target.IsSpectatedBy(hub))
                {
                    action(hub);
                }
            }
        }

        public static bool IsSpectatedBy(this ReferenceHub target, ReferenceHub spectator)
        {
            if (spectator?.roleManager?.CurrentRole is SpectatorRole spectatorRole)
            {
                return spectatorRole.SyncedSpectatedNetId == target.netId;
            }
            return false;
        }

        public static bool IsLocallySpectated(this ReferenceHub target)
        {
            if (SpectatorTargetTracker.TryGetTrackedPlayer(out ReferenceHub currentlyTracked))
            {
                return currentlyTracked == target;
            }
            return false;
        }
    }
}