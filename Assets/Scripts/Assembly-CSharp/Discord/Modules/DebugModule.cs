using Discord.Basic;
using UnityEngine;
using GameCore;

namespace Discord.Modules
{
    public class DebugModule : DiscordModuleBase
    {
        // Discord brand color: #7289DA
        private static readonly Color32 DiscordColor = new Color32(114, 137, 218, 255);

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (!IsEnabled) return;
            
            CallbackController._onError -= OnErrorCallback;
            CallbackController._onReady -= OnReadyCallback;
            CallbackController._onSpectate -= OnSpectateCallback;
             CallbackController._onDisconnected -= OnDisconnectedCallback;
        }

        private void Start()
        {
            if (!IsEnabled) return;
            
            CallbackController._onError += OnErrorCallback;
            CallbackController._onReady += OnReadyCallback;
            CallbackController._onSpectate  += OnSpectateCallback;
            CallbackController._onDisconnected += OnDisconnectedCallback;
        }

        private void OnReadyCallback(DiscordUser connectedUse)
        {
            if (this == null) return;
            
            string message = string.Format("Discord: Connected to {0}#{1}: {2}",
                connectedUse.username,
                connectedUse.discriminator,
                connectedUse.userId);
            
            Debug.Log(message);
            Console.AddLog("Discord: Ready!", DiscordColor);
        }

        private void OnDisconnectedCallback(int errorCode, string message)
        {
            if (this == null) return;
            
            string log = string.Format("Discord: Disconnect {0}: {1}", errorCode, message);
            Debug.Log(log);
        }

        private void OnErrorCallback(int errorCode, string message)
        {
            if (this == null) return;
            
            string log = string.Format("Discord: Error {0}: {1}", errorCode, message);
            Debug.Log(log);
            
            string consoleLog = string.Format("Discord: Error - {0} ({1})", log, message);
            Console.AddLog(consoleLog, DiscordColor);
        }

        private void OnSpectateCallback(string secret)
        {
            if (this == null) return;
            
            string log = string.Concat("Discord: Spectate (", secret, ")");
            Debug.Log(log);
            Console.AddLog("Discord: SpectateCallback fired.", DiscordColor);
        }
    }
}
