using System;
using UnityEngine;

namespace GameCore
{
    internal class ConsoleCommandSender : CommandSender
    {
        public override string SenderId => "GAME CONSOLE";
        public override string Nickname => "GAME CONSOLE";
        public override ulong Permissions => ulong.MaxValue;
        public override byte KickPower => 255;
        public override bool FullPermissions => true;

        public override void Print(string text)
        {
            Console.AddLog(text, Color.white);
        }

        public override void Print(string text, ConsoleColor c)
        {
            Color rgbColor = ServerConsole.ConsoleColorToColor(c);
            Console.AddLog(text, rgbColor);
        }

        public override void Respond(string message, bool success = true)
        {
            Color responseColor = success ? Color.green : Color.red;
            Console.AddLog(message, responseColor);
        }

        public override void RaReply(string text, bool success, bool logToConsole, string overrideDisplay)
        {
            string formatted = "[RA Reply] " + text;
            Console.AddLog(formatted, success ? Color.green : Color.red);
        }
    }
}