using CustomPlayerEffects;
using Discord;
using GameCore;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Spectating;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class NicknameSync : NetworkBehaviour
{
    public LayerMask RaycastMask;

    private ReferenceHub _hub;

    private const float FlashedThreshold = 0.5f;

    private Text _nText;

    private CanvasRenderer _renderer;

    private float _transparency;

    private Flashed _localFlashed;

    private Regex _nickFilter;

    private string _replacement;

    [SyncVar]
    public float ViewRange;

    [SyncVar(hook = nameof(SetCustomInfo))]
    private string _customPlayerInfoString;

    [SyncVar]
    private PlayerInfoArea _playerInfoToShow = PlayerInfoArea.Nickname | PlayerInfoArea.Badge | PlayerInfoArea.CustomInfo | PlayerInfoArea.Role | PlayerInfoArea.UnitName | PlayerInfoArea.PowerStatus;

    [SyncVar(hook = nameof(UpdatePlayerlistInstance))]
    private string _myNickSync;

    private string _firstNickname;

    private const ushort MaxNicknameLen = 48;

    [SyncVar(hook = nameof(UpdateCustomName))]
    private string _displayName;

    private string _cleanDisplayName;

    public bool NickSet { get; private set; }

    public string SanitizedPlayerInfoString { get; private set; }

    public PlayerInfoArea ShownPlayerInfo
    {
        get
        {
            return _playerInfoToShow;
        }
        set
        {
            if (NetworkServer.active)
            {
                _playerInfoToShow = value;
            }
        }
    }

    public string CustomPlayerInfo
    {
        get
        {
            return _customPlayerInfoString;
        }
        set
        {
            if (NetworkServer.active)
            {
                SetCustomInfo(_customPlayerInfoString, value);
                _customPlayerInfoString = value;
            }
        }
    }

    public string DisplayName
    {
        get
        {
            return (HasCustomName ? _cleanDisplayName : MyNick) ?? string.Empty;
        }
        set
        {
            _displayName = value;
            UpdatePlayerlistInstance(null, _displayName);
        }
    }

    public bool HasCustomName => _cleanDisplayName != null;

    public string CombinedName
    {
        get
        {
            if (!HasCustomName)
            {
                return MyNick;
            }
            return _cleanDisplayName + " (" + MyNick + ")";
        }
    }

    public string MyNick
    {
        get
        {
            if (NickSet)
            {
                return _firstNickname;
            }
            if (_myNickSync == null)
            {
                return "(null)";
            }
            NickSet = true;
            _firstNickname = Misc.SanitizeRichText(_myNickSync.Replace("\n", string.Empty).Replace("\r", string.Empty), "＜", "＞");
            if (_firstNickname.Length > 48)
            {
                _firstNickname = _firstNickname[..48];
            }
            return _firstNickname;
        }
        private set
        {
            value ??= "(null)";
            string text = Misc.SanitizeRichText(value, "＜", "＞");
            _myNickSync = ((value.Length > 48) ? text[..48] : text);
            if (NetworkServer.active)
            {
                NickSet = true;
                _firstNickname = _myNickSync;
            }
        }
    }

    private void UpdatePlayerlistInstance(string p, string username)
    {
        if (_hub == null)
            ReferenceHub.TryGetHub(base.gameObject, out _hub);
        PlayerList.UpdatePlayerNickname(_hub);
    }

    private void UpdateCustomName(string p, string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            _cleanDisplayName = null;
        }
        else
        {
            _cleanDisplayName = Misc.SanitizeRichText(username.Replace("\n", string.Empty).Replace("\r", string.Empty)).Trim();
            if (_cleanDisplayName.Length > 48)
            {
                _cleanDisplayName = _cleanDisplayName.Substring(0, 48);
            }
            _cleanDisplayName += "<color=#855439>*</color>";
        }
        UpdatePlayerlistInstance(p, username);
    }

    private void Start()
    {
        _hub = ReferenceHub.GetHub(base.gameObject);
        _nickFilter = null;
        _replacement = "";
        if (NetworkServer.active)
        {
            ViewRange = ConfigFile.ServerConfig.GetFloat("player_info_range", 10f);
            string text = ConfigFile.ServerConfig.GetString("nickname_filter") ?? "";
            if (!string.IsNullOrEmpty(text))
            {
                _nickFilter = new Regex(text, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(500.0));
                _replacement = ConfigFile.ServerConfig.GetString("nickname_filter_replacement") ?? "";
            }
        }
        if (base.isLocalPlayer)
        {
            string personaName = "";
            switch (CentralAuthManager.Platform)
            {
                case GameCore.DistributionPlatform.Steam:
                    personaName = SteamManager.GetPersonaName(SteamManager.SteamId64);
                    break;
                default:
                    GameCore.Console.AddLog("Unknown platform, setting nickname to default.", Color.red);
                    break;
            }

            if ((string.IsNullOrEmpty(personaName) || personaName == "Unknown") && SteamManager.IsSteamReady())
            {
                personaName = Steamworks.SteamClient.Name;
            }

            if (string.IsNullOrEmpty(personaName) || personaName == "Unknown")
            {
                personaName = "SCP:SL Player";
            }

            if (!PlayerPrefsSl.HasKey("nickname", PlayerPrefsSl.DataType.String))
            {
                PlayerPrefsSl.Set("nickname", personaName);
            }

            string savedNick = PlayerPrefsSl.Get("nickname", "");

            if ((savedNick == "SCP:SL Player" || savedNick == "Unknown") && !string.IsNullOrEmpty(personaName) && personaName != "SCP:SL Player")
            {
                savedNick = personaName;
                PlayerPrefsSl.Set("nickname", personaName);
            }

            if (string.IsNullOrEmpty(savedNick))
            {
                savedNick = "SCP:SL Player";
            }

            CmdSetNick(savedNick);
            if (ServerStatic.IsDedicated)
            {
                SetNick("Dedicated Server");
            }
            GameObject nicknameTextObj = GameObject.Find("Nickname Text");
            _nText = nicknameTextObj.GetComponent<Text>();
            _renderer = _nText.GetComponent<CanvasRenderer>();
        }
    }

    private bool TryGetRayTransform(out Transform tr, out ReferenceHub cameraOwner)
    {
        if (_hub.roleManager.CurrentRole is IFpcRole)
        {
            tr = _hub.PlayerCameraReference;
            cameraOwner = _hub;
            return true;
        }
        if (SpectatorTargetTracker.TryGetTrackedPlayer(out var hub) && hub.roleManager.CurrentRole is IFpcRole)
        {
            tr = MainCameraController.CurrentCamera;
            cameraOwner = hub;
            return true;
        }
        tr = null;
        cameraOwner = null;
        return false;
    }

    private void Update()
    {
        if (!base.isLocalPlayer)
        {
            return;
        }

        bool shown = false;

        // The camera owner's own hitboxes are only disabled while he is the local player
        // (CharacterModel.SpawnObject -> SetColliders(!OwnerHub.isLocalPlayer)), so while
        // spectating we must exclude the tracked player explicitly or the ray, which starts
        // at his head, reports him as the aimed-at target.
        if (TryGetRayTransform(out Transform rayTransform, out ReferenceHub cameraOwner))
        {
            Ray ray = new Ray(rayTransform.position, rayTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, ViewRange, RaycastMask) && hit.collider != null)
            {
                Transform hitTransform = hit.collider.transform;
                ReferenceHub hitHub = null;
                while (hitTransform != null && !ReferenceHub.TryGetHub(hitTransform.gameObject, out hitHub))
                {
                    hitTransform = hitTransform.parent;
                }

                if (hitHub != null && hitHub != _hub && hitHub != cameraOwner && !hitHub.isLocalPlayer
                    && !(_localFlashed != null && _localFlashed.Intensity > FlashedThreshold)
                    && hitHub.roleManager.CurrentRole is IFpcRole)
                {
                    StringBuilder sb = StringBuilderPool.Shared.Rent();
                    Color color;
                    WriteDefaultInfo(hitHub, sb, out color);

                    string info = sb.ToString();
                    StringBuilderPool.Shared.Return(sb);

                    _nText.text = info;
                    _renderer.SetColor(color);
                    _transparency = 1f;
                    _renderer.SetAlpha(1f);
                    shown = true;
                }
            }
        }

        if (!shown)
        {
            _transparency -= Time.deltaTime;
            if (_transparency < 0f)
            {
                _transparency = 0f;
                _nText.text = "";
            }
            _renderer.SetAlpha(Mathf.Clamp01(_transparency));
        }
    }

    private void SetCustomInfo(string oldValue, string newValue)
    {
        SanitizedPlayerInfoString = Misc.SanitizeRichText(newValue ?? string.Empty, "＜", "＞");
    }

    [Command(channel = 4)]
    private void CmdSetNick(string n)
    {
        if (base.isLocalPlayer)
        {
            MyNick = n;
            return;
        }

        if (NickSet)
        {
            return;
        }

        NickSet = true;
        if (n == null)
        {
            ServerConsole.AddLog("Banned " + base.connectionToClient.address + " for passing null name.", ConsoleColor.Gray);
            BanPlayer.BanUser(_hub, "Null name", 1577847600L);
            SetNick("(null)");
            return;
        }

        if (n.Length > 1024)
        {
            ServerConsole.AddLog("Banned " + base.connectionToClient.address + " for passing a too long name.", ConsoleColor.Gray);
            BanPlayer.BanUser(_hub, "Too long name", 1577847600L);
            SetNick("(too long)");
            return;
        }

        string cleaned = CleanNickName(n, out bool printable);
        if (!printable)
        {
            ServerConsole.AddLog("Kicked " + base.connectionToClient.address + " for having an empty name.", ConsoleColor.Gray);
            ServerConsole.Disconnect(base.connectionToClient, "You may not have an empty name.");
            SetNick("Empty Name");
            return;
        }

        cleaned = Misc.SanitizeRichText(cleaned, "＜", "＞");
        cleaned = cleaned.Replace("[", "(");
        cleaned = cleaned.Replace("]", ")");
        if (cleaned.Length > 48)
        {
            cleaned = cleaned.Substring(0, 48);
        }

        SetNick(cleaned);
        _hub.characterClassManager.SyncServerCmdBinding();
    }

    [ServerCallback]
    public void UpdateNickname(string n)
    {
        if (!NetworkServer.active)
        {
            return;
        }

        NickSet = true;
        if (n == null)
        {
            ServerConsole.AddLog("Banned " + base.connectionToClient.address + " for passing null name.");
            BanPlayer.BanUser(_hub, "Null name", 1577847600L);
            SetNick("(null)");
            return;
        }

        if (n.Length > 1024)
        {
            ServerConsole.AddLog("Banned " + base.connectionToClient.address + " for passing a too long name.");
            BanPlayer.BanUser(_hub, "Too long name", 1577847600L);
            SetNick("(too long)");
            return;
        }

        StringBuilder sb = StringBuilderPool.Shared.Rent(n.Length);
        char highSurrogate = '\0';
        bool hasPrintable = false;
        foreach (char c in n)
        {
            if (char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c))
            {
                hasPrintable = true;
                sb.Append(c);
            }
            else if (char.IsWhiteSpace(c) && c != '\n' && c != '\r' && c != '\t')
            {
                sb.Append(c);
            }
            else if (char.IsHighSurrogate(c))
            {
                highSurrogate = c;
            }
            else if (char.IsLowSurrogate(c) && char.IsSurrogatePair(highSurrogate, c))
            {
                sb.Append(highSurrogate);
                sb.Append(c);
                hasPrintable = true;
            }
        }

        if (!hasPrintable)
        {
            ServerConsole.AddLog("Kicked " + base.connectionToClient.address + " for having an empty name.");
            ServerConsole.Disconnect(base.connectionToClient, "You may not have an empty name.");
            SetNick("Empty Name");
            StringBuilderPool.Shared.Return(sb);
            return;
        }

        string cleaned = sb.ToString();
        StringBuilderPool.Shared.Return(sb);
        if (cleaned.Length > 48)
        {
            cleaned = cleaned.Substring(0, 48);
        }

        SetNick(cleaned);
    }

    [Server]
    private void SetNick(string nick)
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning("[Server] function 'System.Void NicknameSync::SetNick(System.String)' called when server was not active");
            return;
        }

        MyNick = nick;
        string filtered;
        try
        {
            filtered = _nickFilter?.Replace(nick, _replacement) ?? nick;
        }
        catch (Exception ex)
        {
            ServerConsole.AddLog($"Error when filtering nick {nick}: {ex}");
            filtered = "(filter failed)";
        }

        if (nick != filtered)
        {
            DisplayName = filtered;
        }

        if (!base.isLocalPlayer || !ServerStatic.IsDedicated)
        {
            ServerConsole.AddLog("Nickname of " + _hub.characterClassManager.UserId + " is now " + nick + ".");
            ServerLogs.AddLog(ServerLogs.Modules.Networking, "Nickname of " + _hub.characterClassManager.UserId + " is now " + nick + ".", ServerLogs.ServerLogType.ConnectionUpdate);
        }
    }

    public static void WriteDefaultInfo(ReferenceHub owner, StringBuilder sb, out Color texColor, PlayerInfoArea? flagsOverride = null)
    {
        PlayerRoles.PlayerRoleBase currentRole = owner.roleManager.CurrentRole;
        texColor = currentRole.RoleColor;
        PlayerInfoArea flags = flagsOverride ?? owner.nicknameSync.ShownPlayerInfo;

        if ((flags & PlayerInfoArea.Badge) != 0)
        {
            string badge = owner.serverRoles.GetColoredRoleString();
            if (!string.IsNullOrEmpty(badge))
            {
                sb.AppendLine(badge);
            }
        }

        if ((flags & PlayerInfoArea.CustomInfo) != 0 && !string.IsNullOrEmpty(owner.nicknameSync.CustomPlayerInfo))
        {
            sb.AppendLine(owner.nicknameSync.SanitizedPlayerInfoString);
        }

        if ((flags & PlayerInfoArea.Nickname) != 0)
        {
            sb.AppendLine(owner.nicknameSync.CombinedName);
        }

        if ((flags & PlayerInfoArea.Role) != 0)
        {
            sb.Append(currentRole.RoleName);
            if ((flags & PlayerInfoArea.UnitName) != 0 && currentRole is PlayerRoles.HumanRole humanRole && humanRole.UsesUnitNames)
            {
                sb.Append(" (").Append(humanRole.UnitName).Append(')');
            }
        }
    }

    private string CleanNickName(string input, out bool printable)
    {
        StringBuilder sb = StringBuilderPool.Shared.Rent(input.Length);
        char highSurrogate = '\0';
        printable = false;
        foreach (char c in input)
        {
            if (char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c))
            {
                printable = true;
                sb.Append(c);
            }
            else if (char.IsWhiteSpace(c) && c != '\n' && c != '\r' && c != '\t')
            {
                sb.Append(c);
            }
            else if (char.IsHighSurrogate(c))
            {
                highSurrogate = c;
            }
            else if (char.IsLowSurrogate(c) && char.IsSurrogatePair(highSurrogate, c))
            {
                sb.Append(highSurrogate);
                sb.Append(c);
                printable = true;
            }
        }

        return StringBuilderPool.Shared.ToStringReturn(sb);
    }
}
