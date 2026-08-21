namespace CommandSystem.Commands.Shared
{
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.RemoteAdminCommandHandler))]
	[global::CommandSystem.CommandHandler(typeof(global::CommandSystem.GameConsoleCommandHandler))]
	public class BuildInfoCommand : global::CommandSystem.ICommand
	{
		public static string ModDescription;

		public string Command { get; } = "buildinfo";

		public string[] Aliases { get; }

		public string Description { get; } = "Displays information about the current build.";

		internal static string BuildInfoString => string.Format("Build info:\nGame version: {0}\nPreauth version: {1}.{2}.{3}\nBackward compatibility: {4}\nBuild timestamp: {5}\nBuild type: {6}\nAlways accept release builds: {7}\nBuild GUID: {8}\nUnity version: {9}\n\nPrivate beta: {10}\nPublic beta: {11}\nRelease candidate: {12}\nStreaming allowed: {13}\nHeadless: {14}\nModded: {15}", global::GameCore.Version.VersionString, global::GameCore.Version.Major, global::GameCore.Version.Minor, global::GameCore.Version.Revision, (!global::GameCore.Version.BackwardCompatibility || global::GameCore.Version.ExtendedVersionCheckNeeded) ? "False" : $"{(global::GameCore.Version.Major)}.{(global::GameCore.Version.Minor)}.{(global::GameCore.Version.BackwardRevision)} and newer", "2023-01-16 18:37:52Z", global::GameCore.Version.BuildType, global::GameCore.Version.AlwaysAcceptReleaseBuilds, PlatformInfo.singleton.BuildGuid, global::UnityEngine.Application.unityVersion, global::GameCore.Version.PrivateBeta, global::GameCore.Version.PublicBeta, global::GameCore.Version.ReleaseCandidate, global::GameCore.Version.StreamingAllowed, PlatformInfo.singleton.IsHeadless, CustomNetworkManager.Modded ? $"{true}\nMod Description:\n{ModDescription}" : false.ToString());

		public bool Execute(global::System.ArraySegment<string> arguments, global::CommandSystem.ICommandSender sender, out string response)
		{
			response = BuildInfoString;
			return true;
		}
	}
}
