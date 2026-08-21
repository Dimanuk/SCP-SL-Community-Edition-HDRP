using Mirror;
using UnityEngine;

public static class BreakableWindowStatusSerializer
{
    public static void WriteBreakableWindowStatus(this NetworkWriter writer, BreakableWindow.BreakableWindowStatus value)
    {
        writer.WriteVector3(value.position);
        writer.WriteQuaternion(value.rotation);
        writer.WriteBool(value.broken);
    }

    public static BreakableWindow.BreakableWindowStatus ReadBreakableWindowStatus(this NetworkReader reader)
    {
        return new BreakableWindow.BreakableWindowStatus
        {
            position = reader.ReadVector3(),
            rotation = reader.ReadQuaternion(),
            broken   = reader.ReadBool()
        };
    }
}