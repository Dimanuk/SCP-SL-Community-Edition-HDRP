using System.Linq;
using RemoteAdmin.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.Common;

namespace RemoteAdmin.Communication
{
    public class RaPlayerQR : IClientCommunication
    {
        public int DataId => 2;

        public void ReceiveData(string data, bool secure)
        {
            string[] array = data.Split(' ');
            if (array.Length < 2)
                return;

            if (!int.TryParse(array[0], out int flag))
                return;

            string content = string.Join(" ", array.Skip(1));

            if (flag == 1)
            {
                LargeDataPrinter.Display(content, true);
                return;
            }

            if (content.Length > 30)
                return;

            PlayerInfoQR.Display(content);
        }

        public static void Send(CommandSender sender, bool isBig, string data)
        {
            sender.RaReply(
                $"$2 {(isBig ? 1 : 0)} {data}",
                success: true,
                logToConsole: false,
                string.Empty);
        }
    }
}