using Mirror;
using System;

namespace Hints
{
    public static class FloatHintParameterFunctions
    {
        public static void Serialize(this NetworkWriter writer, FloatHintParameter value)
        {
            if (value == null) throw new NullReferenceException();

            value.Serialize(writer);
        }

        public static FloatHintParameter Deserialize(this NetworkReader reader)
        {
            return FloatHintParameter.FromNetwork(reader);
        }
    }
}