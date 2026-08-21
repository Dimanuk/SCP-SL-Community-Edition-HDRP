internal class UserPrint : CommandSender
{
    private readonly QueryUser _qu;

    public override string SenderId => "Query";

    public override string Nickname => "Query";

    public override ulong Permissions => _qu.Permissions;

    public override byte KickPower => _qu.KickPower;

    public override bool FullPermissions => false;

    public UserPrint(QueryUser usr)
    {
        _qu = usr;
    }

    public override void Print(string text)
    {
        _qu.Send(text);
    }

    public override void RaReply(string text, bool success, bool logToConsole, string overrideDisplay)
    {
        _qu.Send(JsonSerialize.ToJson(new QueryRaReply(text, success, logToConsole, overrideDisplay)));
    }
}
