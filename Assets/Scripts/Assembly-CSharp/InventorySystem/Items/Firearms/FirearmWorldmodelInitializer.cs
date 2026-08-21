namespace InventorySystem.Items.Firearms
{
    public class FirearmWorldmodelInitializer : global::UnityEngine.MonoBehaviour
    {
        [global::UnityEngine.SerializeField]
        private bool _fixLayerForPlayerModel = true;

        public void Initialize(bool isPickup)
        {
            if (!isPickup && _fixLayerForPlayerModel)
            {
                base.gameObject.layer = global::UnityEngine.LayerMask.NameToLayer("Hitbox");
            }
        }
    }
}
