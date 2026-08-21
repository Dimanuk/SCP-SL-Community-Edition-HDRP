using System;
using GameCore;

namespace Respawning
{
    public abstract class ConfigBasedTeamSpawnHandler : SpawnableTeamHandlerBase
    {
        private readonly string _maxWaveSizeConfig;
        private readonly string _startTokensConfig;
        private readonly int _defaultMaxWaveSize;
        private readonly int _defaultStartTokens;

        private int _maxWaveSize;
        private int _startTokens;

        public override int MaxWaveSize => _maxWaveSize;
        public override int StartTokens => _startTokens;

        public ConfigBasedTeamSpawnHandler(
            string maxWaveSizeConfig,
            int defaultMaxWaveSize,
            string startTokensConfig,
            int defaultStartTokens)
        {
            _maxWaveSizeConfig = maxWaveSizeConfig;
            _defaultMaxWaveSize = defaultMaxWaveSize;
            _startTokensConfig = startTokensConfig;
            _defaultStartTokens = defaultStartTokens;

            RefreshConfigs();

            ConfigFile.OnConfigReloaded = (Action)Delegate.Combine(
                ConfigFile.OnConfigReloaded,
                new Action(RefreshConfigs));
        }

        private void RefreshConfigs()
        {
            _maxWaveSize = ConfigFile.ServerConfig.GetInt(_maxWaveSizeConfig, _defaultMaxWaveSize);
            _startTokens = ConfigFile.ServerConfig.GetInt(_startTokensConfig, _defaultStartTokens);
        }
    }
}