using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameCore;
using MEC;
using NorthwoodLib;
using RemoteAdmin.Interfaces;
using UnityEngine;
using UnityEngine.Networking;
using Console = GameCore.Console;

namespace RemoteAdmin.Communication
{
    public class RaGlobalBan : IServerCommunication, IClientCommunication
    {
        private static string _toBan;
        private static string _toBanNick;
        private static string _toBanUserId;
        private static string _toBanUserId2;
        private static string _toBanAuth;

        public int DataId => 5;

        public void ReceiveData(CommandSender sender, string data)
        {
            string[] array = data.Split(' ');
            if (array.Length < 2 || !int.TryParse(array[0], out int result))
                return;

            bool searchById = result == 1;
            data = string.Join(" ", array.Skip(1));

            if (!(sender is PlayerCommandSender playerCommandSender) || !playerCommandSender.ServerRoles.Staff)
                return;

            ReferenceHub referenceHub = null;
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if ((searchById && $"{allHub.PlayerId}" == data) ||
                    (!searchById && string.Equals(allHub.nicknameSync.MyNick, data, StringComparison.CurrentCultureIgnoreCase)))
                {
                    referenceHub = allHub;
                    break;
                }
            }

            if (referenceHub == null)
                sender.RaReply($"${DataId} 0", success: true, logToConsole: false, string.Empty);
            else
                sender.RaReply($"${DataId} 1 {referenceHub.characterClassManager.AuthToken}", success: true, logToConsole: false, string.Empty);
        }

        public static void Request(string key, int keytype)
        {
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                bool match;
                if (keytype == 1)
                {
                    match = $"{allHub.PlayerId}" == key;
                }
                else
                {
                    match = string.Equals(allHub.nicknameSync.MyNick, key, StringComparison.CurrentCultureIgnoreCase);
                }

                if (match)
                {
                    _toBanNick = allHub.nicknameSync.MyNick;
                    break;
                }
            }

            _toBan = key;

            ReferenceHub localHub = ReferenceHub.LocalHub;
            if (localHub != null ? localHub.queryProcessor : null != null)
            {
                localHub.queryProcessor.CmdSendQuery($"$5 {keytype} {key}", false);
            }
        }

        public void ReceiveData(string data, bool secure)
        {
            if (string.IsNullOrEmpty(_toBan))
            {
                Console.AddLog("You don't have any pending global ban request to confirm.", Color.yellow);
                return;
            }

            string[] array = data.Split(' ');
            if (array.Length < 1 || !int.TryParse(array[0], out int type))
                return;

            if (type == 0)
            {
                Console.AddLog("Requested player can't be found!", Color.red);
                ClearBanData();
            }
            else if (type == 1)
            {
                if (array.Length > 2)
                {
                    string remaining = string.Join(" ", array.Skip(1));

                    string userId = CentralAuth.ValidateForGlobalBanning(remaining, out string userId2);

                    if (userId == "-1")
                    {
                        Console.AddLog("Aborting global banning....", Color.red);
                        ClearBanData();
                        return;
                    }

                    _toBanUserId = userId;
                    _toBanUserId2 = userId2;

                    if (array.Length > 1)
                        _toBanAuth = array[1];

                    Console.AddLog("==== GLOBAL BANNING FINAL STEP ====", Color.cyan);

                    if (!secure)
                        Console.AddLog("Token obtained over **UNENCRYPTED** connection.", Color.yellow);
                    else
                        Console.AddLog("Token obtained over encrypted connection.", Color.cyan);

                    Console.AddLog("Nick: " + _toBanNick, Color.cyan);
                    Console.AddLog("ID on this server: " + _toBan, Color.cyan);
                    Console.AddLog("User ID: " + _toBanUserId, Color.cyan);
                    Console.AddLog("User ID 2: " + (string.IsNullOrEmpty(_toBanUserId2) ? "(none)" : _toBanUserId2), Color.cyan);
                    Console.AddLog("", Color.cyan);
                    Console.AddLog("To confirm ban please execute \"CONFIRM\" command.", Color.cyan);
                    Console.AddLog("==== GLOBAL BANNING FINAL STEP ====", Color.cyan);
                    _toBanNick = string.Empty;
                }
            }
            else
            {
                Console.AddLog($"Recevied global ban response but type {type}!", Color.red);
            }
        }

        internal static void ConfirmGlobalBanning()
        {
            Timing.RunCoroutine(IssueGlobalBan(), Segment.Update);
        }

        private static IEnumerator<float> IssueGlobalBan()
        {
            if (string.IsNullOrEmpty(_toBanUserId))
            {
                Console.AddLog("You don't have any pending global ban request to confirm.", Color.yellow);
                yield break;
            }

            Console.AddLog("Issuing global ban for " + _toBanUserId, Color.cyan);

            WWWForm form = new();

            string appFolder = FileManager.GetAppFolder(true, false, "");
            string staffApiPath = appFolder + "StaffAPI.txt";

            using (StreamReader reader = new StreamReader(staffApiPath))
            {
                string token = reader.ReadToEnd();
                form.AddField("token", token);
            }

            form.AddField("action", "ban");
            form.AddField("userid", _toBanUserId);

            if (!string.IsNullOrEmpty(_toBanUserId2))
                form.AddField("userid2", _toBanUserId2);
            else
                form.AddField("userid2", "N/A");

            string encodedAuth = StringUtils.Base64Encode(_toBanAuth);
            form.AddField("auth", encodedAuth);

            string url = CentralServer.MasterUrl + "v5/globalbanning.php";

            using UnityWebRequest www = UnityWebRequest.Post(url, form);

            yield return Timing.WaitUntilDone(www.SendWebRequest());

            if (!string.IsNullOrEmpty(www.error))
            {
                Console.AddLog("Error during global ban issuance: " + www.error, Color.red);
            }
            else
            {
                string response = www.downloadHandler.text;

                if (response == "Banned")
                {
                    Console.AddLog("Global ban issued, kicking player from server...", Color.cyan);

                    ReferenceHub localHub = ReferenceHub.LocalHub;
                    if (localHub != null ? localHub.queryProcessor : null != null)
                    {
                        localHub.queryProcessor.CmdSendQuery("GBAN-KICK " + _toBan, true);
                    }

                    Console.AddLog("==== GLOBAL BANNING CONFIRMATION ====", Color.green);
                    Console.AddLog("ID on this server: " + _toBan, Color.green);
                    Console.AddLog("User ID: " + _toBanUserId, Color.green);
                    Console.AddLog("", Color.green);
                    Console.AddLog("Player has been globally banned.", Color.green);
                    Console.AddLog("Request to kick this player has been sent to game server.", Color.green);
                    Console.AddLog("==== GLOBAL BANNING CONFIRMATION ====", Color.green);

                    ClearBanData();
                }
                else
                {
                    Console.AddLog("Server error during global ban issuance: " + response, Color.red);
                }
            }
        }

        private static void ClearBanData()
        {
            _toBan = string.Empty;
            _toBanNick = string.Empty;
            _toBanUserId = string.Empty;
            _toBanUserId2 = string.Empty;
            _toBanAuth = string.Empty;
        }
    }
}