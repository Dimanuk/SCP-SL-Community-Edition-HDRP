using Mirror;
using System;

namespace Hints
{
    public static class ByteHintParameterFunctions
    {
        public static void Serialize(this NetworkWriter writer, ByteHintParameter value)
        {
            if (value == null) throw new NullReferenceException();
            value.Dispose();
        }

        public static ByteHintParameter Deserialize(this NetworkReader reader)
        {
            return ByteHintParameter.FromNetwork(reader);
        }
    }
}