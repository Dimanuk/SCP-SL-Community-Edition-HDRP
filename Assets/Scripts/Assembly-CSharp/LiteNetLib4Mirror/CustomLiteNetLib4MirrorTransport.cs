using LiteNetLib;
using LiteNetLib.Utils;
using Mirror.LiteNetLib4Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using UnityEngine;

public class CustomLiteNetLib4MirrorTransport : LiteNetLib4MirrorTransport
{
    private static readonly NetDataWriter RequestWriter = new NetDataWriter();

    public static GeoblockingMode Geoblocking = GeoblockingMode.None;
    public static ChallengeType ChallengeMode = ChallengeType.Reply;
    public static ushort ChallengeInitLen = 16;
    public static ushort ChallengeSecretLen = 5;

    public static readonly Dictionary<IPEndPoint, PreauthItem> UserIds = new Dictionary<IPEndPoint, PreauthItem>();
    public static readonly HashSet<string> UserIdFastReload = new HashSet<string>();
    public static readonly Dictionary<string, PreauthChallengeItem> Challenges = new Dictionary<string, PreauthChallengeItem>();

    public static bool UserRateLimiting;
    public static bool IpRateLimiting;
    public static bool UseGlobalBans;
    public static bool GeoblockIgnoreWhitelisted;
    public static bool UseChallenge = true;
    public static bool DisplayPreauthLogs;

    private static bool _delayConnections = true;

    public static bool SuppressRejections;
    public static bool SuppressIssued;

    public static uint Rejected;
    public static uint ChallengeIssued;

    public static byte DelayTime = 3;
    internal static byte DelayVolume;
    internal static byte DelayVolumeThreshold = 255;

    public static readonly HashSet<string> UserRateLimit = new HashSet<string>();
    public static readonly HashSet<IPAddress> IpRateLimit = new HashSet<IPAddress>();
    public static readonly HashSet<string> GeoblockingList = new HashSet<string>();

    public static RejectionReason LastRejectionReason = RejectionReason.NotSpecified;
    public static string LastCustomReason = string.Empty;

    public static string VerificationChallenge;
    public static string VerificationResponse;

    public static long LastBanExpiration;
    public static bool IpPassthroughEnabled;
    public static HashSet<IPAddress> TrustedProxies = new HashSet<IPAddress>();
    public static Dictionary<int, string> RealIpAddresses = new Dictionary<int, string>();

    public static uint RejectionThreshold = 60;
    public static uint IssuedThreshold = 50;

    private static readonly byte[] EmptyByte = Array.Empty<byte>();

    private static int clientChallengeId;
    private static byte _redirectCounter;
    private static ushort clientChallengeSecretLen;
    private static byte[] clientChallenge;
    private static byte[] clientChallengeBase;
    private static byte[] clientChallengeResponse;
    private static ChallengeType clientChallengeType;
    public static ChallengeState ClientChallengeState;

    private static readonly Thread ChallengeThread;

    static CustomLiteNetLib4MirrorTransport()
    {
        ChallengeThread = new Thread(ProcessChallenge)
        {
            Name = "Challenge thread",
            Priority = System.Threading.ThreadPriority.BelowNormal,
            IsBackground = true
        };
        ChallengeThread.Start();
    }

    public static bool DelayConnections
    {
        get => _delayConnections;
        set
        {
            if (_delayConnections == value)
                return;

            if (!value)
                UserIds.Clear();

            _delayConnections = value;

            ServerConsole.AddLog(value
                ? $"Incoming connections will be now delayed by {DelayTime} seconds."
                : "Incoming connections will be no longer delayed.");
        }
    }

    internal static void SetReconnectionParameters(bool roundRestart)
    {
        LiteNetLib4MirrorTransport.Singleton.reconnectDelay = 600;
        LiteNetLib4MirrorTransport.Singleton.maxConnectAttempts = 3;
    }

    protected internal override void ProcessConnectionRequest(ConnectionRequest request)
    {
        try
        {
            if (!request.Data.TryGetByte(out var result) || result >= 2)
            {
                RequestWriter.Reset();
                RequestWriter.Put((byte)2);
                request.Reject(RequestWriter);
                return;
            }
            if (result == 1)
            {
                if (VerificationChallenge != null && request.Data.TryGetString(out var result2) && result2 == VerificationChallenge)
                {
                    RequestWriter.Reset();
                    RequestWriter.Put((byte)18);
                    RequestWriter.Put(VerificationResponse);
                    request.Reject(RequestWriter);
                    VerificationChallenge = null;
                    VerificationResponse = null;
                    ServerConsole.AddLog("Verification challenge and response have been sent.", global::System.ConsoleColor.Green);
                    return;
                }
                Rejected++;
                if (Rejected > RejectionThreshold)
                    SuppressRejections = true;
                if (!SuppressRejections && DisplayPreauthLogs)
                    ServerConsole.AddLog($"Invalid verification challenge has been received from endpoint {request.RemoteEndPoint}.");
                RequestWriter.Reset();
                RequestWriter.Put((byte)19);
                request.Reject(RequestWriter);
                return;
            }
            byte result3 = 0;
            if (!request.Data.TryGetByte(out var result4) || !request.Data.TryGetByte(out var result5) || !request.Data.TryGetByte(out var result6) || !request.Data.TryGetBool(out var result7) || (result7 && !request.Data.TryGetByte(out result3)))
            {
                RequestWriter.Reset();
                RequestWriter.Put((byte)3);
                request.Reject(RequestWriter);
                return;
            }
            if (!global::GameCore.Version.CompatibilityCheck(global::GameCore.Version.Major, global::GameCore.Version.Minor, global::GameCore.Version.Revision, result4, result5, result6, result7, result3))
            {
                RequestWriter.Reset();
                RequestWriter.Put((byte)3);
                request.Reject(RequestWriter);
                return;
            }
            bool flag = request.Data.TryGetInt(out int result8);
            if (!request.Data.TryGetBytesWithLength(out var result9))
                flag = false;
            if (!flag)
            {
                RequestWriter.Reset();
                RequestWriter.Put((byte)15);
                request.Reject(RequestWriter);
                return;
            }
            if (DelayConnections)
            {
                PreauthDisableIdleMode();
                RequestWriter.Reset();
                RequestWriter.Put((byte)17);
                RequestWriter.Put(DelayTime);
                if (DelayVolume < byte.MaxValue)
                    DelayVolume++;
                if (DelayVolume < DelayVolumeThreshold)
                {
                    if (DisplayPreauthLogs)
                        ServerConsole.AddLog($"Delayed connection incoming from endpoint {request.RemoteEndPoint} by {DelayTime} seconds.");
                    request.Reject(RequestWriter);
                }
                else
                {
                    if (DisplayPreauthLogs)
                        ServerConsole.AddLog($"Force delayed connection incoming from endpoint {request.RemoteEndPoint} by {DelayTime} seconds.");
                    request.Reject(RequestWriter);
                }
                return;
            }
            if (UseChallenge)
            {
                if (result8 == 0 || result9 == null || result9.Length == 0)
                {
                    if (!CheckIpRateLimit(request))
                        return;
                    int num = 0;
                    string key = string.Empty;
                    for (byte b = 0; b < 3; b++)
                    {
                        num = RandomGenerator.GetInt32();
                        if (num == 0)
                            num = 1;
                        key = string.Concat(request.RemoteEndPoint.Address, "-", num);
                        if (!Challenges.ContainsKey(key))
                            break;
                        if (b == 2)
                        {
                            RequestWriter.Reset();
                            RequestWriter.Put((byte)4);
                            request.Reject(RequestWriter);
                            if (DisplayPreauthLogs)
                                ServerConsole.AddLog($"Failed to generate ID for challenge for incoming connection from endpoint {request.RemoteEndPoint}.");
                            return;
                        }
                    }
                    byte[] bytes = RandomGenerator.GetBytes(ChallengeInitLen + ChallengeSecretLen, secure: true);
                    ChallengeIssued++;
                    if (ChallengeIssued > IssuedThreshold)
                        SuppressIssued = true;
                    if (!SuppressIssued && DisplayPreauthLogs)
                        ServerConsole.AddLog($"Requested challenge for incoming connection from endpoint {request.RemoteEndPoint}.");
                    RequestWriter.Reset();
                    RequestWriter.Put((byte)13);
                    RequestWriter.Put((byte)ChallengeMode);
                    RequestWriter.Put(num);
                    switch (ChallengeMode)
                    {
                        case ChallengeType.MD5:
                            RequestWriter.PutBytesWithLength(bytes, 0, ChallengeInitLen);
                            RequestWriter.Put(ChallengeSecretLen);
                            RequestWriter.PutBytesWithLength(global::Cryptography.Md.Md5(bytes));
                            Challenges.Add(key, new PreauthChallengeItem(new global::System.ArraySegment<byte>(bytes, ChallengeInitLen, ChallengeSecretLen)));
                            break;
                        case ChallengeType.SHA1:
                            RequestWriter.PutBytesWithLength(bytes, 0, ChallengeInitLen);
                            RequestWriter.Put(ChallengeSecretLen);
                            RequestWriter.PutBytesWithLength(global::Cryptography.Sha.Sha1(bytes));
                            Challenges.Add(key, new PreauthChallengeItem(new global::System.ArraySegment<byte>(bytes, ChallengeInitLen, ChallengeSecretLen)));
                            break;
                        default:
                            RequestWriter.PutBytesWithLength(bytes);
                            Challenges.Add(key, new PreauthChallengeItem(new global::System.ArraySegment<byte>(bytes)));
                            break;
                    }
                    request.Reject(RequestWriter);
                    PreauthDisableIdleMode();
                    return;
                }
                string key2 = string.Concat(request.RemoteEndPoint.Address, "-", result8);
                if (!Challenges.ContainsKey(key2))
                {
                    Rejected++;
                    if (Rejected > RejectionThreshold)
                        SuppressRejections = true;
                    if (!SuppressRejections && DisplayPreauthLogs)
                        ServerConsole.AddLog($"Security challenge response of incoming connection from endpoint {request.RemoteEndPoint} has been REJECTED (invalid Challenge ID).");
                    RequestWriter.Reset();
                    RequestWriter.Put((byte)14);
                    request.Reject(RequestWriter);
                    return;
                }
                global::System.ArraySegment<byte> validResponse = Challenges[key2].ValidResponse;
                if (!global::System.Linq.Enumerable.SequenceEqual(result9, validResponse))
                {
                    Rejected++;
                    if (Rejected > RejectionThreshold)
                        SuppressRejections = true;
                    if (!SuppressRejections && DisplayPreauthLogs)
                        ServerConsole.AddLog($"Security challenge response of incoming connection from endpoint {request.RemoteEndPoint} has been REJECTED (invalid response).");
                    RequestWriter.Reset();
                    RequestWriter.Put((byte)15);
                    request.Reject(RequestWriter);
                    return;
                }
                Challenges.Remove(key2);
                PreauthDisableIdleMode();
                if (DisplayPreauthLogs)
                    ServerConsole.AddLog($"Security challenge response of incoming connection from endpoint {request.RemoteEndPoint} has been accepted.");
            }
            else if (!CheckIpRateLimit(request))
            {
                return;
            }
            int position = request.Data.Position;
            if (!CharacterClassManager.OnlineMode)
            {
                global::System.Collections.Generic.KeyValuePair<BanDetails, BanDetails> keyValuePair = BanHandler.QueryBan(null, request.RemoteEndPoint.Address.ToString());
                if (keyValuePair.Value != null)
                {
                    if (DisplayPreauthLogs)
                        ServerConsole.AddLog($"Player tried to connect from banned endpoint {request.RemoteEndPoint}.");
                    RequestWriter.Reset();
                    RequestWriter.Put((byte)6);
                    RequestWriter.Put(keyValuePair.Value.Expires);
                    RequestWriter.Put(keyValuePair.Value?.Reason ?? string.Empty);
                    request.Reject(RequestWriter);
                    ResetIdleMode();
                }
                else
                {
                    PreauthCancellationData preauthCancellationData = new PreauthCancellationData();
                    if (ProcessCancellationData(request, preauthCancellationData, new object[8]
                    {
                        null,
                        request.RemoteEndPoint.Address.ToString(),
                        0,
                        CentralAuthPreauthFlags.None,
                        null,
                        null,
                        request,
                        position
                    }))
                    {
                        request.Accept();
                        PreauthDisableIdleMode();
                    }
                }
                return;
            }
            if (!request.Data.TryGetString(out var result10) || result10 == string.Empty)
            {
                RequestWriter.Reset();
                RequestWriter.Put((byte)5);
                request.Reject(RequestWriter);
                return;
            }
            if (!request.Data.TryGetLong(out var result11) || !request.Data.TryGetByte(out var result12) || !request.Data.TryGetString(out var result13) || !request.Data.TryGetBytesWithLength(out var result14))
            {
                RequestWriter.Reset();
                RequestWriter.Put((byte)4);
                request.Reject(RequestWriter);
                return;
            }
            string result15 = null;
            string text = ((IpPassthroughEnabled && TrustedProxies.Contains(request.RemoteEndPoint.Address) && request.Data.TryGetString(out result15)) ? $"{result15} [routed via {request.RemoteEndPoint}]" : request.RemoteEndPoint.ToString());
            CentralAuthPreauthFlags centralAuthPreauthFlags = (CentralAuthPreauthFlags)result12;
            try
            {
                if (!global::Cryptography.ECDSA.VerifyBytes($"{result10};{result12};{result13};{result11}", result14, ServerConsole.PublicKey))
                {
                    Rejected++;
                    if (Rejected > RejectionThreshold)
                        SuppressRejections = true;
                    if (!SuppressRejections && DisplayPreauthLogs)
                        ServerConsole.AddLog("Player from endpoint " + text + " sent preauthentication token with invalid digital signature.");
                    RequestWriter.Reset();
                    RequestWriter.Put((byte)2);
                    request.Reject(RequestWriter);
                    ResetIdleMode();
                    return;
                }
                if (TimeBehaviour.CurrentUnixTimestamp > result11)
                {
                    Rejected++;
                    if (Rejected > RejectionThreshold)
                        SuppressRejections = true;
                    if (!SuppressRejections && DisplayPreauthLogs)
                    {
                        ServerConsole.AddLog("Player from endpoint " + text + " sent expired preauthentication token.");
                        ServerConsole.AddLog("Make sure that time and timezone set on server is correct. We recommend synchronizing the time.");
                    }
                    RequestWriter.Reset();
                    RequestWriter.Put((byte)11);
                    request.Reject(RequestWriter);
                    ResetIdleMode();
                    return;
                }
                if (UserRateLimiting)
                {
                    if (UserRateLimit.Contains(result10))
                    {
                        Rejected++;
                        if (Rejected > RejectionThreshold)
                            SuppressRejections = true;
                        if (!SuppressRejections && DisplayPreauthLogs)
                            ServerConsole.AddLog("Incoming connection from " + result10 + " (" + text + ") rejected due to exceeding the rate limit.");
                        RequestWriter.Reset();
                        RequestWriter.Put((byte)12);
                        request.Reject(RequestWriter);
                        ResetIdleMode();
                        return;
                    }
                    UserRateLimit.Add(result10);
                }
                if (!centralAuthPreauthFlags.HasFlagFast(CentralAuthPreauthFlags.IgnoreBans) || !ServerStatic.GetPermissionsHandler().IsVerified)
                {
                    global::System.Collections.Generic.KeyValuePair<BanDetails, BanDetails> keyValuePair2 = BanHandler.QueryBan(result10, result15 ?? request.RemoteEndPoint.Address.ToString());
                    if (keyValuePair2.Key != null || keyValuePair2.Value != null)
                    {
                        Rejected++;
                        if (Rejected > RejectionThreshold)
                            SuppressRejections = true;
                        if (!SuppressRejections && DisplayPreauthLogs)
                        {
                            ServerConsole.AddLog(((keyValuePair2.Key == null) ? "Player" : "Banned player") + " " + result10 + " tried to connect from " + ((keyValuePair2.Value == null) ? "" : "banned ") + " endpoint " + text + ".");
                            ServerLogs.AddLog(ServerLogs.Modules.Networking, ((keyValuePair2.Key == null) ? "Player" : "Banned player") + " " + result10 + " tried to connect from " + ((keyValuePair2.Value == null) ? "" : "banned ") + " endpoint " + text + ".", ServerLogs.ServerLogType.ConnectionUpdate);
                        }
                        RequestWriter.Reset();
                        RequestWriter.Put((byte)6);
                        RequestWriter.Put(keyValuePair2.Key?.Expires ?? keyValuePair2.Value.Expires);
                        RequestWriter.Put(keyValuePair2.Key?.Reason ?? keyValuePair2.Value?.Reason ?? string.Empty);
                        request.Reject(RequestWriter);
                        ResetIdleMode();
                        return;
                    }
                }
                if (centralAuthPreauthFlags.HasFlagFast(CentralAuthPreauthFlags.AuthRejected))
                {
                    if (DisplayPreauthLogs)
                        ServerConsole.AddLog("Player " + result10 + " (" + text + ") kicked due to auth rejection by central server.");
                    RequestWriter.Reset();
                    RequestWriter.Put((byte)20);
                    request.Reject(RequestWriter);
                    ResetIdleMode();
                    return;
                }
                if (centralAuthPreauthFlags.HasFlagFast(CentralAuthPreauthFlags.GloballyBanned) && (ServerStatic.PermissionsHandler.IsVerified || UseGlobalBans))
                {
                    if (DisplayPreauthLogs)
                        ServerConsole.AddLog("Player " + result10 + " (" + text + ") kicked due to an active global ban.");
                    RequestWriter.Reset();
                    RequestWriter.Put((byte)8);
                    request.Reject(RequestWriter);
                    ResetIdleMode();
                    return;
                }
                if ((!centralAuthPreauthFlags.HasFlagFast(CentralAuthPreauthFlags.IgnoreWhitelist) || !ServerStatic.GetPermissionsHandler().IsVerified) && !WhiteList.IsWhitelisted(result10))
                {
                    if (DisplayPreauthLogs)
                        ServerConsole.AddLog("Player " + result10 + " tried joined from endpoint " + text + ", but is not whitelisted.");
                    RequestWriter.Reset();
                    RequestWriter.Put((byte)7);
                    request.Reject(RequestWriter);
                    ResetIdleMode();
                    return;
                }
                if (Geoblocking != GeoblockingMode.None && (!centralAuthPreauthFlags.HasFlagFast(CentralAuthPreauthFlags.IgnoreGeoblock) || !ServerStatic.PermissionsHandler.BanTeamBypassGeo) && (!GeoblockIgnoreWhitelisted || !WhiteList.IsOnWhitelist(result10)) && ((Geoblocking == GeoblockingMode.Whitelist && !GeoblockingList.Contains(result13.ToUpper())) || (Geoblocking == GeoblockingMode.Blacklist && GeoblockingList.Contains(result13.ToUpper()))))
                {
                    Rejected++;
                    if (Rejected > RejectionThreshold)
                        SuppressRejections = true;
                    if (!SuppressRejections && DisplayPreauthLogs)
                        ServerConsole.AddLog("Player " + result10 + " (" + text + ") tried joined from blocked country " + result13.ToUpper() + ".");
                    RequestWriter.Reset();
                    RequestWriter.Put((byte)9);
                    request.Reject(RequestWriter);
                    ResetIdleMode();
                    return;
                }
                if (UserIdFastReload.Contains(result10))
                    UserIdFastReload.Remove(result10);
                if (global::Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorCore.Host.PeersCount < CustomNetworkManager.slots || (global::Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorCore.Host.PeersCount != global::Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorNetworkManager.singleton.maxConnections && ((centralAuthPreauthFlags.HasFlagFast(CentralAuthPreauthFlags.ReservedSlot) && ServerStatic.PermissionsHandler.BanTeamSlots) || (global::GameCore.ConfigFile.ServerConfig.GetBool("use_reserved_slots", def: true) && ReservedSlot.HasReservedSlot(result10, out var bypass) && (bypass || global::Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorCore.Host.PeersCount < CustomNetworkManager.slots + CustomNetworkManager.reservedSlots)))))
                {
                    PreauthCancellationData preauthCancellationData = new PreauthCancellationData();
                    if (!ProcessCancellationData(request, preauthCancellationData, new object[8]
                    {
                        result10,
                        request.RemoteEndPoint.Address.ToString(),
                        result11,
                        centralAuthPreauthFlags,
                        result13,
                        result14,
                        request,
                        position
                    }))
                    {
                        return;
                    }
                    if (UserIds.ContainsKey(request.RemoteEndPoint))
                        UserIds[request.RemoteEndPoint].SetUserId(result10);
                    else
                        UserIds.Add(request.RemoteEndPoint, new PreauthItem(result10));
                    global::LiteNetLib.NetPeer netPeer = request.Accept();
                    if (result15 != null)
                    {
                        if (RealIpAddresses.ContainsKey(netPeer.Id))
                            RealIpAddresses[netPeer.Id] = result15;
                        else
                            RealIpAddresses.Add(netPeer.Id, result15);
                    }
                    ServerConsole.AddLog("Player " + result10 + " preauthenticated from endpoint " + text + ".");
                    ServerLogs.AddLog(ServerLogs.Modules.Networking, result10 + " preauthenticated from endpoint " + text + ".", ServerLogs.ServerLogType.ConnectionUpdate);
                    PreauthDisableIdleMode();
                }
                else
                {
                    RequestWriter.Reset();
                    RequestWriter.Put((byte)1);
                    request.Reject(RequestWriter);
                    ResetIdleMode();
                }
            }
            catch (global::System.Exception ex)
            {
                Rejected++;
                if (Rejected > RejectionThreshold)
                    SuppressRejections = true;
                if (!SuppressRejections && DisplayPreauthLogs)
                    ServerConsole.AddLog("Player from endpoint " + text + " sent an invalid preauthentication token. " + ex.Message);
                RequestWriter.Reset();
                RequestWriter.Put((byte)2);
                request.Reject(RequestWriter);
                ResetIdleMode();
            }
        }
        catch (global::System.Exception ex2)
        {
            Rejected++;
            if (Rejected > RejectionThreshold)
                SuppressRejections = true;
            if (!SuppressRejections)
                ServerConsole.AddLog($"Player from endpoint {request.RemoteEndPoint} failed to preauthenticate: {ex2.Message}");
            RequestWriter.Reset();
            RequestWriter.Put((byte)4);
            request.Reject(RequestWriter);
        }
    }

    private static bool ProcessCancellationData(ConnectionRequest request, PreauthCancellationData pcd, object[] parameters)
    {
        if (!pcd.IsCancelled)
            return true;

        if (DisplayPreauthLogs)
            ServerConsole.AddLog($"Incoming connection from {request.RemoteEndPoint} rejected by a plugin.");

        NetDataWriter writer = pcd.GenerateWriter(out bool forced);
        if (writer == null)
            return false;

        request.Reject(writer);
        return false;
    }

    private static bool CheckIpRateLimit(ConnectionRequest request)
    {
        if (!IpRateLimiting)
            return true;

        if (IpRateLimit.Contains(request.RemoteEndPoint.Address))
        {
            Rejected++;
            if (Rejected > RejectionThreshold)
                SuppressRejections = true;
            if (!SuppressRejections)
            {
                ServerConsole.AddLog($"Incoming connection from endpoint {request.RemoteEndPoint} rejected due to exceeding the rate limit.");
                ServerLogs.AddLog(ServerLogs.Modules.Networking, $"Incoming connection from endpoint {request.RemoteEndPoint} rejected due to exceeding the rate limit.", ServerLogs.ServerLogType.RateLimit);
            }
            RequestWriter.Reset();
            RequestWriter.Put((byte)12);
            request.Reject(RequestWriter);
            return false;
        }
        IpRateLimit.Add(request.RemoteEndPoint.Address);
        return true;
    }

    protected internal override void GetConnectData(NetDataWriter writer)
    {
        writer.Put((byte)0); // ClientType.GameClient
        writer.Put(GameCore.Version.Major);
        writer.Put(GameCore.Version.Minor);
        writer.Put(GameCore.Version.Revision);
        writer.Put(GameCore.Version.BackwardCompatibility);
        // The server reads the backward-revision byte ONLY when BackwardCompatibility is true. Writing
        // it unconditionally (when the flag is false) shifts every following field by one byte and
        // desyncs the whole preauth packet — the challenge id/response are then misread and rejected.
        if (GameCore.Version.BackwardCompatibility)
            writer.Put(GameCore.Version.BackwardRevision);

        bool challengeDone = ClientChallengeState == ChallengeState.Done;
        writer.Put(challengeDone ? clientChallengeId : 0);
        writer.PutBytesWithLength(challengeDone && clientChallengeResponse != null ? clientChallengeResponse : EmptyByte);

        // Online mode: append the central-auth preauth token (userId/expiration/flags/country/signature)
        // so the server can verify identity. In offline mode — and until a token has actually been
        // obtained — the server never reads past the challenge response, so we send nothing extra.
        CentralAuthPreauthToken token = CentralAuthManager.PreauthToken;
        if (!string.IsNullOrEmpty(token.UserId))
        {
            writer.Put(token.UserId);
            writer.Put(token.Expiration);
            writer.Put(token.Flags);
            writer.Put(token.Country ?? string.Empty);
            writer.PutBytesWithLength(string.IsNullOrEmpty(token.Signature) ? EmptyByte : Convert.FromBase64String(token.Signature));
        }
    }

    protected internal override void OnConncetionRefused(DisconnectInfo disconnectinfo)
    {
        if (!disconnectinfo.AdditionalData.TryGetByte(out var result))
        {
            LastRejectionReason = RejectionReason.NotSpecified;
            LastCustomReason = string.Empty;
            LastBanExpiration = 0L;
            return;
        }

        LastRejectionReason = (RejectionReason)result;
        // Clear carry-over state so a previous ban/custom reason can't leak into this rejection.
        LastCustomReason = string.Empty;
        LastBanExpiration = 0L;

        switch (LastRejectionReason)
        {
            case RejectionReason.Banned:
                LastBanExpiration = disconnectinfo.AdditionalData.TryGetLong(out var expiry) ? expiry : 0L;
                LastCustomReason = disconnectinfo.AdditionalData.TryGetString(out var banReason) ? banReason : string.Empty;
                break;

            case RejectionReason.Custom:
                LastCustomReason = disconnectinfo.AdditionalData.TryGetString(out var custom) ? custom : string.Empty;
                break;

            case RejectionReason.Delay:
                // Server asked us to wait N seconds and reconnect. The reconnect itself is
                // scheduled by CustomNetworkManager.OnClientDisconnect.
                if (disconnectinfo.AdditionalData.TryGetByte(out var delay))
                    CustomNetworkManager.triggerReconnectTime = delay;
                break;

            case RejectionReason.Redirect:
                if (disconnectinfo.AdditionalData.TryGetUShort(out var port))
                {
                    LiteNetLib4MirrorTransport.Singleton.reconnectDelay = 600;
                    LiteNetLib4MirrorTransport.Singleton.maxConnectAttempts = 3;
                    _redirectCounter++;
                    LiteNetLib4MirrorTransport.Singleton.port = port;
                }
                else
                {
                    _redirectCounter = 0;
                    LastRejectionReason = RejectionReason.NotSpecified;
                }
                break;

            case RejectionReason.Challenge:
                // The server issued a preauth security challenge (reject code 13). We MUST parse
                // it and compute a response here — otherwise the response is never built and the
                // connection can never be accepted (UseChallenge is on by default). The actual
                // reconnect with the response happens in CustomNetworkManager.OnClientDisconnect.
                if (!ParseChallenge(disconnectinfo))
                    LastRejectionReason = RejectionReason.Error;
                break;

            case RejectionReason.InvalidChallengeKey:
            case RejectionReason.InvalidChallenge:
                // Our challenge id/response is stale (challenges are single-use and expire in
                // seconds). Drop it so the next attempt requests a fresh challenge.
                ResetClientChallenge();
                break;
        }
    }

    private static bool ParseChallenge(DisconnectInfo disconnectinfo)
    {
        if (!disconnectinfo.AdditionalData.TryGetByte(out var typeByte))
            return false;
        clientChallengeType = (ChallengeType)typeByte;

        if (!disconnectinfo.AdditionalData.TryGetInt(out var id))
            return false;
        clientChallengeId = id;

        switch (clientChallengeType)
        {
            case ChallengeType.Reply:
                // Reply mode (default): the server sent the full random payload; the valid
                // response is simply that payload echoed back. No work needed.
                if (!disconnectinfo.AdditionalData.TryGetBytesWithLength(out var challenge))
                    return false;
                clientChallenge = challenge;
                clientChallengeResponse = challenge;
                ClientChallengeState = ChallengeState.Done;
                return true;

            case ChallengeType.MD5:
            case ChallengeType.SHA1:
                // Proof-of-work mode: server sends base + secret length + hash(base+secret).
                // The background ProcessChallenge thread brute-forces the secret.
                if (disconnectinfo.AdditionalData.TryGetBytesWithLength(out var challengeBase)
                    && disconnectinfo.AdditionalData.TryGetUShort(out var secretLen)
                    && disconnectinfo.AdditionalData.TryGetBytesWithLength(out var hashed))
                {
                    clientChallengeBase = challengeBase;
                    clientChallengeSecretLen = secretLen;
                    clientChallenge = hashed;
                    clientChallengeResponse = new byte[secretLen];
                    ClientChallengeState = ChallengeState.Processing;
                    return true;
                }
                return false;

            default:
                return false;
        }
    }

    internal static void ResetRedirectCounter()
    {
        _redirectCounter = 0;
    }

    /// <summary>
    /// Clears the client-side preauth challenge state. Challenges are single-use and expire on the
    /// server after preauth_challenge_time_window seconds — replaying a stale id on the next connect
    /// gets rejected with "invalid Challenge ID". Called when the server rejects our challenge id or
    /// when a connection attempt is abandoned, so the next attempt starts a fresh handshake.
    /// </summary>
    internal static void ResetClientChallenge()
    {
        ClientChallengeState = ChallengeState.None;
        clientChallengeId = 0;
        clientChallenge = null;
        clientChallengeBase = null;
        clientChallengeResponse = null;
    }

    private static void ProcessChallenge()
    {
        // Background proof-of-work solver for the MD5/SHA1 challenge modes. The server sends the
        // challenge base in plaintext plus hash(base + secret); we must recover the secret bytes by
        // brute force so the server knows we spent CPU (anti-DDoS). The default "reply" mode never
        // reaches this thread — its response is the echoed payload, set directly in ParseChallenge.
        while (true)
        {
            Thread.Sleep(250);

            if (ClientChallengeState != ChallengeState.Processing)
                continue;

            byte[] challengeBase = clientChallengeBase;
            byte[] target = clientChallenge;
            if (challengeBase == null || target == null)
                continue;

            int baseLen = challengeBase.Length;
            int secretLen = clientChallengeSecretLen;
            byte[] buffer = new byte[baseLen + secretLen];
            Array.Copy(challengeBase, buffer, baseLen);

            bool sha1 = clientChallengeType == ChallengeType.SHA1;
            bool found = false;
            bool exhausted = false;

            // Enumerate the secret region [baseLen .. end) as a little-endian odometer, testing the
            // current value first (so the all-zero secret is also covered).
            while (!found && !exhausted)
            {
                byte[] hash = sha1 ? Cryptography.Sha.Sha1(buffer) : Cryptography.Md.Md5(buffer);
                if (Enumerable.SequenceEqual(hash, target))
                {
                    Array.Copy(buffer, baseLen, clientChallengeResponse, 0, secretLen);
                    ClientChallengeState = ChallengeState.Done;
                    found = true;
                    break;
                }

                int i = baseLen;
                while (true)
                {
                    if (i >= buffer.Length)
                    {
                        exhausted = true;
                        break;
                    }
                    if (buffer[i] == 0xFF)
                    {
                        buffer[i] = 0;
                        i++;
                    }
                    else
                    {
                        buffer[i]++;
                        break;
                    }
                }
            }

            if (!found)
            {
                // Could not solve the challenge — fall back to None so the reconnect path surfaces an
                // error instead of looping forever with a bogus response.
                ClientChallengeState = ChallengeState.None;
                Debug.LogError("Failed to solve the preauthentication challenge.");
            }
        }
    }

    private static void ResetIdleMode()
    {
        if (global::Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorCore.Host.PeersCount == 0)
            IdleMode.SetIdleMode(state: true);
    }

    public static void PreauthDisableIdleMode()
    {
        if (ServerStatic.IsDedicated && IdleMode.IdleModeActive)
        {
            IdleMode.PreauthStopwatch.Restart();
            IdleMode.SetIdleMode(state: false);
        }
    }

    public static void ReloadChallengeOptions()
    {
        UseChallenge = global::GameCore.ConfigFile.ServerConfig.GetBool("preauth_challenge", def: true);
        ChallengeInitLen = global::GameCore.ConfigFile.ServerConfig.GetUShort("preauth_challenge_base_length", 16);
        ChallengeSecretLen = global::GameCore.ConfigFile.ServerConfig.GetUShort("preauth_challenge_secret_length", 5);
        string text = global::GameCore.ConfigFile.ServerConfig.GetString("preauth_challenge_mode", "reply").ToLower();
        if (text == "md5")
        {
            ChallengeMode = ChallengeType.MD5;
        }
        else if (text == "sha1")
        {
            ChallengeMode = ChallengeType.SHA1;
        }
        else
        {
            ChallengeMode = ChallengeType.Reply;
            ChallengeSecretLen = 0;
        }
    }
}