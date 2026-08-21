namespace InventorySystem.Items.Firearms.Modules
{
	public interface IInspectorModule : IFirearmModuleBase
	{
		bool CanInspect { get; }

		void OnInspect();
	}
}
