namespace InventorySystem.Items.Usables.Scp330
{
    public class CandyPurple : global::InventorySystem.Items.Usables.Scp330.ICandy
    {
        private const int ReductionDuration = 15;

        private const int ReductionIntensity = 40;

        private const bool ReductionStacking = true;

        private const float RegenerationDuration = 10f;

        private const float RegenerationPerSecond = 1.5f;

        public global::InventorySystem.Items.Usables.Scp330.CandyKindID Kind => global::InventorySystem.Items.Usables.Scp330.CandyKindID.Purple;

        public float SpawnChanceWeight => 1f;

        public void ServerApplyEffects(ReferenceHub hub)
        {
            global::InventorySystem.Items.Usables.Scp330.Scp330Bag.AddSimpleRegeneration(hub, 1.5f, 10f);
            global::CustomPlayerEffects.DamageReduction effect = hub.playerEffectsController.GetEffect<global::CustomPlayerEffects.DamageReduction>();
            effect.Intensity = (byte)global::UnityEngine.Mathf.Max(effect.Intensity, 40);
            effect.ServerChangeDuration(15f, addDuration: true);
        }
    }
}
