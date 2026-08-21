namespace InventorySystem.Items.Usables
{
    public class Scp500 : global::InventorySystem.Items.Usables.Consumable
    {
        [global::UnityEngine.SerializeField]
        private global::UnityEngine.AnimationCurve _healProgress;

        private const int InstantHealth = 100;

        private const float TotalRegenerationTime = 10f;

        private const int TotalHpToRegenerate = 100;

        private const float AchievementMaxHp = 20f;

        protected override void OnEffectsActivated()
        {
            global::PlayerStatsSystem.HealthStat module = base.Owner.playerStats.GetModule<global::PlayerStatsSystem.HealthStat>();
            if (module.CurValue < 20f)
            {
                global::Achievements.AchievementHandlerBase.ServerAchieve(base.Owner.networkIdentity.connectionToClient, global::Achievements.AchievementName.CrisisAverted);
            }
            module.ServerHeal(100f);
            ServerAddRegeneration(_healProgress, 0.1f, 100f);
            base.Owner.playerEffectsController.UseMedicalItem(this);
        }
    }
}
