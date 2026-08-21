using Mirror;

public struct TeslaHitMsg : NetworkMessage
{
    public readonly sbyte TeslaGateId;

    public TeslaHitMsg(sbyte teslaGateId)
    {
        TeslaGateId = teslaGateId;
    }
}
