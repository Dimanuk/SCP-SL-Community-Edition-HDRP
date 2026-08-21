namespace RemoteAdmin.Communication
{
	public class RaTeamStatus : RaClientDataRequest
	{
		public override int DataId => 8;

        protected override void GatherData()
        {
            AppendData(global::Respawning.RespawnTokensManager.GetTeamDominance(global::Respawning.SpawnableTeamType.NineTailedFox));
            AppendData(global::Respawning.RespawnTokensManager.GetTeamDominance(global::Respawning.SpawnableTeamType.ChaosInsurgency));
            AppendData((byte)global::Respawning.RespawnTokensManager.DominatingTeam);
            AppendData(global::Respawning.RespawnManager.Singleton.TimeTillRespawn);
        }
	}
}
