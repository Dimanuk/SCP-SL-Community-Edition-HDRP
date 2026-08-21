namespace Respawning
{
    public class ChaosInsurgencySpawnHandler : global::Respawning.ConfigBasedTeamSpawnHandler
    {
        private const float LogicerPercent = 0.2f;

        private const float ShotgunPercent = 0.3f;

        public override float EffectTime => 13.49f;

        public override void GenerateQueue(global::System.Collections.Generic.Queue<global::PlayerRoles.RoleTypeId> queueToFill, int playersToSpawn)
        {
            int num = global::UnityEngine.Mathf.FloorToInt((float)playersToSpawn * 0.2f);
            int num2 = global::UnityEngine.Mathf.FloorToInt((float)(playersToSpawn - num) * 0.3f);
            for (int i = 0; i < num; i++)
            {
                queueToFill.Enqueue(global::PlayerRoles.RoleTypeId.ChaosMarauder);
            }
            for (int j = 0; j < num2; j++)
            {
                queueToFill.Enqueue(global::PlayerRoles.RoleTypeId.ChaosRepressor);
            }
            for (int k = 0; k < playersToSpawn - num2 - num; k++)
            {
                queueToFill.Enqueue(global::PlayerRoles.RoleTypeId.ChaosRifleman);
            }
        }

        public ChaosInsurgencySpawnHandler(string maxWaveSizeConfig, int defaultMaxWaveSize, string startTokensConfig, int defaultStartTokens)
            : base(maxWaveSizeConfig, defaultMaxWaveSize, startTokensConfig, defaultStartTokens)
        {
        }
    }
}
