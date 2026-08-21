namespace InventorySystem.Items.Firearms.Modules
{
	public interface IAdsModule : IFirearmModuleBase
	{
		bool ServerAds { get; set; }

		float ClientAdsAmount { get; }

		bool ClientAllowAds { get; }

		void ClientUpdateAds(bool state);
	}
}
