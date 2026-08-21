namespace Security
{
	internal static class RateLimitCreator
	{
		public static readonly string[] ServerRateLimits = new string[9] { "playerInteract", "modPref", "commands", "inventory", "itemSync", "footstep", "movementSync", "cameraSync", "medicalItem" };

		private static readonly uint[] DefaultThresholds = new uint[9] { 60u, 30u, 150u, 60u, 300u, 20u, 150u, 60u, 4u };

		private static readonly uint[] DefaultWindows = new uint[9] { 5u, 3u, 3u, 5u, 1u, 5u, 1u, 5u, 3u };

		private static uint[][] _limits;

		private static int _limitsAmount;

		private static bool _init;

		private static global::Security.RateLimit _dummy;

		private static global::Security.RateLimit[] _dummyTable;

		internal static void Load()
		{
			_init = true;
			_limitsAmount = ServerRateLimits.Length;
			_limits = new uint[_limitsAmount][];
			for (ushort num = 0; num < _limitsAmount; num++)
			{
				_limits[num] = new uint[2];
				_limits[num][0] = global::GameCore.ConfigFile.ServerConfig.GetUInt("ratelimit_" + ServerRateLimits[num] + "_threshold", DefaultThresholds[num]);
				_limits[num][1] = global::GameCore.ConfigFile.ServerConfig.GetUInt("ratelimit_" + ServerRateLimits[num] + "_window", DefaultWindows[num]);
			}
			_dummy = new global::Security.DummyRateLimit();
			_dummyTable = new global::Security.RateLimit[_limitsAmount];
			for (ushort num2 = 0; num2 < _limitsAmount; num2++)
			{
				_dummyTable[num2] = _dummy;
			}
			ServerConsole.AddLog("Rate limiting loaded");
		}

		internal static global::Security.RateLimit[] CreateRateLimit(global::Mirror.NetworkConnection connection, bool dummy = false)
		{
			if (global::Mirror.NetworkServer.active && !dummy)
			{
				global::Security.RateLimit[] array = new global::Security.RateLimit[_limitsAmount];
				for (ushort num = 0; num < _limitsAmount; num++)
				{
					array[num] = new global::Security.RateLimit((int)_limits[num][0], _limits[num][1], connection);
				}
				return array;
			}
			if (!_init)
			{
				Load();
			}
			return _dummyTable;
		}
	}
}
