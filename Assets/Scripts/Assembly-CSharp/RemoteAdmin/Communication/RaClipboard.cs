using System;
using System.Linq;
using RemoteAdmin.Interfaces;

namespace RemoteAdmin.Communication
{
    public class RaClipboard : IClientCommunication
    {
        public enum RaClipBoardType
        {
            Ip = 0,
            UserId = 1,
            PlayerId = 2
        }

        public static string UserIds { get; set; } = string.Empty;
        public static string PlayerIps { get; set; } = string.Empty;
        public static string PlayerIds { get; set; } = string.Empty;

        public int DataId => 6;

        public static void Reset()
        {
            UserIds = string.Empty;
            PlayerIps = string.Empty;
            PlayerIds = string.Empty;
        }

        public void ReceiveData(string data, bool secure = true)
        {
            string[] parts = data.Split(' ');
            if (parts.Length < 2)
                return;

            if (!int.TryParse(parts[0], out int type))
                return;

            string content = string.Join(" ", parts.Skip(1));

            switch ((RaClipBoardType)type)
            {
                case RaClipBoardType.Ip:
                    PlayerIps = content;
                    break;

                case RaClipBoardType.UserId:
                    UserIds = content;
                    break;

                case RaClipBoardType.PlayerId:
                    PlayerIds = content;
                    break;
            }
        }

        public static void Send(CommandSender sender, RaClipBoardType type, string data)
        {
            sender.RaReply($"$6 {(int)type} {data}", success: true, logToConsole: false, string.Empty);
        }
    }
}