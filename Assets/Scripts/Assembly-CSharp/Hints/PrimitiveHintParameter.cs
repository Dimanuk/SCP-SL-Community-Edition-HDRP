using System;
using Mirror;

namespace Hints
{
    public abstract class PrimitiveHintParameter<TValue> : HintParameter
    {
        private readonly Func<NetworkReader, TValue> _deserializer;
        private readonly Action<NetworkWriter, TValue> _serializer;

        private bool _stopFormatting;

        public TValue Value { get; protected set; }

        protected PrimitiveHintParameter(Func<NetworkReader, TValue> deserializer, Action<NetworkWriter, TValue> serializer)
        {
            this._deserializer = deserializer;
            this._serializer = serializer;
        }

        protected PrimitiveHintParameter(TValue value, Func<NetworkReader, TValue> deserializer, Action<NetworkWriter, TValue> serializer)
            : this(deserializer, serializer)
        {
            this.Value = value;
        }

        public override void Deserialize(NetworkReader reader)
        {
            this.Value = this._deserializer(reader);
        }

        public override void Serialize(NetworkWriter writer)
        {
            this._serializer(writer, this.Value);
        }

        protected override string UpdateState(float progress)
        {
            if (!this._stopFormatting)
            {
                return this.FormatValue(progress, out this._stopFormatting);
            }
            return null;
        }

        protected virtual string FormatValue(float progress, out bool stopFormatting)
        {
            stopFormatting = false;
            return this.Value?.ToString();
        }
    }
}