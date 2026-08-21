using System;
using System.Collections.Generic;
using Discord.Basic;
using Discord.Elements;
using GameCore;
using Mirror.LiteNetLib4Mirror;
using NorthwoodLib;
using PlayerRoles;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Discord.Modules
{
    public class RichPresenceModule : DiscordModuleBase
    {
        public static readonly Dictionary<DiscordGameStatus, DiscordGameStateElement> GameState = new();
        public static readonly Dictionary<RoleTypeId, DiscordRoleElement> PlayableRoles = new();
        public static readonly Dictionary<Team, DiscordTeamElement> PlayableTeams = new();

        private static bool _dictionariesPopulated;

        [SerializeField]
        private DiscordElementBase[] _elements;

        private DiscordElementBase _elementBase;
        private long _timeSinceJoin;

        public override bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                if (!value)
                {
                    CallbackController.UpdatePresence(new RichPresence());
                }
                else
                {
                    RefreshPresence();
                }
            }
        }

        private string ServerAddress
        {
            get
            {
                var manager = DiscordManager.Singleton;
                if (manager?.NetworkManager == null)
                    return string.Empty;

                var transport = LiteNetLib4MirrorTransport.Singleton;
                if (transport == null)
                    return string.Empty;

                string address = manager.NetworkManager.networkAddress;
                if (!address.Contains("."))
                    return string.Empty;

                var parts = new object[]
                {
                    address,
                    ":",
                    transport.port,
                    ":",
                    GameCore.Version.VersionString
                };

                return StringUtils.Base64Encode(string.Concat(parts));
            }
        }

        public void RefreshPresence()
        {
            if (_elementBase == null)
                return;

            string serverAddress = ServerAddress;
            var presence = new RichPresence();

            presence.partyId = "LOBBY#" + serverAddress;
            presence.joinSecret = serverAddress;
            presence.state = _elementBase.Title;
            presence.smallImageKey = _elementBase.SmallImageId;
            presence.largeImageKey = _elementBase.LargeImageId;
            presence.startTimestamp = _timeSinceJoin;
            presence.partyMax = CustomNetworkManager.slots;
            presence.partySize = CustomNetworkManager.reconnecting ? 1 : 0;

            CallbackController.UpdatePresence(presence);
        }

        public void SetStatus(DiscordGameStatus status, bool updatePresence = true)
        {
            if (GameState.TryGetValue(status, out var element))
            {
                _elementBase = element;
                if (updatePresence)
                    RefreshPresence();
            }
        }

        public void SetStatus(RoleTypeId roleTypeId, bool refreshPresence = true)
        {
            if (PlayableRoles.TryGetValue(roleTypeId, out var element))
            {
                _elementBase = element;
                if (refreshPresence)
                    RefreshPresence();
            }
        }

        public void SetStatus(Team team, bool updatePresence = true)
        {
            if (PlayableTeams.TryGetValue(team, out var element))
            {
                _elementBase = element;
                if (updatePresence)
                    RefreshPresence();
            }
        }

        public void ResetTimer(bool updatePresence = true)
        {
            _timeSinceJoin = GenerateUnixTime();
            if (updatePresence)
                RefreshPresence();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            PlayerRoleManager.OnRoleChanged -= OnRoleChanged;

            if (ReferenceHub.OnPlayerAdded != null)
                ReferenceHub.OnPlayerAdded -= OnPlayerAddedHandler;

            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }

        private void OnPlayerAddedHandler(ReferenceHub _)
        {
            RefreshPresence();
        }

        private void Start()
        {
            PlayerRoleManager.OnRoleChanged += OnRoleChanged;

            ReferenceHub.OnPlayerAdded += OnPlayerAddedHandler;
            SceneManager.sceneLoaded += OnLevelFinishedLoading;

            _timeSinceJoin = GenerateUnixTime();
        }

        private void Awake()
        {
            if (!_dictionariesPopulated)
                PopulateDictionaries();
        }

        private void ClearPresence()
        {
            CallbackController.UpdatePresence(new RichPresence());
        }

        private long GenerateUnixTime()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        private void OnRoleChanged(ReferenceHub userHub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            if (ReferenceHub.LocalHub != userHub)
                return;

            if (newRole != null && PlayableRoles.TryGetValue(newRole.RoleTypeId, out var roleElement))
            {
                _elementBase = roleElement;
                RefreshPresence();
                return;
            }

            if (newRole != null && PlayableTeams.TryGetValue(newRole.Team, out var teamElement))
            {
                _elementBase = teamElement;
                RefreshPresence();
            }
        }

        private void OnLevelFinishedLoading(Scene scene, LoadSceneMode _)
        {
            string name = scene.name;
            if (name.Contains("Menu"))
            {
                SetStatus(DiscordGameStatus.MainMenu, true);
            }
            else if (name == "Facility")
            {
                SetStatus(DiscordGameStatus.Lobby, true);
            }
            else
            {
                SetStatus(DiscordGameStatus.None, true);
            }
        }

        private void PopulateDictionaries()
        {
            _dictionariesPopulated = true;

            if (_elements == null)
                return;

            foreach (var element in _elements)
            {
                if (element is DiscordRoleElement roleElement)
                {
                    PlayableRoles[roleElement.RoleId] = roleElement;
                }
                else if (element is DiscordGameStateElement gameStateElement)
                {
                    GameState[gameStateElement.Status] = gameStateElement;
                }
                else if (element is DiscordTeamElement teamElement)
                {
                    PlayableTeams[teamElement.TeamId] = teamElement;
                }
                else
                {
                    Debug.LogWarning("Attempted to populate DiscordManager's elements with an unknown type.");
                }
            }
        }

        static RichPresenceModule()
        {
            GameState = new Dictionary<DiscordGameStatus, DiscordGameStateElement>();
            PlayableRoles = new Dictionary<RoleTypeId, DiscordRoleElement>();
            PlayableTeams = new Dictionary<Team, DiscordTeamElement>();
        }
    }
}