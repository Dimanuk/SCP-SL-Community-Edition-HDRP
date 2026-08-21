namespace InventorySystem.Items.Usables
{
    public class Scp207 : global::InventorySystem.Items.Usables.Consumable
    {
        private const int InstantHealth = 30;

        private const byte MaxColas = 4;

        protected override void OnEffectsActivated()
        {
            base.Owner.playerStats.GetModule<global::PlayerStatsSystem.StaminaStat>().CurValue = 1f;
            base.Owner.playerStats.GetModule<global::PlayerStatsSystem.HealthStat>().ServerHeal(InstantHealth);
            if (base.Owner.playerEffectsController.TryGetEffect<global::CustomPlayerEffects.Scp207>(out var playerEffect))
            {
                byte intensity = playerEffect.Intensity;
                if (intensity < MaxColas)
                {
                    base.Owner.playerEffectsController.ChangeState<global::CustomPlayerEffects.Scp207>(++intensity);
                }
            }
        }
    }
}
