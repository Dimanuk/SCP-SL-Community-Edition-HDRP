using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoles.Voice
{
    public class VoiceChatReceivePrefs : MonoBehaviour
    {
        [Serializable]
        private class ToggleFlagPair
        {
            public Toggle Checkbox;
            public GroupMuteFlags Flag;
        }

        public struct GroupMuteFlagsMessage : NetworkMessage
        {
            public byte Flags;
        }

        private const string PrefsKey = "ClientMuteFlags";

        [SerializeField]
        private ToggleFlagPair[] _checkboxes;

        private static GroupMuteFlags _loadedFlags;

        private static readonly Dictionary<NetworkConnection, GroupMuteFlags> RememberedFlags = new();
        private static readonly HashSet<VoiceChatReceivePrefs> ActiveInstances = new();

        public static event Action<NetworkConnection, GroupMuteFlags> OnFlagsReceived;

        private void OnEnable()
        {
            ActiveInstances.Add(this);
            SetCheckboxes();
        }

        private void OnDisable()
        {
            ActiveInstances.Remove(this);
        }

        private void SetCheckboxes()
        {
            if (_checkboxes == null)
                return;

            foreach (var pair in _checkboxes)
            {
                if (pair?.Checkbox != null)
                {
                    bool isOn = (_loadedFlags & pair.Flag) != 0;
                    pair.Checkbox.SetIsOnWithoutNotify(isOn);
                }
            }
        }

        public void OnToggled()
        {
            if (_checkboxes == null)
                return;

            GroupMuteFlags newFlags = GroupMuteFlags.None;

            foreach (var pair in _checkboxes)
            {
                if (pair?.Checkbox != null && pair.Checkbox.isOn)
                    newFlags |= pair.Flag;
            }

            _loadedFlags = newFlags;

            PlayerPrefsSl.Set(PrefsKey, (int)_loadedFlags);
            ClientSendMessage();

            foreach (var instance in ActiveInstances)
            {
                instance?.SetCheckboxes();
            }
        }

        private static void ClientSendMessage()
        {
            NetworkClient.Send(new GroupMuteFlagsMessage { Flags = (byte)_loadedFlags });
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += OnClientReady;
        }

        private static void OnClientReady()
        {
            NetworkServer.ReplaceHandler<GroupMuteFlagsMessage>(ProcessMessage, requireAuthentication: true);
            _loadedFlags = (GroupMuteFlags)PlayerPrefsSl.Get(PrefsKey, 0);
            ClientSendMessage();
        }

        private static void ProcessMessage(NetworkConnection conn, GroupMuteFlagsMessage msg)
        {
            GroupMuteFlags flags = (GroupMuteFlags)msg.Flags;

            OnFlagsReceived?.Invoke(conn, flags);

            if (conn != null)
                RememberedFlags[conn] = flags;

            if (conn?.identity != null &&
                ReferenceHub.TryGetHubNetID(conn.identity.netId, out ReferenceHub hub) &&
                hub.roleManager.CurrentRole is IVoiceRole voiceRole)
            {
                if (voiceRole.VoiceModule != null)
                    voiceRole.VoiceModule.ReceiveFlags = flags;
            }
        }

        public static GroupMuteFlags GetFlagsForUser(ReferenceHub hub)
        {
            if (hub == null)
                return GroupMuteFlags.None;

            var conn = hub.connectionToClient;
            if (conn != null && RememberedFlags.TryGetValue(conn, out GroupMuteFlags flags))
                return flags;

            return GroupMuteFlags.None;
        }
    }
}