namespace CommandSystem.Commands.RemoteAdmin
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class RedirectCommand : global::CommandSystem.ICommand, global::CommandSystem.IUsageProvider
	{
		public string Command { get; } = "redirect";

		public string[] Aliases { get; } = new string[3] { "rstop", "rexit", "rquit" };

		public string Description { get; } = "Shutdowns the server and redirects all the players to a server on another port.";

		public string[] Usage { get; } = new string[1] { "port number" };

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			if (arguments.Count != 1 || !ushort.TryParse(arguments.At(0), out var result))
			{
				response = "First argument must be a valid port number.";
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, $"{sender.LogName} redirected all players to port {result}.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
			CustomLiteNetLib4MirrorTransport.DelayConnections = true;
			IdleMode.SetIdleMode(state: false);
			IdleMode.PauseIdleMode = true;
			ServerConsole.AddOutputEntry(default(global::ServerOutput.ExitActionShutdownEntry));
			global::Utils.Networking.NetworkUtils.SendToAuthenticated(new global::RoundRestarting.RoundRestartMessage(global::RoundRestarting.RoundRestartType.RedirectRestart, 0.1f, result, reconnect: true, extendedReconnectionPeriod: false));
			global::MEC.Timing.RunCoroutine(ScheduleShutdown(), global::MEC.Segment.FixedUpdate);
			response = $"Players have been redirected to port {result}. Server will shutdown in a couple of seconds.";
			return true;
		}

		private global::System.Collections.Generic.IEnumerator<float> ScheduleShutdown()
		{
			yield return global::MEC.Timing.WaitForSeconds(5f);
			Shutdown.Quit();
		}
	}
}
