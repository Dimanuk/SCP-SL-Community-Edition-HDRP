namespace PlayerStatsSystem
{
    public static class DamageHandlerReaderWriter
    {
        public static void WriteDamageHandler(this global::Mirror.NetworkWriter writer, global::PlayerStatsSystem.DamageHandlerBase info)
        {
            writer.WriteByte(global::PlayerStatsSystem.DamageHandlers.IdsByTypeHash[global::Mirror.Extensions.GetStableHashCode(info.GetType().FullName)]);
            info.WriteAdditionalData(writer);
        }

        public static global::PlayerStatsSystem.DamageHandlerBase ReadDamageHandler(this global::Mirror.NetworkReader reader)
        {
            byte b = reader.ReadByte();
            if (!global::PlayerStatsSystem.DamageHandlers.ConstructorsById.TryGetValue(b, out var value))
            {
                throw new global::System.InvalidOperationException("DamageType " + b + " does not have a defined handler!");
            }
            global::PlayerStatsSystem.DamageHandlerBase damageHandlerBase = value();
            damageHandlerBase.ReadAdditionalData(reader);
            return damageHandlerBase;
        }
    }
}
