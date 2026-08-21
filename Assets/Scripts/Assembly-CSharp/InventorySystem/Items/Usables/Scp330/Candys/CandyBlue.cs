namespace InventorySystem.Items.Usables.Scp330
{
    public class CandyBlue : global::InventorySystem.Items.Usables.Scp330.ICandy
    {
        private const int AhpInstant = 30;

        private const float AhpDecay = 0f;

        public global::InventorySystem.Items.Usables.Scp330.CandyKindID Kind => global::InventorySystem.Items.Usables.Scp330.CandyKindID.Blue;

        public float SpawnChanceWeight => 1f;

        public void ServerApplyEffects(ReferenceHub hub)
        {
            hub.playerStats.GetModule<global::PlayerStatsSystem.AhpStat>().ServerAddProcess(30f).DecayRate = 0f;
        }
    }
}
