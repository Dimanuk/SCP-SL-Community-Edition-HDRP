using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using UnityEngine;

namespace Scp914.Processors
{
	public abstract class Scp914ItemProcessor : MonoBehaviour
	{
		public abstract ItemBase OnInventoryItemUpgraded(Scp914KnobSetting setting, ReferenceHub hub, ushort serial);

		public abstract ItemPickupBase OnPickupUpgraded(Scp914KnobSetting setting, ItemPickupBase ipb, Vector3 newPosition);
	}
}
