namespace RoundRestarting
{
    public readonly struct RoundRestartMessage : global::Mirror.NetworkMessage
    {
        public readonly global::RoundRestarting.RoundRestartType Type;

        public readonly float TimeOffset;

        public readonly ushort NewPort;

        public readonly bool Reconnect;

        public readonly bool ExtendedReconnectionPeriod;

        public RoundRestartMessage(global::RoundRestarting.RoundRestartType type, float offset, ushort newport, bool reconnect, bool extendedReconnectionPeriod)
        {
            Type = type;
            TimeOffset = offset;
            NewPort = newport;
            Reconnect = reconnect;
            ExtendedReconnectionPeriod = extendedReconnectionPeriod;
        }
    }
}
