namespace ServerOutput
{
    public struct TextOutputEntry : global::ServerOutput.IOutputEntry
    {
        public readonly string Text;

        public readonly byte Color;

        private const int offset = 5;

        private string HexColor => Color.ToString("X");

        public TextOutputEntry(string text, global::System.ConsoleColor color)
        {
            Text = text;
            Color = (byte)color;
        }

        public string GetString()
        {
            return HexColor + Text;
        }

        public int GetBytesLength()
        {
            return global::System.Text.Encoding.UTF8.GetMaxByteCount(Text.Length) + 5;
        }

        public void GetBytes(ref byte[] buffer, out int length)
        {
            length = Utf8.GetBytes(Text, buffer, 5);
            buffer[0] = Color;
            buffer[1] = (byte)((length & 0xFF000000u) >> 24);
            buffer[2] = (byte)((length & 0xFF0000) >> 16);
            buffer[3] = (byte)((length & 0xFF00) >> 8);
            buffer[4] = (byte)(length & 0xFF);
            length += 5;
        }
    }
}
