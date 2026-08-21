namespace InventorySystem.Hotkeys
{
	public interface IHotkeyItemSelector
	{
		ActionName KeyActionName { get; }

		ushort GetCorrespondingItemSerial(ReferenceHub ply, ushort[] itemsOrder, bool smartFeatureEnabled);
	}
}
