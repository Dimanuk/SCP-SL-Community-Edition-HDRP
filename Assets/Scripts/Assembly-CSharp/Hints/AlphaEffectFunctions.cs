using Mirror;
using System;

namespace Hints
{
    public static class AlphaEffectFunctions
    {
        public static void Serialize(this NetworkWriter writer, AlphaEffect value)
        {
            if (value == null)
            {
                throw new NullReferenceException();
            }

            value.Dispose();
        }

        public static AlphaEffect Deserialize(this NetworkReader reader)
        {
            return AlphaEffect.FromNetwork(reader);
        }
    }
}