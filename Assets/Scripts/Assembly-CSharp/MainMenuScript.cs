using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class MainMenuScript : MonoBehaviour
{
    public GameObject[] submenus;

    public AudioMixer mainMixer;

    public TMP_InputField DirectConnectTextField;

    private CustomNetworkManager _mng;

    public int CurMenu;

    public static bool Openinfo;

    public bool SetIPOrHost(string ip, bool skipValidation = false)
    {
        if (string.IsNullOrEmpty(ip))
            return false;

        // Handle IPv6 with brackets
        if (ip.Contains(":") && ip.StartsWith("[") && ip.Contains("]:"))
        {
            int end = ip.IndexOf("]:", 1);
            if (end > 0)
            {
                string host = ip.Substring(1, end - 1);
                string portStr = ip.Substring(end + 2);

                if (skipValidation || Misc.ValidateIpOrHostname(host, out _, true, true))
                {
                    if (ushort.TryParse(portStr, out ushort port))
                    {
                        _mng.networkAddress = host;
                        Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorTransport.Singleton.port = port;
                        return true;
                    }
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

                if (skipValidation || Misc.ValidateIpOrHostname(host, out _, true, true))
                {
                    if (ushort.TryParse(portStr, out ushort port))
                    {
                        _mng.networkAddress = host;
                        Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorTransport.Singleton.port = port;
                        return true;
                    }
                }
            }
        }

        // Regular IP/hostname
        if (skipValidation || Misc.ValidateIpOrHostname(ip, out _, true, true))
        {
            _mng.networkAddress = ip;
            Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorTransport.Singleton.port = 7777; // default
            return true;
        }

        // Invalid
        GameCore.Console.AddLog("Invalid IP or Hostname.", Color.red);
        _mng.ShowLog(11, "", "", "Invalid IP or Hostname.", "");
        return false;
    }

    public void ChangeMenu(int id)
    {
        CurMenu = id;

        for (int i = 0; i < submenus.Length; i++)
        {
            submenus[i].SetActive(i == id);
        }

        MenuAnimator.wasEverZoomed = id > 0;
    }

    public void ResetMenu()
    {
        for (int i = 0; i < submenus.Length; i++)
        {
            submenus[i].SetActive(i == CurMenu);
        }
    }

    public void QuitGame()
    {
        Shutdown.Quit(true);
    }

    private void Start()
    {
        /*
        if (mainMixer != null)
        {
            mainMixer.SetFloat("MasterVolumeLowpassFreq", 22000f);
            mainMixer.SetFloat("MasterVolumeHighpassWet", -80f);
        }
        */

        _mng = FindFirstObjectByType<CustomNetworkManager>();

        for (int i = 0; i < submenus.Length; i++)
        {
            submenus[i].SetActive(i == 0);
        }

        CurMenu = 0;

        SetAudioLevelsFromPrefs();

        SensitivitySettings.RawInput = PlayerPrefsSl.Get("RawInput", false);
        SensitivitySettings.SmoothInput = PlayerPrefsSl.Get("SmoothInput", false);
        SensitivitySettings.SensMultiplier = PlayerPrefsSl.Get("Sens", 1f);
        SensitivitySettings.AdsReductionMultiplier = PlayerPrefsSl.Get("SensAds", 1f);
    }

    private void SetAudioLevelsFromPrefs()
    {
        if (mainMixer == null)
            return;

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
        if (_mng != null)
        {
            _mng.onlineScene = "Facility";
            _mng.MaxPlayers = 20;
            _mng.createpop.SetActive(true);
        }
    }

    public void StartTutorial(string scene)
    {
        if (_mng != null)
        {
            _mng.onlineScene = scene;
            _mng.MaxPlayers = 1;
            _mng.ShowLog(15, "", "", "", "");
            _mng.StartHost();
        }
    }

    public void Connect()
    {
        string ip = DirectConnectTextField?.text ?? "";

        if (CrashDetector.singleton.Show())
            return;

        if (SetIPOrHost(ip))
        {
            CustomLiteNetLib4MirrorTransport.SetReconnectionParameters(false);
            _mng.LateUpdate();
        }
    }

    public void Connect(string ip, bool skipValidation = false)
    {
        if (CrashDetector.singleton.Show())
            return;

        if (SetIPOrHost(ip, skipValidation))
        {
            CustomLiteNetLib4MirrorTransport.SetReconnectionParameters(false);
            _mng.LateUpdate();
        }
    }
}