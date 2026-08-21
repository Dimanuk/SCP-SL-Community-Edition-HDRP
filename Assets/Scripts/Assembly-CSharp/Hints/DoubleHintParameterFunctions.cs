using Mirror;
using System;

namespace Hints
{
    public static class DoubleHintParameterFunctions
    {
        public static void Serialize(this NetworkWriter writer, DoubleHintParameter value)
        {
            if (value == null) throw new NullReferenceException();

            value.Dispose();
        }

        public static DoubleHintParameter Deserialize(this NetworkReader reader)
        {
            return DoubleHintParameter.FromNetwork(reader);
        }
    }
}