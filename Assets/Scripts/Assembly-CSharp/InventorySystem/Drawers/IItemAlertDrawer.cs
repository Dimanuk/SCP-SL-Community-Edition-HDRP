namespace InventorySystem.Drawers
{
	public interface IItemAlertDrawer : IItemDrawer
	{
		string AlertText { get; }

	}
}
