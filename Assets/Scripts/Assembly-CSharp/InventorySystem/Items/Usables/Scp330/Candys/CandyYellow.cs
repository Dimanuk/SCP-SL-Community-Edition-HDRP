namespace InventorySystem.Items.Usables.Scp330
{
    public class CandyYellow : global::InventorySystem.Items.Usables.Scp330.ICandy
    {
        private const int InstantStamina = 25;

        private const int InvigorationDuration = 8;

        private const bool InvigorationDurationStacking = true;

        private const int BoostDuration = 8;

        private const bool BoostDurationStacking = true;

        private const int BoostIntensity = 10;

        private const bool BoostIntensityStacking = true;

        public global::InventorySystem.Items.Usables.Scp330.CandyKindID Kind => global::InventorySystem.Items.Usables.Scp330.CandyKindID.Yellow;

        public float SpawnChanceWeight => 1f;

        public void ServerApplyEffects(ReferenceHub hub)
        {
            hub.playerStats.GetModule<global::PlayerStatsSystem.StaminaStat>().ModifyAmount(0.25f);
            hub.playerEffectsController.EnableEffect<global::CustomPlayerEffects.Invigorated>(8f, addDuration: true);
            global::CustomPlayerEffects.MovementBoost effect = hub.playerEffectsController.GetEffect<global::CustomPlayerEffects.MovementBoost>();
            int value = effect.Intensity + 10;
            effect.Intensity = (byte)global::UnityEngine.Mathf.Clamp(value, 0, 255);
            effect.ServerChangeDuration(8f, addDuration: true);
        }
    }
}
