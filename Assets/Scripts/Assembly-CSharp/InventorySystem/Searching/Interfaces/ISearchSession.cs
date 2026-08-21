using InventorySystem.Items.Pickups;

namespace InventorySystem.Searching
{
	public interface ISearchSession
	{
		ItemPickupBase Target { get; set; }

		double InitialTime { get; set; }

		double FinishTime { get; set; }

		double Progress { get; }
	}
}
