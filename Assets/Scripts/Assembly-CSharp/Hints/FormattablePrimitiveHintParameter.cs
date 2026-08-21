using System;
using Mirror;

namespace Hints
{
    public abstract class FormattablePrimitiveHintParameter<TValue> : PrimitiveHintParameter<TValue>
    {
        private readonly Func<TValue, string, string> _formatter;

        public string Format { get; protected set; }

        protected FormattablePrimitiveHintParameter(Func<NetworkReader, TValue> deserializer, Action<NetworkWriter, TValue> serializer)
            : base(deserializer, serializer)
        {
        }

        protected FormattablePrimitiveHintParameter(TValue value, string format, Func<TValue, string, string> formatter, Func<NetworkReader, TValue> deserializer, Action<NetworkWriter, TValue> serializer)
            : base(value, deserializer, serializer)
        {
            this.Format = format;
            this._formatter = formatter;
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            this.Format = reader.ReadString();
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.WriteString(this.Format);
        }

        protected override string FormatValue(float progress, out bool stopFormatting)
        {
            stopFormatting = true;
            if (this._formatter != null)
            {
                return this._formatter(this.Value, this.Format);
            }
            return this.Value?.ToString();
        }
    }
}