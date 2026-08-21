using Mirror;
using System;

namespace Hints
{
    public static class AmmoHintParameterFunctions
    {
        public static void Serialize(this NetworkWriter writer, AmmoHintParameter value)
        {
            if (value == null) throw new NullReferenceException();

            value.Dispose();
        }

        public static AmmoHintParameter Deserialize(this NetworkReader reader)
        {
            return AmmoHintParameter.FromNetwork(reader);
        }
    }
}