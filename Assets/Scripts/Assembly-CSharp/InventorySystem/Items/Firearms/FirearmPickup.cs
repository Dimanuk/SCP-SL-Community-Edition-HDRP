namespace InventorySystem.Items.Firearms
{
    public class FirearmPickup : global::InventorySystem.Items.Pickups.CollisionDetectionPickup, global::InventorySystem.Items.Pickups.IPickupDistributorTrigger
    {
        [global::System.NonSerialized]
        public bool Distributed;

        [global::Mirror.SyncVar]
        public global::InventorySystem.Items.Firearms.FirearmStatus Status;

        [global::UnityEngine.SerializeField]
        private global::InventorySystem.Items.Firearms.FirearmWorldmodel _worldmodel;

        public void OnDistributed()
        {
            Distributed = true;
            Status = new global::InventorySystem.Items.Firearms.FirearmStatus(2, global::InventorySystem.Items.Firearms.FirearmStatusFlags.MagazineInserted, global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.GetRandomAttachmentsCode(Info.ItemId));
        }

        private void Update()
        {
            if (_worldmodel.ApplyStatus(Status, Info.ItemId))
            {
                Rb.ResetCenterOfMass();
                Rb.ResetInertiaTensor();
            }
        }
    }
}
