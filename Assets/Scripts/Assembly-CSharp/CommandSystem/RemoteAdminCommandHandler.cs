namespace CommandSystem
{
    public class RemoteAdminCommandHandler : global::CommandSystem.CommandHandler
    {
        private RemoteAdminCommandHandler()
        {
        }

        public static global::CommandSystem.RemoteAdminCommandHandler Create()
        {
            global::CommandSystem.RemoteAdminCommandHandler remoteAdminCommandHandler = new global::CommandSystem.RemoteAdminCommandHandler();
            remoteAdminCommandHandler.LoadGeneratedCommands();
            return remoteAdminCommandHandler;
        }

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new global::CommandSystem.Commands.CassieClear());
            RegisterCommand(new global::CommandSystem.Commands.Shared.BuildInfoCommand());
            RegisterCommand(global::CommandSystem.Commands.Shared.ConfigCommand.Create());
            RegisterCommand(new global::CommandSystem.Commands.Shared.HelloCommand());
            RegisterCommand(new global::CommandSystem.Commands.Shared.HelpCommand(this));
            RegisterCommand(new global::CommandSystem.Commands.Shared.RefreshCommandsCommand(this));
            RegisterCommand(new global::CommandSystem.Commands.Shared.RestartNextRoundCommand());
            RegisterCommand(new global::CommandSystem.Commands.Shared.RidListCommand());
            RegisterCommand(new global::CommandSystem.Commands.Shared.SoftRestartCommand());
            RegisterCommand(new global::CommandSystem.Commands.Shared.StopNextRoundCommand());
            RegisterCommand(new global::CommandSystem.Commands.Shared.UptimeRoundsCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.BanCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.GbanKickCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.OfflineBanCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.UnbanCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.BringCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.BypassCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.CassieCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.CassieSilentCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.CassieWordsCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ChangeColorCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ChangeCustomPlayerInfoCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ChangeNameCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ClearEffectsCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ContactCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.DestroyToyCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.DisarmCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.DisplayNameCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.EffectCommand());
            RegisterCommand(global::CommandSystem.Commands.RemoteAdmin.ElevatorCommand.Create());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ExternalLookupCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ForceAttachmentsCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ForceRoleCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ForceStartCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.FriendlyFireDetectorCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.GiveLoadoutCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.GlobalTagCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.GodCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.GotoCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.GunDebugCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.HealCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.HideTagCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.AddCandyCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.PlayerInventoryCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.StripCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.KillCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.LobbyLockCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.IntercomResetCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.IntercomSpeakCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.IntercomTimeoutCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.NoclipCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.OverchargeCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.OverwatchCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.PermCommand());
            RegisterCommand(global::CommandSystem.Commands.RemoteAdmin.PermissionsManagementCommand.Create());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.PingCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.PocketDimensionDebug());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.RAConfigCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.RconCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ReleaseCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ReloadConfigCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.RoomTPCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.RoundLockCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.RoundTimeCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.AddExperienceCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.SetExperienceCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.SetLevelCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.SetGroupCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.SetHealthCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.ShowTagCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.SpawnToyCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.StareCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.StateCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.VersionCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.WikiCommand());
            RegisterCommand(global::CommandSystem.Commands.RemoteAdmin.Warhead.WarheadCommand.Create());
            RegisterCommand(global::CommandSystem.Commands.RemoteAdmin.Tickets.TokensCommand.Create());
            RegisterCommand(global::CommandSystem.Commands.RemoteAdmin.ServerEvent.ServerEventCommand.Create());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.MutingAndIntercom.IntercomMuteCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.MutingAndIntercom.IntercomTextCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.MutingAndIntercom.IntercomUnmuteCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.MutingAndIntercom.MuteCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.MutingAndIntercom.UnmuteCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Inventory.GiveCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Inventory.RemoveItemCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Doors.CloseDoorCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Doors.DestroyDoorCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Doors.DoorTPCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Doors.DoorsListCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Doors.LockDoorCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Doors.LockdownCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Doors.OpenDoorCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Doors.UnlockDoorCommand());
            RegisterCommand(global::CommandSystem.Commands.RemoteAdmin.Decontamination.DecontaminationCommand.Create());
            RegisterCommand(global::CommandSystem.Commands.RemoteAdmin.Cleanup.CleanupCommand.Create());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Broadcasts.BroadcastCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Broadcasts.ClearBroadcastCommand());
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.Broadcasts.PlayerBroadcastCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.PocketDimensionCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.RoundRestartCommand());
        }
    }
}
