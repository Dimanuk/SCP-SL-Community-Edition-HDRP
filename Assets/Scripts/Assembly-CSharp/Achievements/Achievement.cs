using GameCore;

namespace Achievements
{
    public readonly struct Achievement
    {
        private readonly string _steamName;
        private readonly string _steamProgress;
        private readonly long _discordId;
        private readonly int _maxValue;
        public readonly bool ActivatedByServer;

        public Achievement(string steamName, long discordId, bool byServer = false)
        {
            _steamName = steamName;
            _discordId = discordId;
            _steamProgress = string.Empty;
            _maxValue = 0;
            ActivatedByServer = byServer;
        }

        public Achievement(string steamName, string steamParameter, long discordId, int maxValue, bool byServer = false)
        {
            _steamName = steamName;
            _steamProgress = steamParameter;
            _discordId = discordId;
            _maxValue = maxValue;
            ActivatedByServer = byServer;
        }

        public void Achieve()
        {
              switch (CentralAuthManager.Platform)
            {
                case DistributionPlatform.Steam:
                    SetAchievementSteam(true);
                    break;
                case DistributionPlatform.Discord:
                    SetAchievementDiscord(true);
                    break;
            }
        }

        public void AddProgress(int amt = 1)
        {
             switch (CentralAuthManager.Platform)
            {
                case DistributionPlatform.Steam:
                    AddProgressSteam(amt);
                    break;
                case DistributionPlatform.Discord:
                    AddProgressDiscord(amt);
                    break;
            }
        }

        public void Reset()
        {
            switch (CentralAuthManager.Platform)
            {
                case DistributionPlatform.Steam:
                    SetAchievementSteam(false);
                    break;
                case DistributionPlatform.Discord:
                    SetAchievementDiscord(false);
                    break;
            }
        }

        private void SetAchievementDiscord(bool state)
        {
            if (_discordId == 0L || CentralAuthManager.Discord == null)
                return;
            try
            {
                CentralAuthManager.Discord.GetAchievementManager()
                    .SetUserAchievement(_discordId, state ? (byte)100 : (byte)0, delegate { });
            }
            catch
            {
                // Discord SDK not ready — achievements are best-effort.
            }
        }

        private void SetAchievementSteam(bool state)
        {
            if (string.IsNullOrEmpty(_steamName))
                return;
            if (state)
                SteamManager.SetAchievement(_steamName);
            else
                SteamManager.ResetAchievement(_steamName);
        }

        private void AddProgressDiscord(int progress)
        {
            
        }

        private void AddProgressSteam(int progress)
        {
              if (string.IsNullOrEmpty(_steamProgress))
                return;
            int total = SteamManager.GetStat(_steamProgress) + progress;
            SteamManager.SetStat(_steamProgress, total);
            if (_maxValue > 0 && total >= _maxValue)
                SetAchievementSteam(true);
        }
    }
}