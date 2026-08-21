namespace InventorySystem.Items.Pickups
{
	public interface IPickupPhysicsModule
	{
		void DestroyModule();

		void UpdatePhysics();

		void UpdateInfo(PickupSyncInfo psi);
	}
}
