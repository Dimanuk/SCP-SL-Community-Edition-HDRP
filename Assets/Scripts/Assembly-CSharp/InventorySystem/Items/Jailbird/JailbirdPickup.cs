
using InventorySystem.Items.Pickups;
using UnityEngine;

namespace InventorySystem.Items.Jailbird
{
	public class JailbirdPickup : CollisionDetectionPickup
	{
		[SerializeField]
		private JailbirdMaterialController _materialController;

		public float TotalMelee { get; set; }

		public int TotalCharges { get; set; }

		protected override void Start()
		{
			base.Start();
            _materialController.SetSerial(Info.Serial);
        }
	}
}
