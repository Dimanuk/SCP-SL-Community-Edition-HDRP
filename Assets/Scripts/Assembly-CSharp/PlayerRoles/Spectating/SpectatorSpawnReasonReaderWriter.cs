namespace PlayerRoles.Spectating
{
    public static class SpectatorSpawnReasonReaderWriter
    {
        public static void WriteSpawnReason(this global::Mirror.NetworkWriter writer, global::PlayerRoles.Spectating.SpectatorSpawnReason reason)
        {
            writer.WriteByte((byte)reason);
        }

        public static global::PlayerRoles.Spectating.SpectatorSpawnReason ReadSpawnReason(this global::Mirror.NetworkReader reader)
        {
            return (global::PlayerRoles.Spectating.SpectatorSpawnReason)reader.ReadByte();
        }
    }
}
