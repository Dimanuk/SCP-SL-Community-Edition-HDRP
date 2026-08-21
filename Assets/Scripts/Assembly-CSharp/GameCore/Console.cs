using CommandSystem;
using Mirror;
using Org.BouncyCastle.Crypto;
using RemoteAdmin;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using ToggleableMenus;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GameCore
{
    public class Console : SimpleToggleableMenu
    {
        public enum ConsoleLogType
        {
            DoNotLog = 0,
            Log = 1,
            Warning = 2,
            Error = 3
        }

        public CommandHint[] hints;

        public readonly GameConsoleCommandHandler ConsoleCommandHandler = GameConsoleCommandHandler.Create();

        public Text txt;

        public InputField cmdField;

        private int _clientCommandPosition;

        private List<string> _clientCommandLogs = new List<string>();

        public static bool HideBadge;

        public static bool HideLocalBadge;

        public static bool NeverHideLocalBadge;

        public static bool NeverCover;

        private static bool _noConsoleLogOutput;

        internal static bool DisplayCreateServer;

        internal static bool TranslationDebugMode;

        internal static AsymmetricKeyParameter _publicKey;

        private static readonly ConsoleCommandSender Ccs = new ConsoleCommandSender();

        internal static readonly string SyncbindPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "/SCP Secret Laboratory/internal/SyncCmd");

        internal static readonly List<Log> Logs = new List<Log>();

        private static readonly ConcurrentQueue<Log> logQueue = new ConcurrentQueue<Log>();

        private int _scrollup;

        private int _previousScrlup;

        internal static bool _alwaysRefreshing;

        internal static bool _change;

        internal static bool _allowBindSyncing = true;

        internal static bool _bindSyncingContinue;

        private string _content;

        public static Console Singleton { get; private set; }

        public static bool EnableSCP { get; private set; }

        public static bool DisableSLML { get; private set; }

        public static bool DisableRemoteSLML { get; private set; }

        public static bool RequestDNT { get; private set; }

        internal static string BinariesRootPath { get; private set; }

        internal static bool AuthDebug { get; private set; }

        internal static bool BindSyncingEnabled { get; private set; }

        internal static bool SkipIpValidation { get; private set; }

        public override bool LockMovement => IsEnabled;

        public override bool CanToggle
        {
            get
            {
                if (!LockMovement)
                    return true;

                InputField field = cmdField;
                if (field.isFocused && field.text.Length > 1)
                    return false;

                return true;
            }
        }


        protected override void Awake()
        {
            base.Awake();
            BinariesRootPath = Path.GetDirectoryName(Path.GetFullPath(Application.dataPath));
            DontDestroyOnLoad(gameObject);

            if (Singleton != null && Singleton != this)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Singleton = this;
        }

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            Debug.Log("Hi there! Initializing console...");
            logQueue.Enqueue(new Log("Hi there! Initializing console...", Color.green, false));
            Debug.Log("Done! Type 'help' to print the list of available commands.");
            logQueue.Enqueue(new Log("Done! Type 'help' to print the list of available commands.", Color.green, false));

            CentralAuthManager.InitAuth();
            
            if (StartupArgs.Args.Any(arg =>
                    string.Equals(arg, "-smartclasspicker", StringComparison.OrdinalIgnoreCase)))
            {
                EnableSCP = true;
                AddLog("Smart Class Picker will be enabled for your server.", new Color32(0, 255, 0, 255));
            }

            if (StartupArgs.Args.Any(arg =>
                    string.Equals(arg, "-tdm", StringComparison.OrdinalIgnoreCase)))
            {
                TranslationDebugMode = true;
                AddLog("Translation debug mode has been enabled (startup argument).",
                    new Color32(0, 255, 0, 255));
            }
            else
            {
                TranslationDebugMode = false;
            }

            // ── Background public-key refresh thread ──────────────────────────────
            Thread pkThread = new Thread(RefreshPublicKey)
            {
                Name = "Public key refreshing",
                IsBackground = true,
                Priority = System.Threading.ThreadPriority.AboveNormal
            };
            pkThread.Start();

            // ── -disableremoteslml ─────────────────────────────────────────────────
            if (StartupArgs.Args.Any(arg =>
                    string.Equals(arg, "-disableremoteslml", StringComparison.OrdinalIgnoreCase)))
            {
                DisableRemoteSLML = true;
                AddLog("Remote SLML disabled by the startup argument.", new Color32(0, 255, 0, 255));
            }

            // ── -disableslml ───────────────────────────────────────────────────────
            if (StartupArgs.Args.Any(arg =>
                    string.Equals(arg, "-disableslml", StringComparison.OrdinalIgnoreCase)))
            {
                DisableSLML = true;
                AddLog("SLML disabled by the startup argument.", new Color32(0, 255, 0, 255));
            }

            // ── -dnt (do not track) ────────────────────────────────────────────────
            if (StartupArgs.Args.Any(arg =>
                    string.Equals(arg, "-dnt", StringComparison.OrdinalIgnoreCase)))
            {
                RequestDNT = true;
                AddLog(
                    "\"Do not track\" request will be sent to all servers you are joining - enabled by startup argument.",
                    new Color32(0, 255, 0, 255));
            }

            // ── -hidebadge ─────────────────────────────────────────────────────────
            if (StartupArgs.Args.Any(arg =>
                    string.Equals(arg, "-hidebadge", StringComparison.OrdinalIgnoreCase)))
            {
                HideBadge = true;
                AddLog("Your global badge will be automatically hidden.", new Color32(0, 255, 0, 255));
            }

            // ── -hidelocalbadge ────────────────────────────────────────────────────
            if (StartupArgs.Args.Any(arg =>
                    string.Equals(arg, "-hidelocalbadge", StringComparison.OrdinalIgnoreCase)))
            {
                HideLocalBadge = true;
                AddLog("Your local badge will be automatically hidden.", new Color32(0, 255, 0, 255));
            }

            // ── -neverhidelocalbadge ───────────────────────────────────────────────
            if (StartupArgs.Args.Any(arg =>
                    string.Equals(arg, "-neverhidelocalbadge", StringComparison.OrdinalIgnoreCase)))
            {
                NeverHideLocalBadge = true;
            }

            // ── -nevercover ────────────────────────────────────────────────────────
            if (StartupArgs.Args.Any(arg =>
                    string.Equals(arg, "-nevercover", StringComparison.OrdinalIgnoreCase)))
            {
                NeverCover = true;
            }

            // ── -skipipvalidation ──────────────────────────────────────────────────
            if (StartupArgs.Args.Any(arg =>
                    string.Equals(arg, "-skipipvalidation", StringComparison.OrdinalIgnoreCase)))
            {
                SkipIpValidation = true;
                AddLog("IP validation will be skipped.", new Color32(0, 255, 0, 255));
            }

            // ── -authdebug ─────────────────────────────────────────────────────────
            if (StartupArgs.Args.Any(arg =>
                    string.Equals(arg, "-authdebug", StringComparison.OrdinalIgnoreCase)))
            {
                AuthDebug = true;
                AddLog("Authentication debug mode enabled.", new Color32(0, 255, 0, 255));
            }

            // ── -bindsync ──────────────────────────────────────────────────────────
            if (StartupArgs.Args.Any(arg =>
                    string.Equals(arg, "-bindsync", StringComparison.OrdinalIgnoreCase)))
            {
                BindSyncingEnabled = true;
                _bindSyncingContinue = true;
                AddLog("Bind syncing enabled.", new Color32(0, 255, 0, 255));
            }

            // ── -createserver ──────────────────────────────────────────────────────
            if (StartupArgs.Args.Any(arg =>
                    string.Equals(arg, "-createserver", StringComparison.OrdinalIgnoreCase)))
            {
                DisplayCreateServer = true;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            string msg = string.Concat("Scene Manager: Loaded scene '", scene.name, "' [", scene.path, "]");
            Debug.Log(msg);
            logQueue.Enqueue(new Log(msg, Color.green, false));
            RefreshConsoleScreen();
        }

        private void Update()
        {
            CentralAuthManager.Discord?.RunCallbacks();

            if (_change)
            {
                txt.text = _content;
                _change = false;
            }
        }

        private void LateUpdate()
        {
            if (!IsEnabled)
            {
                _clientCommandPosition = 0;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (!string.IsNullOrEmpty(cmdField.text))
                {
                    TypeCommand(cmdField.text);
                    cmdField.text = string.Empty;
                    EventSystem.current.SetSelectedGameObject(cmdField.gameObject);
                }
                cmdField.ActivateInputField();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (_clientCommandLogs.Count > _clientCommandPosition)
                {
                    _clientCommandPosition++;
                    int index = _clientCommandLogs.Count - _clientCommandPosition;
                    if (index >= 0)
                        cmdField.text = _clientCommandLogs[index];
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (_clientCommandPosition > 0)
                {
                    _clientCommandPosition--;
                    int index = _clientCommandLogs.Count - _clientCommandPosition;
                    if (index < _clientCommandLogs.Count)
                        cmdField.text = _clientCommandLogs[index];
                    else
                        cmdField.text = string.Empty;
                }
            }

            float scroll = Input.GetAxisRaw("Mouse ScrollWheel") * 10f;
            if (scroll != 0)
            {
                _scrollup += (int)scroll;
                if (_scrollup < 0) _scrollup = 0;
                if (_scrollup > Logs.Count) _scrollup = Logs.Count;
            }

            if (_scrollup != _previousScrlup)
            {
                _previousScrlup = _scrollup;
                RefreshConsoleScreen();
            }

            if (_alwaysRefreshing)
            {
                RefreshConsoleScreen();
            }
        }

        private void FixedUpdate()
        {
            bool logged = false;
            while (logQueue.TryDequeue(out Log log))
            {
                string timeHms = TimeBehaviour.FormatTime("HH:mm:ss");
                string timeMs = TimeBehaviour.FormatTime("fff");
                string fullMsg = string.Concat(
                    "<color=#808080><size=18>[",
                    timeHms,
                    "<size=16>.",
                    timeMs,
                    "</size>]</size></color> ",
                    log.text);

                string[] lines = fullMsg.Split('\n');
                foreach (var line in lines)
                {
                    string cleanLine = line.Replace("\r", string.Empty);
                    Logs.Add(new Log(cleanLine, log.color, log.nospace));
                }
                logged = true;
            }

            if (logged)
            {
                _scrollup = 0;
                RefreshConsoleScreen();
            }
        }

        private void RefreshConsoleScreen()
        {
            if (txt == null) return;

            if (Logs == null || Logs.Count == 0)
            {
                _content = string.Empty;
                _change = true;
                return;
            }

            string content = string.Empty;

            int limit = Logs.Count - _scrollup;
            for (int i = 0; i < limit; i++)
            {
                Log item = Logs[i];
                string sep = (i != 0) ? (item.nospace ? "\n\n" : "\n") : string.Empty;
                content = string.Concat(content, sep, "<color=", Misc.ToHex(item.color), ">", item.text, "</color>");
                _content = content;

                if (content.Length > 15000)
                {
                    Logs.RemoveAt(0);
                    RefreshConsoleScreen();
                    return;
                }
            }

            _change = true;
        }

        public static void AddDebugLog(string debugKey, string message, MessageImportance importance, bool nospace = false)
        {
            if (ConsoleDebugMode.CheckImportance(debugKey, importance, out var color))
            {
                AddLog("[DEBUG_" + debugKey + "] " + message, color, nospace);
            }
            Debug.Log(message);
        }

        public static void AddLog(string text, Color c, bool nospace = false, ConsoleLogType type = ConsoleLogType.Log)
        {
            if (!_noConsoleLogOutput)
            {
                switch (type)
                {
                    case ConsoleLogType.DoNotLog:
                        break;
                    case ConsoleLogType.Error:
                        Debug.LogError(text);
                        break;
                    case ConsoleLogType.Warning:
                        Debug.LogWarning(text);
                        break;
                    case ConsoleLogType.Log:
                        Debug.Log(text);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }

            logQueue.Enqueue(new Log(text, c, nospace));
        }

        public static GameObject FindConnectedRoot(NetworkConnection conn)
        {
            if (conn == null) return null;

            NetworkIdentity identity = conn.identity;
            if (identity == null) return null;

            GameObject go = identity.gameObject;
            if (go == null) return null;

            if (go.CompareTag("Player"))
                return go;

            return null;
        }


        internal string TypeCommand(string cmd, CommandSender sender = null)
        {
            Debug.Log(">>> " + cmd);
            sender ??= Ccs;

            _clientCommandLogs?.Add(cmd);

            if (cmd.StartsWith(".", StringComparison.Ordinal) && cmd.Length > 1)
            {
                if (!NetworkClient.active && !NetworkServer.active)
                {
                    string err = "You must be connected to a server to use this command.";
                    Debug.Log(err);
                    logQueue.Enqueue(new Log(err, Color.red, false));
                    return "You must be connected to a server to use remote admin commands!";
                }

                string text = cmd.Substring(1);
                string text2 = "Sending command to server: " + text;
                sender?.Print(text2, ConsoleColor.Green);
                ReferenceHub.LocalHub.gameConsoleTransmission.SendToServer(text);
                return text2;
            }

            bool flag = cmd.StartsWith("@", StringComparison.Ordinal);
            if ((cmd.StartsWith("/", StringComparison.Ordinal) || flag) && cmd.Length > 1)
            {
                string raQuery = flag ? cmd : cmd.Substring(1);

                if (!flag)
                {
                    raQuery = raQuery.TrimStart('$');
                    if (string.IsNullOrEmpty(raQuery))
                    {
                        sender?.Print("Command cant be empty!", ConsoleColor.Green);
                        return "Command cant be empty!";
                    }
                }

                if (NetworkServer.active)
                    return RemoteAdmin.CommandProcessor.ProcessQuery(raQuery, sender);

                if (NetworkClient.active)
                {
                    string info2 = "Sending remote admin request to server: " + raQuery;
                    sender?.Print(info2, ConsoleColor.Green);
                    ReferenceHub.LocalHub.queryProcessor.CmdSendQuery(raQuery, false);
                    return info2;
                }
            }

            string[] array = cmd.Trim().Split(QueryProcessor.SpaceArray, 512, StringSplitOptions.RemoveEmptyEntries);

            cmd = array[0];
            if (!ConsoleCommandHandler.TryGetCommand(cmd, out var command))
            {
                string text4 = "Command " + cmd + " does not exist!";
                sender?.Print(text4, ConsoleColor.DarkYellow, new Color32(255, 180, 0, 255));
                return text4;
            }

            try
            {
                string response;
                bool flag2 = command.Execute(new ArraySegment<string>(array, 1, array.Length - 1), sender, out response);
                response = Misc.CloseAllRichTextTags(response);
                if (string.IsNullOrWhiteSpace(response))
                {
                    return null;
                }
                sender?.Print(response, flag2 ? ConsoleColor.Green : ConsoleColor.Red);
                return response;
            }
            catch (Exception ex)
            {
                string text5 = "Command execution failed! Error: " + Misc.RemoveStacktraceZeroes(ex.ToString());
                sender?.Print(text5, ConsoleColor.Red);
                return text5;
            }
        }

        public void ProceedButton()
        {
            if (!string.IsNullOrEmpty(cmdField.text))
            {
                TypeCommand(cmdField.text);
            }
            cmdField.text = string.Empty;
            EventSystem.current.SetSelectedGameObject(cmdField.gameObject);
        }


        internal static void CopyPastebin()
        {
            if (ReferenceHub.TryGetHostHub(out ReferenceHub hub))
            {
                string text = ConfigFile.ServerConfig.GetString("pastebin_read_id", "");
                if (!string.IsNullOrWhiteSpace(text))
                {
                    if (Misc.ValidatePastebin(text))
                    {
                        string url = "https://pastebin.com/raw/" + text;
                        Misc.CopyToClipboard(url);
                        return;
                    }
                    string err = "Pastebin ID is invalid: " + text;
                    Debug.Log(err);
                    logQueue.Enqueue(new Log(err, Color.red, false));
                }
                else
                {
                    Debug.Log("Pastebin is null.");
                    logQueue.Enqueue(new Log("Pastebin is null.", Color.red, false));
                }
            }
            else
            {
                string msg = "Can't find host's reference hub. Please join a server.";
                Debug.Log(msg);
                logQueue.Enqueue(new Log(msg, Color.red, false));
            }
        }

        protected override void OnToggled()
        {
            base.OnToggled();
            cmdField.text = string.Empty;
            if (IsEnabled)
            {
                cmdField.ActivateInputField();
                EventSystem.current.SetSelectedGameObject(cmdField.gameObject);
            }
        }

        private void RefreshPublicKey()
        {
            string cached = CentralServerKeyCache.ReadCache();
            string cachedSha = string.Empty;
            AsymmetricKeyParameter publicKey;
            if (!string.IsNullOrEmpty(cached))
            {
                publicKey = Cryptography.ECDSA.PublicKeyFromString(cached);
                string keyStr = Cryptography.ECDSA.KeyToString(publicKey);
                cachedSha = Cryptography.Sha.HashToString(Cryptography.Sha.Sha256(keyStr));

                AddLog("Loaded central server public key from cache.", Color.green);
                AddLog("SHA256 of public key: " + cachedSha, Color.green);

                _publicKey = publicKey;
                ServerConsole.PublicKey = publicKey;
            }

            Thread.Sleep(500);

            try
            {
                string url = string.Format(
                    "{0}v5/publickey.php?credits=2&major={1}",
                    CentralServer.StandardUrl,
                    Version.Major);

                string raw = HttpQuery.Get(url);
                PublicKeyResponse pkResponse = JsonSerialize.FromJson<PublicKeyResponse>(raw);

                if (pkResponse == null || string.IsNullOrEmpty(pkResponse.key))
                {
                    AddLog("Received empty or invalid public key response.", Color.red);
                    return;
                }

                if (!Cryptography.ECDSA.Verify(pkResponse.key, pkResponse.signature, CentralServerKeyCache.MasterKey))
                {
                    AddLog("Can't refresh central server public key - invalid signature!", Color.red);
                    AddLog("Response: " + raw, Color.red);
                    return;
                }

                publicKey = Cryptography.ECDSA.PublicKeyFromString(pkResponse.key);
                _publicKey = publicKey;
                ServerConsole.PublicKey = publicKey;

                try
                {
                    string decodedCredits = NorthwoodLib.StringUtils.Base64Decode(pkResponse.credits);
                    CreditsData.LoadData(decodedCredits);
                }
                catch (Exception creditsEx)
                {
                    AddLog("Can't load credits data: " + creditsEx.Message, Color.yellow);
                }

                string newKeyStr = Cryptography.ECDSA.KeyToString(publicKey);
                string newSha = Cryptography.Sha.HashToString(Cryptography.Sha.Sha256(newKeyStr));

                AddLog("Downloaded public key from central server.", Color.green);
                AddLog("SHA256 of public key: " + newSha, Color.green);

                if (newSha != cachedSha)
                {
                    CentralServerKeyCache.SaveCache(pkResponse.key, pkResponse.signature);
                    AddLog("Public key cache updated.", Color.green);
                }
                else
                {
                    AddLog("SHA256 of cached key matches, no need to update cache.", Color.green);
                }
            }
            catch (Exception ex)
            {
                AddLog("Can't refresh central server public key! Error: " + ex.Message, Color.red);
            }
        }

        private void OnApplicationQuit()
        {
            Shutdown.Quit(quit: false);
        }
    }
}