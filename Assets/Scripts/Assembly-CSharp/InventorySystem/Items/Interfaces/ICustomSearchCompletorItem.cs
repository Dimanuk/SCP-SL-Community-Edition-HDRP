using InventorySystem.Items.Pickups;
using InventorySystem.Searching;

namespace InventorySystem.Items
{
	public interface ICustomSearchCompletorItem
	{
		SearchCompletor GetCustomSearchCompletor(ReferenceHub hub, ItemPickupBase ipb, ItemBase ib, double disSqrt);
	}
}
