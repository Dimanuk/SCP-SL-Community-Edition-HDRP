using RemoteAdmin.Communication;
using Respawning;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin.Menus
{
    public class RoundMenu : RaCommandMenu
    {
        public enum Panel
        {
            ServerEvents = 0,
            TicketSystem = 1
        }

        private const string DisabledCode = "\uFFFD"; 
        private const string EnabledCode = "\uFFFD";  
        private const float BackgroundAlpha = 0.09f;

        [SerializeField] private Color _enabledStatus;
        [SerializeField] private Color _disabledStatus;
        [SerializeField] private Slider _teamDominationBar;
        [SerializeField] private TMP_Text _mtfTokens;
        [SerializeField] private TMP_Text _chaosTokens;
        [SerializeField] private TMP_Text _respawnTimer;
        [SerializeField] private TMP_Text _nextTeam;
        [SerializeField] private TMP_Text _roundLock;
        [SerializeField] private TMP_Text _lobbyLock;
        [SerializeField] private TMP_Text _warheadLock;
        [SerializeField] private TMP_Text _friendlyFire;
        [SerializeField] private TMP_Text _spawnProtect;
        [SerializeField] private GameObject _iconParent;
        [SerializeField] private Image _teamBorder;
        [SerializeField] private Image _teamBackground;
        [SerializeField] private Image _teamImage;
        [SerializeField] private Sprite _mtfSprite;
        [SerializeField] private Sprite _ciSprite;
        [SerializeField] private Color _ciColor;
        [SerializeField] private Color _mtfColor;

        private RaServerStatus _serverChannel;
        private RaTeamStatus _teamChannel;
        private Panel _selectedPanel;
        private float _updateTimer;

        public void SetPanel(int panel)
        {
            _selectedPanel = (Panel)panel;
        }

        public void RequestData()
        {
            _updateTimer = Time.time + RaSettings.Singleton.BandwidthCooldown.Value;

            if (_selectedPanel == Panel.ServerEvents)
            {
                _serverChannel?.Request();
            }
            else if (_selectedPanel == Panel.TicketSystem)
            {
                _teamChannel?.Request();
            }
        }

        protected override void OnUpdate()
        {
            if (_updateTimer > Time.time)
                return;

            _updateTimer = Time.time + RaSettings.Singleton.BandwidthCooldown.Value;

            if (_selectedPanel == Panel.ServerEvents)
            {
                _serverChannel?.Request();
            }
            else if (_selectedPanel == Panel.TicketSystem)
            {
                _teamChannel?.Request();
            }
        }

        private void Awake()
        {
            _serverChannel = CommunicationProcessor.RequestClientChannel <RaServerStatus>();
            _teamChannel = CommunicationProcessor.RequestClientChannel <RaTeamStatus>();

            if (_serverChannel != null)
                _serverChannel.OnClientReceiveData += RefreshServerWindow;

            if (_teamChannel != null)
                _teamChannel.OnClientReceiveData += RefreshTeamWindow;
        }

        private void OnDestroy()
        {
            if (_serverChannel != null)
                _serverChannel.OnClientReceiveData -= RefreshServerWindow;

            if (_teamChannel != null)
                _teamChannel.OnClientReceiveData -= RefreshTeamWindow;
        }

        private void RefreshTeamWindow(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;

            string[] parts = data.Substring(3).Split(',');

            // parts[0] = MTF tokens
            // parts[1] = Chaos tokens  
            // parts[2] = Next team (SpawnableTeamType: 0=None, 1=MTF, 2=Chaos)
            // parts[3] = Respawn timer (seconds as double)

            if (parts.Length > 0)
                RefreshTokens(parts[0], _mtfTokens, updateDominationBar: true);

            if (parts.Length > 1)
                RefreshTokens(parts[1], _chaosTokens, updateDominationBar: false);

            if (parts.Length > 2 && byte.TryParse(parts[2], out byte teamType))
            {
                if (System.Enum.IsDefined(typeof(SpawnableTeamType), teamType))
                {
                    _iconParent.SetActive(true);

                    switch ((SpawnableTeamType)teamType)
                    {
                        case SpawnableTeamType.NineTailedFox:
                            _teamImage.sprite = _mtfSprite;
                            _teamBorder.color = _mtfColor;
                            _teamImage.color = Color.white;
                            _teamBackground.color = GenerateBackground(_mtfColor);
                            _nextTeam.text = "<color=#70A5FF>Nine-Tailed Fox</color>";
                            break;

                        case SpawnableTeamType.ChaosInsurgency:
                            _teamImage.sprite = _ciSprite;
                            _teamBorder.color = _ciColor;
                            _teamImage.color = Color.white;
                            _teamBackground.color = GenerateBackground(_ciColor);
                            _nextTeam.text = "<color=#3EB735>Chaos Insurgency</color>";
                            break;

                        default:
                            _iconParent.SetActive(false);
                            _teamImage.color = Color.white;
                            _teamBorder.color = Color.white;
                            _teamBackground.color = Color.white;
                            _nextTeam.text = "Next team hasn't been decided";
                            break;
                    }
                }
            }

            if (parts.Length > 3 && double.TryParse(parts[3], out double seconds))
            {
                TimeSpan time = TimeSpan.FromSeconds(seconds);
                _respawnTimer.text = time.ToString(@"mm\:ss");
            }
        }

        private void RefreshServerWindow(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;

            string[] parts = data.Substring(3).Split(',');

            // parts[0] = Round lock
            // parts[1] = Lobby lock
            // parts[2] = Warhead lock  
            // parts[3] = Friendly fire
            // parts[4] = Spawn protection

            if (parts.Length > 0)
                UpdateIcon(_roundLock, parts[0] == "0", changeIcon: true, inversedColors: true);

            if (parts.Length > 1)
                UpdateIcon(_lobbyLock, parts[1] == "0", changeIcon: true, inversedColors: true);

            if (parts.Length > 2)
                UpdateIcon(_warheadLock, parts[2] == "0", changeIcon: true, inversedColors: true);

            if (parts.Length > 3)
                UpdateIcon(_friendlyFire, parts[3] == "1", changeIcon: false);

            if (parts.Length > 4)
                UpdateIcon(_spawnProtect, parts[4] == "1", changeIcon: false);
        }

        private void RefreshTokens(string tokens, TMP_Text textComponent, bool updateDominationBar = true)
        {
            if (!float.TryParse(tokens, out float value))
            {
                Debug.LogError($"Couldn't parse {tokens} as a valid float number.", this);
                return;
            }

            float percent = Mathf.Round(value * 100000f) * 0.001f;
            textComponent.text = string.Format("Tokens:<color=white> {0}%", percent);

            if (updateDominationBar && _teamDominationBar != null)
                _teamDominationBar.value = value;
        }

        private void UpdateIcon(TMP_Text textComponent, bool isEnabled,
            bool changeIcon = true, bool inversedColors = false)
        {
            if (textComponent == null)
            {
                Debug.LogError("[RoundBuilder] TextComponent is null, make sure they've been setup correctly on the content gameObject.");
                return;
            }

            if (changeIcon)
                textComponent.text = isEnabled ? "🔓" : "🔒";

            textComponent.color = isEnabled
                ? (inversedColors ? _disabledStatus : _enabledStatus)
                : (inversedColors ? _enabledStatus : _disabledStatus);
        }

        private Color GenerateBackground(Color originalColor)
        {
            return new Color(originalColor.r, originalColor.g, originalColor.b, BackgroundAlpha);
        }
    }
}