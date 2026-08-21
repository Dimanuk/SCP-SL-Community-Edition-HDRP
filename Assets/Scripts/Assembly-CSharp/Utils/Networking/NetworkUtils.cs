using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Networking
{
    public static class NetworkUtils
    {
        private static readonly Dictionary<uint, NetworkIdentity> EmptyDict = new Dictionary<uint, NetworkIdentity>();

        public static Dictionary<uint, NetworkIdentity> SpawnedNetIds
        {
            get
            {
                if (NetworkServer.active) return NetworkServer.spawned;
                if (NetworkClient.active) return NetworkClient.spawned;

                return EmptyDict;
            }
        }

        public static void SendToAuthenticated<T>(this T message, int channelId = 0) where T : struct, NetworkMessage
        {
            message.SendToHubsConditionally((ReferenceHub x) => x != null && x.Mode != ClientInstanceMode.Unverified, channelId);
        }

        public static void SendToHubsConditionally<T>(this T msg, Func<ReferenceHub, bool> predicate, int channelId = 0) where T : struct, NetworkMessage
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning($"[NetworkUtils] SendToHubsConditionally: NetworkServer is not active! Message {typeof(T).Name} ignored.");
                return;
            }

            foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
            {
                if (referenceHub != null && predicate(referenceHub))
                {
                    NetworkConnectionToClient conn = referenceHub.connectionToClient;

                    if (conn != null && conn.isReady)
                    {
                        conn.Send(msg, channelId);
                    }
                }
            }
        }
    }
}