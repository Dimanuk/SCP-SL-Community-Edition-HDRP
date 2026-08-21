namespace Authenticator
{
    public static class AuthenticatorQuery
    {
        public static bool SendData(global::System.Collections.Generic.IEnumerable<string> data)
        {
            try
            {
                string text = HttpQuery.Post(CentralServer.MasterUrl + "v5/authenticator.php", HttpQuery.ToPostArgs(data));
                return text.StartsWith("{\"") ? ProcessResponse(text) : ProcessLegacyResponse(text);
            }
            catch (global::System.Exception ex)
            {
                ServerConsole.AddLog("Could not update server data on server list - (LOCAL EXCEPTION) " + ex.Message, global::System.ConsoleColor.DarkRed);
                return false;
            }
        }

        private static void SendContactAddress()
        {
            try
            {
                global::System.Collections.Generic.List<string> list = new global::System.Collections.Generic.List<string>
                {
                    "ip=" + ServerConsole.Ip,
                    "port=" + ServerConsole.PortToReport,
                    "version=2",
                    "address=" + global::NorthwoodLib.StringUtils.Base64Encode(global::GameCore.ConfigFile.ServerConfig.GetString("contact_email"))
                };
                if (!string.IsNullOrEmpty(ServerConsole.Password))
                {
                    list.Add("passcode=" + ServerConsole.Password);
                }
                HttpQuery.Post(CentralServer.MasterUrl + "v5/contactaddress.php", HttpQuery.ToPostArgs(list));
            }
            catch
            {
            }
        }

        private static bool ProcessResponse(string response)
        {
            try
            {
                AuthenticatorResponse authenticatorResponse = JsonSerialize.FromJson<AuthenticatorResponse>(response);
                if (!string.IsNullOrEmpty(authenticatorResponse.verificationChallenge) && !string.IsNullOrEmpty(authenticatorResponse.verificationResponse))
                {
                    CustomLiteNetLib4MirrorTransport.VerificationChallenge = authenticatorResponse.verificationChallenge;
                    CustomLiteNetLib4MirrorTransport.VerificationResponse = authenticatorResponse.verificationResponse;
                    ServerConsole.AddLog("Verification challenge and response have been obtained.", global::System.ConsoleColor.Green);
                }
                if (!authenticatorResponse.success)
                {
                    ServerConsole.AddLog("Could not update server data on server list - " + authenticatorResponse.error, global::System.ConsoleColor.DarkRed);
                    return false;
                }
                if (!string.IsNullOrEmpty(authenticatorResponse.token))
                {
                    ServerConsole.AddLog("Received verification token from central server.");
                    SaveNewToken(authenticatorResponse.token);
                }
                if (authenticatorResponse.actions != null && authenticatorResponse.actions.Length != 0)
                {
                    string[] actions = authenticatorResponse.actions;
                    for (int i = 0; i < actions.Length; i++)
                    {
                        HandleAction(actions[i]);
                    }
                }
                if (authenticatorResponse.messages != null && authenticatorResponse.messages.Length != 0)
                {
                    string[] actions = authenticatorResponse.messages;
                    foreach (string text in actions)
                    {
                        ServerConsole.AddLog("[MESSAGE FROM CENTRAL SERVER] " + text, global::System.ConsoleColor.Cyan);
                    }
                }
                if (authenticatorResponse.authAccepted != null && authenticatorResponse.authAccepted.Length != 0)
                {
                    string[] actions = authenticatorResponse.authAccepted;
                    foreach (string text2 in actions)
                    {
                        ServerConsole.AddLog("Authentication token of player " + text2 + " has been confirmed by central server.");
                    }
                }
                if (authenticatorResponse.authRejected != null && authenticatorResponse.authRejected.Length != 0)
                {
                    foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
                    {
                        CharacterClassManager ccm = allHub.characterClassManager;
                        if (!global::System.Linq.Enumerable.All(authenticatorResponse.authRejected, (AuthenticatiorAuthReject rj) => rj.Id != ccm.UserId))
                        {
                            string text3 = global::System.Linq.Enumerable.FirstOrDefault(authenticatorResponse.authRejected, (AuthenticatiorAuthReject rj) => rj.Id == ccm.UserId).Reason ?? "<ERROR>";
                            ServerConsole.AddLog("Authentication token of player " + ccm.UserId + " has been revoked by central server with reason: " + text3);
                            ccm.GetComponent<GameConsoleTransmission>().SendToClient(ccm.connectionToClient, text3, "red");
                            ServerConsole.Disconnect(ccm.connectionToClient, text3);
                        }
                    }
                }
                return authenticatorResponse.verified;
            }
            catch (global::System.Exception ex)
            {
                ServerConsole.AddLog("Could not update server data on server list - (LOCAL EXCEPTION) " + ex.Message, global::System.ConsoleColor.DarkRed);
                return false;
            }
        }

        private static bool ProcessLegacyResponse(string response)
        {
            if (response == "YES")
            {
                return true;
            }
            if (response.StartsWith("New code generated:"))
            {
                ServerStatic.PermissionsHandler.SetServerAsVerified();
                string path = global::System.Environment.GetFolderPath(global::System.Environment.SpecialFolder.ApplicationData) + "/SCP Secret Laboratory/verkey.txt";
                try
                {
                    global::System.IO.File.Delete(path);
                }
                catch
                {
                    ServerConsole.AddLog("New password could not be saved.", global::System.ConsoleColor.DarkRed);
                }
                try
                {
                    global::System.IO.StreamWriter streamWriter = new global::System.IO.StreamWriter(path);
                    string text = response.Remove(0, response.IndexOf(":", global::System.StringComparison.Ordinal)).Remove(response.IndexOf(":", global::System.StringComparison.Ordinal));
                    while (text.Contains(":"))
                    {
                        text = text.Replace(":", string.Empty);
                    }
                    streamWriter.WriteLine(text);
                    streamWriter.Close();
                    ServerConsole.AddLog("New password saved.", global::System.ConsoleColor.DarkRed);
                    ServerConsole.Update = true;
                }
                catch
                {
                    ServerConsole.AddLog("New password could not be saved.", global::System.ConsoleColor.DarkRed);
                }
            }
            else if (response.Contains(":Restart:"))
            {
                HandleAction("Restart");
            }
            else if (response.Contains(":RoundRestart:"))
            {
                HandleAction("RoundRestart");
            }
            else if (response.Contains(":UpdateData:"))
            {
                HandleAction("UpdateData");
            }
            else if (response.Contains(":RefreshKey:"))
            {
                HandleAction("RefreshKey");
            }
            else if (response.Contains(":Message - "))
            {
                string text2 = response.Substring(response.IndexOf(":Message - ", global::System.StringComparison.Ordinal) + 11);
                text2 = text2.Substring(0, text2.IndexOf(":::", global::System.StringComparison.Ordinal));
                ServerConsole.AddLog("[MESSAGE FROM CENTRAL SERVER] " + text2, global::System.ConsoleColor.Cyan);
            }
            else if (response.Contains(":GetContactAddress:"))
            {
                HandleAction("GetContactAddress");
            }
            else
            {
                if (response.Contains("Server is not verified."))
                {
                    return false;
                }
                ServerConsole.AddLog("Could not update data on server list (legacy)- " + response, global::System.ConsoleColor.DarkRed);
            }
            return true;
        }

        internal static void HandleAction(string action)
        {
            switch (action)
            {
                case "Restart":
                    ServerConsole.AddOutputEntry(default(global::ServerOutput.ExitActionRestartEntry));
                    ServerConsole.AddLog("Server restart requested by central server.", global::System.ConsoleColor.DarkRed);
                    Shutdown.Quit();
                    break;
                case "RoundRestart":
                    {
                        ServerConsole.AddLog("Round restart requested by central server.", global::System.ConsoleColor.DarkRed);
                        if (ReferenceHub.TryGetLocalHub(out var hub) && hub.networkIdentity.isServer)
                        {
                            global::RoundRestarting.RoundRestart.InitiateRoundRestart();
                        }
                        break;
                    }
                case "UpdateData":
                    ServerConsole.Update = true;
                    break;
                case "RefreshKey":
                    ServerConsole.RunRefreshPublicKeyOnce();
                    break;
                case "GetContactAddress":
                    {
                        global::System.Threading.Thread thread = new global::System.Threading.Thread(SendContactAddress);
                        thread.Name = "SCP:SL Response to central servers (contact data request)";
                        thread.Priority = global::System.Threading.ThreadPriority.BelowNormal;
                        thread.IsBackground = true;
                        thread.Start();
                        break;
                    }
            }
        }

        private static void SaveNewToken(string token)
        {
            ServerStatic.PermissionsHandler.SetServerAsVerified();
            string path = global::System.Environment.GetFolderPath(global::System.Environment.SpecialFolder.ApplicationData) + "/SCP Secret Laboratory/verkey.txt";
            try
            {
                global::System.IO.File.Delete(path);
            }
            catch (global::System.Exception ex)
            {
                ServerConsole.AddLog("New verification token could not be saved (1): " + ex.Message, global::System.ConsoleColor.DarkRed);
            }
            try
            {
                global::System.IO.StreamWriter streamWriter = new global::System.IO.StreamWriter(path);
                streamWriter.WriteLine(token);
                streamWriter.Close();
                ServerConsole.AddLog("New verification token saved.", global::System.ConsoleColor.DarkRed);
                ServerConsole.Update = true;
                ServerConsole.ScheduleTokenRefresh = true;
            }
            catch (global::System.Exception ex2)
            {
                ServerConsole.AddLog("New verification token could not be saved (2): " + ex2.Message, global::System.ConsoleColor.DarkRed);
            }
        }
    }
}
