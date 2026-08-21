namespace Discord
{
	public struct UserAchievement
	{
		public long UserId;

		public long AchievementId;

		public byte PercentComplete;

		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 64)]
		public string UnlockedAt;
	}
}
