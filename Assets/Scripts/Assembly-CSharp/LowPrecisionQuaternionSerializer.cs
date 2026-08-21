using Mirror;

public static class LowPrecisionQuaternionSerializer
{
    public static void WriteLowPrecisionQuaternion(this NetworkWriter writer, LowPrecisionQuaternion value)
    {
        UnityEngine.Quaternion q = value.Value;
        writer.WriteSByte((sbyte)(q.x * 127f));
        writer.WriteSByte((sbyte)(q.y * 127f));
        writer.WriteSByte((sbyte)(q.z * 127f));
        writer.WriteSByte((sbyte)(q.w * 127f));
    }

    public static LowPrecisionQuaternion ReadLowPrecisionQuaternion(this NetworkReader reader)
    {
        float x = reader.ReadSByte() / 127f;
        float y = reader.ReadSByte() / 127f;
        float z = reader.ReadSByte() / 127f;
        float w = reader.ReadSByte() / 127f;
        return new LowPrecisionQuaternion(new UnityEngine.Quaternion(x, y, z, w));
    }
}