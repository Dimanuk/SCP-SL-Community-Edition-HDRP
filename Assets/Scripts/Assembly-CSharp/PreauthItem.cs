public struct PreauthItem
{
    public string UserId { get; private set; }

    public long Added { get; private set; }

    public PreauthItem(string userId)
    {
        UserId = userId;
        Added = global::System.DateTime.Now.Ticks;
    }

    public void SetUserId(string userId)
    {
        UserId = userId;
        Added = global::System.DateTime.Now.Ticks;
    }
}
