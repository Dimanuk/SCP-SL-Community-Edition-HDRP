using InventorySystem.Items.Firearms.BasicMessages;

namespace InventorySystem.Items.Firearms.Modules
{
	public interface IHitregModule : IFirearmModuleBase
	{
		bool ClientCalculateHit(out ShotMessage message);

		void ServerProcessShot(ShotMessage message);
	}
}
