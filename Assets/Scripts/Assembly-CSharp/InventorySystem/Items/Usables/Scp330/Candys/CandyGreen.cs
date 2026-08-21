namespace InventorySystem.Items.Usables.Scp330
{
    public class CandyGreen : global::InventorySystem.Items.Usables.Scp330.ICandy
    {
        private const float RegenerationDuration = 80f;

        private const float RegenerationPerSecond = 1.5f;

        private const int VitalityDuration = 30;

        private const bool VitalityDurationStacking = true;

        public global::InventorySystem.Items.Usables.Scp330.CandyKindID Kind => global::InventorySystem.Items.Usables.Scp330.CandyKindID.Green;

        public float SpawnChanceWeight => 1f;

        public void ServerApplyEffects(ReferenceHub hub)
        {
            global::InventorySystem.Items.Usables.Scp330.Scp330Bag.AddSimpleRegeneration(hub, 1.5f, 80f);
            hub.playerEffectsController.EnableEffect<global::CustomPlayerEffects.Vitality>(30f, addDuration: true);
        }
    }
}
