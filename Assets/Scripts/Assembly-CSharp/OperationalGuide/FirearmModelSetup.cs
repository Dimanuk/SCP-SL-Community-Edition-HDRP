
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using UnityEngine;

namespace OperationalGuide
{
	public class FirearmModelSetup : MonoBehaviour
	{
		public FirearmWorldmodel HeldFirarm;

		public ItemType Firearm;

		private void Start()
		{
			FirearmStatus firearmStatus = new FirearmStatus();
			HeldFirarm = GetComponentInChildren<FirearmWorldmodel>();
			HeldFirarm.ApplyStatus(firearmStatus, Firearm);
		}
	}
}
