namespace InventorySystem.Items.Firearms
{
    public static class FirearmAnimatorHashes
    {
        public static readonly global::System.Collections.Generic.Dictionary<global::InventorySystem.Items.Firearms.Attachments.AttachmentSlot, int> Slots = new global::System.Collections.Generic.Dictionary<global::InventorySystem.Items.Firearms.Attachments.AttachmentSlot, int>
        {
            [global::InventorySystem.Items.Firearms.Attachments.AttachmentSlot.Sight] = global::UnityEngine.Animator.StringToHash("SightId"),
            [global::InventorySystem.Items.Firearms.Attachments.AttachmentSlot.Barrel] = global::UnityEngine.Animator.StringToHash("BarrelId"),
            [global::InventorySystem.Items.Firearms.Attachments.AttachmentSlot.SideRail] = global::UnityEngine.Animator.StringToHash("SideRailId"),
            [global::InventorySystem.Items.Firearms.Attachments.AttachmentSlot.BottomRail] = global::UnityEngine.Animator.StringToHash("BottomRailId"),
            [global::InventorySystem.Items.Firearms.Attachments.AttachmentSlot.Ammunition] = global::UnityEngine.Animator.StringToHash("AmmunitionId"),
            [global::InventorySystem.Items.Firearms.Attachments.AttachmentSlot.Stock] = global::UnityEngine.Animator.StringToHash("StockId"),
            [global::InventorySystem.Items.Firearms.Attachments.AttachmentSlot.Stability] = global::UnityEngine.Animator.StringToHash("StabilityId"),
            [global::InventorySystem.Items.Firearms.Attachments.AttachmentSlot.Body] = global::UnityEngine.Animator.StringToHash("BodyId")
        };

        public static readonly int IsCocked = global::UnityEngine.Animator.StringToHash("IsCocked");

        public static readonly int IsChambered = global::UnityEngine.Animator.StringToHash("IsChambered");

        public static readonly int IsMagInserted = global::UnityEngine.Animator.StringToHash("IsMagInserted");

        public static readonly int IsUnloaded = global::UnityEngine.Animator.StringToHash("IsUnloaded");

        public static readonly int DrawSpeedMultiplier = global::UnityEngine.Animator.StringToHash("DrawSpeedMultiplier");

        public static readonly int ReloadSpeedMultiplier = global::UnityEngine.Animator.StringToHash("ReloadSpeedMultiplier");

        public static readonly int FireRateMultiplier = global::UnityEngine.Animator.StringToHash("FireRateMultiplier");

        public static readonly int Random = global::UnityEngine.Animator.StringToHash("Random");

        public static readonly int Inspect = global::UnityEngine.Animator.StringToHash("Inspect");

        public static readonly int Idle = global::UnityEngine.Animator.StringToHash("Idle");

        public static readonly int Reload = global::UnityEngine.Animator.StringToHash("Reload");

        public static readonly int Unload = global::UnityEngine.Animator.StringToHash("Unload");

        public static readonly int Fire = global::UnityEngine.Animator.StringToHash("Fire");

        public static readonly int DryFire = global::UnityEngine.Animator.StringToHash("DryFire");

        public static readonly int Ammo = global::UnityEngine.Animator.StringToHash("Ammo");

        public static readonly int RecoilPatternState = global::UnityEngine.Animator.StringToHash("RecoilPatternState");

        public static readonly int GripBlend = global::UnityEngine.Animator.StringToHash("GripBlend");

        public static readonly int Shoot = global::UnityEngine.Animator.StringToHash("Shoot");
    }
}
