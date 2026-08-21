namespace InventorySystem.Items.Firearms.Modules
{
	public interface IActionModule : IFirearmModuleBase
	{
		float CyclicRate { get; }

		bool IsTriggerHeld { get; }

		FirearmStatus PredictedStatus { get; }

		ActionModuleResponse DoClientsideAction(bool isTriggerPressed);

		bool ServerAuthorizeShot();

		bool ServerAuthorizeDryFire();
	}
}
