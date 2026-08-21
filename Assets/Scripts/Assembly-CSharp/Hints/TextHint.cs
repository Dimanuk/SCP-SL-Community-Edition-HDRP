using System;
using Mirror;

namespace Hints
{
    public class TextHint : FormattableHint<TextHint>
    {
        protected override Func<TextHint, float, string> FormattedTextStrategy
        {
            get
            {
                return base.Parameters.Length switch
                {
                    0 => FormattedTextStrategy0,
                    1 => FormattedTextStrategy1,
                    2 => FormattedTextStrategy2,
                    3 => FormattedTextStrategy3,
                    _ => FormattedTextStrategyOther,
                };
            }
        }

        public string Text { get; protected set; }

        private static string FormattedTextStrategy0(TextHint self, float progress)
        {
            return self.Text;
        }

        private static string FormattedTextStrategy1(TextHint self, float progress)
        {
            return string.Format(self.Text, self.Parameters[0].Formatted);
        }

        private static string FormattedTextStrategy2(TextHint self, float progress)
        {
            return string.Format(self.Text, self.Parameters[0].Formatted, self.Parameters[1].Formatted);
        }

        private static string FormattedTextStrategy3(TextHint self, float progress)
        {
            return string.Format(self.Text, self.Parameters[0].Formatted, self.Parameters[1].Formatted, self.Parameters[2].Formatted);
        }

        private static string FormattedTextStrategyOther(TextHint self, float progress)
        {
            string[] array = new string[self.Parameters.Length];
            for (int i = 0; i < self.Parameters.Length; i++)
            {
                array[i] = self.Parameters[i].Formatted;
            }
            return string.Format(self.Text, array);
        }

        public static TextHint FromNetwork(NetworkReader reader)
        {
            TextHint textHint = new TextHint();
            textHint.Deserialize(reader);
            return textHint;
        }

        private TextHint() : base(null, null, 0f)
        {
        }

        public TextHint(string text, HintParameter[] parameters = null, HintEffect[] effects = null, float durationScalar = 3f)
            : base(parameters, effects, durationScalar)
        {
            this.Text = text;
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            this.Text = reader.ReadString();
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.WriteString(this.Text);
        }
    }
}