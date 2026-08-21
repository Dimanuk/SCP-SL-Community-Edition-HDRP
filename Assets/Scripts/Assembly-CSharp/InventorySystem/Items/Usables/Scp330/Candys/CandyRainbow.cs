namespace InventorySystem.Items.Usables.Scp330
{
    public class CandyRainbow : global::InventorySystem.Items.Usables.Scp330.ICandy
    {
        private const int HealthInstant = 15;

        private const int InvigorationDuration = 5;

        private const bool InvigorationDurationStacking = true;

        private const int AhpInstant = 20;

        private const int AhpSustainDuration = 10;

        private const bool AhpSustainDurationStacking = false;

        private const int RainbowDuration = 10;

        private const bool RainbowDurationStacking = false;

        private const bool BodyshotReductionStacking = true;

        private global::PlayerStatsSystem.AhpStat.AhpProcess _previousProcess;

        public global::InventorySystem.Items.Usables.Scp330.CandyKindID Kind => global::InventorySystem.Items.Usables.Scp330.CandyKindID.Rainbow;

        public float SpawnChanceWeight => 1f;

        public void ServerApplyEffects(ReferenceHub hub)
        {
            hub.playerStats.GetModule<global::PlayerStatsSystem.HealthStat>().ServerHeal(15f);
            hub.playerEffectsController.EnableEffect<global::CustomPlayerEffects.Invigorated>(5f, addDuration: true);
            bool num = _previousProcess != null;
            float num2 = (num ? _previousProcess.CurrentAmount : 0f);
            float num3 = 0f;
            if (num)
            {
                _previousProcess.CurrentAmount = 0f;
            }
            _previousProcess = hub.playerStats.GetModule<global::PlayerStatsSystem.AhpStat>().ServerAddProcess(num2 + 20f);
            _previousProcess.SustainTime = num3 + 10f;
            hub.playerEffectsController.EnableEffect<global::CustomPlayerEffects.RainbowTaste>(10f);
            global::CustomPlayerEffects.BodyshotReduction effect = hub.playerEffectsController.GetEffect<global::CustomPlayerEffects.BodyshotReduction>();
            if (effect.Intensity < byte.MaxValue)
            {
                effect.Intensity++;
            }
        }
    }
}
