using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Broadcast : NetworkBehaviour
{
    [Flags]
    public enum BroadcastFlags : byte
    {
        Normal = 0,
        Truncated = 1,
        AdminChat = 2
    }

    private static Broadcast _broadcast;
    private static bool _broadcastSet;

    public static readonly Queue<BroadcastMessage> Messages = new Queue<BroadcastMessage>();

    public static Broadcast Singleton
    {
        get
        {
            if (!_broadcastSet)
            {
                if (ReferenceHub.LocalHub != null)
                {
                    _broadcast = ReferenceHub.LocalHub.GetComponent<Broadcast>();
                    _broadcastSet = (_broadcast != null);
                }
            }
            return _broadcast;
        }
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            Messages.Clear();
        }
    }

    private void OnDestroy()
    {
        if (this == _broadcast)
        {
            _broadcastSet = false;
            _broadcast = null;
        }
    }

    [TargetRpc]
    public void TargetAddElement(NetworkConnection conn, string data, ushort time, BroadcastFlags flags)
    {
        AddElement(data, time, flags);
    }

    [ClientRpc]
    public void RpcAddElement(string data, ushort time, BroadcastFlags flags)
    {
        AddElement(data, time, flags);
    }

    [TargetRpc]
    public void TargetClearElements(NetworkConnection conn)
    {
        Messages.Clear();
    }

    [ClientRpc]
    public void RpcClearElements()
    {
        Messages.Clear();
    }

    public static void AddElement(string data, ushort time, BroadcastFlags flags)
    {
        bool truncated = (flags & BroadcastFlags.Truncated) != 0;
        bool adminChat = (flags & BroadcastFlags.AdminChat) != 0;

        if (time < 1) return;

        if (Messages.Count > 25) return;

        if (string.IsNullOrEmpty(data)) return;

        if (data.Length > 3072) return;

        if (time > 300)
        {
            time = 10;
        }

        string processedText = data.Replace("\n", Environment.NewLine);

        if (adminChat)
        {
            processedText = "<color=#FFA500><b>[Admin Chat]</b></color> <color=green>" + processedText + "</color>";
        }

        Messages.Enqueue(new BroadcastMessage(processedText, time, truncated));

        BroadcastAssigner.Displaying = true;

        if (GameCore.Console.Singleton != null)
        {
            string prefix = adminChat
                ? "<color=#FFA500><b>[Admin Chat]</b></color> "
                : "[BROADCAST FROM SERVER] ";

            string fullLog = string.Concat(
                prefix,
                data.Replace("<", "[").Replace(">", "]"),
                ", time: ",
                time.ToString(),
                ", monospace: ",
                truncated ? "YES" : "NO");

            GameCore.Console.AddLog(fullLog, adminChat ? Color.green : Color.white);
        }
    }
}