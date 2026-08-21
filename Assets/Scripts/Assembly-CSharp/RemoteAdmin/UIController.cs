using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using MEC;
using System.Text;
using RemoteAdmin.Communication;
using RemoteAdmin.Menus;
using TMPro;
using ToggleableMenus;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin
{
    public class UIController : SimpleToggleableMenu
    {
        public static UIController Singleton;

        public GameObject root_login;
        public GameObject root_panel;
        public GameObject root_tbra;
        public Texture wrongPasswordTexture;
        public Button confirmButton;
        public InputField passwordField;
        public Canvas RACanvas;

        [SerializeField]
        private TMP_Dropdown _playerSortingDropdown;

        [NonSerialized]
        public bool LoggedIn;

        [NonSerialized]
        public byte AwaitingLogin;

        [NonSerialized]
        private bool _textBasedVersion;

        public GameObject consoleToolTip;
        public RectTransform consoleToolTipBackground;
        public TMP_Text consoleToolTipText;

        private float _clientToolTipWaitTime;
        private float _clientLastTipTime;
        private float _toolTipDelayTimer;

        [SerializeField]
        private Vector3 toolTipOffset;

        [SerializeField]
        private float toolTipPadding = 4f;

        private TMP_InputField[] _allTmpFields;
        private InputField[] _allRegularFields;

        public TMP_Dropdown PlayerSortingDropdown
        {
            get => _playerSortingDropdown;
            set => _playerSortingDropdown = value;
        }

        public override bool CanToggle => !IsAnyInputFieldFocused();

        public override bool LockMovement
        {
            get
            {
                if (IsAnyInputFieldFocused())
                    return true;

                if (!IsEnabled)
                    return false;

                var settings = RaSettings.Singleton;
                return settings != null && settings.ToggleMovement != null && settings.ToggleMovement.Value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            toolTipOffset.z = 0f;
            Singleton = this;
            _allTmpFields = GetComponentsInChildren<TMP_InputField>(true);
            _allRegularFields = GetComponentsInChildren<InputField>(true);
        }

        private void Update()
        {
            if (PlayerList.singleton != null
                && PlayerList.singleton.reportForm != null
                && PlayerList.singleton.reportForm.activeSelf)
            {
                return;
            }

            if (consoleToolTip != null && consoleToolTip.activeSelf)
            {
                Vector3 pos = Input.mousePosition + toolTipOffset;
                float scale = RACanvas != null ? RACanvas.scaleFactor : 1f;

                if (consoleToolTipBackground != null)
                {
                    float halfW = consoleToolTipBackground.rect.width * scale * 0.5f;
                    float halfH = consoleToolTipBackground.rect.height * scale * 0.5f;

                    pos.x = Mathf.Clamp(pos.x, halfW, Screen.width - halfW);
                    pos.y = Mathf.Clamp(pos.y, halfH, Screen.height - halfH);

                    // pos is a SCREEN-pixel coordinate. Assigning it straight to RectTransform.position
                    // only lands under the cursor on a Screen-Space-Overlay canvas; on a Screen-Space-Camera
                    // (or World) canvas that value is world-space and the tooltip sticks at a fixed off-screen
                    // spot ignoring the mouse. Convert through the canvas so it follows the cursor in any mode.
                    Camera uiCam = (RACanvas != null && RACanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                        ? RACanvas.worldCamera
                        : null;

                    if (RectTransformUtility.ScreenPointToWorldPointInRectangle(consoleToolTipBackground, pos, uiCam, out Vector3 world))
                    {
                        consoleToolTipBackground.position = world;
                        if (consoleToolTipText != null)
                            consoleToolTipText.transform.position = world;
                    }
                }
            }

            if (Time.time > _clientLastTipTime + _clientToolTipWaitTime + 0.5f)
            {
                _clientToolTipWaitTime = 0f;
                if (consoleToolTip != null)
                    consoleToolTip.SetActive(false);
            }
        }

        public static RaPlayerList.PlayerSorting GetSorting()
        {
            var dd = Singleton?._playerSortingDropdown;
            if (dd == null)
                return default;

            return (RaPlayerList.PlayerSorting)Mathf.Min(dd.value, dd.options.Count - 1);
        }

        private bool IsAnyInputFieldFocused()
        {
            if (_allTmpFields != null && _allTmpFields.Any(f => f != null && f.isFocused))
                return true;

            if (_allRegularFields != null && _allRegularFields.Any(f => f != null && f.isFocused))
                return true;

            return false;
        }

        public void ChangeConsoleStage()
        {
            IsEnabled = !IsEnabled;
            QueryProcessor.StaticRefreshPlayerList();
            RefreshStatus();
        }

        public void CallSendPassword()
        {
            Timing.RunCoroutine(_SendPassword(), Segment.Update);
        }

        public void ChangeTextMode(bool b)
        {
            if (_textBasedVersion && !b)
                PlayerInfoQR.Clear();

            _textBasedVersion = b;
            RefreshStatus();
        }

        public void RefreshStatus()
        {
            bool isOpen = IsEnabled;

            if (root_panel != null)
                root_panel.SetActive(isOpen && LoggedIn && !_textBasedVersion);

            if (root_tbra != null)
                root_tbra.SetActive(isOpen && LoggedIn && _textBasedVersion);

            if (root_login != null)
                root_login.SetActive(isOpen && !LoggedIn);
        }

        public void ActivateRemoteAdmin()
        {
            LoggedIn = true;
            RefreshStatus();
        }

        public void DeactivateRemoteAdmin()
        {
            LoggedIn = false;
            RefreshStatus();
        }

        protected override void OnToggled()
        {
            base.OnToggled();
            RefreshStatus();
        }

        private IEnumerator<float> _SendPassword()
        {
            var hub = ReferenceHub.LocalHub;
            var queryProc = hub?.queryProcessor;
            if (queryProc == null)
                yield break;

            if (!queryProc.OverridePasswordEnabled)
            {
                GameCore.Console.AddLog("Password authentication is disabled on this server!", Color.magenta);
                yield break;
            }

            if (AwaitingLogin == 1)
                yield break;

            confirmButton.interactable = false;

            float t = 0f;

            // Generate and remember our client salt (once).
            if (queryProc.ClientSalt == null)
            {
                byte[] clientSalt = new byte[32];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(clientSalt);
                }
                queryProc.ClientSalt = clientSalt;
            }

            // Ask the server for its salt (only if we don't have it yet).
            if (queryProc.Salt == null)
                queryProc.CmdRequestSalt(queryProc.ClientSalt);

            // Wait until the server's salt arrives (TargetSaltGenerated sets queryProc.Salt).
            while (queryProc.Salt == null)
            {
                t += Time.fixedDeltaTime;
                if (t > 5f)
                    break;
                yield return 0f;
            }

            if (queryProc.Salt == null)
            {
                GameCore.Console.AddLog("Can't obtain salt from server!", Color.magenta);
                confirmButton.interactable = true;
                yield break;
            }

            // Derive the key from server salt + client salt and the typed password.
            string sSalt = Convert.ToBase64String(queryProc.Salt);
            string cSalt = Convert.ToBase64String(queryProc.ClientSalt);
            byte[] sha = Cryptography.Sha.Sha512(sSalt + cSalt);
            queryProc.Key = Cryptography.PBKDF2.Pbkdf2HashBytes(passwordField.text, sha, 250, 512);

            byte[] hmac = queryProc.HmacSign("Login", -1);
            queryProc.CmdSendPassword(hmac);

            GameCore.Console.AddLog("Sent auth request to the server!", Color.blue);
            AwaitingLogin = 1;

            // Wait for the server's verdict (TargetReplyPassword sets AwaitingLogin = 2 on success, 0 on failure).
            t = 0f;
            while (AwaitingLogin == 1)
            {
                t += Time.fixedDeltaTime;
                if (t > 5f)
                    break;
                yield return 0f;
            }

            if (AwaitingLogin == 2)
            {
                queryProc.PasswordSent = true;
                LoggedIn = true;
                RefreshStatus();
            }
            else
            {
                var raw = passwordField.GetComponent<RawImage>();
                if (raw != null && wrongPasswordTexture != null)
                    raw.texture = wrongPasswordTexture;
            }

            confirmButton.interactable = true;
            AwaitingLogin = 0;
        }

        public void SetToolTip(string tip, float blockseconds = 0.1f, bool delayedDisplay = false)
        {
            var settings = RaSettings.Singleton;
            if (settings?.ToggleTooltip != null && !settings.ToggleTooltip.Value)
                return;

            if (Time.time < _clientLastTipTime + _clientToolTipWaitTime)
                return;

            _clientToolTipWaitTime = 0f;
            if (blockseconds > 0f)
                _clientToolTipWaitTime = blockseconds;

            _clientLastTipTime = Time.time;

            if (delayedDisplay)
                _toolTipDelayTimer = blockseconds;

            if (consoleToolTip != null)
                consoleToolTip.SetActive(true);

            if (consoleToolTipText != null)
            {
                consoleToolTipText.text = tip;

                if (consoleToolTipBackground != null)
                {
                    var sz = consoleToolTipText.GetPreferredValues(tip);
                    consoleToolTipBackground.sizeDelta = new Vector2(
                        sz.x + toolTipPadding * 2,
                        sz.y + toolTipPadding * 2);
                }
            }
        }

        private void HideToolTip()
        {
            if (Time.time < _clientLastTipTime + _clientToolTipWaitTime + 0.5f)
                return;

            _clientToolTipWaitTime = 0f;
            if (consoleToolTip != null)
                consoleToolTip.SetActive(false);
        }
    }
}