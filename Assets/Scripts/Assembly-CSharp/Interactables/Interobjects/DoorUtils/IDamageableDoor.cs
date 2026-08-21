namespace Interactables.Interobjects.DoorUtils
{
	public interface IDamageableDoor
	{
		bool IsDestroyed { get; set; }

		bool ServerDamage(float hp, DoorDamageType type);

		void ClientDestroyEffects();

		float GetHealthPercent();
	}
}
