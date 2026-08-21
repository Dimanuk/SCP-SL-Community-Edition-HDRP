namespace InventorySystem.Items.Usables
{
    public class Painkillers : global::InventorySystem.Items.Usables.Consumable
    {
        [global::UnityEngine.SerializeField]
        private global::UnityEngine.AnimationCurve _healProgress;

        private const float TotalRegenerationTime = 15f;

        private const int TotalHpToRegenerate = 50;

        protected override void OnEffectsActivated()
        {
            ServerAddRegeneration(_healProgress, 1f / TotalRegenerationTime, TotalHpToRegenerate);
            base.Owner.playerEffectsController.UseMedicalItem(this);
        }
    }
}
