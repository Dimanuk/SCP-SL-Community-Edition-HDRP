using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
using LiteNetLib;
using GameCore;
using MEC;
using Mirror.LiteNetLib4Mirror;

public class PlayButton : MonoBehaviour
{
    public Text Motd;
    public Text Players;
    public uint ServerID;
    public string Ip;
    public string Port;
    public string InfoType;

    public static int maxPlayers = 20;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Facility")
        {
            Destroy(gameObject);
        }
    }

    public void Click()
    {
        if (CrashDetector.singleton != null && CrashDetector.singleton.Show())
            return;

        CustomNetworkManager manager = FindFirstObjectByType<CustomNetworkManager>();
        if (NetworkClient.active)
        {
            manager.StopClient();
        }
        else
        {
            manager.networkAddress = Ip;
        }

        LiteNetLib4MirrorTransport transport = LiteNetLib4MirrorTransport.Singleton;
        try
        {
            transport.port = ushort.Parse(Port);
        }
        catch
        {
            transport.port = 7777;
            GameCore.Console.AddLog("Wrong server port, parsing to 7777!", GetDefaultColor(), true, 0);
        }

        string connectMsg = string.Concat("Connecting to ", Ip, ":", Port, "!");
        GameCore.Console.AddLog(connectMsg, GetDefaultColor(), true, 0);

        CustomLiteNetLib4MirrorTransport.SetReconnectionParameters(false);
        manager.StartClient();

        SetMaxPlayers(Players.text);
    }

    private void SetMaxPlayers(string s)
    {
        char[] separator = { '/' };
        string[] parts = s.Split(separator);
        if (parts.Length > 1)
            maxPlayers = int.Parse(parts[1]);
    }

    public void ShowInfo()
    {
        ServerInfo.ShowInfo(InfoType);
    }

    private Color GetDefaultColor()
    {
        return new Color32(182, 182, 182, 255);
    }
}