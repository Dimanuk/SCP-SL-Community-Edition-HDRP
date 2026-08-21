using CommandSystem;
using Mirror;
using Org.BouncyCastle.Security;
using RemoteAdmin.Communication;
using RemoteAdmin.Menus;
using Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static GameCore.Console;
using static RemoteAdmin.Communication.RaPlayerList;

namespace RemoteAdmin
{
    public class QueryProcessor : NetworkBehaviour
    {
        public struct CommandData : IEquatable<CommandData>
        {
            public string Command;
            public string[] Usage;
            public string Description;
            public string AliasOf;
            public bool Hidden;

            public override bool Equals(object obj)
            {
                if (obj is CommandData other)
                    return Equals(other);
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Command, Usage, Description, AliasOf, Hidden);
            }

            public bool Equals(CommandData other)
            {
                return Command == other.Command &&
                       (Usage ?? Array.Empty<string>()).SequenceEqual(other.Usage ?? Array.Empty<string>()) &&
                       Description == other.Description &&
                       AliasOf == other.AliasOf &&
                       Hidden == other.Hidden;
            }

            public static bool operator ==(CommandData lhs, CommandData rhs) => lhs.Equals(rhs);
            public static bool operator !=(CommandData lhs, CommandData rhs) => !lhs.Equals(rhs);
        }

        public RemoteAdminCryptographicManager CryptoManager;

        public GameConsoleTransmission GCT;

        private ServerRoles _roles;

        private PlayerCommandSender _sender;

        private RateLimit _commandRateLimit;

        private static SecureRandom _secureRandom;

        private static CommandData[] _commands;

        public static bool Lockdown;

        private const int HashIterations = 250;

        internal int PasswordTries;

        internal int SignaturesCounter;

        private int _signaturesCounter;

        internal byte[] Key;

        internal byte[] Salt;

        internal byte[] ClientSalt;

        private byte[] _key;

        private byte[] _salt;

        private byte[] _clientSalt;

        private float _lastPlayerlistRequest;

        private bool _commandsSynced;

        [SyncVar(hook = nameof(SetServerRandom))]
        private string _syncServerRandom;

        private static string _serverStaticRandom;

        private static bool _eventsAssgined;

        [NonSerialized]
        [SyncVar]
        public bool OverridePasswordEnabled;

        internal bool PasswordSent;

        private bool _gameplayData;

        private bool _gdDirty;

        private ReferenceHub _hub;

        private const int CommandDescriptionSyncMaxLength = 80;

        internal static readonly char[] SpaceArray = new char[1] { ' ' };

        public DateTime ExpectingURL = DateTime.MinValue;

        public static readonly ClientCommandHandler DotCommandHandler = ClientCommandHandler.Create();

        private string _ipAddress;

        private NetworkConnection _conns;

        public string ServerRandom => ReferenceHub.HostHub.queryProcessor._syncServerRandom;

        public bool IsHost => !string.IsNullOrEmpty(_syncServerRandom);

        public bool GameplayData
        {
            get => _gameplayData;
            set
            {
                _gameplayData = value;
                _gdDirty = true;
            }
        }

        private void Awake()
        {
            _hub = ReferenceHub.GetHub(base.gameObject);
            _roles = _hub.serverRoles;
            CryptoManager = GetComponent<RemoteAdminCryptographicManager>();
            GCT = GetComponent<GameConsoleTransmission>();
            _sender = new PlayerCommandSender(_hub);
        }

        private void Start()
        {
            _commandRateLimit = _hub.playerRateLimitHandler.RateLimits[2];
            if (_secureRandom == null)
                _secureRandom = new SecureRandom();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            if (string.IsNullOrEmpty(_serverStaticRandom))
            {
                using var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
                byte[] arr = new byte[32];
                rng.GetBytes(arr);
                _serverStaticRandom = System.Convert.ToBase64String(arr);
            }

            if (connectionToClient == null || connectionToClient == global::Mirror.NetworkServer.localConnection)
                _syncServerRandom = _serverStaticRandom;

            _conns = connectionToClient;
            _ipAddress = connectionToClient?.address;
            if (ServerStatic.PermissionsHandler == null)
            {
                CustomNetworkManager.LoadServerPermissions();
            }
            OverridePasswordEnabled = ServerStatic.PermissionsHandler != null && ServerStatic.PermissionsHandler.OverrideEnabled;
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            if (global::Mirror.NetworkServer.active)
            {
                _commands = ParseCommandsToStruct(CommandProcessor.GetAllCommands());
                if (string.IsNullOrEmpty(_syncServerRandom))
                    _syncServerRandom = _serverStaticRandom;
            }

            InvokeRepeating(nameof(RefreshPlayerList), 2f, 1f);
            if (!_eventsAssgined)
            {
                ReferenceHub.OnPlayerAdded += delegate { StaticRefreshPlayerList(); };
                ReferenceHub.OnPlayerRemoved += delegate { StaticRefreshPlayerList(); };
                _eventsAssgined = true;
            }
        }

        private void Update()
        {
            if (base.isLocalPlayer && _lastPlayerlistRequest < 1f)
                _lastPlayerlistRequest += global::UnityEngine.Time.deltaTime;

            if (_gdDirty)
            {
                _gdDirty = false;
                if (global::Mirror.NetworkServer.active)
                    TargetSyncGameplayData(base.connectionToClient, _gameplayData);
            }
        }

        public void SetServerRandom(string prev, string random)
        {
            if (base.isLocalPlayer)
                global::GameCore.Console.AddDebugLog("SDAUTH", "Obtained server round random: " + random, MessageImportance.Normal, false);
        }

        internal void RefreshPlayerList()
        {
            if (!base.isLocalPlayer)
                return;
            if (!_roles.LocalRemoteAdmin)
                return;
            UIController singleton = UIController.Singleton;
            if (singleton == null)
                return;
            if (!singleton.IsEnabled)
                return;
            if (_lastPlayerlistRequest < 1f)
                return;
            _lastPlayerlistRequest = 0f;
            int sorting = 0;
            if (singleton.PlayerSortingDropdown != null && singleton.PlayerSortingDropdown.options.Count > 0)
            {
                sorting = global::UnityEngine.Mathf.Min(
                    singleton.PlayerSortingDropdown.value,
                    singleton.PlayerSortingDropdown.options.Count - 1);
            }
            bool descending = RaSettings.Singleton != null && RaSettings.Singleton.ToggleListOrder != null && RaSettings.Singleton.ToggleListOrder.Value;
            RaPlayerList.Request(true, (PlayerSorting)sorting, descending);
        }

        internal static void StaticRefreshPlayerList()
        {
            if (!ReferenceHub.TryGetLocalHub(out ReferenceHub local))
                return;
            if (!ReferenceHub.TryGetHostHub(out _))
                return;
            local.queryProcessor.RefreshPlayerList();
            SteamManager.ChangeLobbyStatus(ReferenceHub.AllHubs.Count, 20);
        }

        [Command]
        public void CmdRequestSalt(byte[] clSalt)
        {
            if (!_commandRateLimit.CanExecute())
            {
                return;
            }
            if (!ServerStatic.PermissionsHandler.OverrideEnabled)
            {
                _hub.gameConsoleTransmission.SendToClient("Password authentication is disabled on this server!", "magenta");
                return;
            }
            if (_clientSalt == null)
            {
                if (clSalt == null)
                {
                    _hub.gameConsoleTransmission.SendToClient("Please generate and send your salt!", "red");
                    return;
                }
                if (clSalt.Length < 32)
                {
                    _hub.gameConsoleTransmission.SendToClient("Generated salt is too short. Please generate longer salt and try again!", "red");
                    return;
                }
                _clientSalt = clSalt;
                if (_key == null && _salt != null)
                {
                    _key = ServerStatic.PermissionsHandler.DerivePassword(_salt, _clientSalt);
                }
                _hub.gameConsoleTransmission.SendToClient("Your salt " + global::System.Convert.ToBase64String(clSalt) + " has been accepted by the server.", "cyan");
            }
            if (_salt != null)
            {
                TargetSaltGenerated(base.connectionToClient, _salt);
                return;
            }
            byte[] array;
            using (global::System.Security.Cryptography.RandomNumberGenerator randomNumberGenerator = new global::System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                array = new byte[32];
                randomNumberGenerator.GetBytes(array);
            }
            _salt = array;
            _key = ServerStatic.PermissionsHandler.DerivePassword(_salt, _clientSalt);
            TargetSaltGenerated(base.connectionToClient, _salt);
        }

        [TargetRpc]
        public void TargetSaltGenerated(NetworkConnection conn, byte[] salt)
        {
            if (salt != null && salt.Length >= 32)
            {
                global::GameCore.Console.AddLog("Obtained server's salt " + global::System.Convert.ToBase64String(salt) + " from server.", global::UnityEngine.Color.cyan);
                Salt = salt;
            }
            else
            {
                global::GameCore.Console.AddLog("Rejected salt " + salt + " because it's too short!", global::UnityEngine.Color.red);
            }
        }

        internal void SyncCommandsToClient()
        {
            if (!_commandsSynced)
            {
                _commandsSynced = true;
                TargetUpdateCommandList(_commands);
            }
        }

        [Server]
        private static CommandData[] ParseCommandsToStruct(List<ICommand> list)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                global::UnityEngine.Debug.LogWarning("[Server] function 'RemoteAdmin.QueryProcessor/CommandData[] RemoteAdmin.QueryProcessor::ParseCommandsToStruct(System.Collections.Generic.List`1<CommandSystem.ICommand>)' called when server was not active");
                return null;
            }
            List<CommandData> result = new List<CommandData>();
            foreach (ICommand item in list)
            {
                string description = item.Description;
                if (string.IsNullOrWhiteSpace(description))
                    description = null;
                else if (description.Length > 80)
                    description = description.Substring(0, 80) + "...";

                CommandData data = new CommandData
                {
                    Command = item.Command,
                    Usage = item is IUsageProvider up ? up.Usage : null,
                    Description = description,
                    AliasOf = null,
                    Hidden = item is IHiddenCommand
                };
                result.Add(data);
                if (item.Aliases != null)
                {
                    foreach (string alias in item.Aliases)
                    {
                        result.Add(new CommandData
                        {
                            Command = alias,
                            Usage = null,
                            Description = null,
                            AliasOf = data.Command,
                            Hidden = data.Hidden
                        });
                    }
                }
            }
            return result.ToArray();
        }

        [TargetRpc]
        private void TargetUpdateCommandList(CommandData[] commands)
        {
            List<CommandData> commandList = TextBasedRemoteAdmin.Commands;
            commandList.Clear();

            foreach (CommandData cmd in commands)
            {
                if (cmd.Command == null) continue;

                if (Misc.CommandRegex != null && !Misc.CommandRegex.IsMatch(cmd.Command))
                    continue;

                if (!string.IsNullOrEmpty(cmd.Description) &&
                    Misc.CommandDescriptionRegex != null &&
                    !Misc.CommandDescriptionRegex.IsMatch(cmd.Description))
                    continue;

                if (cmd.Usage != null)
                {
                    bool allValid = true;
                    foreach (string usage in cmd.Usage)
                    {
                        if (string.IsNullOrWhiteSpace(usage) ||
                            (Misc.CommandDescriptionRegex != null && !Misc.CommandDescriptionRegex.IsMatch(usage)))
                        {
                            allValid = false;
                            break;
                        }
                    }
                    if (!allValid) continue;
                }

                if (commandList.Any(x => string.Equals(x.Command, cmd.Command, StringComparison.OrdinalIgnoreCase)))
                    continue;

                commandList.Add(new CommandData
                {
                    Command = cmd.Command.ToLowerInvariant(),
                    Description = cmd.Description ?? "",
                    Usage = cmd.Usage ?? Array.Empty<string>(),
                    AliasOf = cmd.AliasOf,
                    Hidden = cmd.Hidden
                });
            }
        }

        [Command]
        public void CmdSendPassword(byte[] authSignature)
        {
            if (!_commandRateLimit.CanExecute())
                return;

            bool b = false;
            if (_roles.RemoteAdmin)
            {
                b = true;
                PasswordTries = 0;
            }
            else
            {
                if (_salt == null || _clientSalt == null)
                {
                    _hub.gameConsoleTransmission.SendToClient("Can't verify your remote admin password - please generate salt first!", "red");
                    return;
                }
                if (_clientSalt.Length < 16)
                {
                    _hub.gameConsoleTransmission.SendToClient("Generated salt is too short. Please rejoin the server and try again!", "red");
                    return;
                }
                if (VerifyHmacSignature("Login", -1, authSignature, false))
                {
                    PasswordTries = 0;
                    UserGroup overrideGroup = ServerStatic.PermissionsHandler.OverrideGroup;
                    if (overrideGroup != null)
                    {
                        ServerConsole.AddLog("Assigned group " + overrideGroup.BadgeText + " to " + _hub.nicknameSync.CombinedName + " - override password.");
                        _roles.SetGroup(overrideGroup, true);
                        b = true;
                    }
                    else
                    {
                        _hub.gameConsoleTransmission.SendToClient("Non-existing group is assigned for override password!", "red");
                    }
                }
                else
                {
                    PasswordTries++;
                    ServerConsole.AddLog("Rejected override password sent by " + Misc.LoggedNameFromRefHub(_hub) + ".");
                    ServerLogs.AddLog(ServerLogs.Modules.Permissions, "Rejected override password sent by " + Misc.LoggedNameFromRefHub(_hub) + ".", ServerLogs.ServerLogType.RemoteAdminActivity_Misc);
                }
            }
            if (PasswordTries >= 3)
            {
                ServerLogs.AddLog(ServerLogs.Modules.Permissions, Misc.LoggedNameFromRefHub(_hub) + " has been kicked from the server for sending too many invalid override passwords.", ServerLogs.ServerLogType.RemoteAdminActivity_Misc);
                ServerConsole.Disconnect(connectionToClient, "You have been kicked for too many Remote Admin login attempts.");
            }
            else
            {
                TargetReplyPassword(connectionToClient, b);
            }
        }

        [TargetRpc]
        private void TargetReplyPassword(NetworkConnection conn, bool b)
        {
            UIController.Singleton.AwaitingLogin = (byte)(b ? 2 : 0);
        }

        [TargetRpc]
        internal void TargetAdminChatAccessDenied(NetworkConnection conn)
        {
            global::GameCore.Console.AddLog("You don't have permission to use the admin chat.", global::UnityEngine.Color.red, false, ConsoleLogType.Error);
        }

        [Server]
        internal void TargetReply(NetworkConnection conn, string content, bool isSuccess, bool logInConsole, string overrideDisplay)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                global::UnityEngine.Debug.LogWarning("[Server] function 'System.Void RemoteAdmin.QueryProcessor::TargetReply(Mirror.NetworkConnection,System.String,System.Boolean,System.Boolean,System.String)' called when server was not active");
                return;
            }
            if (CryptoManager.EncryptionKey == null)
            {
                if (ServerStatic.IsDedicated && base.isLocalPlayer)
                {
                    ServerConsole.AddLog("[RA output] " + content);
                }
                else if (!CryptoManager.ExchangeRequested)
                {
                    TargetReplyPlain(conn, content, isSuccess, logInConsole, overrideDisplay);
                }
                else
                {
                    TargetReplyPlain(conn, "ERROR#ECDHE exchange was requested, please complete it on client side.", false, true, "");
                }
            }
            else
            {
                TargetReplyEncrypted(conn, global::Cryptography.AES.AesGcmEncrypt(Utf8.GetBytes(content), CryptoManager.EncryptionKey, _secureRandom), isSuccess, logInConsole, overrideDisplay);
            }
        }

        [TargetRpc]
        private void TargetReplyPlain(NetworkConnection conn, string content, bool isSuccess, bool logInConsole, string overrideDisplay)
        {
            ProcessReply(content, isSuccess, logInConsole, overrideDisplay, false);
        }

        [TargetRpc]
        private void TargetReplyEncrypted(NetworkConnection conn, byte[] content, bool isSuccess, bool logInConsole, string overrideDisplay)
        {
            string decrypted = Utf8.GetString(global::Cryptography.AES.AesGcmDecrypt(content, CryptoManager.EncryptionKey));
            ProcessReply(decrypted, isSuccess, logInConsole, overrideDisplay, true);
        }

        private void ProcessReply(string content, bool isSuccess, bool logInConsole, string overrideDisplay, bool secure)
        {
            if (content.StartsWith("$", StringComparison.Ordinal))
            {
                string body = content.Remove(0, 1);
                string[] parts = body.Split(new char[] { ' ' });
                if (parts.Length > 1 && int.TryParse(parts[0], out int commId))
                {
                    if (CommunicationProcessor.ClientCommunication.TryGetValue(commId, out Interfaces.IClientCommunication comm))
                    {
                        string data = string.Join(" ", parts.Skip(1));
                        comm.ReceiveData(data, secure);
                    }
                }
                return;
            }

            if (content.StartsWith("@", StringComparison.Ordinal))
            {
                bool isPersistent = content.StartsWith("@@", StringComparison.Ordinal);
                string broadcastContent = content.Substring(isPersistent ? 2 : 1);
                ushort duration = 5;

                if (broadcastContent.StartsWith("!", StringComparison.Ordinal) &&
                    NorthwoodLib.StringUtils.Contains(broadcastContent, " ", StringComparison.Ordinal))
                {
                    int spaceIdx = broadcastContent.IndexOf(" ", StringComparison.Ordinal);
                    string durationStr = broadcastContent.Substring(1, spaceIdx - 1);
                    if (global::System.UInt16.TryParse(durationStr, out ushort parsedDuration))
                    {
                        duration = parsedDuration;
                        broadcastContent = broadcastContent.Substring(spaceIdx + 1);
                    }
                }

                byte finalDuration = (byte)global::UnityEngine.Mathf.Clamp(duration, 0, 15);
                Broadcast.AddElement(broadcastContent, finalDuration, isPersistent ? global::Broadcast.BroadcastFlags.AdminChat : global::Broadcast.BroadcastFlags.Normal);
                return;
            }

            if (content.StartsWith("%", StringComparison.Ordinal))
            {
                if (!(ExpectingURL < global::System.DateTime.Now))
                {
                    ExpectingURL = global::System.DateTime.MinValue;
                    ServerConfigSynchronizer serverConfig = ServerConfigSynchronizer.Singleton;
                    string lookupMode = serverConfig.RemoteAdminExternalPlayerLookupMode;
                    if (!lookupMode.Contains("disabled"))
                    {
                        string urlContent = content.Substring(1);
                        string extraParam = string.Empty;
                        int percentIdx = urlContent.IndexOf("%", StringComparison.Ordinal);
                        if (percentIdx > 0)
                        {
                            bool fullAuth = lookupMode.Contains("fullauth");
                            string playerId = urlContent.Substring(0, percentIdx);
                            extraParam = string.Concat(fullAuth ? "&" : "?", "lookup=", playerId);
                            urlContent = urlContent.Substring(percentIdx + 1);
                        }

                        string finalUrl;
                        if (lookupMode.Contains("fullauth"))
                            finalUrl = serverConfig.RemoteAdminExternalPlayerLookupURL + extraParam;
                        else
                            finalUrl = string.Concat(serverConfig.RemoteAdminExternalPlayerLookupURL, extraParam);

                        global::UnityEngine.Application.OpenURL(finalUrl);
                        Steamworks.SteamFriends.OpenWebOverlay(finalUrl, false);
                    }
                    else
                    {
                        TextBasedRemoteAdmin.AddLog("<color=white>[</color><color=red><link=TBRA_InternalError> (INTERNAL ERROR)</link></color><color=white>]</color> <color=red>Server attempted to send us a URL but external lookup is disabled! (This should never happen!)</color>");
                    }
                }
                return;
            }

            string commandName = string.Empty;
            string response = content;
            if (NorthwoodLib.StringUtils.Contains(content, "#", StringComparison.Ordinal))
            {
                int hashIdx = content.IndexOf("#", StringComparison.Ordinal);
                commandName = content.Remove(hashIdx);
                response = content.Remove(0, hashIdx + 1);
            }

            if (logInConsole)
            {
                string securityPart = secure
                    ? string.Empty
                    : "<color=white>[</color><color=red><link=TBRA_EncryptionError> (UNECRYPTED)</link></color><color=white>]</color>";

                string successPart = isSuccess
                    ? "<color=green></color><color=white></link>] "
                    : "<link=TBRA_CommandFail><color=red></color><color=white></link>]</color> <color=orange>";

                string log = string.Concat(new string[]
                {
                    securityPart,
                    "<color=white>[</color><link=TBRA_CommandSuccess>",
                    successPart,
                    "[",
                    commandName,
                    "] ",
                    response,
                    "</color>"
                });
                TextBasedRemoteAdmin.AddLog(log);
            }

            if (string.IsNullOrEmpty(commandName) || overrideDisplay == "void")
                return;

            if (overrideDisplay == string.Empty)
            {
                if (string.Equals(commandName, "WIKI"))
                    global::UnityEngine.Application.OpenURL("https://en.scpslgame.com/index.php?title=Remote_Admin");

                if (string.Equals(commandName, "LOGOUT"))
                    UIController.Singleton.LoggedIn = false;
            }

            SubmenuSelector.Singleton?.SelectedMenu?.SetResponse(isSuccess, response);
        }

        [Client]
        public void CmdSendQuery(string query, bool gban = false)
        {
            if (!global::Mirror.NetworkClient.active)
            {
                global::UnityEngine.Debug.LogWarning("[Client] function 'System.Void RemoteAdmin.QueryProcessor::CmdSendQuery(System.String,System.Boolean)' called when client was not active");
                return;
            }
            if (!global::Mirror.NetworkClient.ready)
                return;
            if (string.IsNullOrEmpty(ServerRandom))
            {
                global::GameCore.Console.AddLog("Failed to send command - empty ServerRandom.", global::UnityEngine.Color.magenta);
                return;
            }
            if (ServerRandom.Length < 32)
            {
                global::GameCore.Console.AddLog("Failed to send command - too short ServerRandom.", global::UnityEngine.Color.magenta);
                return;
            }
            if (!gban && query.StartsWith("GBAN-KICK ", StringComparison.OrdinalIgnoreCase))
            {
                global::GameCore.Console.AddLog("\"GBAN-KICK\" command can't be executed manually.", global::UnityEngine.Color.magenta);
                return;
            }
            SignaturesCounter++;
            if (CryptoManager.EncryptionKey != null)
            {
                byte[] encrypted = global::Cryptography.AES.AesGcmEncrypt(Utf8.GetBytes(query + ":[:COUNTER:]:" + SignaturesCounter), CryptoManager.EncryptionKey, _secureRandom);
                CmdSendEncryptedQuery(encrypted);
            }
            else if (!PasswordSent && !CentralAuth.GlobalBadgeIssued)
            {
                // The exchange never completed on this client (usually a lost startup RPC). Ask the
                // server to re-run it so the next command can go through, instead of failing forever.
                CryptoManager.CmdRequestEcdheExchange();
                global::GameCore.Console.AddLog("Secure channel not ready yet - re-requesting ECDHE exchange, please try again in a moment.", global::UnityEngine.Color.magenta);
            }
            else
            {
                byte[] signature = (!_roles.PublicKeyAccepted && _roles.RemoteAdminMode == ServerRoles.AccessMode.PasswordOverride)
                    ? HmacSign(query)
                    : EcdsaSign(query);

                if (signature == null)
                {
                    // HmacSign already logged the reason (missing key / message too long); don't send an
                    // unsigned query the server would reject anyway, and don't crash.
                    return;
                }

                CmdSendQuery(query, SignaturesCounter, signature);
            }

            if (!query.StartsWith("$"))
            {
                string sanitized = query.Replace("<", "\uFF1C").Replace(">", "\uFF1E").Replace("\n", string.Empty).Replace("\r", string.Empty);
                TextBasedRemoteAdmin.AddLog(string.Concat("<color=purple> ", sanitized, "</color>"));
            }
        }

        [Command]
        public void CmdSendEncryptedQuery(byte[] query)
        {
            if (!_commandRateLimit.CanExecute() || query == null)
            {
                return;
            }
            if (!_roles.RemoteAdmin)
            {
                GCT.SendToClient(base.connectionToClient, "You are not logged in to remote admin panel!", "red");
                return;
            }
            if (CryptoManager.EncryptionKey == null)
            {
                GCT.SendToClient(base.connectionToClient, "Please complete ECDHE exchange before sending encrypted remote admin requests.", "magenta");
                return;
            }
            string text;
            try
            {
                text = Utf8.GetString(global::Cryptography.AES.AesGcmDecrypt(query, CryptoManager.EncryptionKey));
            }
            catch
            {
                GCT.SendToClient(base.connectionToClient, "Decryption or verification of remote admin request failed.", "magenta");
                return;
            }
            if (!text.Contains(":[:COUNTER:]:"))
            {
                GCT.SendToClient(base.connectionToClient, "Remote admin request doesn't contain a signatures counter.", "magenta");
                return;
            }
            int num = text.LastIndexOf(":[:COUNTER:]:", global::System.StringComparison.Ordinal);
            if (!int.TryParse(text.Substring(num + 13), out var result))
            {
                GCT.SendToClient(base.connectionToClient, "Remote admin request contains non-integer signatures counter.", "magenta");
                return;
            }
            if (result <= _signaturesCounter)
            {
                GCT.SendToClient(base.connectionToClient, "Remote admin request contains smaller signatures counter than previous request.", "magenta");
                return;
            }
            _signaturesCounter = result;
            global::RemoteAdmin.CommandProcessor.ProcessQuery(text.Substring(0, num), _sender);
        }

        [Command]
        public void CmdSendQuery(string query, int counter, byte[] signature)
        {
            if (!_commandRateLimit.CanExecute() || query == null || signature == null)
                return;

            if (string.IsNullOrEmpty(ServerRandom))
            {
                GCT.SendToClient(connectionToClient, "Remote Admin error - ServerRandom is empty or null.", "magenta");
                return;
            }
            if (!_roles.RemoteAdmin)
            {
                GCT.SendToClient(connectionToClient, "You are not logged in to remote admin panel!", "red");
                return;
            }
            if (CryptoManager.ExchangeRequested)
            {
                GCT.SendToClient(connectionToClient, "ECDHE exchange was requested, please use encrypted channel for remote admin commands.", "magenta");
                return;
            }
            if (!VerifyRequestSignature(query, counter, signature))
            {
                GCT.SendToClient(connectionToClient, "Signature verification of request \"" + query + "\" failed!", "magenta");
                return;
            }
            CommandProcessor.ProcessQuery(query, _sender);
        }

        internal void ProcessGameConsoleQuery(string query)
        {
            string[] args = query.Trim().Split(SpaceArray, 512, global::System.StringSplitOptions.RemoveEmptyEntries);
            if (DotCommandHandler.TryGetCommand(args[0], out ICommand command))
            {
                try
                {
                    bool success = command.Execute(args.Segment(1), _sender, out string response);
                    GCT.SendToClient(base.connectionToClient, args[0].ToUpperInvariant() + "#" + response, "");
                    return;
                }
                catch (global::System.Exception ex)
                {
                    string error = "Command execution failed! Error: " + ex;
                    GCT.SendToClient(base.connectionToClient, args[0].ToUpperInvariant() + "#" + error, "");
                    return;
                }
            }
            GCT.SendToClient(base.connectionToClient, "Command not found.", "red");
        }

        private bool VerifyRequestSignature(string message, int counter, byte[] signature, bool validateCounter = true)
        {
            if (!_roles.PublicKeyAccepted && _roles.RemoteAdminMode == ServerRoles.AccessMode.PasswordOverride)
                return VerifyHmacSignature(message, counter, signature, validateCounter);
            return VerifyEcdsaSignature(message, counter, signature, validateCounter);
        }

        private byte[] SignRequest(string message, int counter = -2)
        {
            if (!_roles.PublicKeyAccepted && _roles.RemoteAdminMode == ServerRoles.AccessMode.PasswordOverride)
                return HmacSign(message, counter);
            return EcdsaSign(message, counter);
        }

        private bool VerifyHmacSignature(string message, int counter, byte[] signature, bool validateCounter = true)
        {
            if (counter > _signaturesCounter)
                _signaturesCounter = counter;
            else if (validateCounter)
                return false;

            if (!OverridePasswordEnabled)
                return false;

            string text = string.Concat(new object[] { message, ":[:COUNTER:]:", counter, ":[:SALT:]:", ServerRandom });
            // Must be an exact-length array, not a pooled one: Sha512Hmac uses the ENTIRE first
            // argument as the HMAC key, so any garbage bytes past the text (leftovers from previous
            // pool renters) would make client and server signatures diverge and reject even a
            // correct password. See the matching note in HmacSign.
            byte[] data = global::System.Text.Encoding.UTF8.GetBytes(text);
            if (_key == null || data.Length > _key.Length)
                return false;
            return global::Cryptography.Sha.Sha512Hmac(data, 0, data.Length, _key).SequenceEqual(signature);
        }

        private bool VerifyEcdsaSignature(string message, int counter, byte[] signature, bool validateCounter = true)
        {
            if (!_roles.PublicKeyAccepted || _roles.PublicKey == null)
            {
                global::GameCore.Console.AddLog("VerifyEcdsaSignature called with empty Public Key", global::UnityEngine.Color.red);
                global::UnityEngine.Debug.LogError("VerifyEcdsaSignature called with empty Public Key");
                return false;
            }
            if (counter > _signaturesCounter)
                _signaturesCounter = counter;
            else if (validateCounter)
                return false;

            string text = string.Concat(new object[] { message, ":[:COUNTER:]:", counter, ":[:SALT:]:", ServerRandom });
            return global::Cryptography.ECDSA.VerifyBytes(text, signature, _roles.PublicKey);
        }

        private byte[] EcdsaSign(string message, int counter = -2)
        {
            if (counter == -2)
                counter = SignaturesCounter;
            string text = string.Concat(new object[] { message, ":[:COUNTER:]:", counter, ":[:SALT:]:", ServerRandom });
            return global::Cryptography.ECDSA.SignBytes(text, CentralAuthManager.SessionKeys.Private);
        }

        public byte[] HmacSign(string message, int counter = -2)
        {
            if (counter == -2)
                counter = SignaturesCounter;
            // Sha512Hmac hashes the first byteCount bytes OUT OF Key (this matches the shipped game and
            // the server's verify side, which both call Sha512Hmac(textBytes, 0, len, key)). That means
            // the signable message length is bounded by Key.Length. The password key is a 512-byte PBKDF2
            // output, so a long command (e.g. a big CASSIE announcement) would read past Key and throw an
            // ArgumentException. The real client never hits this because it sends long commands over the
            // encrypted ECDHE channel; guard here so a missing/short key aborts the send cleanly instead
            // of crashing the whole RA panel.
            if (Key == null)
            {
                global::GameCore.Console.AddLog("Failed to sign remote admin command - password key not established. Log in via the RA password panel first.", global::UnityEngine.Color.magenta);
                return null;
            }
            string text = string.Concat(new object[] { message, ":[:COUNTER:]:", counter, ":[:SALT:]:", ServerRandom });
            // Exact-length array on purpose (see VerifyHmacSignature): Sha512Hmac keys the HMAC with
            // the whole first argument, so a pooled buffer's dirty tail would randomize the signature.
            // The shipped game rented from ArrayPool here and got away with it only when both sides
            // happened to hold zero-clean pool arrays; with our dirty pools the password login always
            // failed and three "failed" attempts kicked the player even on a correct password.
            byte[] data = global::System.Text.Encoding.UTF8.GetBytes(text);
            if (data.Length > Key.Length)
            {
                global::GameCore.Console.AddLog("Command too long to sign with the password key - use a public-key admin login (or complete the secure ECDHE exchange) for long commands such as CASSIE.", global::UnityEngine.Color.magenta);
                return null;
            }
            return global::Cryptography.Sha.Sha512Hmac(data, 0, data.Length, Key);
        }

        public static byte[] DerivePassword(string password, byte[] serversalt, byte[] clientsalt)
        {
            byte[] salt = global::Cryptography.Sha.Sha512(global::System.Convert.ToBase64String(serversalt) + global::System.Convert.ToBase64String(clientsalt));
            return global::Cryptography.PBKDF2.Pbkdf2HashBytes(password, salt, 250, 512);
        }

        [TargetRpc]
        public void TargetSyncGameplayData(NetworkConnection conn, bool gd)
        {
            _gameplayData = gd;
        }

        private void OnDestroy()
        {
            if (global::Mirror.NetworkServer.active && (!base.isLocalPlayer || !ServerStatic.IsDedicated))
            {
                ServerLogs.AddLog(
                    ServerLogs.Modules.Networking,
                    string.Format(
                        "{0} {1} disconnected from IP address {2}. Last class: {3} ({4})",
                        GetComponent<NicknameSync>().CombinedName,
                        string.IsNullOrEmpty(_hub.characterClassManager.UserId) ? "(no ID)" : _hub.characterClassManager.UserId,
                        _ipAddress,
                        global::PlayerRoles.PlayerRolesUtils.GetRoleId(_hub),
                        _hub.roleManager.CurrentRole.RoleName),
                    ServerLogs.ServerLogType.ConnectionUpdate);
            }
        }
    }
}