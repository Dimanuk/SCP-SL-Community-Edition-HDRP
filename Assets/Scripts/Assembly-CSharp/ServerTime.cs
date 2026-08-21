using GameCore;
using Mirror;
using System.Runtime.InteropServices;
using UnityEngine;

public class ServerTime : NetworkBehaviour
{
    [SyncVar]
    public int timeFromStartup;

    public static int time;

    private const int AllowedDeviation = 2;

    private bool _rateLimit;

    public static bool CheckSynchronization(int myTime)
    {
        int num = Mathf.Abs(myTime - time);
        if (num > 2)
        {
            Console.AddLog("Damage sync error.", new Color32(byte.MaxValue, 200, 0, byte.MaxValue));
        }
        return num <= 2;
    }

    private void Update()
    {
        _rateLimit = false;
        if (timeFromStartup != 0)
        {
            time = timeFromStartup;
        }
    }

    private void Start()
    {
        if (base.isLocalPlayer && NetworkServer.active)
        {
            InvokeRepeating(nameof(IncreaseTime), 1f, 1f);
        }
    }

    private void IncreaseTime()
    {
        timeFromStartup = timeFromStartup + 1;
    }
}
