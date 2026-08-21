using System;
using System.Diagnostics;
using Mirror;
using ServerOutput;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IdleMode : MonoBehaviour
{
    public static uint IdleModeTime;

    public static uint IdleModePreauthTime;

    private static bool _idleModeEnabled = true;

    private static bool _pauseIdleMode = true;

    private static short _idleModeTickrate = 1;

    private static readonly Stopwatch _st = new Stopwatch();

    internal static readonly Stopwatch PreauthStopwatch = new Stopwatch();

    public static bool IdleModeActive { get; private set; }

    public static bool PauseIdleMode
    {
        get
        {
            return _pauseIdleMode;
        }
        set
        {
            if (_pauseIdleMode == value)
            {
                return;
            }
            _pauseIdleMode = value;
            if (value)
            {
                SetIdleMode(state: false);
                _st.Reset();
                PreauthStopwatch.Reset();
                if (_idleModeEnabled)
                {
                    ServerConsole.AddLog("Idle mode is now temporarily blocked.");
                }
            }
            else if (_idleModeEnabled)
            {
                ServerConsole.AddLog("Idle mode is now available.");
                _st.Restart();
                PreauthStopwatch.Restart();
            }
        }
    }

    public static bool IdleModeEnabled
    {
        get
        {
            return _idleModeEnabled;
        }
        set
        {
            _idleModeEnabled = value;
            if (!_idleModeEnabled && IdleModeActive)
            {
                SetIdleMode(state: false);
            }
        }
    }

    internal static short IdleModeTickrate
    {
        get
        {
            return _idleModeTickrate;
        }
        set
        {
            _idleModeTickrate = (short)((value < -1 || value == 0) ? 1 : value);
            if (IdleModeActive)
            {
                SetIdleMode(state: true, force: true);
            }
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        ReferenceHub.OnPlayerAdded = (Action<ReferenceHub>)Delegate.Combine(ReferenceHub.OnPlayerAdded, (Action<ReferenceHub>)delegate
        {
            SetIdleMode(state: false);
        });
        ReferenceHub.OnPlayerRemoved = (Action<ReferenceHub>)Delegate.Combine(ReferenceHub.OnPlayerRemoved, (Action<ReferenceHub>)delegate
        {
            if (ReferenceHub.AllHubs.Count <= 1)
            {
                SetIdleMode(state: true);
            }
        });
    }

    private void FixedUpdate()
    {
        if (_st.ElapsedMilliseconds >= IdleModeTime && !_pauseIdleMode && (!PreauthStopwatch.IsRunning || PreauthStopwatch.ElapsedMilliseconds >= IdleModePreauthTime))
        {
            _st.Reset();
            PreauthStopwatch.Reset();
            if (ReferenceHub.AllHubs.Count <= 1)
            {
                SetIdleMode(state: true);
            }
        }
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _st.Reset();
        if (ServerStatic.IsDedicated && !_pauseIdleMode && IdleModeEnabled && scene.name == "Facility")
        {
            _st.Start();
        }
    }

    public static void SetIdleMode(bool state)
    {
        SetIdleMode(state, force: false);
    }

    private static void SetIdleMode(bool state, bool force)
    {
        if (NetworkServer.active && (!state || !_pauseIdleMode || force) && (state != IdleModeActive || force) && (!state || IdleModeEnabled) && ServerStatic.IsDedicated)
        {
            if (state)
            {
                Application.targetFrameRate = IdleModeTickrate;
                Time.timeScale = 0.01f;
                ServerConsole.AddLog("Server has entered the idle mode.");
                ServerConsole.AddOutputEntry(default(IdleEnterEntry));
            }
            else
            {
                Application.targetFrameRate = ServerStatic.ServerTickrate;
                Time.timeScale = 1f;
                ServerConsole.AddLog("Server has exited the idle mode.");
                ServerConsole.AddOutputEntry(default(IdleExitEntry));
            }
            IdleModeActive = state;
        }
    }
}
