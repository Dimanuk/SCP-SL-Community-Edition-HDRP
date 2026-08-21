namespace InventorySystem.Items.Usables
{
    public class Scp1853Item : global::InventorySystem.Items.Usables.Consumable
    {
        protected override void OnEffectsActivated()
        {
            base.Owner.playerEffectsController.GetEffect<global::CustomPlayerEffects.Scp1853>().Intensity++;
        }
    }
}
