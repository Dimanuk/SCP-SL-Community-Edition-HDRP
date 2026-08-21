public static class RecyclablePlayerIdReaderWriter
{
    public static void WriteRecyclablePlayerId(this global::Mirror.NetworkWriter writer, RecyclablePlayerId val)
    {
        for (int num = val.Value; num >= 0; num -= 255)
        {
            writer.WriteByte((byte)global::System.Math.Min(num, 255));
        }
    }

    public static RecyclablePlayerId ReadRecyclablePlayerId(this global::Mirror.NetworkReader reader)
    {
        int num = 0;
        byte b;
        do
        {
            b = reader.ReadByte();
            num += b;
        }
        while (b == byte.MaxValue);
        return new RecyclablePlayerId(num);
    }
}
