using System;
using UnityEngine;

namespace RoundRestarting
{
    public static class RoundRestart
    {
        private const string RoundrestartTimeKey = "LastRoundrestartTime";

        private static DateTime _lastRestartTime;

        public static bool IsRoundRestarting { get; private set; }

        public static int UptimeRounds;

        private static int LastRestartTime => PlayerPrefsSl.Get(RoundrestartTimeKey, 5000);

        public static event Action OnRestartTriggered;

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            InventorySystem.Inventory.OnServerStarted += OnServerStarted;
            InventorySystem.Inventory.OnLocalClientStarted += OnClientStarted;
        }

        private static void OnMessageReceived(RoundRestartMessage msg)
        {
            if (!Mirror.NetworkServer.active)
                return;

            if (IsRoundRestarting)
                return;

            IsRoundRestarting = true;
            GameObjectPools.PoolManager.Singleton?.ReturnAllPoolObjects();

            switch (msg.Type)
            {
                case RoundRestartType.FastRestart:
                    ServerConsole.AddLog(
                        "Server has requested to kill all coroutines by triggering a fast round restart.",
                        ConsoleColor.Cyan);
                    FastRoundRestartController.FastRestartInProgress = true;
                    MEC.Timing.KillCoroutines();
                    return;

                case RoundRestartType.RedirectRestart:
                    ServerConsole.AddLog(
                        $"Server is performing a full round restart with connection redirection to port {msg.NewPort}. Reconnection time: {msg.TimeOffset}.",
                        ConsoleColor.Cyan);
                    Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorTransport.Singleton.port = msg.NewPort;
                    break;

                case RoundRestartType.FullRestart:
                    if (msg.Reconnect)
                    {
                        ServerConsole.AddLog(
                            $"Server is performing a full round restart. Reconnection time: {msg.TimeOffset}.",
                            ConsoleColor.Cyan);
                    }
                    else
                    {
                        ServerConsole.AddLog("Server is shutting down.", ConsoleColor.Cyan);
                        return;
                    }
                    break;
            }

            CustomLiteNetLib4MirrorTransport.SetReconnectionParameters(msg.ExtendedReconnectionPeriod);
            ChangeLevel(noShutdownMessage: false);
        }

        private static void OnClientStarted()
        {
            IsRoundRestarting = false;
            Mirror.NetworkClient.RegisterHandler <RoundRestartMessage>(OnMessageReceived, true);
        }

        private static void OnServerStarted()
        {
            TimeSpan span = DateTime.Now - _lastRestartTime;
            if (span.TotalSeconds <= 20.0)
            {
                int avg = (LastRestartTime + (int)span.TotalMilliseconds) / 2;
                PlayerPrefsSl.Set(RoundrestartTimeKey, avg);
            }
        }

        public static void InitiateRoundRestart()
        {
            if (!Mirror.NetworkServer.active)
                throw new InvalidOperationException("Round restart can only be triggerred by the server!");

            GameObjectPools.PoolManager.Singleton.ReturnAllPoolObjects();

            if (IsRoundRestarting)
                return;

            IsRoundRestarting = true;
            CustomLiteNetLib4MirrorTransport.DelayConnections = true;
            CustomLiteNetLib4MirrorTransport.UserIdFastReload.Clear();
            IdleMode.PauseIdleMode = true;

            if (CustomNetworkManager.EnableFastRestart)
            {
                foreach (ReferenceHub hub in ReferenceHub.AllHubs)
                {
                    if (hub.Mode == ClientInstanceMode.DedicatedServer)
                        continue;

                    try
                    {
                        CustomLiteNetLib4MirrorTransport.UserIdFastReload.Add(hub.characterClassManager.UserId);
                    }
                    catch (Exception ex)
                    {
                        ServerConsole.AddLog(
                            "Exception occured during processing online player list for Fast Restart: " + ex.Message,
                            ConsoleColor.Yellow);
                    }
                }

                Mirror.NetworkServer.SendToAll(new RoundRestartMessage(
                    RoundRestartType.FastRestart, 0f, 0, reconnect: true, extendedReconnectionPeriod: true));

                ChangeLevel(noShutdownMessage: false);
            }
            else
            {
                if (ServerStatic.StopNextRound == ServerStatic.NextRoundAction.DoNothing)
                {
                    float offset = LastRestartTime / 1000f;
                    Mirror.NetworkServer.SendToAll(new RoundRestartMessage(
                        RoundRestartType.FullRestart, offset, 0, reconnect: true, extendedReconnectionPeriod: true));
                }

                ChangeLevel(noShutdownMessage: false);
            }
        }

        internal static void ChangeLevel(bool noShutdownMessage)
        {
            if (!Mirror.NetworkServer.active)
            {
                Mirror.NetworkManager.singleton.StopClient();
                return;
            }

            IdleMode.PauseIdleMode = true;
            bool roundLimitReached = false;

            OnRestartTriggered?.Invoke();

            try
            {
                int limit = GameCore.ConfigFile.ServerConfig.GetInt("restart_after_rounds");
                roundLimitReached = limit > 0 && UptimeRounds >= limit;
            }
            catch (Exception ex)
            {
                ServerConsole.AddLog(
                    "Failed to check the restart_after_rounds config value: " + ex.Message,
                    ConsoleColor.Red);
            }

            switch ((byte)ServerStatic.StopNextRound)
            {
                case 0: // DoNothing
                    if (!roundLimitReached)
                    {
                        GC.Collect();
                        _lastRestartTime = DateTime.Now;
                        UptimeRounds++;
                        Mirror.NetworkManager.singleton.ServerChangeScene(
                            Mirror.NetworkManager.singleton.onlineScene);
                        break;
                    }
                    goto case 1;

                case 1: // Restart
                    ServerShutdown.ShutdownState = ServerShutdown.ServerShutdownState.Complete;
                    ServerConsole.AddOutputEntry(default(ServerOutput.ExitActionRestartEntry));

                    if (!noShutdownMessage)
                    {
                        ServerConsole.AddLog(
                            roundLimitReached
                                ? "Restarting the server (rounds limit set in the server config exceeded)..."
                                : "Restarting the server (RestartNextRound command was used)...");
                    }

                    float rejoinTime = GameCore.ConfigFile.ServerConfig.GetInt("full_restart_rejoin_time", 25);
                    Mirror.NetworkServer.SendToAll(new RoundRestartMessage(
                        RoundRestartType.FullRestart, rejoinTime, 0, reconnect: true, extendedReconnectionPeriod: true));
                    Shutdown.Quit();
                    break;

                case 2: // Shutdown
                    ServerConsole.AddOutputEntry(default(ServerOutput.ExitActionShutdownEntry));

                    if (!noShutdownMessage)
                    {
                        ServerConsole.AddLog("Shutting down the server (StopNextRound command was used)...");
                    }

                    Mirror.NetworkServer.SendToAll(new RoundRestartMessage(
                        RoundRestartType.FullRestart, 0f, 0, reconnect: false, extendedReconnectionPeriod: false));
                    Shutdown.Quit();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}