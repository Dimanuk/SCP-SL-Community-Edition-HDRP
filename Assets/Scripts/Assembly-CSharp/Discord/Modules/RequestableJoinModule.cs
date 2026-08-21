using Discord.Basic;
using TMPro;
using UnityEngine;
using GameCore;
using Mirror.LiteNetLib4Mirror;
using System;
using System.Text;

namespace Discord.Modules
{
    public class RequestableJoinModule : DiscordModuleBase
    {
        private static readonly int Requested;
        private const string RequestedAnimKey = "Requested";

        [SerializeField]
        private Animator _joinAnimator;

        [SerializeField]
        private TMP_Text _joinText;

        private DiscordUser _joinRequest;

        public override bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                if (!value)
                {
                    SendResponse(Reply.No);
                }
            }
        }

        public bool RequestAvailable { get; set; }

        public void SendResponse(Reply reply)
        {
            if (!RequestAvailable) return;

            string replyValue = reply.ToString(); // boxing Reply enum
            Debug.Log($"Discord: Sent {replyValue} as the response to join request.");
            
            string username = _joinRequest.username;
            string consoleMessage = $"Discord: Sent {replyValue} as the response to {username}'s join request.";
            
            Color discordColor = new Color32(114, 137, 218, 255);
            GameCore.Console.AddLog(consoleMessage, discordColor);
            
            if (_joinAnimator != null)
            {
                _joinAnimator.SetBool(Requested, false);
            }
            
              CallbackController.Respond(_joinRequest.userId, reply);
            
            RequestAvailable = false;
        }

        protected override void OnUpdateModule()
        {
            if (!IsEnabled) return;

            // KeyCode.LeftShift = 306
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (_joinAnimator != null && _joinAnimator.GetBool(Requested))
                {
                    if (Input.GetKeyDown(KeyCode.Y))
                    {
                        SendResponse(Reply.Yes); // Reply.Yes = 1
                    }
                    
                    if (Input.GetKeyDown(KeyCode.N))
                    {
                        SendResponse(Reply.No); // Reply.No = 0
                    }
                }
            }
        }

        protected override void OnDestroy()
        {
              CallbackController._onRequest -= OnReceivedRequest;
              CallbackController._onJoin -= OnRequestAccepted;
        }

        private void OnReceivedRequest(DiscordUser request)
        {
            if (!IsEnabled) return;

            string debugLog = $"Discord: Join request {request.username}#{request.discriminator}: {request.userId}";
            Debug.Log(debugLog);
            
            string consoleLog = $"Discord: Join request {request.username}#{request.discriminator}: {request.userId}";
            Color discordColor = new Color32(114, 137, 218, 255);
            GameCore.Console.AddLog(consoleLog, discordColor);
            
            string richText = $"<b><color=#7289DA>{request.username}<color=#99AAB5>#</color>{request.discriminator}</color></b> would like to join your match!";
            
            if (_joinText != null)
            {
                _joinText.text = richText;
            }
            
            if (_joinAnimator != null)
            {
                _joinAnimator.SetBool(Requested, true);
            }
            
            _joinRequest = request;
            RequestAvailable = true;
        }

        private void OnRequestAccepted(string secret)
        {
            Debug.Log($"Discord: Joining ({secret})");
            
            try
            {
                byte[] bytes = Convert.FromBase64String(secret);
                string decoded = Encoding.UTF8.GetString(bytes);
                
                string[] parts = decoded.Split(':');
                
                if (parts.Length != 3)
                {
                    throw new Exception("Invalid connection data format");
                }
                
                if (!ushort.TryParse(parts[1], out ushort port))
                {
                    throw new Exception("No specified port Exception");
                }
                
                CustomNetworkManager manager = CustomNetworkManager.TypedSingleton;
                if (manager != null)
                {
                    manager.networkAddress = parts[0];
                }
                
                LiteNetLib4MirrorTransport transport = LiteNetLib4MirrorTransport.Singleton;
                if (transport != null)
                {
                    transport.port = port;
                }
                
                string requiredVersion = GameCore.Version.VersionString;
                if (!requiredVersion.Equals(parts[2], StringComparison.Ordinal))
                {
                    string errorMsg = $"Discord: Could not join the server - version mismatch. Required version: {parts[2]}";
                    Color errorColor = new Color32(114, 137, 218, 255);
                    GameCore.Console.AddLog(errorMsg, errorColor);
                    return;
                }
                
                CustomLiteNetLib4MirrorTransport.SetReconnectionParameters(false);
                manager.ShowLoadingScreen(0);
                manager.StartClient();
                
            }
            catch (Exception ex)
            {
                string errorMsg = $"Discord: Could not join the server - invalid connection data - {ex.Message}";
                Color errorColor = new Color32(114, 137, 218, 255);
                GameCore.Console.AddLog(errorMsg, errorColor);
                
                Debug.LogException(ex);
            }
        }

        private void Start()
        {
            CallbackController._onRequest += OnReceivedRequest;
              CallbackController._onJoin += OnRequestAccepted;
        }

        private void Update()
        {
            UpdateModule();
        }

        static RequestableJoinModule()
        {
            Requested = Animator.StringToHash("Requested");
        }
    }
}
