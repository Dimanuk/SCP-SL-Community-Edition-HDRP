namespace VoiceChat
{
    public static class VoiceChatMutes
    {
        private const string Filename = "mutes.txt";

        private const string IntercomPrefix = "ICOM-";

        private static string _path;

        private static bool _everLoaded;

        private static readonly global::System.Collections.Generic.HashSet<string> Mutes = new global::System.Collections.Generic.HashSet<string>();

        private static readonly global::System.Collections.Generic.Dictionary<ReferenceHub, global::VoiceChat.VcMuteFlags> Flags = new global::System.Collections.Generic.Dictionary<ReferenceHub, global::VoiceChat.VcMuteFlags>();

        public static event global::System.Action<ReferenceHub, global::VoiceChat.VcMuteFlags> OnFlagsSet;

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            global::GameCore.ConfigFile.OnConfigReloaded = (global::System.Action)global::System.Delegate.Combine(global::GameCore.ConfigFile.OnConfigReloaded, new global::System.Action(LoadMutes));
            CustomNetworkManager.OnClientReady += delegate
            {
                if (!_everLoaded)
                {
                    LoadMutes();
                }
            };
            ReferenceHub.OnPlayerRemoved = (global::System.Action<ReferenceHub>)global::System.Delegate.Combine(ReferenceHub.OnPlayerRemoved, (global::System.Action<ReferenceHub>)delegate (ReferenceHub hub)
            {
                if (GetFlags(hub) != global::VoiceChat.VcMuteFlags.None)
                {
                    global::VoiceChat.VoiceChatMutes.OnFlagsSet?.Invoke(hub, global::VoiceChat.VcMuteFlags.None);
                }
                Flags.Remove(hub);
            });
            CharacterClassManager.OnSyncedUserIdAssigned += delegate (ReferenceHub hub)
            {
                if (!global::Mirror.NetworkServer.active && QueryLocalMute(hub.characterClassManager.SyncedUserId))
                {
                    SetFlags(hub, global::VoiceChat.VcMuteFlags.LocalRegular);
                }
            };
            LoadMutes();
        }

        private static void LoadMutes()
        {
            _path = global::GameCore.ConfigSharing.Paths[1];
            if (string.IsNullOrEmpty(_path))
            {
                return;
            }
            _path += "mutes.txt";
            _everLoaded = true;
            try
            {
                using (global::System.IO.StreamReader streamReader = new global::System.IO.StreamReader(_path))
                {
                    while (true)
                    {
                        string text = streamReader.ReadLine();
                        if (text != null)
                        {
                            if (TryValidateId(text, intercom: false, out var validated))
                            {
                                Mutes.Add(validated);
                            }
                            continue;
                        }
                        break;
                    }
                }
            }
            catch
            {
                global::GameCore.Console.AddLog("Can't load the mute file!", global::UnityEngine.Color.yellow);
            }
        }

        private static bool TryValidateId(string raw, bool intercom, out string validated)
        {
            validated = raw?.Trim();
            if (string.IsNullOrEmpty(raw))
            {
                return false;
            }
            if (intercom)
            {
                validated = "ICOM-" + validated;
            }
            return true;
        }

        private static bool TryGetHub(string userId, out ReferenceHub hub)
        {
            return global::Utils.NonAllocLINQ.HashsetExtensions.TryGetFirst(ReferenceHub.AllHubs, (ReferenceHub x) => CheckHub(x, userId), out hub);
        }

        private static bool CheckHub(ReferenceHub hub, string id)
        {
            CharacterClassManager characterClassManager = hub.characterClassManager;
            if (!(characterClassManager.UserId == id))
            {
                if (global::Mirror.NetworkServer.active)
                {
                    return characterClassManager.SyncedUserId == id;
                }
                return false;
            }
            return true;
        }

        private static global::VoiceChat.VcMuteFlags GetLocalFlag(bool intercom)
        {
            if (!intercom)
            {
                return global::VoiceChat.VcMuteFlags.LocalRegular;
            }
            return global::VoiceChat.VcMuteFlags.LocalIntercom;
        }

        public static bool QueryLocalMute(string userId, bool intercom = false)
        {
            if (TryValidateId(userId, intercom, out var validated))
            {
                return Mutes.Contains(validated);
            }
            return false;
        }

        public static void IssueLocalMute(string userId, bool intercom = false)
        {
            if (TryValidateId(userId, intercom, out var validated) && Mutes.Add(validated))
            {
                global::System.IO.File.AppendAllText(_path, "\r\n" + validated);
                if (TryGetHub(userId, out var hub))
                {
                    SetFlags(hub, GetFlags(hub) | GetLocalFlag(intercom));
                }
            }
        }

        public static void RevokeLocalMute(string userId, bool intercom = false)
        {
            if (TryValidateId(userId, intercom, out var validated) && Mutes.Remove(validated))
            {
                FileManager.WriteToFile(Mutes, _path);
                if (TryGetHub(userId, out var hub))
                {
                    SetFlags(hub, GetFlags(hub) & ~GetLocalFlag(intercom));
                }
            }
        }

        public static void SetFlags(ReferenceHub hub, global::VoiceChat.VcMuteFlags flags)
        {
            Flags[hub] = flags;
            global::VoiceChat.VoiceChatMutes.OnFlagsSet?.Invoke(hub, flags);
        }

        public static global::VoiceChat.VcMuteFlags GetFlags(ReferenceHub hub)
        {
            if (!Flags.TryGetValue(hub, out var value))
            {
                return global::VoiceChat.VcMuteFlags.None;
            }
            return value;
        }
    }
}
