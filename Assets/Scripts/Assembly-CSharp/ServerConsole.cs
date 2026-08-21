public class ServerConsole : global::UnityEngine.MonoBehaviour, global::System.IDisposable
{
    public static ServerConsole singleton;

    private static string _serverName = string.Empty;

    private static readonly global::System.Func<float, float> _roundNormal = global::UnityEngine.Mathf.Round;

    private static readonly global::System.Func<float, float> _roundCeil = global::UnityEngine.Mathf.Ceil;

    private static readonly global::System.Func<float, float> _roundFloor = global::UnityEngine.Mathf.Floor;

    private static readonly global::System.Func<float, float, float> _pow = global::UnityEngine.Mathf.Pow;

    private static bool _disposing;

    public static global::System.Diagnostics.Process ConsoleProcess;

    public static string Password;

    public static string Ip;

    public static global::Org.BouncyCastle.Crypto.AsymmetricKeyParameter PublicKey;

    public static bool Update;

    public static bool ScheduleTokenRefresh;

    public static bool FriendlyFire = false;

    public static bool WhiteListEnabled = false;

    public static bool AccessRestriction = false;

    public static bool RateLimitKick;

    internal static bool EnforceSameIp;

    internal static bool EnforceSameAsn;

    internal static bool SkipEnforcementForLocalAddresses;

    internal static bool CustomGamemodeServerConfig;

    internal static bool HeavilyModdedServerConfig;

    private static bool _printedNotVerifiedMessage;

    private static bool _emailSet;

    private static float _heartbeatTimer;

    public static readonly ServerConsoleSender Scs = new ServerConsoleSender();

    public static ushort PortOverride;

    private static readonly global::System.Collections.Concurrent.ConcurrentBag<ReferenceHub> NewPlayers = new global::System.Collections.Concurrent.ConcurrentBag<ReferenceHub>();

    private static int _playersAmount;

    public static global::System.Collections.Generic.List<IOutput> ConsoleOutputs;

    internal static readonly global::System.Collections.Concurrent.ConcurrentQueue<string> PrompterQueue = new global::System.Collections.Concurrent.ConcurrentQueue<string>();

    private static readonly PlayerListSerialized PlayersListRaw = new PlayerListSerialized(new global::System.Collections.Generic.List<string>());

    private static string _verificationPlayersList = string.Empty;

    private static global::System.Threading.Thread _checkProcessThread;

    private static global::System.Threading.Thread _refreshPublicKeyThread;

    private static global::System.Threading.Thread _refreshPublicKeyOnceThread;

    private static global::System.Threading.Thread _verificationRequestThread;

    private static readonly global::System.Text.RegularExpressions.Regex _sizeRegex = new global::System.Text.RegularExpressions.Regex("(<size=(.*?)<\\/size>)", global::System.Text.RegularExpressions.RegexOptions.IgnoreCase | global::System.Text.RegularExpressions.RegexOptions.Compiled);

    private static readonly global::System.Text.RegularExpressions.Regex _colorRegex = new global::System.Text.RegularExpressions.Regex("(<color=(.*?)<\\/color>)", global::System.Text.RegularExpressions.RegexOptions.IgnoreCase | global::System.Text.RegularExpressions.RegexOptions.Compiled);

    public global::Utils.CommandInterpolation.InterpolatedCommandFormatter NameFormatter { get; private set; }

    internal static ushort PortToReport
    {
        get
        {
            if (PortOverride != 0)
            {
                return PortOverride;
            }
            return global::Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorTransport.Singleton.port;
        }
    }

    public static void ReloadServerName()
    {
        _serverName = global::GameCore.ConfigFile.ServerConfig.GetString("server_name", "My Server Name");
    }

    public void Dispose()
    {
        _disposing = true;
        ServerStatic.ServerOutput?.Dispose();
        if (_checkProcessThread != null && _checkProcessThread.IsAlive)
        {
            _checkProcessThread.Abort();
        }
        if (_verificationRequestThread != null && _verificationRequestThread.IsAlive)
        {
            _verificationRequestThread.Abort();
        }
        if (_refreshPublicKeyThread != null && _refreshPublicKeyThread.IsAlive)
        {
            _refreshPublicKeyThread.Abort();
        }
        if (_refreshPublicKeyOnceThread != null && _refreshPublicKeyOnceThread.IsAlive)
        {
            _refreshPublicKeyOnceThread.Abort();
        }
        if (_verificationRequestThread != null && _verificationRequestThread.IsAlive)
        {
            _verificationRequestThread.Abort();
        }
    }

    [global::System.Runtime.InteropServices.DllImport("libc", EntryPoint = "getuid")]
    private static extern uint GetUserId();

    private static void CheckRoot()
    {
        if (global::System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(global::System.Runtime.InteropServices.OSPlatform.Linux) && GetUserId() == 0)
        {
            global::GameCore.Console.AddLog("Running the game as ROOT is NOT recommended, please create a separate user!", global::UnityEngine.Color.red);
        }
    }

    private void Start()
    {
        CheckRoot();
        ConsoleOutputs = new global::System.Collections.Generic.List<IOutput>();
        NameFormatter = new global::Utils.CommandInterpolation.InterpolatedCommandFormatter
        {
            StartClosure = '{',
            EndClosure = '}',
            Escape = '\\',
            ArgumentSplitter = ',',
            Commands = new global::System.Collections.Generic.Dictionary<string, global::System.Func<global::System.Collections.Generic.List<string>, string>>
            {
                {
                    "ip",
                    (global::System.Collections.Generic.List<string> args) => Ip
                },
                {
                    "port",
                    (global::System.Collections.Generic.List<string> args) => global::Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorTransport.Singleton.port.ToString()
                },
                {
                    "number",
                    (global::System.Collections.Generic.List<string> args) => (global::Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorTransport.Singleton.port - 7776).ToString()
                },
                {
                    "version",
                    (global::System.Collections.Generic.List<string> args) => global::GameCore.Version.VersionString
                },
                {
                    "player_count",
                    (global::System.Collections.Generic.List<string> args) => ReferenceHub.AllHubs.Count.ToString()
                },
                {
                    "full_player_count",
                    delegate(global::System.Collections.Generic.List<string> args)
                    {
                        int count = ReferenceHub.AllHubs.Count;
                        if (count == CustomNetworkManager.TypedSingleton.ReservedMaxPlayers)
                        {
                            switch (args.Count)
                            {
                            case 1:
                                return "FULL";
                            case 2:
                                return NameFormatter.ProcessExpression(args[1]);
                            default:
                                throw new global::System.ArgumentOutOfRangeException("args", args, "Invalid arguments. Use: full_player_count OR full_player_count,[full]");
                            }
                        }
                        return $"{count}/{CustomNetworkManager.TypedSingleton.ReservedMaxPlayers}";
                    }
                },
                {
                    "max_players",
                    (global::System.Collections.Generic.List<string> args) => CustomNetworkManager.TypedSingleton.ReservedMaxPlayers.ToString()
                },
                {
                    "round_duration_minutes",
                    (global::System.Collections.Generic.List<string> args) => global::GameCore.RoundStart.RoundLength.Minutes.ToString("00")
                },
                {
                    "round_duration_seconds",
                    (global::System.Collections.Generic.List<string> args) => global::GameCore.RoundStart.RoundLength.Seconds.ToString("00")
                },
                {
                    "kills",
                    (global::System.Collections.Generic.List<string> args) => GetRoundInfo((RoundSummary summary) => RoundSummary.Kills.ToString(), intValue: true)
                },
                {
                    "alive_role",
                    delegate(global::System.Collections.Generic.List<string> args)
                    {
                        if (args.Count != 2)
                        {
                            throw new global::Utils.CommandInterpolation.CommandInputException("args", args, "Invalid arguments. Use: alive_role,[role ID]");
                        }
                        if (!global::System.Enum.TryParse<global::PlayerRoles.RoleTypeId>(NameFormatter.ProcessExpression(args[1]), out var role))
                        {
                            throw new global::Utils.CommandInterpolation.CommandInputException("role ID", args[1], "Could not parse.");
                        }
                        return GetRoundInfo((RoundSummary s) => s.CountRole(role).ToString(), intValue: true);
                    }
                },
                {
                    "alive_team",
                    delegate(global::System.Collections.Generic.List<string> args)
                    {
                        if (args.Count != 2)
                        {
                            throw new global::Utils.CommandInterpolation.CommandInputException("args", args, "Invalid arguments. Use: alive_team,[team ID]");
                        }
                        if (!global::System.Enum.TryParse<global::PlayerRoles.Team>(NameFormatter.ProcessExpression(args[1]), out var team))
                        {
                            throw new global::Utils.CommandInterpolation.CommandInputException("team ID", args[1], "Could not parse.");
                        }
                        return GetRoundInfo((RoundSummary s) => s.CountTeam(team).ToString(), intValue: true);
                    }
                },
                {
                    "zombies_recalled",
                    (global::System.Collections.Generic.List<string> args) => GetRoundInfo((RoundSummary summary) => RoundSummary.ChangedIntoZombies.ToString(), intValue: true)
                },
                {
                    "scp_counter",
                    (global::System.Collections.Generic.List<string> args) => GetRoundInfo((RoundSummary summary) => $"{summary.CountTeam(global::PlayerRoles.Team.SCPs) - summary.CountRole(global::PlayerRoles.RoleTypeId.Scp0492)}/{summary.classlistStart.scps_except_zombies}")
                },
                {
                    "scp_start",
                    (global::System.Collections.Generic.List<string> args) => GetRoundInfo((RoundSummary summary) => summary.classlistStart.scps_except_zombies.ToString(), intValue: true)
                },
                {
                    "scp_killed",
                    (global::System.Collections.Generic.List<string> args) => GetRoundInfo((RoundSummary summary) => (summary.classlistStart.scps_except_zombies - summary.CountTeam(global::PlayerRoles.Team.SCPs) - summary.CountRole(global::PlayerRoles.RoleTypeId.Scp0492)).ToString(), intValue: true)
                },
                {
                    "scp_kills",
                    (global::System.Collections.Generic.List<string> args) => GetRoundInfo((RoundSummary summary) => RoundSummary.KilledBySCPs.ToString(), intValue: true)
                },
                {
                    "classd_counter",
                    (global::System.Collections.Generic.List<string> args) => GetRoundInfo((RoundSummary summary) => $"{RoundSummary.EscapedClassD}/{summary.classlistStart.class_ds}", intValue: true)
                },
                {
                    "classd_start",
                    (global::System.Collections.Generic.List<string> args) => GetRoundInfo((RoundSummary summary) => summary.classlistStart.class_ds.ToString(), intValue: true)
                },
                {
                    "classd_escaped",
                    (global::System.Collections.Generic.List<string> args) => GetRoundInfo((RoundSummary summary) => RoundSummary.EscapedClassD.ToString(), intValue: true)
                },
                {
                    "scientist_counter",
                    (global::System.Collections.Generic.List<string> args) => GetRoundInfo((RoundSummary summary) => $"{RoundSummary.EscapedScientists}/{summary.classlistStart.scientists}")
                },
                {
                    "scientist_start",
                    (global::System.Collections.Generic.List<string> args) => GetRoundInfo((RoundSummary summary) => summary.classlistStart.scientists.ToString(), intValue: true)
                },
                {
                    "scientist_escaped",
                    (global::System.Collections.Generic.List<string> args) => GetRoundInfo((RoundSummary summary) => RoundSummary.EscapedScientists.ToString(), intValue: true)
                },
                {
                    "mtf_respawns",
                    (global::System.Collections.Generic.List<string> args) => GetRoundInfo((RoundSummary summary) => (global::Respawning.NamingRules.UnitNameMessageHandler.ReceivedNames.TryGetValue(global::Respawning.SpawnableTeamType.NineTailedFox, out var value) ? (value.Count - 1) : 0).ToString(), intValue: true)
                },
                {
                    "warhead_detonated",
                    delegate(global::System.Collections.Generic.List<string> args)
                    {
                        switch (args.Count)
                        {
                        case 1:
                            return GetRoundInfo((RoundSummary s) => (!AlphaWarheadController.Detonated) ? string.Empty : "☢ WARHEAD DETONATED ☢");
                        case 3:
                            return GetRoundInfo((RoundSummary s) => NameFormatter.ProcessExpression(args[AlphaWarheadController.Detonated ? 1 : 2]));
                        default:
                            throw new global::Utils.CommandInterpolation.CommandInputException("args", args, "Invalid arguments. Use: warhead_detonated OR warhead_detonated,[detonated],[undetonated]");
                        }
                    }
                },
                {
                    "random",
                    delegate(global::System.Collections.Generic.List<string> args)
                    {
                        float result;
                        float result2;
                        switch (args.Count)
                        {
                        case 2:
                        {
                            result = 0f;
                            string text2 = NameFormatter.ProcessExpression(args[1]);
                            if (!float.TryParse(text2, out result2))
                            {
                                throw new global::Utils.CommandInterpolation.CommandInputException("max", text2, "Could not parse.");
                            }
                            break;
                        }
                        case 3:
                        {
                            string text = NameFormatter.ProcessExpression(args[1]);
                            if (!float.TryParse(text, out result))
                            {
                                throw new global::Utils.CommandInterpolation.CommandInputException("min", text, "Could not parse.");
                            }
                            string text2 = NameFormatter.ProcessExpression(args[2]);
                            if (!float.TryParse(text2, out result2))
                            {
                                throw new global::Utils.CommandInterpolation.CommandInputException("max", text2, "Could not parse.");
                            }
                            break;
                        }
                        default:
                            throw new global::Utils.CommandInterpolation.CommandInputException("args", args, "Invalid arguments. Use: random,[max] OR random,[min],[max]");
                        }
                        return global::UnityEngine.Random.Range(result, result2).ToString();
                    }
                },
                {
                    "random_list",
                    delegate(global::System.Collections.Generic.List<string> args)
                    {
                        if (args.Count < 3)
                        {
                            throw new global::Utils.CommandInterpolation.CommandInputException("args", args, "Invalid arguments. Use: random_list,[entry 1],[entry 2]...");
                        }
                        return NameFormatter.ProcessExpression(args[global::UnityEngine.Random.Range(1, args.Count)]);
                    }
                },
                {
                    "constant_e",
                    (global::System.Collections.Generic.List<string> args) => ((float)global::System.Math.E).ToString()
                },
                {
                    "constant_pi",
                    (global::System.Collections.Generic.List<string> args) => ((float)global::System.Math.PI).ToString()
                },
                {
                    "add",
                    (global::System.Collections.Generic.List<string> args) => StandardizedFloatComparison("add", args, (float a, float b) => a + b)
                },
                {
                    "subtract",
                    (global::System.Collections.Generic.List<string> args) => StandardizedFloatComparison("subtract", args, (float a, float b) => a - b)
                },
                {
                    "multiply",
                    (global::System.Collections.Generic.List<string> args) => StandardizedFloatComparison("multiply", args, (float a, float b) => a * b)
                },
                {
                    "division",
                    (global::System.Collections.Generic.List<string> args) => StandardizedFloatComparison("division", args, (float a, float b) => a / b)
                },
                {
                    "power",
                    (global::System.Collections.Generic.List<string> args) => StandardizedFloatComparison("power", args, _pow)
                },
                {
                    "log",
                    delegate(global::System.Collections.Generic.List<string> args)
                    {
                        float result;
                        float result2;
                        switch (args.Count)
                        {
                        case 2:
                        {
                            string text = NameFormatter.ProcessExpression(args[1]);
                            if (!float.TryParse(text, out result))
                            {
                                throw new global::Utils.CommandInterpolation.CommandInputException("value", text, "Could not parse.");
                            }
                            result2 = 10f;
                            break;
                        }
                        case 3:
                        {
                            string text = NameFormatter.ProcessExpression(args[1]);
                            if (!float.TryParse(text, out result))
                            {
                                throw new global::Utils.CommandInterpolation.CommandInputException("value", text, "Could not parse.");
                            }
                            string text2 = NameFormatter.ProcessExpression(args[2]);
                            if (!float.TryParse(text2, out result2))
                            {
                                throw new global::Utils.CommandInterpolation.CommandInputException("base", text2, "Could not parse.");
                            }
                            break;
                        }
                        default:
                            throw new global::Utils.CommandInterpolation.CommandInputException("args", args, "Invalid arguments. Use log,[value] OR log,[value],[base]");
                        }
                        return global::UnityEngine.Mathf.Log(result, result2).ToString();
                    }
                },
                {
                    "ln",
                    delegate(global::System.Collections.Generic.List<string> args)
                    {
                        if (args.Count < 2)
                        {
                            throw new global::Utils.CommandInterpolation.CommandInputException("args", args, "Invalid arguments. Use ln,[value]");
                        }
                        string text = NameFormatter.ProcessExpression(args[1]);
                        if (!float.TryParse(text, out var result))
                        {
                            throw new global::Utils.CommandInterpolation.CommandInputException("value", text, "Error parsing value.");
                        }
                        return global::UnityEngine.Mathf.Log(result).ToString();
                    }
                },
                {
                    "round",
                    (global::System.Collections.Generic.List<string> args) => StandardizedFloatRound("round", args, _roundNormal)
                },
                {
                    "round_up",
                    (global::System.Collections.Generic.List<string> args) => StandardizedFloatRound("round_up", args, _roundCeil)
                },
                {
                    "round_down",
                    (global::System.Collections.Generic.List<string> args) => StandardizedFloatRound("round_down", args, _roundFloor)
                },
                {
                    "equals",
                    delegate(global::System.Collections.Generic.List<string> args)
                    {
                        if (args.Count != 3)
                        {
                            throw new global::Utils.CommandInterpolation.CommandInputException("args", args, "Invalid arguments. Use equals,[object A],[object B]");
                        }
                        return (args[1] == args[2]).ToString();
                    }
                },
                {
                    "greater",
                    (global::System.Collections.Generic.List<string> args) => StandardizedFloatComparison("greater", args, (float a, float b) => a > b)
                },
                {
                    "lesser",
                    (global::System.Collections.Generic.List<string> args) => StandardizedFloatComparison("lesser", args, (float a, float b) => a < b)
                },
                {
                    "greater_or_equal",
                    (global::System.Collections.Generic.List<string> args) => StandardizedFloatComparison("greater_or_equal", args, (float a, float b) => a >= b)
                },
                {
                    "lesser_or_equal",
                    (global::System.Collections.Generic.List<string> args) => StandardizedFloatComparison("lesser_or_equal", args, (float a, float b) => a <= b)
                },
                {
                    "not",
                    delegate(global::System.Collections.Generic.List<string> args)
                    {
                        if (args.Count != 2)
                        {
                            throw new global::Utils.CommandInterpolation.CommandInputException("args", args, "Invalid arguments. Use not,[value]");
                        }
                        string text = NameFormatter.ProcessExpression(args[1]);
                        if (!bool.TryParse(text, out var result))
                        {
                            throw new global::Utils.CommandInterpolation.CommandInputException("value", text, "Error parsing value.");
                        }
                        return (!result).ToString();
                    }
                },
                {
                    "or",
                    (global::System.Collections.Generic.List<string> args) => StandardizedBoolComparison("or", args, (bool a, bool b) => a || b)
                },
                {
                    "xor",
                    (global::System.Collections.Generic.List<string> args) => StandardizedBoolComparison("xor", args, (bool a, bool b) => a ^ b)
                },
                {
                    "and",
                    (global::System.Collections.Generic.List<string> args) => StandardizedBoolComparison("and", args, (bool a, bool b) => a && b)
                },
                {
                    "if",
                    delegate(global::System.Collections.Generic.List<string> args)
                    {
                        string text;
                        string text2;
                        switch (args.Count)
                        {
                        case 3:
                            text = args[2];
                            text2 = string.Empty;
                            break;
                        case 4:
                            text = args[2];
                            text2 = args[3];
                            break;
                        default:
                            throw new global::Utils.CommandInterpolation.CommandInputException("args", args, "Invalid arguments. Use if,[condition],[action] OR if,[condition],[action],[else action]");
                        }
                        string text3 = NameFormatter.ProcessExpression(args[1]);
                        if (!bool.TryParse(text3, out var result))
                        {
                            throw new global::Utils.CommandInterpolation.CommandInputException("condition", text3, "Could not parse.");
                        }
                        return NameFormatter.ProcessExpression(result ? text : text2);
                    }
                }
            }
        };
        CharacterClassManager.OnInstanceModeChanged += HandlePlayerJoin;
        ReferenceHub.OnPlayerRemoved = (global::System.Action<ReferenceHub>)global::System.Delegate.Combine(ReferenceHub.OnPlayerRemoved, (global::System.Action<ReferenceHub>)delegate
        {
            RefreshOnlinePlayers();
        });
        if (ServerStatic.IsDedicated && ServerStatic.ProcessIdPassed)
        {
            _checkProcessThread = new global::System.Threading.Thread(CheckProcess)
            {
                Priority = global::System.Threading.ThreadPriority.Lowest,
                IsBackground = true,
                Name = "Dedicated server console running check"
            };
            _checkProcessThread.Start();
        }
    }

    private static void HandlePlayerJoin(ReferenceHub rh, ClientInstanceMode mode)
    {
        if (mode == ClientInstanceMode.ReadyClient)
        {
            NewPlayers.Add(rh);
            RefreshOnlinePlayers();
        }
    }

    private void FixedUpdate()
    {
        if (ServerStatic.EnableConsoleHeartbeat)
        {
            _heartbeatTimer += global::UnityEngine.Time.fixedUnscaledDeltaTime;
            if (_heartbeatTimer >= 5f)
            {
                _heartbeatTimer = 0f;
                AddOutputEntry(default(global::ServerOutput.HeartbeatEntry));
            }
        }
        string result;
        while (PrompterQueue.TryDequeue(out result))
        {
            if (!string.IsNullOrWhiteSpace(result))
            {
                EnterCommand(result, Scs);
            }
        }
    }

    private static void RefreshOnlinePlayers()
    {
        try
        {
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.Mode == ClientInstanceMode.ReadyClient && !string.IsNullOrEmpty(allHub.characterClassManager.UserId) && (!allHub.isLocalPlayer || !ServerStatic.IsDedicated))
                {
                    PlayersListRaw.objects.Add(allHub.characterClassManager.UserId);
                }
            }
            _verificationPlayersList = JsonSerialize.ToJson(PlayersListRaw);
            _playersAmount = PlayersListRaw.objects.Count;
            PlayersListRaw.objects.Clear();
        }
        catch (global::System.Exception ex)
        {
            AddLog("[VERIFICATION] Exception in Players Online processing: " + ex.Message);
            AddLog(ex.StackTrace);
        }
    }

    private string StandardizedBoolComparison<T>(string source, global::System.Collections.Generic.IReadOnlyList<string> args, global::System.Func<bool, bool, T> comparison)
    {
        return StandardizedComparison(source, args, (string arg) => (success: bool.TryParse(arg, out var result), value: result), comparison);
    }

    private string StandardizedFloatComparison<T>(string source, global::System.Collections.Generic.IReadOnlyList<string> args, global::System.Func<float, float, T> comparison)
    {
        return StandardizedComparison(source, args, (string arg) => (success: float.TryParse(arg, out var result), value: result), comparison);
    }

    private string StandardizedComparison<TArg, TResult>(string source, global::System.Collections.Generic.IReadOnlyList<string> args, global::System.Func<string, (bool success, TArg value)> parse, global::System.Func<TArg, TArg, TResult> comparison)
    {
        if (args.Count != 3)
        {
            throw new global::Utils.CommandInterpolation.CommandInputException("args", args, "Invalid arguments. Use " + source + ",[value A],[value B]");
        }
        string arg = NameFormatter.ProcessExpression(args[1]);
        var (flag, arg2) = parse(arg);
        if (!flag)
        {
            throw new global::Utils.CommandInterpolation.CommandInputException("value A", args[1], "Could not parse.");
        }
        string text = NameFormatter.ProcessExpression(args[2]);
        var (flag2, arg3) = parse(text);
        if (!flag2)
        {
            throw new global::Utils.CommandInterpolation.CommandInputException("value B", text, "Could not parse.");
        }
        return comparison(arg2, arg3).ToString();
    }

    private string StandardizedFloatRound(string source, global::System.Collections.Generic.IReadOnlyList<string> args, global::System.Func<float, float> rounder)
    {
        float result;
        int result2;
        switch (args.Count)
        {
            case 2:
                {
                    string text = NameFormatter.ProcessExpression(args[1]);
                    if (!float.TryParse(text, out result))
                    {
                        throw new global::Utils.CommandInterpolation.CommandInputException("value", text, "Could not parse.");
                    }
                    result2 = 0;
                    break;
                }
            case 3:
                {
                    string text = NameFormatter.ProcessExpression(args[1]);
                    if (!float.TryParse(text, out result))
                    {
                        throw new global::Utils.CommandInterpolation.CommandInputException("value", text, "Could not parse.");
                    }
                    string text2 = NameFormatter.ProcessExpression(args[1]);
                    if (!int.TryParse(text2, out result2))
                    {
                        throw new global::Utils.CommandInterpolation.CommandInputException("precision", text2, "Could not parse.");
                    }
                    break;
                }
            default:
                throw new global::Utils.CommandInterpolation.CommandInputException("args", args, "Invalid arguments. Use " + source + ",[value] OR " + source + ",[value],[precision]");
        }
        float num = global::UnityEngine.Mathf.Pow(10f, result2);
        return (rounder(result * num) / num).ToString(global::System.Globalization.CultureInfo.InvariantCulture);
    }

    private static string GetRoundInfo(global::System.Func<RoundSummary, string> getter, bool intValue = false)
    {
        if (!(RoundSummary.singleton == null))
        {
            return getter(RoundSummary.singleton);
        }
        if (!intValue)
        {
            return "-";
        }
        return "-1";
    }

    public string RefreshServerName()
    {
        return NameFormatter.ProcessExpression(_serverName);
    }

    public string RefreshServerNameSafe()
    {
        if (NameFormatter.TryProcessExpression(_serverName, "server name", out var result))
        {
            return result;
        }
        AddLog(result);
        return "Command errored";
    }

    private void Awake()
    {
        singleton = this;
    }

    private static void CheckProcess()
    {
        while (!_disposing)
        {
            global::System.Threading.Thread.Sleep(4000);
            if (ConsoleProcess == null || ConsoleProcess.HasExited)
            {
                ConsoleProcess?.Dispose();
                ConsoleProcess = null;
                DisposeStatic();
            }
        }
    }

    public void OnDestroy()
    {
        Dispose();
    }

    public void OnApplicationQuit()
    {
        Dispose();
    }

    public static void DisposeStatic()
    {
        singleton.Dispose();
    }

    public static void AddLog(string q, global::System.ConsoleColor color = global::System.ConsoleColor.Gray)
    {
        PrintOnOutputs(q, color);
        PrintFormattedString(q, color);
    }

    public static void AddOutputEntry(global::ServerOutput.IOutputEntry entry)
    {
        PrintOnOutputs(entry.ToString(), global::System.ConsoleColor.Gray);
        ServerStatic.ServerOutput?.AddOutput(entry);
    }

    public static string GetClientInfo(global::Mirror.NetworkConnectionToClient conn)
    {
        global::UnityEngine.GameObject gameObject = global::GameCore.Console.FindConnectedRoot(conn);
        return gameObject.GetComponent<NicknameSync>().MyNick + " ( " + gameObject.GetComponent<CharacterClassManager>().UserId + " | " + conn.address + " )";
    }

    public static string GetClientInfo(global::UnityEngine.GameObject gameObject)
    {
        return gameObject.GetComponent<NicknameSync>().MyNick + " ( " + gameObject.GetComponent<CharacterClassManager>().UserId + " | " + gameObject.GetComponent<global::Mirror.NetworkBehaviour>().connectionToClient.address + " )";
    }

    public static void Disconnect(global::UnityEngine.GameObject player, string message)
    {
        if (player == null)
        {
            return;
        }
        global::Mirror.NetworkBehaviour component = player.GetComponent<global::Mirror.NetworkBehaviour>();
        if (!(component == null))
        {
            CharacterClassManager component2 = player.GetComponent<CharacterClassManager>();
            if (component2 == null)
            {
                component.connectionToClient.Disconnect();
            }
            else
            {
                component2.DisconnectClient(component.connectionToClient, message);
            }
        }
    }

    public static void Disconnect(global::Mirror.NetworkConnection conn, string message)
    {
        global::UnityEngine.GameObject gameObject = global::GameCore.Console.FindConnectedRoot(conn);
        if (gameObject == null)
        {
            conn.Disconnect();
        }
        else
        {
            Disconnect(gameObject, message);
        }
    }

    public static string ColorText(string text, global::System.ConsoleColor color)
    {
        return "<color=" + ConsoleColorToHex(color) + ">" + text + "</color>";
    }

    public static void ColorDebugLog(string text, global::System.ConsoleColor color)
    {
        global::UnityEngine.Debug.Log(ColorText(text, color), null);
    }

    public static void PrintFormattedString(string text, global::System.ConsoleColor defaultColor)
    {
        text = _sizeRegex.Replace(text, "").Trim();
        string[] array = _colorRegex.Split(text);
        for (int i = 0; i < array.Length; i++)
        {
            string text2 = array[i];
            if (!text2.ToLowerInvariant().StartsWith("<color=", global::System.StringComparison.Ordinal))
            {
                ServerStatic.ServerOutput?.AddLog(text2, defaultColor);
                continue;
            }
            string text3 = text2.Substring(7);
            text3 = text3.Substring(0, text3.IndexOf('>')).Replace("\"", "").Replace("'", "");
            string text4 = text2.Substring(text2.IndexOf('>') + 1);
            text4 = text4.Substring(0, text4.IndexOf('<'));
            bool flag = text3.StartsWith("#", global::System.StringComparison.Ordinal);
            global::System.ConsoleColor color = defaultColor;
            global::System.ConsoleColor result;
            if (flag && Misc.TryParseColor(text3, out var color2))
            {
                color = Misc.ClosestConsoleColor(color2, excludeDark: false);
            }
            else if (!flag && global::System.Enum.TryParse<global::System.ConsoleColor>(text3, ignoreCase: true, out result))
            {
                color = result;
            }
            ServerStatic.ServerOutput?.AddLog(text4, color);
            i++;
        }
    }

    public static string ConsoleColorToHex(global::System.ConsoleColor color)
    {
        switch (color)
        {
            case global::System.ConsoleColor.Black:
                return "#000000";
            case global::System.ConsoleColor.Blue:
                return "#0078F8";
            case global::System.ConsoleColor.Cyan:
                return "#00FFFF";
            case global::System.ConsoleColor.DarkBlue:
                return "#0058F8";
            case global::System.ConsoleColor.DarkCyan:
                return "#00B7EB";
            case global::System.ConsoleColor.DarkGray:
                return "#787878";
            case global::System.ConsoleColor.DarkGreen:
                return "#005800";
            case global::System.ConsoleColor.DarkMagenta:
                return "#FF0090";
            case global::System.ConsoleColor.DarkRed:
                return "#A80020";
            case global::System.ConsoleColor.DarkYellow:
                return "#AC7C00";
            case global::System.ConsoleColor.Gray:
                return "#D8D8D8";
            case global::System.ConsoleColor.Green:
                return "#00B800";
            case global::System.ConsoleColor.Magenta:
                return "#FF00FF";
            case global::System.ConsoleColor.Red:
                return "#DC143C";
            case global::System.ConsoleColor.White:
                return "#FCFCFC";
            case global::System.ConsoleColor.Yellow:
                return "#F8B800";
            default:
                return "#6844FC";
        }
    }

    public static global::UnityEngine.Color ConsoleColorToColor(global::System.ConsoleColor color)
    {
        switch (color)
        {
            case global::System.ConsoleColor.Black:
                return global::UnityEngine.Color.black;
            case global::System.ConsoleColor.Blue:
                return global::UnityEngine.Color.blue;
            case global::System.ConsoleColor.Cyan:
                return global::UnityEngine.Color.cyan;
            case global::System.ConsoleColor.DarkBlue:
                return new global::UnityEngine.Color(0f, 88f, 248f);
            case global::System.ConsoleColor.DarkCyan:
                return new global::UnityEngine.Color(0f, 183f, 235f);
            case global::System.ConsoleColor.DarkGray:
                return global::UnityEngine.Color.gray;
            case global::System.ConsoleColor.DarkGreen:
                return new global::UnityEngine.Color(0f, 88f, 0f);
            case global::System.ConsoleColor.DarkMagenta:
                return new global::UnityEngine.Color(255f, 0f, 144f);
            case global::System.ConsoleColor.DarkRed:
                return new global::UnityEngine.Color(168f, 0f, 32f);
            case global::System.ConsoleColor.DarkYellow:
                return new global::UnityEngine.Color(172f, 124f, 0f);
            case global::System.ConsoleColor.Gray:
                return new global::UnityEngine.Color(216f, 216f, 216f);
            case global::System.ConsoleColor.Green:
                return global::UnityEngine.Color.green;
            case global::System.ConsoleColor.Magenta:
                return global::UnityEngine.Color.magenta;
            case global::System.ConsoleColor.Red:
                return global::UnityEngine.Color.red;
            case global::System.ConsoleColor.White:
                return new global::UnityEngine.Color(252f, 252f, 252f);
            case global::System.ConsoleColor.Yellow:
                return global::UnityEngine.Color.yellow;
            default:
                return new global::UnityEngine.Color(104f, 68f, 252f);
        }
    }

    public static string EnterCommand(string cmds, CommandSender sender = null)
    {
        if (sender == null)
        {
            sender = Scs;
        }
        string text = string.Empty;
        string[] args = cmds.Split(' ');
        global::System.ConsoleColor c = global::System.ConsoleColor.Gray;
        if (args.Length == 0)
        {
            return text;
        }
        string cmd = args[0];
        switch (cmd.ToUpper())
        {
            case "FS":
            case "FORCESTART":
                text = (CharacterClassManager.ForceRoundStart() ? "Forced round start." : "Failed to force start.");
                c = global::System.ConsoleColor.DarkRed;
                break;
            case "SNR":
            case "STOPNEXTROUND":
                if (!ServerStatic.IsDedicated)
                {
                    text = "This command can be only executed on a dedicated servers.";
                }
                else if (ServerStatic.StopNextRound == ServerStatic.NextRoundAction.Shutdown)
                {
                    ServerStatic.StopNextRound = ServerStatic.NextRoundAction.DoNothing;
                    AddOutputEntry(default(global::ServerOutput.ExitActionResetEntry));
                    text = "Server WON'T stop after next round.";
                }
                else
                {
                    ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Shutdown;
                    AddOutputEntry(default(global::ServerOutput.ExitActionShutdownEntry));
                    text = "Server WILL stop after next round.";
                }
                break;
            case "RNR":
            case "RESTARTNEXTROUND":
                if (!ServerStatic.IsDedicated)
                {
                    text = "This command can be only executed on a dedicated servers.";
                }
                else if (ServerStatic.StopNextRound == ServerStatic.NextRoundAction.Restart)
                {
                    ServerStatic.StopNextRound = ServerStatic.NextRoundAction.DoNothing;
                    AddOutputEntry(default(global::ServerOutput.ExitActionResetEntry));
                    text = "Server WON'T restart after next round.";
                }
                else
                {
                    ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                    AddOutputEntry(default(global::ServerOutput.ExitActionRestartEntry));
                    text = "Server WILL restart after next round.";
                }
                break;
            case "CONFIG":
            case "C":
                if (global::System.IO.File.Exists(FileManager.GetAppFolder(addSeparator: true, serverConfig: true) + "config_gameplay.txt"))
                {
                    global::UnityEngine.Application.OpenURL(FileManager.GetAppFolder(addSeparator: true, serverConfig: true) + "config_gameplay.txt");
                }
                else
                {
                    text = "Config file not found!";
                }
                break;
            case "IDLE":
            case "I":
                if (args.Length == 1)
                {
                    text = "Server is " + (IdleMode.IdleModeActive ? "" : "**NOT** ") + "in the idle mode.";
                    break;
                }
                switch (args[1].ToUpper())
                {
                    case "E":
                    case "-E":
                        if (IdleMode.IdleModeActive)
                        {
                            text = "Server is already in the idle mode.";
                        }
                        else if (ReferenceHub.AllHubs.Count > 1)
                        {
                            text = "You can't enable the idle mode when players are connected to the server.";
                        }
                        else
                        {
                            IdleMode.SetIdleMode(state: true);
                        }
                        break;
                    case "EF":
                    case "-EF":
                        if (IdleMode.IdleModeActive)
                        {
                            text = "Server is already in the idle mode.";
                        }
                        else
                        {
                            IdleMode.SetIdleMode(state: true);
                        }
                        break;
                    case "D":
                    case "-D":
                        if (!IdleMode.IdleModeActive)
                        {
                            text = "Server isn't in the idle mode.";
                        }
                        else
                        {
                            IdleMode.SetIdleMode(state: false);
                        }
                        break;
                }
                break;
            default:
                if (cmd.StartsWith("!", global::System.StringComparison.Ordinal) && cmd.Length > 1)
                {
                    if (cmd.StartsWith("!verify", global::System.StringComparison.OrdinalIgnoreCase) && !_emailSet)
                    {
                        return "You have to set the contact email address (\"contact_email\" key in the gameplay config) before running this command!";
                    }
                    text = "Sending command to central servers...";
                    global::System.Threading.Thread thread = new global::System.Threading.Thread((global::System.Threading.ThreadStart)delegate
                    {
                        RunCentralServerCommand(cmd.Substring(1).ToLower(), (args.Length == 1) ? "" : global::System.Linq.Enumerable.Aggregate(global::System.Linq.Enumerable.Skip(args, 1), (string current, string next) => current + " " + next));
                    });
                    thread.IsBackground = true;
                    thread.Priority = global::System.Threading.ThreadPriority.AboveNormal;
                    thread.Name = "SCP:SL Central server command execution";
                    thread.Start();
                    break;
                }
                return global::GameCore.Console.Singleton.TypeCommand(cmds, sender ?? Scs);
        }
        sender.Print(text, c);
        return text;
    }

    public void RunServer()
    {
        if (_verificationRequestThread != null && _verificationRequestThread.IsAlive)
        {
            _verificationRequestThread.Abort();
        }
        _verificationRequestThread = new global::System.Threading.Thread(RefreshServerData)
        {
            IsBackground = true,
            Priority = global::System.Threading.ThreadPriority.AboveNormal,
            Name = "SCP:SL Server list thread"
        };
        _verificationRequestThread.Start();
    }

    internal static void RunRefreshPublicKey()
    {
        if (_refreshPublicKeyThread != null && _refreshPublicKeyThread.IsAlive)
        {
            _refreshPublicKeyThread.Abort();
        }
        _refreshPublicKeyThread = new global::System.Threading.Thread(RefreshPublicKey)
        {
            IsBackground = true,
            Priority = global::System.Threading.ThreadPriority.Normal,
            Name = "SCP:SL Public key refreshing"
        };
        _refreshPublicKeyThread.Start();
    }

    internal static void RunRefreshPublicKeyOnce()
    {
        if (_refreshPublicKeyOnceThread != null && _refreshPublicKeyOnceThread.IsAlive)
        {
            _refreshPublicKeyOnceThread.Abort();
        }
        _refreshPublicKeyOnceThread = new global::System.Threading.Thread(RefreshPublicKeyOnce)
        {
            IsBackground = true,
            Priority = global::System.Threading.ThreadPriority.AboveNormal,
            Name = "SCP:SL Public key refreshing ON DEMAND"
        };
        _refreshPublicKeyOnceThread.Start();
    }

    private static void RefreshPublicKey()
    {
        string text = CentralServerKeyCache.ReadCache();
        string text2 = string.Empty;
        string text3 = string.Empty;
        bool flag = true;
        if (!string.IsNullOrEmpty(text))
        {
            PublicKey = global::Cryptography.ECDSA.PublicKeyFromString(text);
            text2 = global::Cryptography.Sha.HashToString(global::Cryptography.Sha.Sha256(global::Cryptography.ECDSA.KeyToString(PublicKey)));
            AddLog("Loaded central server public key from cache.");
            AddLog("SHA256 of public key: " + text2);
        }
        AddLog("Downloading public key from central server...");
        while (!_disposing)
        {
            try
            {
                PublicKeyResponse publicKeyResponse = JsonSerialize.FromJson<PublicKeyResponse>(HttpQuery.Get($"{CentralServer.StandardUrl}v5/publickey.php?major={(global::GameCore.Version.Major)}"));
                if (!global::Cryptography.ECDSA.Verify(publicKeyResponse.key, publicKeyResponse.signature, CentralServerKeyCache.MasterKey))
                {
                    global::GameCore.Console.AddLog("Can't refresh central server public key - invalid signature!", global::UnityEngine.Color.red);
                    global::System.Threading.Thread.Sleep(360000);
                    continue;
                }
                PublicKey = global::Cryptography.ECDSA.PublicKeyFromString(publicKeyResponse.key);
                string text4 = global::Cryptography.Sha.HashToString(global::Cryptography.Sha.Sha256(global::Cryptography.ECDSA.KeyToString(PublicKey)));
                if (text4 != text3)
                {
                    text3 = text4;
                    AddLog("Downloaded public key from central server.");
                    AddLog("SHA256 of public key: " + text4);
                    if (text4 != text2)
                    {
                        CentralServerKeyCache.SaveCache(publicKeyResponse.key, publicKeyResponse.signature);
                    }
                    else
                    {
                        AddLog("SHA256 of cached key matches, no need to update cache.");
                    }
                }
                else if (flag)
                {
                    flag = false;
                    AddLog("Refreshed public key of central server - key hash not changed.");
                }
            }
            catch (global::System.Exception ex)
            {
                AddLog("Can't refresh central server public key - " + ex.Message);
            }
            global::System.Threading.Thread.Sleep(360000);
        }
    }

    private static void RefreshPublicKeyOnce()
    {
        try
        {
            PublicKeyResponse publicKeyResponse = JsonSerialize.FromJson<PublicKeyResponse>(HttpQuery.Get($"{CentralServer.StandardUrl}v5/publickey.php?major={(global::GameCore.Version.Major)}"));
            if (!global::Cryptography.ECDSA.Verify(publicKeyResponse.key, publicKeyResponse.signature, CentralServerKeyCache.MasterKey))
            {
                global::GameCore.Console.AddLog("Can't refresh central server public key - invalid signature!", global::UnityEngine.Color.red);
                return;
            }
            PublicKey = global::Cryptography.ECDSA.PublicKeyFromString(publicKeyResponse.key);
            string text = global::Cryptography.Sha.HashToString(global::Cryptography.Sha.Sha256(global::Cryptography.ECDSA.KeyToString(PublicKey)));
            AddLog("Downloaded public key from central server.");
            AddLog("SHA256 of public key: " + text);
            CentralServerKeyCache.SaveCache(publicKeyResponse.key, publicKeyResponse.signature);
        }
        catch (global::System.Exception ex)
        {
            AddLog("Can't refresh central server public key - " + ex.Message);
        }
    }

    private static void RunCentralServerCommand(string cmd, string args)
    {
        cmd = cmd.ToLower();
        global::System.Collections.Generic.List<string> list = new global::System.Collections.Generic.List<string>
        {
            "ip=" + Ip,
            "port=" + PortToReport,
            "cmd=" + global::NorthwoodLib.StringUtils.Base64Encode(cmd),
            "args=" + global::NorthwoodLib.StringUtils.Base64Encode(args)
        };
        if (!string.IsNullOrEmpty(Password))
        {
            list.Add("passcode=" + Password);
        }
        try
        {
            string text = HttpQuery.Post(CentralServer.MasterUrl + "centralcommands/" + cmd + ".php", HttpQuery.ToPostArgs(list));
            AddLog("[" + cmd + "] " + text);
        }
        catch (global::System.Exception ex)
        {
            AddLog("Could not execute the central server command \"" + cmd + "\" - (LOCAL EXCEPTION) " + ex.Message, global::System.ConsoleColor.Red);
        }
    }

    internal static void RefreshEmailSetStatus()
    {
        _emailSet = !string.IsNullOrEmpty(global::GameCore.ConfigFile.ServerConfig.GetString("contact_email"));
    }

    private void RefreshServerData()
    {
        bool flag = true;
        byte b = 0;
        RefreshEmailSetStatus();
        RefreshToken(init: true);
        while (!_disposing)
        {
            b++;
            if (!flag && string.IsNullOrEmpty(Password) && b < 15)
            {
                if (b == 5 || b == 12 || ScheduleTokenRefresh)
                {
                    RefreshToken();
                }
            }
            else
            {
                flag = false;
                Update = Update || b == 10;
                string text = string.Empty;
                try
                {
                    int count = NewPlayers.Count;
                    int num = 0;
                    global::System.Collections.Generic.List<global::Authenticator.AuthenticatorPlayerObject> list = global::NorthwoodLib.Pools.ListPool<global::Authenticator.AuthenticatorPlayerObject>.Shared.Rent();
                    while (!NewPlayers.IsEmpty)
                    {
                        num++;
                        if (num > count + 30)
                        {
                            break;
                        }
                        try
                        {
                            if (NewPlayers.TryTake(out var result) && result != null)
                            {
                                list.Add(new global::Authenticator.AuthenticatorPlayerObject(result.characterClassManager.UserId, (result.characterClassManager.Connection == null || string.IsNullOrEmpty(result.characterClassManager.connectionToClient.address)) ? "N/A" : result.characterClassManager.connectionToClient.address, result.characterClassManager.RequestIp, result.characterClassManager.Asn, result.characterClassManager.AuthTokenSerial, result.characterClassManager.VacSession));
                            }
                        }
                        catch (global::System.Exception ex)
                        {
                            AddLog("[VERIFICATION THREAD] Exception in New Player (inside of loop) processing: " + ex.Message);
                            AddLog(ex.StackTrace);
                        }
                    }
                    text = JsonSerialize.ToJson(new global::Authenticator.AuthenticatorPlayerObjects(list));
                    global::NorthwoodLib.Pools.ListPool<global::Authenticator.AuthenticatorPlayerObject>.Shared.Return(list);
                }
                catch (global::System.Exception ex2)
                {
                    AddLog("[VERIFICATION THREAD] Exception in New Players processing: " + ex2.Message);
                    AddLog(ex2.StackTrace);
                }
                global::System.Collections.Generic.List<string> list3;
                if (!Update)
                {
                    global::System.Collections.Generic.List<string> list2 = new global::System.Collections.Generic.List<string>();
                    list2.Add("ip=" + Ip);
                    list2.Add("players=" + _playersAmount + "/" + CustomNetworkManager.slots);
                    list2.Add("newPlayers=" + text);
                    list2.Add("port=" + PortToReport);
                    list2.Add("version=2");
                    list2.Add("enforceSameIp=" + EnforceSameIp);
                    list2.Add("enforceSameAsn=" + EnforceSameAsn);
                    list3 = list2;
                }
                else
                {
                    global::System.Collections.Generic.List<string> list2 = new global::System.Collections.Generic.List<string>();
                    list2.Add("ip=" + Ip);
                    list2.Add("players=" + _playersAmount + "/" + CustomNetworkManager.slots);
                    list2.Add("playersList=" + _verificationPlayersList);
                    list2.Add("newPlayers=" + text);
                    list2.Add("port=" + PortToReport);
                    list2.Add("pastebin=" + global::GameCore.ConfigFile.ServerConfig.GetString("serverinfo_pastebin_id", "7wV681fT"));
                    list2.Add("gameVersion=" + global::GameCore.Version.VersionString);
                    list2.Add("version=2");
                    list2.Add("update=1");
                    list2.Add("info=" + global::NorthwoodLib.StringUtils.Base64Encode(RefreshServerNameSafe()).Replace('+', '-'));
                    list2.Add("privateBeta=" + global::GameCore.Version.PrivateBeta);
                    list2.Add("staffRA=" + ServerStatic.PermissionsHandler.StaffAccess);
                    list2.Add("friendlyFire=" + FriendlyFire);
                    list2.Add("geoblocking=" + (byte)CustomLiteNetLib4MirrorTransport.Geoblocking);
                    list2.Add("modded=" + (CustomNetworkManager.Modded || HeavilyModdedServerConfig));
                    list2.Add("heavilyModded=" + (HeavilyModdedServerConfig));
                    list2.Add("cgs=" + (CustomGamemodeServerConfig));
                    list2.Add("whitelist=" + WhiteListEnabled);
                    list2.Add("accessRestriction=" + AccessRestriction);
                    list2.Add("emailSet=" + _emailSet);
                    list2.Add("enforceSameIp=" + EnforceSameIp);
                    list2.Add("enforceSameAsn=" + EnforceSameAsn);
                    list3 = list2;
                }
                global::System.Collections.Generic.List<string> list4 = list3;
                if (!string.IsNullOrEmpty(Password))
                {
                    list4.Add("passcode=" + Password);
                }
                Update = false;
                if (!global::Authenticator.AuthenticatorQuery.SendData(list4) && !_printedNotVerifiedMessage)
                {
                    _printedNotVerifiedMessage = true;
                    AddLog("Your server won't be visible on the public server list - (" + Ip + ")", global::System.ConsoleColor.Red);
                    if (!_emailSet)
                    {
                        AddLog("If you are 100% sure that the server is working, can be accessed from the Internet and YOU WANT TO MAKE IT PUBLIC, please set up your email in configuration file (\"contact_email\" value) and restart the server.", global::System.ConsoleColor.Red);
                    }
                    else
                    {
                        AddLog("If you are 100% sure that the server is working, can be accessed from the Internet and YOU WANT TO MAKE IT PUBLIC please email following information:", global::System.ConsoleColor.Red);
                        AddLog("- IP address of server (most likely " + Ip + ")", global::System.ConsoleColor.Red);
                        AddLog("- port of the server (currently the server is running on port " + PortToReport + ")", global::System.ConsoleColor.Red);
                        AddLog("- is this static or dynamic IP address (most of home adresses are dynamic)", global::System.ConsoleColor.Red);
                        AddLog("PLEASE READ rules for verified servers first: https://scpslgame.com/Verified_server_rules.pdf", global::System.ConsoleColor.Red);
                        AddLog("send us that information to: server.verification@scpslgame.com (server.verification at scpslgame.com)", global::System.ConsoleColor.Red);
                        AddLog("if you can't see the AT sign in console (in above line): server.verification AT scpslgame.com", global::System.ConsoleColor.Red);
                        AddLog("email must be sent from email address set as \"contact_email\" in your config file (current value: " + global::GameCore.ConfigFile.ServerConfig.GetString("contact_email") + ").", global::System.ConsoleColor.Red);
                    }
                }
                else
                {
                    _printedNotVerifiedMessage = true;
                }
            }
            if (b >= 15)
            {
                b = 0;
            }
            global::System.Threading.Thread.Sleep(5000);
            if (ScheduleTokenRefresh || b == 0)
            {
                RefreshToken();
            }
        }
    }

    private static void PrintOnOutputs(string text, global::System.ConsoleColor color)
    {
        if (ConsoleOutputs == null)
        {
            return;
        }
        foreach (IOutput consoleOutput in ConsoleOutputs)
        {
            consoleOutput.Print(text, color);
        }
    }

    private static void RefreshToken(bool init = false)
    {
        ScheduleTokenRefresh = false;
        string path = global::System.Environment.GetFolderPath(global::System.Environment.SpecialFolder.ApplicationData) + "/SCP Secret Laboratory/verkey.txt";
        if (!global::System.IO.File.Exists(path))
        {
            return;
        }
        using (global::System.IO.StreamReader streamReader = new global::System.IO.StreamReader(path))
        {
            string text = streamReader.ReadToEnd().Trim();
            if (!init && string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(text))
            {
                AddLog("Verification token loaded! Server probably will be listed on public list.");
            }
            if (Password != text)
            {
                AddLog("Verification token reloaded.");
                Update = true;
            }
            Password = text;
            ServerStatic.PermissionsHandler.SetServerAsVerified();
        }
    }
}
