namespace InventorySystem.Items.Usables.Scp330
{
    public class CandyRed : global::InventorySystem.Items.Usables.Scp330.ICandy
    {
        private const float RegenerationDuration = 5f;

        private const float RegenerationPerSecond = 9f;

        public global::InventorySystem.Items.Usables.Scp330.CandyKindID Kind => global::InventorySystem.Items.Usables.Scp330.CandyKindID.Red;

        public float SpawnChanceWeight => 1f;

        public void ServerApplyEffects(ReferenceHub hub)
        {
            global::InventorySystem.Items.Usables.Scp330.Scp330Bag.AddSimpleRegeneration(hub, 9f, 5f);
        }
    }
}
