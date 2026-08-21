namespace InventorySystem.Items
{
	public interface IEquipDequipModifier
	{
		bool AllowHolster { get; }

		bool AllowEquip { get; }
	}
}
