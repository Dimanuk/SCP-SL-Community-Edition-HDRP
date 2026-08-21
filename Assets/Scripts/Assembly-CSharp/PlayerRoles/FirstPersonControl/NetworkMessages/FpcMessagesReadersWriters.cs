using System;
using Mirror;
using UnityEngine;

namespace PlayerRoles.FirstPersonControl.NetworkMessages
{
    public static class FpcMessagesReadersWriters
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            RegisterServerHandlers();
            CustomNetworkManager.OnServerStarted += RegisterServerHandlers;
            CustomNetworkManager.OnClientReady += delegate
            {
                if (NetworkClient.active)
                {
                    NetworkClient.ReplaceHandler((Action<FpcPositionMessage>)delegate { }, true);

                    NetworkClient.ReplaceHandler(delegate (FpcOverrideMessage msg)
                    {
                        msg.ProcessMessage();
                    });

                    NetworkClient.ReplaceHandler(delegate (FpcFallDamageMessage msg)
                    {
                        msg.ProcessMessage();
                    });
                }
            };
        }

        private static void RegisterServerHandlers()
        {
            NetworkServer.RegisterHandler<FpcFromClientMessage>(OnServerReceiveFpcMessage);
            NetworkServer.RegisterHandler<FpcNoclipToggleMessage>(OnServerReceiveNoclipMessage);
        }

        private static void OnServerReceiveFpcMessage(NetworkConnectionToClient conn, FpcFromClientMessage msg)
        {
            msg.ProcessMessage(conn);
        }

        private static void OnServerReceiveNoclipMessage(NetworkConnectionToClient conn, FpcNoclipToggleMessage msg)
        {
            msg.ProcessMessage(conn);
        }

        public static void WriteFpcFromClientMessage(this NetworkWriter writer, FpcFromClientMessage msg) => msg.Write(writer);
        public static FpcFromClientMessage ReadFpcFromClientMessage(this NetworkReader reader) => new FpcFromClientMessage(reader);

        public static void WriteFpcPositionMessage(this NetworkWriter writer, FpcPositionMessage msg) => msg.Write(writer);
        public static FpcPositionMessage ReadFpcPositionMessage(this NetworkReader reader) => new FpcPositionMessage(reader);

        public static void WriteFpcOverrideMessage(this NetworkWriter writer, FpcOverrideMessage msg) => msg.Write(writer);
        public static FpcOverrideMessage ReadFpcOverrideMessage(this NetworkReader reader) => new FpcOverrideMessage(reader);

        public static void WriteFpcFallDamageMessage(this NetworkWriter writer, FpcFallDamageMessage msg) => msg.Write(writer);
        public static FpcFallDamageMessage ReadFpcFallDamageMessage(this NetworkReader reader) => new FpcFallDamageMessage(reader);
    }
}