namespace InventorySystem.Items
{
    public static class EquipDequipModifierExtensions
    {
        public static bool CanHolster(this global::InventorySystem.Items.ItemBase ib)
        {
            if (ib is global::InventorySystem.Items.IEquipDequipModifier equipDequipModifier)
            {
                return equipDequipModifier.AllowHolster;
            }
            return true;
        }

        public static bool CanEquip(this global::InventorySystem.Items.ItemBase ib)
        {
            if (ib is global::InventorySystem.Items.IEquipDequipModifier equipDequipModifier)
            {
                return equipDequipModifier.AllowEquip;
            }
            return true;
        }
    }
}
