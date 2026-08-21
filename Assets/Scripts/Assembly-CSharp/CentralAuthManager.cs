using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Discord;
using GameCore;
using Org.BouncyCastle.Crypto;

public static class CentralAuthManager
{
    private static bool _initialized;

    private static readonly Stopwatch _timeCounter;

    private static bool _interacted;

    internal static bool ForceRenew;

    internal static bool RequestAuthToken;

    internal static RequestSignatureResponse SignedToken;

    // True once a signed authentication token (from requestsignature.php) is available to send.
    internal static bool SignedTokenReady;

    internal static bool TokenObtained;

    internal static string ApiToken;

    internal static string Nonce;

    internal static AuthStatusType AuthStatusType;

    private static ushort _lifetime;

    private static bool _abort;

    private static Thread _authThread;

    private static string DiscordOAuth2Token;

    public static DistributionPlatform Platform { get; private set; }

    internal static bool Authenticated { get; private set; }

    internal static string GlobalBanReason { get; private set; }

    internal static CentralAuthPreauthToken PreauthToken { get; private set; }

    internal static string DiscordState { get; private set; }

    internal static global::Discord.Discord Discord { get; private set; }

    internal static AsymmetricCipherKeyPair SessionKeys { get; private set; }

    static CentralAuthManager()
    {
        _timeCounter = new Stopwatch();
        ApiToken = string.Empty;
        Nonce = string.Empty;
        DiscordState = "Not initialized!";
    }

    private static void Authenticate()
    {
        if (_authThread != null && _authThread.IsAlive)
            return;

        _authThread = new Thread(Authentication)
        {
            Priority = ThreadPriority.AboveNormal,
            IsBackground = true,
            Name = "Authentication Thread"
        };
        _authThread.Start();
    }

    internal static void Abort()
    {
        if (_authThread != null && _authThread.IsAlive)
        {
            global::UnityEngine.Debug.Log("Stopping authentication thread...");
            _abort = true;
        }
    }

    private static void AuthDebug(string msg, string color = "blue")
    {
        string text = string.Concat(new string[]
        {
            "<color=",
            color,
            ">",
            msg,
            "</color>"
        });
        global::GameCore.Console.AddDebugLog("SDAUTH", text, MessageImportance.LessImportant, false);
    }

    private static void Authentication()
    {
        _abort = false;
        AuthStatusType = AuthStatusType.Connecting;
        Thread.Sleep(25);
        AuthDebug("Authentication initialized.", "gray");

        if (LocalCentralServer.IsActive)
        {
            LocalAuthentication();
            while (!_abort)
            {
                if (RequestAuthToken)
                {
                    RequestAuthToken = false;
                    TokenObtained = false;
                    RequestToken();
                }
                Thread.Sleep(200);
            }
            return;
        }

        List<string> postParams = new List<string>();
        int attempt = 0;
        bool steamErrorLogged = false;

        while (!_abort)
        {
            try
            {
                if (!Authenticated)
                {
                    string ticket = string.Empty;
                    DistributionPlatform platform = Platform;

                    if (platform == DistributionPlatform.Steam)
                    {
                        AuthDebug("Revoking previous Steam authentication ticket...", "gray");
                        SteamManager.CancelTicket();
                        AuthDebug("<color=blue>Obtaining authentication ticket from Steam...</color>", "blue");
                        Steamworks.AuthTicket authSessionTicket = SteamManager.GetAuthSessionTicket();
                        if (authSessionTicket?.Data == null)
                        {
                            AuthStatusType = AuthStatusType.Failure;
                            if (!steamErrorLogged)
                            {
                                steamErrorLogged = true;
                                string steamErr = "Steam authentication ticket unavailable (Steam not running or not logged on). Cannot authenticate via Steam.";
                                global::UnityEngine.Debug.LogError(steamErr);
                                AuthDebug(steamErr, "red");
                            }
                            Thread.Sleep(10000);
                            continue;
                        }
                        ticket = global::System.BitConverter.ToString(authSessionTicket.Data).Replace("-", string.Empty);
                        AuthDebug("<color=blue>Authentication Ticked obtained from Steam.</color>", "blue");
                    }
                    else if (platform == DistributionPlatform.Discord)
                    {
                        ticket = DiscordOAuth2Token;
                    }

                    string signature = Sign(ticket);
                    attempt++;
                    AuthDebug(string.Format("Establishing authentication session (attempt {0})...", attempt), "blue");

                    postParams.Clear();
                    string[] paramArr = new string[5];
                    paramArr[0] = string.Concat("ticket=", ticket);
                    string pubKeyStr = Cryptography.ECDSA.KeyToString(SessionKeys.Public);
                    string pubKeyHashStr = Cryptography.Sha.HashToString(Cryptography.Sha.Sha256(pubKeyStr));
                    paramArr[1] = string.Concat("publickey=", pubKeyHashStr);
                    paramArr[2] = string.Concat("signature=", signature);
                    paramArr[3] = "version=2";
                    paramArr[4] = string.Concat("platform=", platform == DistributionPlatform.Discord ? "Discord" : "Steam");
                    postParams.AddRange(paramArr);

                    if (PlayerPrefsSl.Get("DNT", false))
                        postParams.Add("DNT=true");
                    if (PlayerPrefsSl.Get("DisplaySteamProfile", false))
                        postParams.Add("DisplayProfile=true");
                    if (global::GameCore.Version.PrivateBeta)
                        postParams.Add("privatebeta=true");

                    string endpoint = platform == DistributionPlatform.Discord ? "v5/discord/authenticate.php" : "v5/steam/authenticate.php";
                    string url = string.Concat(CentralServer.StandardUrl, endpoint);
                    string response = HttpQuery.Post(url, HttpQuery.ToPostArgs(postParams));
                    AuthDebug(string.Concat("[AUTH] ", response), "cyan");

                    try
                    {
                        AuthenticateResponse authResp = JsonSerialize.FromJson<AuthenticateResponse>(response);
                        if (!authResp.success || string.IsNullOrEmpty(authResp.token) || string.IsNullOrEmpty(authResp.id))
                            throw new Exception("Authentication rejected by central server.");

                        ApiToken = authResp.token;
                        Nonce = authResp.nonce;
                        GlobalBanReason = authResp.globalBan;
                        _lifetime = authResp.lifetime;
                        Authenticated = true;
                        TokenObtained = true;
                        AuthStatusType = AuthStatusType.Success;
                        AuthDebug("Authentication session established.", "green");
                    }
                    catch (Exception)
                    {
                        Authenticated = false;
                        AuthStatusType = AuthStatusType.Failure;
                        string errMsg = string.Concat("Authentication error (authenticate - response): ", response);
                        global::UnityEngine.Debug.LogError(errMsg);
                        AuthDebug(errMsg, "red");
                        global::UnityEngine.Debug.LogError(string.Concat("[AUTH] ", response));
                        global::UnityEngine.Debug.LogError(string.Concat("Signature1: ", signature));
                        AuthDebug(string.Concat("Signature1: ", signature), "cyan");
                        Thread.Sleep(5000);
                    }

                    postParams.Clear();
                }
                else
                {
                    int lifetimeTicks = global::System.Math.Max((int)_lifetime, 30) * 5;
                    for (int tick = 0; tick < lifetimeTicks && !_abort && !ForceRenew; tick++)
                    {
                        if (RequestAuthToken)
                        {
                            RequestAuthToken = false;
                            TokenObtained = false;
                            try
                            {
                                RequestToken();
                            }
                            catch (Exception ex)
                            {
                                AuthDebug(string.Concat("Authentication error (rqstsgn - web): ", ex.Message), "red");
                                RequestAuthToken = true;
                                Thread.Sleep(2000);
                            }
                        }
                        else
                        {
                            Thread.Sleep(200);
                        }
                    }

                    if (!_abort)
                        Renew(postParams);
                }
            }
            catch (Exception ex)
            {
                if (!Authenticated)
                    AuthStatusType = AuthStatusType.Failure;
                AuthDebug(string.Concat("Authentication error (authenticate): ", ex.Message), "red");
                Thread.Sleep(5000);
            }
        }
    }

    private static void Renew(List<string> postParams)
    {
        AuthDebug("Renewing authentication session...", "gray");
        ForceRenew = false;

        postParams.Clear();
        postParams.Add(string.Concat("token=", ApiToken));
        postParams.Add(string.Concat("nonce=", Nonce));

        if (PlayerPrefsSl.Get("DNT", false))
            postParams.Add("DNT=true");
        if (PlayerPrefsSl.Get("DisplaySteamProfile", false))
            postParams.Add("DisplayProfile=true");
        if (global::GameCore.Version.PrivateBeta)
            postParams.Add("privatebeta=true");

        string url = string.Concat(CentralServer.StandardUrl, "v5/renew.php");
        string response = HttpQuery.Post(url, HttpQuery.ToPostArgs(postParams));
        AuthDebug(string.Concat("[RENEWAL] ", response), "cyan");

        try
        {
            AuthenticateResponse renewResp = JsonSerialize.FromJson<AuthenticateResponse>(response);
            if (!renewResp.success)
                throw new Exception("Renewal rejected by central server.");

            Nonce = renewResp.nonce;
            _lifetime = renewResp.lifetime;
            GlobalBanReason = renewResp.globalBan;
            PreauthToken = new CentralAuthPreauthToken(renewResp.id, renewResp.flags, renewResp.country, renewResp.expiration, renewResp.preauth);
            Authenticated = true;
            AuthStatusType = AuthStatusType.Success;
            AuthDebug("Authentication session renewed.", "green");
        }
        catch (Exception)
        {
            string errMsg = string.Concat("Authentication error (renewal - response): ", response);
            global::UnityEngine.Debug.LogError(errMsg);
            AuthDebug(errMsg, "red");
            Authenticated = false;
        }

        postParams.Clear();
    }

    
    private static void LocalAuthentication()
    {
        try
        {
            AuthDebug("Local authentication started (no Steam ticket/launcher).", "gray");

            LocalCentralServer.ResolveIdentity(out string userId, out string nickname);
            AuthDebug("Local identity: " + userId + " (" + nickname + ")", "gray");

            string pubKeyStr = Cryptography.ECDSA.KeyToString(SessionKeys.Public);
            string pubKeyHash = Cryptography.Sha.HashToString(Cryptography.Sha.Sha256(pubKeyStr));

            List<string> postParams = new List<string>
            {
                "ticket=local",
                "publickey=" + pubKeyHash,
                "signature=.",
                "version=2",
                "platform=Steam",
                "nickname=" + global::NorthwoodLib.StringUtils.Base64Encode(nickname)
            };
            if (!string.IsNullOrEmpty(userId))
                postParams.Add("userid=" + userId);

            string url = string.Concat(CentralServer.StandardUrl, "v5/steam/authenticate.php");
            string response = HttpQuery.Post(url, HttpQuery.ToPostArgs(postParams));
            AuthDebug("[AUTH-LOCAL] " + response, "cyan");

            AuthenticateResponse authResp = JsonSerialize.FromJson<AuthenticateResponse>(response);
            ApiToken = authResp.token;
            Nonce = authResp.nonce;
            _lifetime = (ushort)(authResp.lifetime == 0 ? 400 : authResp.lifetime);
            GlobalBanReason = authResp.globalBan;
            PreauthToken = new CentralAuthPreauthToken(authResp.id, authResp.flags, authResp.country, authResp.expiration, authResp.preauth);
            Authenticated = true;
            TokenObtained = true;
            AuthStatusType = AuthStatusType.Success;
            AuthDebug("Local authentication established for " + authResp.id, "green");
            RequestToken();
        }
        catch (Exception ex)
        {
            Authenticated = false;
            AuthStatusType = AuthStatusType.Failure;
            AuthDebug("Local authentication error: " + ex.Message, "red");
            global::UnityEngine.Debug.LogError("Local authentication error: " + ex.Message);
        }
    }

    private static void RequestToken()
    {
        AuthDebug("Requesting signature from central servers...", "blue");
        FastRoundRestartController.FastRestartInProgress = false;

        List<string> postParams = new List<string>();
        postParams.Add(string.Concat("token=", ApiToken));
        postParams.Add(string.Concat("nonce=", Nonce));
        postParams.Add(string.Concat("signature=", Sign(Nonce)));
        postParams.Add("signature2=.");
        postParams.Add("version=2");
        postParams.Add(string.Concat("platform=", Platform == DistributionPlatform.Discord ? "Discord" : "Steam"));

        if (LocalCentralServer.IsActive)
        {
            if (!string.IsNullOrEmpty(LocalCentralServer.UserId))
                postParams.Add("userid=" + LocalCentralServer.UserId);
            if (!string.IsNullOrEmpty(LocalCentralServer.Nickname))
                postParams.Add("nickname=" + global::NorthwoodLib.StringUtils.Base64Encode(LocalCentralServer.Nickname));
        }

        if (PlayerPrefsSl.Get("DNT", false))
            postParams.Add("DNT=true");
        if (PlayerPrefsSl.Get("DisplaySteamProfile", false))
            postParams.Add("DisplayProfile=true");
        if (global::GameCore.Version.PrivateBeta)
            postParams.Add("privatebeta=true");

        string url = string.Concat(CentralServer.StandardUrl, "v5/requestsignature.php");
        string response = HttpQuery.Post(url, HttpQuery.ToPostArgs(postParams));
        AuthDebug("Signature obtained from central servers", "green");

        try
        {
            RequestSignatureResponse rqstResp = JsonSerialize.FromJson<RequestSignatureResponse>(response);
            SignedToken = rqstResp;
            SignedTokenReady = !string.IsNullOrEmpty(rqstResp.auth);
            if (!string.IsNullOrEmpty(rqstResp.nonce))
                Nonce = rqstResp.nonce;
            TokenObtained = true;
            RequestAuthToken = false;

            if (response != null)
            {
                string[] lines = response.Split(new string[] { "<br>" }, StringSplitOptions.None);
                for (int i = 0; i < lines.Length; i++)
                    AuthDebug(string.Concat("[AUTH DEBUG - RQST] ", lines[i]), "cyan");
            }
            else
            {
                AuthDebug("[AUTH DEBUG - RQST] (NULL)", "cyan");
            }
        }
        catch (Exception)
        {
            string errMsg = string.Concat("Authentication error (rqstsgn - response): ", response);
            global::UnityEngine.Debug.LogError(errMsg);
            AuthDebug(errMsg, "red");
            TokenObtained = true;
            RequestAuthToken = true;
        }
    }

    private static string Sign(string ticket)
    {
       if (LocalCentralServer.IsActive)
            return ".";
        try
        {
            AuthDebug("Signing auth ticket...", "blue");
            string result = LauncherCommunicator.Send(ticket);
            AuthDebug("Auth ticket signed!", "blue");
            return result;
        }
        catch (Exception ex)
        {
            global::UnityEngine.Debug.Log("[Launcher Debug] Catch");
            global::UnityEngine.Debug.LogError("Laucher signer error");
            global::UnityEngine.Debug.LogError(ex.Message);
            return string.Empty;
        }
    }

    internal static void InitAuth()
    {
        if (_initialized)
            return;

        _initialized = true;
        CentralServer.Init();

        if (ServerStatic.IsDedicated)
        {
            Platform = global::GameCore.DistributionPlatform.Dedicated;
            global::GameCore.Console.AddLog("Running as headless dedicated server. Skipping distribution platform detection.", new global::UnityEngine.Color32(0, byte.MaxValue, 0, byte.MaxValue));
            return;
        }

        global::GameCore.Console.AddLog("CentralAuth initialized.", new global::UnityEngine.Color32(0, 255, 0, 255));

        SessionKeys = Cryptography.ECDSA.GenerateKeys(384);
        global::GameCore.Console.AddLog("Session key pair generated.", global::UnityEngine.Color.green);

        if (LocalCentralServer.IsActive)
        {
            Platform = DistributionPlatform.Steam;
            global::GameCore.Console.AddLog("LOCAL central auth mode — auto identity, no Steam ticket/launcher.", new global::UnityEngine.Color32(0, 255, 0, 255));
            try
            {
                SteamManager.StartClient();
            }
            catch (global::System.Exception ex)
            {
                global::GameCore.Console.AddLog("Steam not available for local identity (" + ex.Message + ") — using fallback id.", global::UnityEngine.Color.yellow);
            }
            Authenticate();
            return;
        }

        string[] args = StartupArgs.Args;

        bool forceSteam = false;
#if UNITY_EDITOR
        forceSteam = true;
#endif

        if (forceSteam || (args.Any(x => x.StartsWith("-steam")) && args.All(x => !x.StartsWith("-discord"))))
        {
            Platform = DistributionPlatform.Steam;
            global::GameCore.Console.AddLog("Detected distribution platform: Steam", new global::UnityEngine.Color32(0, 255, 0, 255));
            SteamManager.StartClient();
            SteamManager.RefreshToken();
            Authenticate();
        }
        else if (args.Any(x => x.StartsWith("-discord")))
        {
            Platform = DistributionPlatform.Discord;
            global::GameCore.Console.AddLog("Detected distribution platform: Discord", new global::UnityEngine.Color32(0, 255, 0, 255));
            Discord = new global::Discord.Discord(0x5D68B6438800000L, 0UL);
            Discord.SetLogHook(global::Discord.LogLevel.Debug, (level, message) =>
            {
                AuthDebug("[Discord] " + message, "cyan");
            });
            Discord.GetApplicationManager().GetOAuth2Token(OnOAuth2TokenCallback);
        }
        else
        {
            Platform = DistributionPlatform.Steam;
            global::GameCore.Console.AddLog("Cannot detect distribution platform. Using default platform: Steam", new global::UnityEngine.Color32(0, 255, 0, 255));
            SteamManager.StartClient();
            SteamManager.RefreshToken();
            Authenticate();
        }

        if (!global::GameCore.Version.PrivateBeta)
            return;

        if (Platform == DistributionPlatform.Steam)
        {
            if (Steamworks.SteamApps.IsDlcInstalled(859330))
                return;
        }

        global::UnityEngine.Debug.Log("You are not authorized to run private beta of SCP: Secret Laboratory.");
        _timeCounter.Start();

        new Thread(() =>
        {
            while (!_interacted && _timeCounter.ElapsedMilliseconds < 30000)
                Thread.Sleep(100);
        })
        {
            IsBackground = true
        }.Start();

        while (!_interacted && _timeCounter.ElapsedMilliseconds < 30000)
            Thread.Sleep(100);

        Shutdown.Quit(true);
    }

    private static void OnOAuth2TokenCallback(Result result, ref OAuth2Token token)
    {
        _interacted = true;

        if (result == Result.Ok)
        {
            DiscordOAuth2Token = token.AccessToken;
            Authenticate();
        }
        else
        {
            DiscordOAuth2Token = null;
            AuthStatusType = AuthStatusType.Failure;
            AuthDebug(string.Concat("Discord OAuth2 token request failed: ", result), "red");
        }
    }
}