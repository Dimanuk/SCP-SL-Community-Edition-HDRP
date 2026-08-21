using System;
using System.IO;
using Mirror;

public static class OffsetSerializer
{
    public unsafe static void WriteOffset(this NetworkWriter writer, Offset value)
    {
        int size = sizeof(Offset);

        writer.WriteBytes(new byte[size], 0, size);

        int writePos = writer.Position - size;

        ArraySegment<byte> segment = writer.ToArraySegment();

        fixed (byte* ptr = &segment.Array[segment.Offset + writePos])
        {
            *(Offset*)ptr = value;
        }
    }

    public unsafe static Offset ReadOffset(this NetworkReader reader)
    {
        int size = sizeof(Offset);

        Offset result;
        ArraySegment<byte> segment = reader.ReadBytesSegment(size);

        fixed (byte* ptr = &segment.Array[segment.Offset])
        {
            result = *(Offset*)ptr;
        }

        return result;
    }
}