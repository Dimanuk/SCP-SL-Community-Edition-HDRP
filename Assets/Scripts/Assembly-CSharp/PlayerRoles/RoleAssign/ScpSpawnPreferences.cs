using System.Collections.Generic;
using Mirror;
using PlayerRoles.PlayableScps;
using UnityEngine;

namespace PlayerRoles.RoleAssign
{
    public static class ScpSpawnPreferences
    {
        public struct SpawnPreferences : NetworkMessage
        {
            public Dictionary<RoleTypeId, int> Preferences;

            public SpawnPreferences(bool autoSetup)
            {
                Preferences = new Dictionary<RoleTypeId, int>();
                if (!autoSetup) return;

                foreach (KeyValuePair<RoleTypeId, PlayerRoleBase> allRole in PlayerRoleLoader.AllRoles)
                {
                    if (allRole.Value is ISpawnableScp)
                    {
                        Preferences[allRole.Key] = GetPreference(allRole.Key);
                    }
                }
            }
        }

        public static readonly Dictionary<NetworkConnectionToClient, SpawnPreferences> Preferences = new Dictionary<NetworkConnectionToClient, SpawnPreferences>();

        public const int MaxPreference = 5;

        private const string PrefsPrefix = "SpawnPreference_Role_";

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += delegate
            {
                Preferences.Clear();
                NetworkServer.ReplaceHandler<SpawnPreferences>(OnMessageReceived);
                NetworkClient.Send(new SpawnPreferences(autoSetup: true));
                NetworkServer.OnDisconnectedEvent += OnClientDisconnected;
            };
        }

        private static void OnClientDisconnected(NetworkConnectionToClient conn)
        {
            Preferences.Remove(conn);
        }

        private static int ClampPreference(int val)
        {
            return Mathf.Clamp(val, -5, 5);
        }

        private static void OnMessageReceived(NetworkConnectionToClient conn, SpawnPreferences msg)
        {
            if (msg.Preferences == null)
            {
                return;
            }

            SpawnPreferences sanitized = new SpawnPreferences(autoSetup: false);
            foreach (KeyValuePair<RoleTypeId, int> kvp in msg.Preferences)
            {
                if (PlayerRoleLoader.TryGetRoleTemplate<PlayerRoleBase>(kvp.Key, out var roleBase) && roleBase is ISpawnableScp)
                {
                    sanitized.Preferences[kvp.Key] = ClampPreference(kvp.Value);
                }
            }
            Preferences[conn] = sanitized;
        }

        public static int GetPreference(RoleTypeId role)
        {
            int num = (int)role;
            return ClampPreference(PlayerPrefsSl.Get(PrefsPrefix + num, 0));
        }

        public static void SavePreference(RoleTypeId role, int value)
        {
            int num = (int)role;
            PlayerPrefsSl.Set(PrefsPrefix + num, value);
            if (NetworkClient.active)
            {
                NetworkClient.Send(new SpawnPreferences(autoSetup: true));
            }
        }
    }
}
