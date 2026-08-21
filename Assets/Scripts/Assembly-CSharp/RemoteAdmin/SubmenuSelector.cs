using RemoteAdmin.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin
{
    public class SubmenuSelector : MonoBehaviour
    {
        [Serializable]
        public class SubMenu
        {
            public string Title;
            public TMP_Text ButtonText;
            public GameObject Panel;
            public TMP_Text RaResponseBox;

            public void Reset()
            {
                if (RaResponseBox != null)
                    RaResponseBox.text = string.Empty;

                if (ButtonText != null)
                    ButtonText.color = SubmenuSelector.Singleton.c_deselected;

                if (Panel != null)
                    Panel.SetActive(false);
            }

            public void Select()
            {
                if (Panel != null)
                    Panel.SetActive(true);

                if (ButtonText != null)
                    ButtonText.color = SubmenuSelector.Singleton.c_selected;
            }

            public void SetResponse(bool isSuccess, string content)
            {
                Color color = isSuccess ? Color.green : Color.red;
                if (RaResponseBox != null)
                {
                    RaResponseBox.color = color;
                    RaResponseBox.text = content;
                }
            }

            public void SetResponse(Color color, string content)
            {
                if (RaResponseBox != null)
                {
                    RaResponseBox.color = color;
                    RaResponseBox.text = content;
                }
            }
        }

        public Color c_selected;
        public Color c_deselected;
        public SubMenu[] menus;
        public TMP_Text WindowTitle;
        public ColorBlock ToggleOn;
        public ColorBlock ToggleOff;

        private SubMenu _currentMenu;
        private int _lastMenuIndex;
        private int _currentMenuIndex;

        public static SubmenuSelector Singleton { get; [CompilerGenerated] private set; }

        public SubMenu SelectedMenu
        {
            get => _currentMenu;
            private set
            {
                _lastMenuIndex = _currentMenuIndex;
                _currentMenuIndex = Array.IndexOf(menus, value);
                _currentMenu = value;
            }
        }

        private void Awake()
        {
            Singleton = this;
        }

        private void Start()
        {
            if (menus == null)
                return;

            foreach (var menu in menus)
                menu.Reset();

            if (menus.Length > 0)
                SelectMenu(menus[0]);
        }

        private void Update()
        {
            if (menus == null || menus.Length <= 1)
                return;

            SubMenu infoMenu = menus[1];
            if (infoMenu == null || infoMenu.Panel == null || !infoMenu.Panel.activeSelf)
                return;

            if (infoMenu.RaResponseBox == null)
                return;

            int linkIndex = TMP_TextUtilities.FindIntersectingLink(
                infoMenu.RaResponseBox,
                Input.mousePosition,
                null);

            if (linkIndex == -1)
                return;

            TMP_LinkInfo linkInfo = infoMenu.RaResponseBox.textInfo.linkInfo[linkIndex];
            string linkId = linkInfo.GetLinkID();

            switch (linkId)
            {
                case "CP_BOLD":
                    UIController.Singleton.SetToolTip("Changes the font to Bold.", 0.1f, false);
                    break;

                case "CP_USERID":
                    UIController.Singleton.SetToolTip("Copy to clipboard.", 0.1f, false);
                    if (Input.GetMouseButtonDown(0))
                        GUIUtility.systemCopyBuffer = RaClipboard.UserIds;
                    break;

                case "CP_IP":
                    UIController.Singleton.SetToolTip("Copy to clipboard.", 0.1f, false);
                    if (Input.GetMouseButtonDown(0))
                        GUIUtility.systemCopyBuffer = RaClipboard.PlayerIps;
                    break;

                case "CP_ID":
                    UIController.Singleton.SetToolTip("Copy to clipboard.", 0.1f, false);
                    if (Input.GetMouseButtonDown(0))
                        GUIUtility.systemCopyBuffer = RaClipboard.PlayerIds;
                    break;
            }
        }

        public void PlayerInfoQuery(string operation)
        {
            string text = string.Empty;

            foreach (var record in PlayerRecord.Instances)
            {
                if (!record.IsSelected)
                    continue;

                if (operation != "externallookup")
                    text += record.PlayerId + ".";
                else
                    text += record.PlayerId;
            }

            if (string.IsNullOrEmpty(text) && operation != "externallookup")
                return;

            var queryProcessor = ReferenceHub.LocalHub.queryProcessor;
            if (queryProcessor == null)
                return;

            if (operation == "short-info")
            {
                RaClipboard.Reset();
                RaPlayer.Request(true, text);
                return;
            }

            if (operation == "info")
            {
                RaClipboard.Reset();
                RaPlayer.Request(false, text);
            }
            else if (operation == "auth")
            {
                RaPlayerAuth.Request(text);
            }
            else if (operation == "externallookup")
            {
                string mode = ServerConfigSynchronizer.Singleton.RemoteAdminExternalPlayerLookupMode;
                if (mode == "fullauth" || mode == "urlonly")
                {
                    queryProcessor.ExpectingURL = DateTime.Now.AddSeconds(10.0);
                    queryProcessor.CmdSendQuery("EXTERNALLOOKUP " + text, false);
                }
            }
        }

        public void AdminToolsConfirm(string operation)
        {
            string playerId = string.Empty;

            if (operation == "GoTo")
            {
                var selected = PlayerRecord.Instances.FirstOrDefault(pl => pl.IsSelected);
                if (selected == null)
                    return;

                playerId = selected.PlayerId;
            }
            else
            {
                foreach (var pl in PlayerRecord.Instances)
                {
                    if (pl.IsSelected)
                        playerId += pl.PlayerId + ".";
                }
            }

            var queryProcessor = ReferenceHub.LocalHub.queryProcessor;
            if (queryProcessor == null)
                return;

            switch (operation)
            {
                case "BypassDisable":
                    queryProcessor.CmdSendQuery("bypass " + playerId + " 0", false);
                    break;

                case "Disarm":
                    queryProcessor.CmdSendQuery("disarm " + playerId, false);
                    break;

                case "OverwatchDisable":
                    queryProcessor.CmdSendQuery("overwatch " + playerId + " 0", false);
                    break;

                case "Unmute":
                    queryProcessor.CmdSendQuery("unmute " + playerId, false);
                    break;

                case "Bring":
                    queryProcessor.CmdSendQuery("bring " + playerId, false);
                    break;

                case "Heal":
                    queryProcessor.CmdSendQuery("heal " + playerId, false);
                    break;

                case "Imute":
                    queryProcessor.CmdSendQuery("imute " + playerId, false);
                    break;

                case "GoTo":
                    queryProcessor.CmdSendQuery("goto " + playerId, false);
                    break;

                case "GodDisable":
                    queryProcessor.CmdSendQuery("god " + playerId + " 0", false);
                    break;

                case "OverwatchEnable":
                    queryProcessor.CmdSendQuery("overwatch " + playerId + " 1", false);
                    break;

                case "Lockdown":
                    queryProcessor.CmdSendQuery("lockdown", false);
                    break;

                case "Release":
                    queryProcessor.CmdSendQuery("release " + playerId, false);
                    break;

                case "Mute":
                    queryProcessor.CmdSendQuery("mute " + playerId, false);
                    break;

                case "BypassEnable":
                    queryProcessor.CmdSendQuery("bypass " + playerId + " 1", false);
                    break;

                case "NoclipEnable":
                    queryProcessor.CmdSendQuery("noclip " + playerId + " enable", false);
                    break;

                case "GodEnable":
                    queryProcessor.CmdSendQuery("god " + playerId + " 1", false);
                    break;

                case "NoclipDisable":
                    queryProcessor.CmdSendQuery("noclip " + playerId + " disable", false);
                    break;

                case "Iunmute":
                    queryProcessor.CmdSendQuery("iunmute " + playerId, false);
                    break;
            }
        }

        public void RunCommand(string command)
        {
            if (command.Contains("[players]"))
            {
                string ids = string.Empty;

                foreach (var record in PlayerRecord.Instances)
                {
                    if (record.IsSelected)
                        ids += record.PlayerId + ".";
                }

                command = command.Replace("[players]", ids.TrimEnd('.'));
            }

            var queryProcessor = ReferenceHub.LocalHub.queryProcessor;
            if (queryProcessor == null)
                return;

            queryProcessor.CmdSendQuery(command, false);
        }

        internal static void ClearAll()
        {
            if (Singleton != null && Singleton.menus != null)
            {
                foreach (var menu in Singleton.menus)
                    menu.Reset();
            }

            PlayerInfoQR.Clear();
        }

        public void ToggleMenu(GameObject panel)
        {
            SubMenu menu = FindMenu(panel);
            int index = Array.IndexOf(menus, menu);

            if (_currentMenuIndex != index)
                SelectMenu(menu);
            else
                SelectMenu(menus[_lastMenuIndex]);
        }

        public void SelectMenu(int index)
        {
            if (menus == null || index < 0 || index >= menus.Length)
                return;

            SelectMenu(menus[index]);
        }

        public void SelectMenu(GameObject panel)
        {
            SelectMenu(FindMenu(panel));
        }

        public void SelectMenu(SubMenu subMenu)
        {
            PlayerInfoQR.Clear();

            if (_currentMenu != null)
                _currentMenu.Reset();

            if (WindowTitle != null)
                WindowTitle.text = subMenu.Title;

            _lastMenuIndex = _currentMenuIndex;
            _currentMenuIndex = Array.IndexOf(menus, subMenu);
            _currentMenu = subMenu;

            if (subMenu.Panel != null)
                subMenu.Panel.SetActive(true);

            if (subMenu.ButtonText != null)
                subMenu.ButtonText.color = c_selected;
        }

        private SubMenu FindMenu(GameObject panel)
        {
            if (menus == null)
                return null;

            foreach (var menu in menus)
            {
                if (menu != null && menu.Panel == panel)
                    return menu;
            }

            return null;
        }
    }
}