using Steamworks;
using Steamworks.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class NewMainMenu : MonoBehaviour
{
    public GameObject[] submenus;

    public AudioMixer mainMixer;

    public GameObject DirectConnectPopup;
    public TMP_InputField DirectConnectTextField;

    public Text Version;
    public GameObject Tabs;

    public Text RejoinText;
    public ButtonAudio RejoinButtonAudio;
    public RawImage RejoinIcon;

    private CustomNetworkManager _mng;

    private void Update()
    {
        if (RejoinText == null || RejoinButtonAudio == null)
            return;

        string lastIp = FavoriteAndHistory.IPHistory.LastOrDefault();

        bool hasLastIp = !string.IsNullOrEmpty(lastIp);

        RejoinText.color = hasLastIp ? Color.white : Color.gray;

        if (RejoinIcon != null)
            RejoinIcon.color = hasLastIp ? Color.white : Color.gray;

        RejoinButtonAudio.enabled = hasLastIp;
    }

    private bool SetIPOrHost(string ip, bool skipValidation = false)
    {
        if (string.IsNullOrEmpty(ip))
            return false;

        if (ip.Contains(":") && ip.StartsWith("[") && ip.Contains("]:"))
        {
            int end = ip.IndexOf("]:", 1);
            if (end > 0)
            {
                string host = ip.Substring(1, end - 1);
                string portStr = ip.Substring(end + 2);

                if ((skipValidation || Misc.ValidateIpOrHostname(host, out _, true, true)) && ushort.TryParse(portStr, out ushort port))
                {
                    _mng.networkAddress = host;
                    Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorTransport.Singleton.port = port;
                    return true;
                }
            }
        }
        else if (ip.Contains(":"))
        {
            int colonIndex = ip.IndexOf(":", 4);
            if (colonIndex > 0)
            {
                string host = ip.Substring(0, colonIndex);
                string portStr = ip.Substring(colonIndex + 1);

                if ((skipValidation || Misc.ValidateIpOrHostname(host, out _, true, true)) && ushort.TryParse(portStr, out ushort port))
                {
                    _mng.networkAddress = host;
                    Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorTransport.Singleton.port = port;
                    return true;
                }
            }
        }

        if (skipValidation || Misc.ValidateIpOrHostname(ip, out _, true, true))
        {
            _mng.networkAddress = ip;
            Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorTransport.Singleton.port = 7777;
            return true;
        }

        GameCore.Console.AddLog("Invalid IP or Hostname.", Color.red);
        _mng.ShowLog(11, "", "", "Invalid IP or Hostname.", "");
        return false;
    }

    public void DirectConnectClick()
    {
        DirectConnectPopup.SetActive(!DirectConnectPopup.activeSelf);
    }

    public void ChangeMenu(int id)
    {
        for (int i = 0; i < submenus.Length; i++)
        {
            submenus[i].SetActive(i == id);
        }
    }

    public void QuitGame()
    {
        Shutdown.Quit(true);
    }

    public void Refresh()
    {
        SimpleMenu.LoadCorrectScene();
    }

    private void Start()
    {
        if (mainMixer != null)
        {
            mainMixer.SetFloat("MasterVolumeLowpassFreq", 22000f);
            mainMixer.SetFloat("MasterVolumeHighpassWet", -80f);
        }

        _mng = FindAnyObjectByType<CustomNetworkManager>();

        for (int i = 0; i < submenus.Length; i++)
        {
            submenus[i].SetActive(i == 0);
        }

        SetAudioLevelsFromPrefs();

        SensitivitySettings.RawInput = PlayerPrefsSl.Get("RawInput", false);
        SensitivitySettings.SmoothInput = PlayerPrefsSl.Get("SmoothInput", false);
        SensitivitySettings.SensMultiplier = PlayerPrefsSl.Get("Sens", 1f);
        SensitivitySettings.AdsReductionMultiplier = PlayerPrefsSl.Get("SensAds", 1f);

        Version.text = GameCore.Version.VersionString;
        Tabs.SetActive(true);

        SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;

        string lastIp = FavoriteAndHistory.IPHistory.LastOrDefault();
        CustomNetworkManager.LastIp = lastIp;
    }

    private void SetAudioLevelsFromPrefs()
    {
        if (mainMixer == null) return;

        var groups = mainMixer.FindMatchingGroups("");
        foreach (var group in groups)
        {
            string key = group.name + "Volume-new";

            if (PlayerPrefsSl.HasKey(key, PlayerPrefsSl.DataType.Float))
            {
                float vol = PlayerPrefsSl.Get(key, 0f);
                vol = Mathf.Clamp(vol, -80f, 20f);
                mainMixer.SetFloat(key, vol);
            }
        }
    }

    public void StartServer()
    {
        CustomNetworkManager.StartNondedicated(false);
    }

    public void ReJoin()
    {
        string lastIp = CustomNetworkManager.LastIp;
        if (!string.IsNullOrEmpty(lastIp))
        {
            Connect(lastIp, false);
            FavoriteAndHistory.ResetServerID();
        }
    }

    public void Connect()
    {
        string ip = DirectConnectTextField?.text ?? "";
        Connect(ip, false);
        FavoriteAndHistory.ResetServerID();
    }

    public void Connect(string ip, bool skipValidation = false)
    {
        if (CrashDetector.singleton.Show())
            return;

        if (SetIPOrHost(ip, skipValidation))
        {
            // v12 recorded the target in SetIPOrHost so a successful direct connect would
            // enable Rejoin with the right address. Do it here on the success path (SetIPOrHost
            // is only reached from Connect) to avoid v12's quirk of also storing invalid input.
            // LastIp drives ReJoin(); IPHistory drives the Rejoin button state in Update().
            CustomNetworkManager.LastIp = ip;
            FavoriteAndHistory.Modify(FavoriteAndHistory.StorageLocation.IPHistory, ip, false);

            if (SteamLobby.singleton?.Lobby != null)
                SteamLobby.singleton.LeaveLobby();

            CustomNetworkManager.ResetPreauthReconnectState();
            CustomLiteNetLib4MirrorTransport.SetReconnectionParameters(false);
            // The original called NetworkManager.LateUpdate() here — in SCP:SL's custom Mirror fork
            // that drove the client connection. This project uses a stock-style Mirror whose
            // LateUpdate() only runs UpdateScene(), so it never connects. StartClient() is the
            // equivalent trigger (identical to the working server-browser path in PlayButton.Click).
            _mng.StartClient();
        }
    }

    private void OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId serverId)
    {
        string address = $"{ip}:{port}";
        Connect(address, false);
    }
}