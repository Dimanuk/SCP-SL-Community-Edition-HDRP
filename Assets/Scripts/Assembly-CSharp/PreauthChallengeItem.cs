public struct PreauthChallengeItem
{
    public long Added { get; private set; }

    public global::System.ArraySegment<byte> ValidResponse { get; private set; }

    public PreauthChallengeItem(global::System.ArraySegment<byte> response)
    {
        ValidResponse = response;
        Added = global::System.DateTime.Now.Ticks;
    }
}
