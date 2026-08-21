namespace InventorySystem.Items.Firearms
{
    public class Com45 : global::InventorySystem.Items.Firearms.AutomaticFirearm
    {
        [global::UnityEngine.SerializeField]
        private global::UnityEngine.Vector3[] _offsets;

        private const int MagCount = 3;

        public override global::InventorySystem.Items.Firearms.FirearmStatus Status
        {
            get
            {
                return base.Status;
            }
            set
            {
                int num = value.Ammo % 3;
                if (num != 0)
                {
                    base.OwnerInventory.ServerAddAmmo(AmmoType, num);
                    value = new global::InventorySystem.Items.Firearms.FirearmStatus((byte)(value.Ammo - num), value.Flags, value.Attachments);
                }
                base.Status = value;
            }
        }

        public override void OnAdded(global::InventorySystem.Items.Pickups.ItemPickupBase pickup)
        {
            base.OnAdded(pickup);
            HitregModule = new global::InventorySystem.Items.Firearms.Modules.MultiShotHitreg(this, base.Owner, (HitregModule as global::InventorySystem.Items.Firearms.Modules.SingleBulletHitreg).RecoilPattern, _offsets);
        }
    }
}
