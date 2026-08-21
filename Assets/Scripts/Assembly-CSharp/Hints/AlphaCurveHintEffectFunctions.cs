using Mirror;

namespace Hints
{
    public static class AlphaCurveHintEffectFunctions
    {
        public static void Serialize(this NetworkWriter writer, AlphaCurveHintEffect value)
        {
            if (value == null)
            {
                writer.WriteBool(false);
                return;
            }
            writer.WriteBool(true);
            value.Serialize(writer);
        }

        public static AlphaCurveHintEffect Deserialize(this NetworkReader reader)
        {
            if (!reader.ReadBool())
                return null;

            return AlphaCurveHintEffect.FromNetwork(reader);
        }
    }
}