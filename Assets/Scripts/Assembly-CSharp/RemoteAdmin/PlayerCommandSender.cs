namespace RemoteAdmin
{
    public class PlayerCommandSender : CommandSender
    {
        public readonly ReferenceHub ReferenceHub;

        public override string SenderId => ReferenceHub.characterClassManager.UserId;

        public int PlayerId => ReferenceHub.PlayerId;

        public override string Nickname => ReferenceHub.nicknameSync.MyNick;

        public override ulong Permissions => ServerRoles.Permissions;

        public override byte KickPower
        {
            get
            {
                if (!ServerRoles.RaEverywhere)
                {
                    return ServerRoles.KickPower;
                }
                return byte.MaxValue;
            }
        }

        public override bool FullPermissions => false;

        public override string LogName => Nickname + " (" + ReferenceHub.characterClassManager.UserId + ")";

        public ServerRoles ServerRoles => ReferenceHub.serverRoles;

        public PlayerCommandSender(ReferenceHub hub)
        {
            ReferenceHub = hub;
        }

        public override void RaReply(string text, bool success, bool logToConsole, string overrideDisplay)
        {
            ReferenceHub.queryProcessor.TargetReply(ReferenceHub.connectionToClient, text, success, logToConsole, overrideDisplay);
        }

        public override void Print(string text)
        {
            ReferenceHub.queryProcessor.TargetReply(ReferenceHub.connectionToClient, text, isSuccess: true, logInConsole: true, "");
        }
    }
}
