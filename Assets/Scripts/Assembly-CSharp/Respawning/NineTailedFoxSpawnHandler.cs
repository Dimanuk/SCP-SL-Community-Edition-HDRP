namespace Respawning
{
    public class NineTailedFoxSpawnHandler : global::Respawning.ConfigBasedTeamSpawnHandler
    {
        public override float EffectTime => 17.95f;

        public override void GenerateQueue(global::System.Collections.Generic.Queue<global::PlayerRoles.RoleTypeId> queueToFill, int playersToSpawn)
        {
            queueToFill.Enqueue(global::PlayerRoles.RoleTypeId.NtfCaptain);
            for (int i = 0; i < 3; i++)
            {
                queueToFill.Enqueue(global::PlayerRoles.RoleTypeId.NtfSergeant);
            }
            for (int j = 0; j < playersToSpawn - 4; j++)
            {
                queueToFill.Enqueue(global::PlayerRoles.RoleTypeId.NtfPrivate);
            }
        }

        public NineTailedFoxSpawnHandler(string maxWaveSizeConfig, int defaultMaxWaveSize, string startTokensConfig, int defaultStartTokens)
            : base(maxWaveSizeConfig, defaultMaxWaveSize, startTokensConfig, defaultStartTokens)
        {
        }
    }
}
