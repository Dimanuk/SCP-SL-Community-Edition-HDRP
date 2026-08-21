using System;
using System.Collections.Generic;
using MEC;
using Mirror;
using NorthwoodLib;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Console = GameCore.Console;

public class ServerListManager : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform contentParent;
    public RectTransform elementPrefab;
    public Text loadingText;

    private List<GameObject> spawnedItems = new List<GameObject>();
    private ServerListItem[] servers;
    private int currentDownloadId;

    private static string nameFilter = string.Empty;
    private static int filterTolerance;

    private void Awake()
    {
        nameFilter = string.Empty;
    }

    private void OnEnable()
    {
        Timing.RunCoroutine(DownloadList(), Segment.Update);
    }

    public void ApplyNameFilter(string filter)
    {
        if (filter == null)
            return;

        string lowered = filter.ToLower();
        if (lowered == nameFilter)
            return;

        nameFilter = lowered;
        filterTolerance = nameFilter.Length >= 5
            ? (int)(nameFilter.Length * 1.5f)
            : nameFilter.Length * 2;

        Timing.RunCoroutine(DisplayList(), Segment.Update);
    }

    public void Refresh()
    {
        Timing.RunCoroutine(DownloadList(), Segment.Update);
    }

    private GameObject CreateServerRecord()
    {
        RectTransform rt = Instantiate(elementPrefab, contentParent, worldPositionStays: true);
        rt.localScale = Vector3.one;
        rt.localPosition = Vector3.zero;

        GameObject go = rt.gameObject;
        spawnedItems.Add(go);

        contentParent.sizeDelta = Vector2.up * (spawnedItems.Count * 150f);
        return go;
    }

    private IEnumerator<float> DownloadList()
    {
        for (int attempt = 0; attempt < 2; attempt++)
        {
            SetLoadingText(TranslationReader.Get("MainMenu", 53, "NO_TRANSLATION"));
            ClearSpawnedItems();

            string url = CentralServer.StandardUrl + "lobbylist.php?format=json-signed-unix&version=2&minimal=1";
            string postData = $"token={CentralAuthManager.ApiToken}&nonce={CentralAuthManager.Nonce}";

            using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, postData))
            {
                www.SetRequestHeader("User-Agent", "SCP SL");
                www.SetRequestHeader("Game-Version", GameCore.Version.VersionString);

                yield return Timing.WaitUntilDone(www.SendWebRequest());

                SetLoadingText(TranslationReader.Get("MainMenu", 54, "NO_TRANSLATION"));

                if (!string.IsNullOrEmpty(www.error))
                {
                    bool changed = CentralServer.ChangeCentralServer(remove: true);
                    string msg = changed
                        ? $"Error: {www.error}\nChecking other servers..."
                        : $"Error: {www.error}\nChecked all servers.";
                    SetLoadingText(msg);

                    servers = null;
                    continue;
                }

                string json = www.downloadHandler.text;
                var signed = JsonSerialize.FromJson<ServerListSigned>(json);

                string signData = signed.payload + "##" + signed.timestamp;
                if (!Cryptography.ECDSA.Verify(signData, signed.signature, ServerConsole.PublicKey))
                {
                    Console.AddLog("Server list has an invalid signature!", Color.red);
                    SetLoadingText("Server list has an invalid signature!");
                    servers = null;
                    continue;
                }

                long now = TimeBehaviour.CurrentUnixTimestamp;
                long issued = signed.timestamp;

                if (Math.Abs(now - issued) > 0x3840)
                {
                    ServerConsole.AddLog(
                        "Server list response has expired! Make sure that time and timezone set on your PC is correct. We recommend synchronizing the time.",
                        ConsoleColor.Yellow);
                    SetLoadingText("Server list response has expired! Set correct time and timezone on your PC.");
                    servers = null;
                    continue;
                }

                string decoded = StringUtils.Base64Decode(signed.payload);
                var data = JsonSerialize.FromJson<ServerListData>(decoded);
                servers = data.servers;
            }

            if (servers != null)
                break;
        }

        if (servers != null)
        {
            Console.AddLog("Servers list loaded", Color.green);
            yield return Timing.WaitUntilDone(
                Timing.RunCoroutine(DisplayList(), Segment.Update));
        }
        else
        {
            Console.AddLog("Failed to load servers list", Color.yellow);
        }
    }

    private IEnumerator<float> DisplayList()
    {
        if (servers == null || servers.Length == 0)
            yield break;

        int downloadId = ++currentDownloadId;
        bool filterActive = !string.IsNullOrWhiteSpace(nameFilter);
        bool anyDisplayed = false;

        ClearSpawnedItems();

        foreach (var item in servers)
        {
            if (currentDownloadId != downloadId)
                yield break;

            if (!GameCore.Version.ListedServerCompatibilityCheck(item.version))
            {
                yield return 0f;
                continue;
            }

            string serverName = StringUtils.Base64Decode(item.info);

            if (filterActive)
            {
                string stripped = StringUtils.StripTags(serverName.ToLower());
                string match = Misc.LongestCommonSubstring(stripped, nameFilter);

                if (match == null || match.Length < filterTolerance)
                {
                    yield return 0f;
                    continue;
                }
            }

            if (!anyDisplayed)
            {
                anyDisplayed = true;
                SetLoadingText(string.Empty);
            }

            GameObject record = CreateServerRecord();
            var btn = record.GetComponent<PlayButton>();

            btn.Ip = item.ip;
            btn.ServerID = item.serverId;
            btn.Port = item.port.ToString();

            if (btn.Motd != null)
                btn.Motd.text = serverName;

            string info = Misc.ValidatePastebin(item.pastebin) ? item.pastebin : "7wV681fT";
            btn.InfoType = info;

            if (btn.Players != null)
                btn.Players.text = item.players;

            yield return 0f;
        }

        if (!anyDisplayed)
            SetLoadingText(TranslationReader.Get("MainMenu", 54, "NO_TRANSLATION"));

        currentDownloadId = 0;
    }

    private void SetLoadingText(string text)
    {
        if (loadingText != null)
            loadingText.text = text;
    }

    private void ClearSpawnedItems()
    {
        if (spawnedItems == null)
            return;

        foreach (var go in spawnedItems)
        {
            if (go != null)
                Destroy(go);
        }

        spawnedItems.Clear();
        contentParent.sizeDelta = Vector2.zero;
    }
}

[Serializable]
public class ServerListData : IJsonSerializable
{
    public ServerListItem[] servers;

    public void FromJson(string json) { }
    public string ToJson() => string.Empty;
}