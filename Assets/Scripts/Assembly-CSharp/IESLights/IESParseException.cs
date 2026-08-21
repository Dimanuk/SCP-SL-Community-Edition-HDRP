namespace IESLights
{
    [global::System.Serializable]
    public class IESParseException : global::System.Exception
    {
        public IESParseException()
        {
        }

        public IESParseException(string message)
            : base(message)
        {
        }

        public IESParseException(string message, global::System.Exception inner)
            : base(message, inner)
        {
        }

        protected IESParseException(global::System.Runtime.Serialization.SerializationInfo info, global::System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
