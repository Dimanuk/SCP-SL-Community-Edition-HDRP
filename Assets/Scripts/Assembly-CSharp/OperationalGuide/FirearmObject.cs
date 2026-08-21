using UnityEngine;
using InventorySystem.Items;
using InventorySystem.Items.Thirdperson;
using PlayerRoles.FirstPersonControl.Thirdperson;

namespace OperationalGuide
{
	public class FirearmObject : ModelObject
	{
		private GameObject _lastFirearm;

		public override void OnPannableStart()
		{
			base.OnPannableStart();
		}

		public override void OnUpdate()
		{
			if (this.HCM == null)
				return;

			ItemIdentifier itemId = new ItemIdentifier(this.ItemHeld, 0);

			this.HCM.UpdateHeldItem(itemId);

			ThirdpersonItemBase lastItem = this.HCM.LastItemInstance;
			if (lastItem == null)
				return;

			GameObject currentFirearm = lastItem.gameObject;
			if (this._lastFirearm == currentFirearm)
				return;

			FirearmModelSetup setup = currentFirearm.AddComponent<FirearmModelSetup>();
			setup.Firearm = this.ItemHeld;

			this._lastFirearm = currentFirearm;
		}

		public FirearmObject()
		{
			this.ItemHeld = ItemType.None;
		}
	}
}
