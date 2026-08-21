namespace InventorySystem.Items.Firearms.Modules
{
	public interface IAmmoManagerModule : IFirearmModuleBase
	{
		byte MaxAmmo { get; }

		bool ClientCanReload { get; }

		bool ClientCanUnload { get; }

		bool ServerTryReload();

		bool ServerTryUnload();

		void ClientReload();

		void ClientUnload();
	}
}
