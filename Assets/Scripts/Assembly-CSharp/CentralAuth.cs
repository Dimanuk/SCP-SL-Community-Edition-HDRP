using GameCore;
using Mirror;
using NorthwoodLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using UnityEngine;

public class CentralAuth : MonoBehaviour
{
    public static bool GlobalBadgeIssued;
    private ICentralAuth _ica;
    private static bool _awaitingTicket;
    private static bool _awaitingRequest;
    public static CentralAuth singleton;

    private void Awake()
    {
        singleton = this;
    }

    private static void AuthDebug(string msg, string color = "blue")
    {
        GameCore.Console.AddDebugLog("SDAUTH", "<color=" + color + ">" + msg + "</color>", MessageImportance.Normal, false);
    }

    private void Update()
    {
        if (!_awaitingTicket)
            return;

        if (global::Mirror.NetworkServer.active)
        {
            _awaitingTicket = false;
            return;
        }

        if (!CentralAuthManager.TokenObtained)
            return;

        if (!CentralAuthManager.SignedTokenReady)
        {
            CentralAuthManager.RequestAuthToken = true;
            return;
        }

        if (!ReferenceHub.TryGetLocalHub(out ReferenceHub hub))
            return;

        _awaitingTicket = false;
        _awaitingRequest = true;

        RequestSignatureResponse signed = CentralAuthManager.SignedToken;
        AuthDebug("Sending your authentication token to game server...", "green");

        _ica.TokenGenerated(signed.auth);
        if (!string.IsNullOrEmpty(signed.badge))
            _ica.RequestBadge(signed.badge);
        if (!string.IsNullOrEmpty(signed.pub))
            _ica.RequestPublicPart(signed.pub);
    }

    internal void GenerateToken(ICentralAuth icaa)
    {
        _ica = icaa;
        _awaitingTicket = true;
        _awaitingRequest = true;
        CentralAuthManager.TokenObtained = false;
        CentralAuthManager.RequestAuthToken = true;
    }

    public void StartValidateToken(ICentralAuth icaa, string token, IPEndPoint endpoint)
    {
        MEC.Timing.RunCoroutine(_ValidateToken(icaa, token, endpoint), MEC.Segment.FixedUpdate);
    }

    private System.Collections.Generic.IEnumerator<float> _ValidateToken(ICentralAuth icaa, string token, IPEndPoint endpoint)
    {
        if (string.IsNullOrEmpty(token) || !token.Contains("<br>Signature: "))
        {
            icaa.FailToken("Malformed token.");
            yield break;
        }

        try
        {
            int sigIndex = token.IndexOf("<br>Signature: ", StringComparison.Ordinal);
            string text = token.Substring(0, sigIndex);
            string text2 = token.Substring(sigIndex + 15).Replace("<br>", "");

            if (!Cryptography.ECDSA.Verify(text, text2, ServerConsole.PublicKey))
            {
                ServerConsole.AddLog("Authentication token signature mismatch.");
                icaa.GetHub().gameConsoleTransmission.SendToClient("Authentication token rejected due to signature mismatch.", "red");
                icaa.FailToken("Failed to validate authentication token signature.");
            }
            else
            {
                var dictionary = text.Split(new[] { "<br>" }, StringSplitOptions.None)
                    .Select(rwr => rwr.Split(new[] { ": " }, StringSplitOptions.None))
                    .ToDictionary(split => split[0], split => split[1]);

                string userId = RemoveSalt(dictionary["User ID"]);

                if (dictionary["Usage"] != "Authentication")
                {
                    ServerConsole.AddLog("Player tried to use token not issued to authentication purposes.");
                    icaa.GetHub().gameConsoleTransmission.SendToClient("Authentication token rejected due to invalid purpose of signature.", "red");
                    _ica.FailToken("Token supplied by your game can't be used for authentication purposes.");
                }
                else if (endpoint != null && (!CustomLiteNetLib4MirrorTransport.UserIds.ContainsKey(endpoint) ||
                         !CustomLiteNetLib4MirrorTransport.UserIds[endpoint].UserId.Equals(userId, StringComparison.Ordinal)) &&
                         !CustomLiteNetLib4MirrorTransport.UserIdFastReload.Contains(userId))
                {
                    ServerConsole.AddLog("Player tried to use token issued to a different user than the preauthentication token.");
                    icaa.GetHub().gameConsoleTransmission.SendToClient("UserID mismatch between authentication and preauthentication token.", "red");
                    icaa.GetHub().gameConsoleTransmission.SendToClient("Preauth: " + (CustomLiteNetLib4MirrorTransport.UserIds.ContainsKey(endpoint) ?
                         CustomLiteNetLib4MirrorTransport.UserIds[endpoint].UserId : "(null)"), "red");
                    icaa.GetHub().gameConsoleTransmission.SendToClient("Auth: " + userId, "red");
                    _ica.FailToken("UserID mismatch between authentication and preauthentication token. Check the game console for more details.");
                }
                else if (dictionary["Test signature"] != "NO" && !CentralServer.TestServer)
                {
                    ServerConsole.AddLog("Player tried to use authentication token issued only for testing. Server: " + dictionary["Issued by"] + ".");
                    icaa.GetHub().gameConsoleTransmission.SendToClient("Authentication token rejected due to testing signature.", "red");
                    _ica.FailToken("Your authentication token is issued only for testing purposes.");
                }
                else if (!dictionary.ContainsKey("Auth Version") || dictionary["Auth Version"] != "2")
                {
                    ServerConsole.AddLog("Player used invalid version of authentication token. Server: " + dictionary["Issued by"] + ".");
                    icaa.GetHub().gameConsoleTransmission.SendToClient("Authentication token version mismatch. Required version: 2.", "red");
                    _ica.FailToken("This version of game requires authentication token version 2.");
                }
                else
                {
                    DateTime expiration = DateTime.ParseExact(dictionary["Expiration time"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    DateTime issuance = DateTime.ParseExact(dictionary["Issuance time"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                    if (expiration < DateTime.UtcNow)
                    {
                        ServerConsole.AddLog("Player tried to use expired authentication token. Server: " + dictionary["Issued by"] + ".");
                        ServerConsole.AddLog("Make sure that time and timezone set on server is correct. We recommend synchronizing the time.");
                        icaa.GetHub().gameConsoleTransmission.SendToClient("Authentication token rejected due to expired signature.", "red");
                        _ica.FailToken("Your authentication token has expired.");
                    }
                    else if (issuance > DateTime.UtcNow.AddMinutes(20.0))
                    {
                        ServerConsole.AddLog("Player tried to use non-issued authentication token. Server: " + dictionary["Issued by"] + ".");
                        ServerConsole.AddLog("Make sure that time and timezone set on server is correct. We recommend synchronizing the time.");
                        icaa.GetHub().gameConsoleTransmission.SendToClient("Authentication token rejected due to non-issued signature.", "red");
                        _ica.FailToken("Your authentication token has invalid issuance date.");
                    }
                    else if (GameCore.Version.PrivateBeta && (!dictionary.ContainsKey("Private beta ownership") || dictionary["Private beta ownership"] != "YES"))
                    {
                        ServerConsole.AddLog("Player " + dictionary["User ID"] + " tried to join this server, but is not Private Beta DLC owner. Server: " + dictionary["Issued by"] + ".");
                        icaa.GetHub().gameConsoleTransmission.SendToClient("Private Beta DLC ownership is required to join private beta server.", "red");
                        _ica.FailToken("Private Beta DLC ownership is required to join private beta server.");
                    }
                    else
                    {
                        ReferenceHub hub = icaa.GetHub();
                        hub.serverRoles.FirstVerResult = dictionary;
                        if (dictionary.ContainsKey("Sync Hashed") && dictionary["Sync Hashed"] == "YES")
                        {
                            hub.serverRoles.SyncHashed = true;
                        }

                        icaa.Ok(
                            userId,
                            dictionary.ContainsKey("User ID 2") ? dictionary["User ID 2"] : null,
                            dictionary["Global ban"],
                            dictionary["Issued by"],
                            dictionary["Bypass bans"] == "YES",
                            dictionary["Bypass WL"] == "YES",
                            dictionary.ContainsKey("Do Not Track") && dictionary["Do Not Track"] == "YES",
                            dictionary.ContainsKey("Serial") ? dictionary["Serial"] : "N/A",
                            dictionary.ContainsKey("VAC session") ? dictionary["VAC session"] : "N/A",
                            dictionary.ContainsKey("Request IP") ? dictionary["Request IP"] : "N/A",
                            dictionary.ContainsKey("ASN") ? dictionary["ASN"] : "N/A",
                            dictionary.ContainsKey("Skip IP Check") && dictionary["Skip IP Check"] == "YES"
                        );

                        string log = string.Format("{0} authenticated from endpoint {1}. Player ID assigned: {2}. Auth token serial number: {3}",
                            RemoveSalt(dictionary["User ID"]),
                            (endpoint == null) ? "(null)" : endpoint.ToString(),
                            hub.PlayerId,
                            dictionary.ContainsKey("Serial") ? dictionary["Serial"] : "N/A");

                        ServerConsole.AddLog(log);
                        ServerLogs.AddLog(ServerLogs.Modules.Networking, log, ServerLogs.ServerLogType.ConnectionUpdate);
                    }
                }

                if (endpoint != null && CustomLiteNetLib4MirrorTransport.UserIds.ContainsKey(endpoint))
                {
                    CustomLiteNetLib4MirrorTransport.UserIds.Remove(endpoint);
                }
                if (CustomLiteNetLib4MirrorTransport.UserIdFastReload.Contains(userId))
                {
                    CustomLiteNetLib4MirrorTransport.UserIdFastReload.Remove(userId);
                }
            }
        }
        catch (Exception ex)
        {
            ServerConsole.AddLog("Error during authentication token verification: " + ex.Message);
            icaa.Fail();
        }
        yield return float.NegativeInfinity;
    }

    internal static string ValidateForGlobalBanning(string token, out string userId2)
    {
        userId2 = null;
        if (string.IsNullOrEmpty(token) || !token.Contains("<br>Signature: "))
        {
            return "-1";
        }

        try
        {
            int sigIndex = token.IndexOf("<br>Signature: ", StringComparison.Ordinal);
            string text = token.Substring(0, sigIndex);
            string text2 = token.Substring(sigIndex + 15).Replace("<br>", "");

            if (!Cryptography.ECDSA.Verify(text, text2, ServerConsole.PublicKey))
            {
                AuthDebug("Authentication token rejected due to signature mismatch.", "red");
                return "-1";
            }

            var dictionary = text.Split(new[] { "<br>" }, StringSplitOptions.None)
                .Select(rwr => rwr.Split(new[] { ": " }, StringSplitOptions.None))
                .ToDictionary(split => split[0], split => split[1]);

            if (dictionary["Usage"] != "Authentication")
            {
                AuthDebug("Authentication token rejected due to usage mismatch.", "red");
                return "-1";
            }

            if (dictionary["Test signature"] != "NO")
            {
                AuthDebug("Authentication token rejected due to test flag.", "red");
                return "-1";
            }

            if (!dictionary.ContainsKey("Auth Version") || dictionary["Auth Version"] != "2")
            {
                AuthDebug("Authentication token rejected due to token version mismatch.", "red");
                return "-1";
            }

            DateTime expiration = DateTime.ParseExact(dictionary["Expiration time"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime issuance = DateTime.ParseExact(dictionary["Issuance time"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            if (expiration < DateTime.UtcNow)
            {
                AuthDebug("Authentication token rejected due to expiration date.", "red");
                return "-1";
            }

            if (issuance > DateTime.UtcNow.AddMinutes(20.0))
            {
                AuthDebug("Authentication token rejected due to issuance date.", "red");
                return "-1";
            }

            string userId = RemoveSalt(dictionary["User ID"]);
            AuthDebug("Accepted authentication token of user " + StringUtils.Base64Decode(dictionary["Nickname"]) + " signed by " + dictionary["Issued by"] + ".", "green");
            userId2 = dictionary.ContainsKey("User ID 2") ? dictionary["User ID 2"] : null;
            return userId;
        }
        catch (Exception ex)
        {
            AuthDebug("Error during authentication token verification: " + ex.Message, "red");
            return "-1";
        }
    }

    internal static Dictionary<string, string> ValidatePartialAuthToken(string token, string userId, string nickname, string authSerial, string usage)
    {
        if (string.IsNullOrEmpty(token) || !token.Contains("<br>Signature: "))
        {
            return null;
        }

        try
        {
            int sigIndex = token.IndexOf("<br>Signature: ", StringComparison.Ordinal);
            string text = token.Substring(0, sigIndex);
            string text2 = token.Substring(sigIndex + 15).Replace("<br>", "");

            if (!Cryptography.ECDSA.Verify(text, text2, ServerConsole.PublicKey))
            {
                ServerConsole.AddLog("Partial auth token signature mismatch.");
                return null;
            }

            var dictionary = text.Split(new[] { "<br>" }, StringSplitOptions.None)
                .Select(rwr => rwr.Split(new[] { ": " }, StringSplitOptions.None))
                .ToDictionary(split => split[0], split => split[1]);

            if (dictionary["Usage"] != usage)
            {
                ServerConsole.AddLog("Player tried to use a partial auth token issued to a different purpose.");
                return null;
            }

            if (authSerial != null && dictionary["Serial"] != authSerial)
            {
                ServerConsole.AddLog("Partial auth token serial mismatch.");
                return null;
            }

            if (dictionary["Test signature"] != "NO")
            {
                ServerConsole.AddLog("Player tried to use a partial auth token issued only for testing. Server: " + dictionary["Issued by"] + ".");
                return null;
            }

            if (string.IsNullOrEmpty(userId))
            {
                ServerConsole.AddLog("Player tried to use a partial auth token issued for different user (User ID mismatch - empty). Server: " + dictionary["Issued by"] + ".");
                return null;
            }

            string hashedInput = Cryptography.Sha.HashToString(Cryptography.Sha.Sha512(userId));
            string hashedToken = Cryptography.Sha.HashToString(Cryptography.Sha.Sha512(dictionary["User ID"]));

            if (dictionary["User ID"] != userId && dictionary["User ID"] != hashedInput && hashedToken != userId && hashedToken != hashedInput)
            {
                ServerConsole.AddLog("Player tried to use a partial auth token issued for different user (User ID mismatch). Server: " + dictionary["Issued by"] + ".");
                return null;
            }

            if (StringUtils.Base64Decode(dictionary["Nickname"]) != nickname)
            {
                ServerConsole.AddLog("Player tried to use a partial auth token issued for different user (nickname mismatch). Server: " + dictionary["Issued by"] + ".");
                return null;
            }

            DateTime expiration = DateTime.ParseExact(dictionary["Expiration time"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime issuance = DateTime.ParseExact(dictionary["Issuance time"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            if (expiration < DateTime.UtcNow)
            {
                ServerConsole.AddLog("Player tried to use expired partial auth request token. Server: " + dictionary["Issued by"] + ".");
                return null;
            }

            if (issuance > DateTime.UtcNow.AddMinutes(20.0))
            {
                ServerConsole.AddLog("Player tried to use non-issued partial auth token. Server: " + dictionary["Issued by"] + ".");
                return null;
            }

            return dictionary;
        }
        catch (Exception ex)
        {
            ServerConsole.AddLog("Error during partial auth token verification: " + ex.Message);
            UnityEngine.Debug.Log("Error during partial auth token verification: " + ex.Message + " StackTrace: " + ex.StackTrace);
            return null;
        }
    }

    public static string RemoveSalt(string userId)
    {
        if (userId.Contains("$"))
        {
            return userId.Substring(0, userId.IndexOf("$", StringComparison.Ordinal));
        }
        return userId;
    }
}