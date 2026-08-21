namespace InventorySystem.Items.Usables
{
    public class Adrenaline : global::InventorySystem.Items.Usables.Consumable
    {
        private const float StaminaRegenerationPercent = 100f;

        private const float InvigoratedTargetDuration = 8f;

        private const bool InvigoratedDurationAdditive = true;

        private const float AhpAddition = 40f;

        protected override void OnEffectsActivated()
        {
            base.Owner.playerStats.GetModule<global::PlayerStatsSystem.StaminaStat>().ModifyAmount(StaminaRegenerationPercent);
            base.Owner.playerStats.GetModule<global::PlayerStatsSystem.AhpStat>().ServerAddProcess(AhpAddition);
            base.Owner.playerEffectsController.EnableEffect<global::CustomPlayerEffects.Invigorated>(InvigoratedTargetDuration, addDuration: InvigoratedDurationAdditive);
            base.Owner.playerEffectsController.UseMedicalItem(this);
        }
    }
}
