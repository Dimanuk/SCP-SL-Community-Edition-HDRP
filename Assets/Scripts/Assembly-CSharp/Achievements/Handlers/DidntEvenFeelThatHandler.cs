
using PlayerRoles;
using PlayerStatsSystem;

namespace Achievements.Handlers
{
	public class DidntEvenFeelThatHandler : AchievementHandlerBase
	{
		internal override void OnInitialize()
		{
			PlayerStats.OnAnyPlayerDamaged += AnyDamage;
		}

		private static void AnyDamage(ReferenceHub ply, DamageHandlerBase handler)
		{
			if (handler is StandardDamageHandler standardDamageHandler && ply.IsHuman())
			{
				HealthStat module = ply.playerStats.GetModule<HealthStat>();
				if (module.CurValue > 0f && module.CurValue - standardDamageHandler.AbsorbedAhpDamage <= 0f)
				{
					ServerAchieve(ply.networkIdentity.connectionToClient, AchievementName.DidntEvenFeelThat);
				}
			}
		}
	}
}
