namespace RelativePositioning
{
    public static class RelativePositionSerialization
    {
        public static void WriteRelativePosition(this global::Mirror.NetworkWriter writer, global::RelativePositioning.RelativePosition msg)
        {
            msg.Write(writer);
        }

        public static global::RelativePositioning.RelativePosition ReadRelativePosition(this global::Mirror.NetworkReader reader)
        {
            return new global::RelativePositioning.RelativePosition(reader);
        }
    }
}
