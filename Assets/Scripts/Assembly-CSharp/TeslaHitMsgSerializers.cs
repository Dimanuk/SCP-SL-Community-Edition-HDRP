public static class TeslaHitMsgSerializers
{
    public static void Serialize(this global::Mirror.NetworkWriter writer, TeslaHitMsg value)
    {
        global::Mirror.NetworkWriterExtensions.WriteSByte(writer, value.TeslaGateId);
    }

    public static TeslaHitMsg Deserialize(this global::Mirror.NetworkReader reader)
    {
        return new TeslaHitMsg(global::Mirror.NetworkReaderExtensions.ReadSByte(reader));
    }
}
