using GameCore;
using MEC;
using Mirror;
using RemoteAdmin;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using ToggleableMenus;
using UnityEngine;
using UnityEngine.UI;
using Utils.ConfigHandler;
using Console = GameCore.Console;

public class PlayerList : SimpleToggleableMenu
{
    [Serializable]
    public class Instance
    {
        public ReferenceHub owner;
        public PlayerListElement listElementReference;
    }

    private static readonly ConfigEntry<float> _refreshRate = new ConfigEntry<float>(
        "player_list_title_rate", 5f, "Player List Title Refresh Rate",
        "The amount of time (in seconds) between refreshing the title of the player list");

    public static readonly ConfigEntry<string> Title = new ConfigEntry<string>(
        "player_list_title", null, "Player List Title",
        "The title at the top of the player list menu.");

    public Transform parent;
    public Transform template;
    public GameObject mainPanel;
    public GameObject reportForm;
    public GameObject reportPopup;
    public TextMeshProUGUI badgeText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI serverNameText;
    public TextMeshProUGUI reportPopupText;

    private static bool _eventsAssigned;
    private static readonly HashSet<ReferenceHub> _alreadyAddedPlayers = new HashSet<ReferenceHub>();
    public static InterfaceColorAdjuster ica;
    public static PlayerList singleton;

    private int _timer;

    private static Transform s_parent;
    private static Transform s_template;
    private static bool _anyAdminOnServer;

    public static readonly List<Instance> instances = new List<Instance>();

    private static string ServerName
    {
        get
        {
            ServerConfigSynchronizer sync = ServerConfigSynchronizer.Singleton;
            return sync == null ? null : sync.ServerName;
        }
        set
        {
            ServerConfigSynchronizer sync = ServerConfigSynchronizer.Singleton;
            if (sync != null)
                sync.ServerName = value;
        }
    }

    public override bool CanToggle
    {
        get
        {
            if (!LockMovement)
                return true;

            InputField[] fields = GetComponentsInChildren<InputField>();
            Func<InputField, bool> predicate = _canTogglePredicate;
            if (predicate == null)
            {
                predicate = (x) => x.isFocused;
                _canTogglePredicate = predicate;
            }
            return !fields.Any(predicate);
        }
    }
    private static Func<InputField, bool> _canTogglePredicate;

    public override bool LockMovement
    {
        get
        {
            bool baseLock = base.LockMovement;
            if (!baseLock)
                return false;
            return reportForm != null && reportForm.activeSelf;
        }
    }

    private void Update()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.localPosition = Vector3.zero;
        rect.sizeDelta = Vector2.zero;

        badgeText.enabled = _anyAdminOnServer;

        ServerConfigSynchronizer sync = ServerConfigSynchronizer.Singleton;
        string serverName = sync != null ? sync.ServerName : null;
        if (!string.IsNullOrEmpty(serverName) && serverNameText.text != serverName)
        {
            serverNameText.text = serverName;
        }
    }

    private void Start()
    {
        _anyAdminOnServer = false;

        timerText.text = string.Empty;

        if (NetworkServer.active)
        {
            ConfigFile.ServerConfig.UpdateConfigValue(_refreshRate);
            ConfigFile.ServerConfig.UpdateConfigValue(Title);
            Timing.RunCoroutine(_RefreshTitleLoop(), Segment.FixedUpdate);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        instances.Clear();
        _alreadyAddedPlayers.Clear();
        singleton = this;
        s_parent = parent;
        s_template = template;

        if (!_eventsAssigned)
        {
            ReferenceHub.OnPlayerRemoved += DestroyPlayer;
            CharacterClassManager.OnInstanceModeChanged += AddPlayer;
            _eventsAssigned = true;
        }

        // Catch-up: hubs whose InstanceMode was assigned before this menu awoke
        // would otherwise never get a row (the event does not re-fire).
        foreach (ReferenceHub hub in ReferenceHub.AllHubs)
            AddPlayer(hub, hub.Mode);
    }

    protected override void OnToggled()
    {
        base.OnToggled();
        mainPanel.SetActive(true);
        reportForm.SetActive(false);
        reportPopup.SetActive(false);
    }

    internal static bool DisplayProfileButton(CharacterClassManager ccm)
    {
        if (SteamUtils.IsOverlayEnabled)
        {
            string userId = ccm.UserId;
            if (!string.IsNullOrEmpty(userId) && userId.Contains("@steam"))
                return true;

            string realUserId = ccm.RealUserId;
            if (!string.IsNullOrEmpty(realUserId) && realUserId.Contains("@steam"))
                return true;
        }
        return false;
    }

    internal static void AddPlayer(ReferenceHub instance, ClientInstanceMode mode)
    {
        if (mode == ClientInstanceMode.DedicatedServer || mode == ClientInstanceMode.Unverified)
            return;

        // Null checks must happen BEFORE the HashSet gate: adding a hub that then
        // fails a null check would permanently blacklist it from the list.
        if (instance == null || instance.nicknameSync == null)
            return;

        if (!_alreadyAddedPlayers.Add(instance))
            return;

        string displayName = instance.nicknameSync.DisplayName;
        Console.AddDebugLog("PLIST", "[PlayerList] AddPlayer: " + displayName, MessageImportance.LessImportant);

        GameObject templateGO = s_template != null ? s_template.gameObject : null;
        if (templateGO == null || s_parent == null)
        {
            _alreadyAddedPlayers.Remove(instance);
            return;
        }

        GameObject newGO = Instantiate(templateGO, s_parent);
        newGO.transform.localScale = Vector3.one;
        // The template lives in the scene as a disabled row (so it doesn't render before
        // being cloned) — Instantiate copies that disabled state onto the clone, so every
        // spawned row must be explicitly re-enabled.
        newGO.SetActive(true);

        PlayerListElement element = newGO.GetComponent<PlayerListElement>();
        if (element == null)
        {
            Destroy(newGO);
            _alreadyAddedPlayers.Remove(instance);
            return;
        }
        if (element.TextNick != null)
            element.TextNick.text = displayName;
        element.instance = instance;

        CharacterClassManager ccm = instance.characterClassManager;
        if (ccm != null && element.OpenProfile != null)
            element.OpenProfile.SetActive(DisplayProfileButton(ccm));

        Instance newInstance = new Instance
        {
            owner = instance,
            listElementReference = element
        };
        instances.Add(newInstance);

        UpdatePlayerRole(instance);
    }

    internal static void RefreshPlayerId(GameObject instance)
    {
        ReferenceHub hub = ReferenceHub.GetHub(instance);
        if (hub == null)
            return;

        foreach (Instance inst in instances)
        {
            if (inst.owner == hub)
            {
                PlayerListElement element = inst.listElementReference;
                if (element != null && element.OpenProfile != null)
                {
                    CharacterClassManager ccm = hub.characterClassManager;
                    if (ccm != null)
                        element.OpenProfile.SetActive(DisplayProfileButton(ccm));
                }
                break;
            }
        }
    }

    public static void UpdatePlayerNickname(ReferenceHub instance)
    {
        foreach (Instance inst in instances)
        {
            if (inst.owner == null || inst.owner != instance)
                continue;

            ReferenceHub hub = ReferenceHub.GetHub(inst.owner);
            if (inst.listElementReference != null && inst.listElementReference.TextNick != null && hub != null)
            {
                inst.listElementReference.TextNick.text = hub.nicknameSync.DisplayName;
            }
            else
            {
                Debug.LogWarning("UpdatePlayerNickname: PlayerList Instance either has a null list element or is updating for an unknown player.");
            }
            break;
        }
    }

    public static void UpdatePlayerRole(ReferenceHub instance)
    {
        _anyAdminOnServer = false;
        bool flagNullInstance = instance == null;

        foreach (Instance inst in instances)
        {
            try
            {
                if (inst == null || inst.owner == null)
                    continue;

                // The admin badge flag is recomputed from EVERY listed player's role,
                // not from the method argument (which may be null = "refresh all").
                if (!_anyAdminOnServer)
                {
                    ServerRoles ownerRoles = inst.owner.serverRoles;
                    if (ownerRoles != null && !string.IsNullOrEmpty(ownerRoles.GetUncoloredRoleString()))
                        _anyAdminOnServer = true;
                }

                if (flagNullInstance || inst.owner != instance)
                    continue;

                ServerRoles serverRoles = instance.serverRoles;
                PlayerListElement element = inst.listElementReference;
                if (element == null || serverRoles == null || element.TextNick == null)
                    continue;

                Color roleColor = serverRoles.GetColor();
                element.TextNick.color = roleColor;
                element.TextNick.text = instance.nicknameSync.DisplayName;

                string roleString2 = serverRoles.GetUncoloredRoleString();
                element.TextBadge.text = roleString2;
                element.TextBadge.color = roleColor;

                // "[...]"-wrapped role strings are global badges: they shift the badge text
                // and show the "verified" checkmark tinted with the badge color.
                bool isGlobalBadge = !string.IsNullOrEmpty(roleString2)
                    && roleString2.StartsWith("[") && roleString2.EndsWith("]");

                RectTransform roleRect = element.TextBadge.rectTransform;
                Vector2 rolePos = roleRect.anchoredPosition;
                rolePos.x = isGlobalBadge ? 412f : 370f;
                roleRect.anchoredPosition = rolePos;

                element.ImgVerified.color = isGlobalBadge ? roleColor : Color.clear;
                element.ImgVerified.gameObject.SetActive(isGlobalBadge);

                // ImgBackground is the Image on the ROW ROOT — only its color may be
                // touched here. Calling SetActive on its gameObject hides the entire row.
                ApplyInterfaceColor(element);
            }
            catch (Exception ex)
            {
                Console.AddLog("Exception caught (UpdatePlayerRole in PlayerList): " + ex.Message, Color.red);
                Debug.LogError("Exception caught (UpdatePlayerRole in PlayerList): " + ex.Message);
            }
        }
    }

    public static void UpdateColors()
    {
        foreach (Instance inst in instances)
        {
            if (inst != null)
                ApplyInterfaceColor(inst.listElementReference);
        }
    }

    private static void ApplyInterfaceColor(PlayerListElement element)
    {
        if (element == null || element.ImgBackground == null)
            return;
        if (ica == null || ica.graphicsToChange == null || ica.graphicsToChange.Length == 0)
            return;

        Graphic source = ica.graphicsToChange[0];
        if (source == null)
            return;

        Color color = source.color;
        color.a = 1f;
        element.ImgBackground.color = color;
    }

    public static void DestroyPlayer(ReferenceHub instance)
    {
        _alreadyAddedPlayers.Remove(instance);

        for (int i = instances.Count - 1; i >= 0; i--)
        {
            Instance inst = instances[i];
            if (inst.owner == instance)
            {
                if (inst.listElementReference != null)
                {
                    GameObject go = inst.listElementReference.gameObject;
                    if (go != null)
                        Destroy(go);
                }
                instances.RemoveAt(i);
                break;
            }
        }
    }

    public void Report(bool toGM = false)
    {
        if (reportForm == null)
            return;

        TMP_Text[] texts = reportForm.GetComponentsInChildren<TMP_Text>();
        string playerId = null;
        foreach (TMP_Text t in texts)
        {
            if (t.name != "Player ID" && int.TryParse(t.text, out int _))
            {
                playerId = t.text;
                break;
            }
        }

        if (string.IsNullOrEmpty(playerId))
            return;

        string command = $"report {playerId} {(toGM ? "1" : "0")} ";
        InputField reasonField = reportForm.GetComponentInChildren<InputField>();
        if (reasonField != null)
            command += reasonField.text;

        ReferenceHub localHub = ReferenceHub.GetHub(gameObject);
        PlayerCommandSender sender = new PlayerCommandSender(localHub);
        Console.Singleton.TypeCommand(command, sender);
        string message = TranslationReader.Get("ReportForm", 6, "Please wait...");

        ShowReportResponse(message);
    }

    public void CloseForm()
    {
        base.OnToggled();
    }

    public void ShowReportResponse(string response)
    {
        reportPopup.SetActive(true);
        reportPopupText.SetText(response);
    }

    public void RefreshTitleSafe()
    {
        string titleValue = Title.Value;
        if (string.IsNullOrEmpty(titleValue))
        {
            ServerName = ServerConsole.singleton.RefreshServerNameSafe();
        }
        else
        {
            if (!ServerConsole.singleton.NameFormatter.TryProcessExpression(titleValue, "player list title", out string result))
            {
                ServerConsole.AddLog(result);
            }
            else
            {
                ServerName = result;
            }
        }
    }

    public void RefreshTitle()
    {
        string titleValue = Title.Value;
        if (string.IsNullOrEmpty(titleValue))
        {
            ServerName = ServerConsole.singleton.RefreshServerName();
        }
        else
        {
            ServerName = ServerConsole.singleton.NameFormatter.ProcessExpression(titleValue);
        }
    }

    private IEnumerator<float> _RefreshTitleLoop()
    {
        while (this != null)
        {
            RefreshTitleSafe();
            for (ushort i = 0; i < 50 * _refreshRate.Value; i++)
            {
                yield return 0f;
            }
        }
    }
}
