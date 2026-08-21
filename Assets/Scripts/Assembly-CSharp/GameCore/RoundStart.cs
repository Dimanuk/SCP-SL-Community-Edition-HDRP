using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameCore
{
    public class RoundStart : NetworkBehaviour
    {
        [Header("UI Elements")]
        public GameObject window;
        public GameObject forceButton;
        public TextMeshProUGUI playersNumber;
        public TextMeshProUGUI startsIn;
        public Image loadingbar;

        [Header("Configuration")]
        [SerializeField]
        private Image _background;

        public ushort players = ushort.MaxValue;

        [SyncVar]
        public short Timer = -2;

        private string _roundStartText;
        private string _roundStartTextPaused;
        private bool _loaded;
        private bool _hideBackground;

        public static RoundStart singleton;
        public static bool LobbyLock;
        private static bool _singletonSet;

        internal static readonly Stopwatch RoundStartTimer = new();

        static RoundStart()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public static bool RoundStarted
        {
            get
            {
                if (!_singletonSet)
                    return false;

                return singleton.Timer == -1;
            }
        }

        public static TimeSpan RoundLength => RoundStartTimer.Elapsed;

        private void Awake()
        {
            singleton = this;
            _singletonSet = true;

            PlayerRoleManager.OnRoleChanged += OnRoleChanged;
        }

        private void OnDestroy()
        {
            PlayerRoleManager.OnRoleChanged -= OnRoleChanged;
            _singletonSet = false;
        }

        private void Start()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.localPosition = Vector3.zero;
            }
        }

        private void Update()
        {
            if (ServerStatic.IsDedicated)
                return;

            if (window == null)
                return;

            bool isActive = Timer != -1;
            window.SetActive(isActive);

            if (!window.activeSelf)
                return;

            if (!_loaded)
            {
                _loaded = true;
                _roundStartText = TranslationReader.Get("Facility", 38, "ROUND STARTS IN: <color=yellow>{0}</color>s");
                _roundStartTextPaused = TranslationReader.Get("Facility", 39, "ROUND START IS <color=#FF5050>PAUSED</color>");
            }

            if (_background != null)
            {
                _background.color = _hideBackground ? Color.clear : Color.black;
            }

            // The bar must only ever be driven by the smooth Lerp below — snapping it
            // to Timer/20 each frame makes it jump in one-second steps.
            if (Timer < 0)
            {
                if (loadingbar != null)
                    loadingbar.fillAmount = 1f;

                if (startsIn != null)
                    startsIn.SetText(_roundStartTextPaused);
            }
            else
            {
                if (loadingbar != null)
                {
                    loadingbar.fillAmount = Mathf.Lerp(loadingbar.fillAmount, Timer / 20f, Time.deltaTime);
                }

                if (startsIn != null)
                {
                    string text = _roundStartText.Replace("{0}", Timer.ToString());
                    startsIn.SetText(text);
                }
            }

            int currentPlayerCount = Utils.NonAllocLINQ.HashsetExtensions.Count<ReferenceHub>(
                ReferenceHub.AllHubs,
                x => x.Mode == ClientInstanceMode.ReadyClient || x.Mode == ClientInstanceMode.Host
            );

            if (players != currentPlayerCount)
            {
                players = (ushort)currentPlayerCount;
                if (playersNumber != null)
                    playersNumber.SetText(players.ToString());
            }
        }

        private void OnRoleChanged(ReferenceHub userHub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            if (userHub != null && userHub.isLocalPlayer)
            {
                _hideBackground = newRole is FpcStandardRoleBase || newRole is PlayerRoles.PlayableScps.ISpawnableScp;
            }
        }

        public void ShowButton()
        {
            if (forceButton != null)
                forceButton.SetActive(true);
        }

        public void UseButton()
        {
            if (forceButton != null)
                forceButton.SetActive(false);

            CharacterClassManager.ForceRoundStart();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RoundStartTimer.Reset();
        }
    }
}
