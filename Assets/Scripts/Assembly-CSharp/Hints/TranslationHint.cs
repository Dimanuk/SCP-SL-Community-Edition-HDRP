using System;
using Mirror;

namespace Hints
{
    public class TranslationHint : FormattableHint<TranslationHint>
    {
        public const string TranslationFile = "GameHints";

        protected override Func<TranslationHint, float, string> FormattedTextStrategy
        {
            get
            {
                return (Parameters?.Length ?? 0) switch
                {
                    0 => FormattedTextStrategy0,
                    1 => FormattedTextStrategy1,
                    2 => FormattedTextStrategy2,
                    3 => FormattedTextStrategy3,
                    _ => FormattedTextStrategyOther,
                };
            }
        }

        protected HintTranslations Translation { get; private set; }

        private static string FormattedTextStrategy0(TranslationHint self, float progress)
        {
            return TranslationReader.GetFormatted("GameHints", (int)self.Translation, "NO_TRANSLATION");
        }

        private static string FormattedTextStrategy1(TranslationHint self, float progress)
        {
            return TranslationReader.GetFormatted("GameHints", (int)self.Translation, "NO_TRANSLATION", self.Parameters[0].Formatted);
        }

        private static string FormattedTextStrategy2(TranslationHint self, float progress)
        {
            return TranslationReader.GetFormatted("GameHints", (int)self.Translation, "NO_TRANSLATION", self.Parameters[0].Formatted, self.Parameters[1].Formatted);
        }

        private static string FormattedTextStrategy3(TranslationHint self, float progress)
        {
            return TranslationReader.GetFormatted("GameHints", (int)self.Translation, "NO_TRANSLATION", self.Parameters[0].Formatted, self.Parameters[1].Formatted, self.Parameters[2].Formatted);
        }

        private static string FormattedTextStrategyOther(TranslationHint self, float progress)
        {
            object[] args = new object[self.Parameters.Length];
            for (int i = 0; i < self.Parameters.Length; i++)
            {
                args[i] = self.Parameters[i].Formatted;
            }
            return TranslationReader.GetFormatted("GameHints", (int)self.Translation, "NO_TRANSLATION", args);
        }

        public static TranslationHint FromNetwork(NetworkReader reader)
        {
            TranslationHint translationHint = new TranslationHint();
            translationHint.Deserialize(reader);
            return translationHint;
        }

        private TranslationHint()
            : base((HintParameter[])null, (HintEffect[])null, 0f)
        {
        }

        public TranslationHint(HintTranslations translation, HintParameter[] parameters = null, HintEffect[] effects = null, float durationScalar = 3f)
            : base(parameters, effects, durationScalar)
        {
            Translation = translation;
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            Translation = (HintTranslations)reader.ReadByte();
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.WriteByte((byte)Translation);
        }
    }
}