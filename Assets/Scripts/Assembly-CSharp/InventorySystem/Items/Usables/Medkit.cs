namespace InventorySystem.Items.Usables
{
    public class Medkit : global::InventorySystem.Items.Usables.Consumable
    {
        private const int HpToHeal = 65;

        protected override void OnEffectsActivated()
        {
            base.Owner.playerStats.GetModule<global::PlayerStatsSystem.HealthStat>().ServerHeal(65f);
            base.Owner.playerEffectsController.UseMedicalItem(this);
        }
    }
}
