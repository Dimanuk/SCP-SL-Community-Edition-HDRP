namespace CommandSystem
{
    public class GameConsoleCommandHandler : global::CommandSystem.CommandHandler
    {
        private GameConsoleCommandHandler()
        {
        }

        public static global::CommandSystem.GameConsoleCommandHandler Create()
        {
            global::CommandSystem.GameConsoleCommandHandler gameConsoleCommandHandler = new();
            gameConsoleCommandHandler.LoadGeneratedCommands();
            return gameConsoleCommandHandler;
        }

        public override void LoadGeneratedCommands()
        {
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
            RegisterCommand(new global::CommandSystem.Commands.RemoteAdmin.RedirectCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.AdminMeCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.ArgsCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.AuthRenewCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.AuthTokenCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.BanCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.ClearBroadcastCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.ClearCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.CmdbindCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.ColorsCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.ConfirmCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.ConnectCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.ContactCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.DebugCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.DisconnectCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.GiveCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.GlobalBanCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.GlobalTagCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.GroupsCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.HidePingCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.HideTagCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.IdCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.IpCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.ItemListCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.KeyCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.KeybindCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.KeyhashCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.LennyCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.LocalhostCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.MeasureCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.MouseSensitivityCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.PastebinCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.PlayersCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.QuitCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.RawInputCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.ReconnectCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.RefreshfixCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.ReloadTranslationsCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.ReportCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.RoleListCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.SeedCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.ShowTagCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.SkinsCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.SmoothInputCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.SrvCfgCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.StartServerCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.SteamDebugCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.SteamLobbyCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.SyncCmdCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.ToggleCreateServer());
            RegisterCommand(new global::CommandSystem.Commands.Console.PocketDimensionCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.RoundRestartCommand());
            RegisterCommand(global::CommandSystem.Commands.Console.Volume.VolumeCommand.Create());
            RegisterCommand(global::CommandSystem.Commands.Console.Prefs.PrefsCommand.Create());
            RegisterCommand(global::CommandSystem.Commands.Console.Overwatch.OverwatchCommand.Create());
            RegisterCommand(new global::CommandSystem.Commands.Console.Noclip.NoclipCommand());
            RegisterCommand(new global::CommandSystem.Commands.Console.Noclip.NoclipSpeedCommand());
            RegisterCommand(global::CommandSystem.Commands.Console.Central.CentralCommand.Create());
        }
    }
}
