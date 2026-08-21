using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static GameCore.Console;

public class CmdBinding : MonoBehaviour
{
    public class Bind
    {
        public string command;
        public KeyCode key;
    }

    public static readonly List<Bind> Bindings;

    static CmdBinding()
    {
        Bindings = new List<Bind>();
        Load();
    }

    private void Update()
    {
        if (GameCore.Console.Singleton == null)
            return;

        if (RemoteAdmin.UIController.Singleton != null)
        {
            if (RemoteAdmin.UIController.Singleton.root_panel.activeSelf ||
                RemoteAdmin.UIController.Singleton.root_login.activeSelf ||
                RemoteAdmin.UIController.Singleton.root_tbra.activeSelf)
                return;
        }

        if (PlayerList.singleton != null)
        {
            if (PlayerList.singleton.reportForm != null && PlayerList.singleton.reportForm.activeSelf)
                return;
        }

        foreach (Bind binding in Bindings)
        {
            if (Input.GetKeyDown(binding.key))
            {
                GameCore.Console.Singleton.TypeCommand(binding.command);
            }
        }
    }

    public static void KeyBind(KeyCode code, string cmd)
    {
        foreach (Bind binding in Bindings)
        {
            if (binding.key == code)
            {
                binding.command = cmd;
                Save();
                return;
            }
        }
        Bindings.Add(new Bind
        {
            command = cmd,
            key = code
        });
    }

    public static void Save()
    {
        string text = "";
        for (int i = 0; i < Bindings.Count; i++)
        {
            string text2 = text;
            int key = (int)Bindings[i].key;
            text = text2 + key + ":" + Bindings[i].command;
            if (i != Bindings.Count - 1)
            {
                text += Environment.NewLine;
            }
        }
        StreamWriter streamWriter = new StreamWriter(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            + "/SCP Secret Laboratory/cmdbinding.txt");
        streamWriter.WriteLine(text);
        streamWriter.Close();
    }

    public static void Load()
    {
        GameCore.Console.AddLog("Loading cmd bindings...", Color.grey);
        try
        {
            Bindings.Clear();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + "/SCP Secret Laboratory/cmdbinding.txt";

            if (!File.Exists(path))
            {
                Revent();
            }

            StreamReader streamReader = new StreamReader(path);
            string text;
            while ((text = streamReader.ReadLine()) != null)
            {
                if (!string.IsNullOrEmpty(text) && text.Contains(":"))
                {
                    string[] split = text.Split(':');
                    Bindings.Add(new Bind
                    {
                        command = split[1],
                        key = (KeyCode)int.Parse(split[0])
                    });
                }
            }
            streamReader.Close();
        }
        catch (Exception ex)
        {
            Debug.Log("REVENT: " + ex.StackTrace + " - " + ex.Message);
            Revent();
        }
    }

    private static void Revent()
    {
        Debug.Log("Reventing!");
        new StreamWriter(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            + "/SCP Secret Laboratory/cmdbinding.txt").Close();
    }

    public static void ChangeKeybinding(KeyCode code, string cmd)
    {
        if (cmd.StartsWith(".") || cmd.StartsWith("/"))
        {
            GameCore.Console.AddLog(
                "Server tries to sync keybinding: (" + code + "):" + cmd
                + " Type SYNCCMD and restart your game to accpet.",
                Color.grey, true, ConsoleLogType.Log);
            return;
        }

        GameCore.Console.AddLog(
            "[SYNC FROM SERVER] (" + code + "):" + cmd,
            Color.grey, true, ConsoleLogType.Log);

        KeyBind(code, cmd);
    }
}