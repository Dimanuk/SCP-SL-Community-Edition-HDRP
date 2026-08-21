using LiteNetLib;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using System;
using UnityEngine;

public class PlayerIpOverride : NetworkBehaviour
{
    private void Start()
    {
        if (isLocalPlayer)
            return;

        if (connectionToClient == null)
            return;

        try
        {
            int connectionId = connectionToClient.connectionId;

            NetPeer[] peers = LiteNetLib4MirrorServer.Peers;
            if (peers == null || connectionId >= peers.Length)
                return;

            NetPeer peer = peers[connectionId];
            if (peer == null)
                return;

            int peerId = peer.Id;

            var realIpAddresses = CustomLiteNetLib4MirrorTransport.RealIpAddresses;
            if (realIpAddresses == null)
                return;

            if (realIpAddresses.TryGetValue(peerId, out string realIp))
            {
                connectionToClient.address = realIp;
            }
            else
            {
                string error = $"Error during IP passthrough processing: {realIp}";
                ServerConsole.AddLog(error, ConsoleColor.Red);
            }
        }
        catch (Exception ex)
        {
            string error = $"Error during IP passthrough processing: {ex.Message}";
            ServerConsole.AddLog(error, ConsoleColor.Red);
        }
    }
}