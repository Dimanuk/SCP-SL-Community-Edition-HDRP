using Mirror;
using UnityEngine;
using GameCore;

public class FastRoundRestartController : NetworkBehaviour
{
    private static bool _fastRestartInProgress;

    internal static bool FastRestartInProgress
    {
        get
        {
            return _fastRestartInProgress;
        }
        set
        {
            if (_fastRestartInProgress != value)
            {
                _fastRestartInProgress = value;

                string logMessage = string.Format("FastRestartInProgress set to: {0}", value);
                GameCore.Console.AddLog(logMessage, Color.white, false, GameCore.Console.ConsoleLogType.Log);
            }
        }
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            UnityEngine.Object.Destroy(this);
            return;
        }

        string startLog = string.Format("FastRestartInProgress set to: {0}", _fastRestartInProgress);
        GameCore.Console.AddLog(startLog, Color.white, false, GameCore.Console.ConsoleLogType.Log);
    }
}