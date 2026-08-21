using System;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using LiteNetLib4Mirror.Open.Nat;
using UnityEngine;

namespace Mirror.LiteNetLib4Mirror
{
    public class LiteNetLib4MirrorTransport : Transport, ISegmentTransport
    {
        public static LiteNetLib4MirrorTransport Singleton;

#if UNITY_EDITOR
        [Header("Connection settings")]
#endif
        public string clientAddress = "127.0.0.1";
#if UNITY_EDITOR
        [Rename("Server IPv4 Bind Address")]
#endif
        public string serverIPv4BindAddress = "0.0.0.0";
#if !DISABLE_IPV6
#if UNITY_EDITOR
        [Rename("Server IPv6 Bind Address")]
#endif
        public string serverIPv6BindAddress = "::";
#endif
        public ushort port = 7777;
#if UNITY_EDITOR
        [Rename("Use UPnP")]
#endif
        public bool useUpnP = true;
        public ushort maxConnections = 20;
#if !DISABLE_IPV6
#if UNITY_EDITOR
        [Rename("IPv6 Enabled")]
#endif
        public bool ipv6Enabled = true;
#endif

#if UNITY_EDITOR
        [ArrayRename("Channel")]
#endif
        public DeliveryMethod[] channels =
        {
            DeliveryMethod.ReliableOrdered,
            DeliveryMethod.Unreliable,
            DeliveryMethod.Sequenced,
            DeliveryMethod.ReliableSequenced,
            DeliveryMethod.ReliableUnordered
        };

#if UNITY_EDITOR
        [Header("Connection additional auth code (optional)")]
#endif
        public string authCode;

        public int updateTime = 15;
        public int pingInterval = 1000;
        public int disconnectTimeout = 5000;
        public int reconnectDelay = 500;
        public int maxConnectAttempts = 10;

        public bool simulatePacketLoss;
        public int simulationPacketLossChance = 10;
        public bool simulateLatency;
        public int simulationMinLatency = 30;
        public int simulationMaxLatency = 100;

        public UnityEventError onClientSocketError;
        public UnityEventIntError onServerSocketError;

        internal static bool Polling;
        private static readonly NetDataWriter ConnectWriter = new NetDataWriter();

        #region Overridable methods
        protected internal virtual void GetConnectData(NetDataWriter writer)
        {
            writer.Put(GetConnectKey());
        }

        protected internal virtual void ProcessConnectionRequest(ConnectionRequest request)
        {
            if (LiteNetLib4MirrorCore.Host.PeersCount >= maxConnections)
            {
                request.Reject();
            }
            else if (request.AcceptIfKey(LiteNetLib4MirrorServer.Code) == null)
            {
                Debug.LogWarning("Client tried to join with an invalid auth code! Current code:" + LiteNetLib4MirrorServer.Code);
            }
        }

        protected internal virtual void OnConncetionRefused(DisconnectInfo disconnectinfo) { }
        #endregion

        internal void InitializeTransport()
        {
            if (Singleton == null)
            {
                Singleton = this;
                LiteNetLib4MirrorCore.State = LiteNetLib4MirrorCore.States.Idle;
            }
        }

        private static string GetConnectKey()
        {
            return LiteNetLib4MirrorUtils.ToBase64(Application.productName + Application.companyName + Application.unityVersion + LiteNetLib4MirrorCore.TransportVersion + Singleton.authCode);
        }

        private void Awake() => InitializeTransport();

        private void LateUpdate()
        {
            if (Polling) LiteNetLib4MirrorCore.Host.PollEvents();
        }

        private void OnDestroy()
        {
            LiteNetLib4MirrorCore.StopTransport();
            if (LiteNetLib4MirrorUtils.LastForwardedPort != 0)
            {
                NatDiscoverer.ReleaseAll();
                LiteNetLib4MirrorUtils.LastForwardedPort = 0;
            }
        }

        public override bool Available() => Application.platform != RuntimePlatform.WebGLPlayer;

        public override bool ClientConnected() => LiteNetLib4MirrorClient.IsConnected();

        public override void ClientConnect(string address)
        {
            clientAddress = address;
            ConnectWriter.Reset();
            GetConnectData(ConnectWriter);
            LiteNetLib4MirrorClient.ConnectClient(ConnectWriter);
        }

        public override void ClientSend(ArraySegment<byte> segment, int channelId = Channels.Reliable)
        {
            byte channel = (byte)(channelId < channels.Length ? channelId : 0);
            LiteNetLib4MirrorClient.Send(channels[0], segment.Array, segment.Offset, segment.Count, channel);
        }

        bool ISegmentTransport.ClientSend(int channelId, ArraySegment<byte> data)
        {
            byte channel = (byte)(channelId < channels.Length ? channelId : 0);
            return LiteNetLib4MirrorClient.Send(channels[0], data.Array, data.Offset, data.Count, channel);
        }

        public override void ClientDisconnect()
        {
            if (LiteNetLib4MirrorServer.IsActive())
                return;

            bool wasActive = LiteNetLib4MirrorClient.IsConnected();
            LiteNetLib4MirrorCore.StopTransport();
            if (wasActive)
                OnClientDisconnected?.Invoke();
        }

        public override bool ServerActive() => LiteNetLib4MirrorServer.IsActive();

        public override void ServerStart() => LiteNetLib4MirrorServer.StartServer(GetConnectKey());

        public override Uri ServerUri()
        {
            UriBuilder builder = new UriBuilder();
            builder.Scheme = "udp";
            builder.Host = serverIPv4BindAddress == "0.0.0.0" ? "localhost" : serverIPv4BindAddress;
            builder.Port = port;
            return builder.Uri;
        }

        public void ServerSend(List<int> connectionIds, int channelId, ArraySegment<byte> data)
        {
            byte channel = (byte)(channelId < channels.Length ? channelId : 0);
            foreach (int id in connectionIds)
            {
                LiteNetLib4MirrorServer.Send(id, channels[0], data.Array, data.Offset, data.Count, channel);
            }
        }

        public override void ServerSend(int connectionId, ArraySegment<byte> segment, int channelId = Channels.Reliable)
        {
            byte channel = (byte)(channelId < channels.Length ? channelId : 0);
            LiteNetLib4MirrorServer.Send(connectionId, channels[0], segment.Array, segment.Offset, segment.Count, channel);
        }

        bool ISegmentTransport.ServerSend(int connectionId, ArraySegment<byte> data, int channelId)
        {
            byte channel = (byte)(channelId < channels.Length ? channelId : 0);
            return LiteNetLib4MirrorServer.Send(connectionId, channels[0], data.Array, data.Offset, data.Count, channel);
        }

        public override void ServerDisconnect(int connectionId)
        {
            if (connectionId != 0) LiteNetLib4MirrorServer.Disconnect(connectionId);
        }

        public override void ServerStop() => LiteNetLib4MirrorCore.StopTransport();

        public override string ServerGetClientAddress(int connectionId) => LiteNetLib4MirrorServer.GetClientAddress(connectionId);

        public override void Shutdown() => LiteNetLib4MirrorCore.StopTransport();

        public override int GetMaxPacketSize(int channelId = Channels.Reliable)
        {
            int idx = channelId < channels.Length ? channelId : 0;
            return LiteNetLib4MirrorCore.GetMaxPacketSize(channels[idx]);
        }

        public override string ToString() => LiteNetLib4MirrorCore.GetState();
    }
}