internal class FriendlyFireDetector
{
    protected bool _triggered;

    private readonly ReferenceHub _hub;

    internal uint Kills { get; private set; }

    internal float Damage { get; private set; }

    internal FriendlyFireDetector(ReferenceHub hub)
    {
        _hub = hub;
    }

    public virtual bool RegisterDamage(float damage)
    {
        Damage += damage;
        return false;
    }

    public virtual bool RegisterKill()
    {
        Kills++;
        return false;
    }

    public virtual void Reset()
    {
        Kills = 0u;
        Damage = 0f;
    }

    protected void TakeAction(ref FriendlyFireAction action, string detector, ref long banDuration, ref string banReason, ref string killReason, ref string adminchat, ref string broadcast, ref bool webhook)
    {
        _triggered = true;
        if (!string.IsNullOrWhiteSpace(adminchat) && FriendlyFireConfig.AdminChatTime > 0)
        {
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.serverRoles.AdminChatPerms && allHub.Mode == ClientInstanceMode.ReadyClient)
                {
                    allHub.queryProcessor.TargetReply(allHub.queryProcessor.connectionToClient, string.Format("@!{0} {1}", FriendlyFireConfig.AdminChatTime, adminchat.Replace("%nick", _hub.nicknameSync.MyNick)), isSuccess: true, logInConsole: false, string.Empty);
                }
            }
        }
        if (!string.IsNullOrWhiteSpace(broadcast) && FriendlyFireConfig.BroadcastTime > 0)
        {
            Broadcast.Singleton.RpcAddElement(broadcast.Replace("%nick", _hub.nicknameSync.MyNick), FriendlyFireConfig.BroadcastTime, Broadcast.BroadcastFlags.Normal);
        }
        if (webhook)
        {
            global::System.Threading.Thread thread = new global::System.Threading.Thread((global::System.Threading.ThreadStart)delegate
            {
                CheaterReport.SubmitReport("Friendly Fire Detector", _hub.characterClassManager.UserId, "Friendly fire has been detected. Detector: " + detector + ".", _hub.netId, "Friendly Fire Detector", _hub.nicknameSync.MyNick, friendlyFire: true);
            });
            thread.Priority = global::System.Threading.ThreadPriority.BelowNormal;
            thread.Name = "TK Reporter";
            thread.IsBackground = true;
            thread.Start();
        }
        switch (action)
        {
            case FriendlyFireAction.Kill:
                ServerLogs.AddLog(ServerLogs.Modules.Detector, _hub.LoggedNameFromRefHub() + " playing as " + _hub.roleManager.CurrentRole.RoleName + " has been automatically killed for teamkilling. Detector: " + detector + ".", ServerLogs.ServerLogType.Teamkill);
                _hub.playerStats.DealDamage(new global::PlayerStatsSystem.UniversalDamageHandler(-1f, global::PlayerStatsSystem.DeathTranslations.FriendlyFireDetector));
                break;
            case FriendlyFireAction.Kick:
                ServerLogs.AddLog(ServerLogs.Modules.Detector, _hub.LoggedNameFromRefHub() + " playing as " + _hub.roleManager.CurrentRole.RoleName + " has been automatically kicked for teamkilling. Detector: " + detector + ".", ServerLogs.ServerLogType.Teamkill);
                BanPlayer.KickUser(_hub, banReason);
                break;
            case FriendlyFireAction.Ban:
                ServerLogs.AddLog(ServerLogs.Modules.Detector, $"{_hub.LoggedNameFromRefHub()} playing as {_hub.roleManager.CurrentRole.RoleName} has been automatically banned for teamkilling. Detector: {detector}. Duration: {banDuration} seconds.", ServerLogs.ServerLogType.Teamkill);
                BanPlayer.BanUser(_hub, banReason, banDuration);
                break;
        }
    }
}
