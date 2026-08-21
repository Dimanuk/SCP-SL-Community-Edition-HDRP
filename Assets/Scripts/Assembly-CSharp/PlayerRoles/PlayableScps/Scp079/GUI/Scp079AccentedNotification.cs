namespace PlayerRoles.PlayableScps.Scp079.GUI
{
    public class Scp079AccentedNotification : Scp079SimpleNotification
    {
        private const string FormatStartColor = "<color={0}>";
        private const string FormatEndColor = "</color>";

        public const char ToggleChar = '$';
        public const string AccentColor = "#00a2ff";

        public Scp079AccentedNotification(string message, string color = AccentColor, char triggerChar = ToggleChar)
            : base(ProcessText(message, color, triggerChar))
        {
        }

        private static string ProcessText(string message, string color, char triggerChar)
        {
            System.Text.StringBuilder stringBuilder = NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();

            bool flag = false;

            string startColorTag = string.Format(FormatStartColor, color);

            foreach (char c in message)
            {
                if (c != triggerChar)
                {
                    stringBuilder.Append(c);
                    continue;
                }

                stringBuilder.Append(flag ? FormatEndColor : startColorTag);

                flag = !flag;
            }

            if (flag)
            {
                stringBuilder.Append(FormatEndColor);
            }

            return NorthwoodLib.Pools.StringBuilderPool.Shared.ToStringReturn(stringBuilder);
        }
    }
}
