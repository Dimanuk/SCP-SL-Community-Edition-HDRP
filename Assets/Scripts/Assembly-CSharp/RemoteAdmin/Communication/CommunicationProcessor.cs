using System.Collections.Generic;
using RemoteAdmin.Interfaces;

namespace RemoteAdmin.Communication
{
    public class CommunicationProcessor
    {
        public static readonly Dictionary<int, IClientCommunication> ClientCommunication = new()
        {
                { 0, new RaPlayerList() },
                { 1, new RaPlayer() },
                { 2, new RaPlayerQR() },
                { 5, new RaGlobalBan() },
                { 6, new RaClipboard() },
                { 7, new RaServerStatus() },
                { 8, new RaTeamStatus() }
        };

        public static readonly Dictionary<int, IServerCommunication> ServerCommunication = new()
        {
                { 0, new RaPlayerList() },
                { 1, new RaPlayer() },
                { 3, new RaPlayerAuth() },
                { 5, new RaGlobalBan() },
                { 7, new RaServerStatus() },
                { 8, new RaTeamStatus() }
        };


        public static T RequestClientChannel<T>() where T : IClientCommunication
        {
            foreach (IClientCommunication value in ClientCommunication.Values)
            {
                if (value is T result)
                    return result;
            }
            return default;
        }

        public static T RequestServerChannel<T>() where T : IServerCommunication
        {
            foreach (IServerCommunication value in ServerCommunication.Values)
            {
                if (value is T result)
                    return result;
            }
            return default;
        }
    }
}