using InventorySystem.Items;
using TMPro;
using UnityEngine;

namespace InventorySystem.GUI.Descriptions
{
	public class EmptyDescriptionGui : RadialDescriptionBase
	{
		[SerializeField]
		private TextMeshProUGUI _desc;

        public override void UpdateInfo(ItemBase targetItem, Color roleColor)
        {
            if (targetItem == null)
            {
                throw new System.NullReferenceException();
            }

            string displayText;

            if (targetItem.ItemTypeId != ItemType.None)
            {
                displayText = string.Empty;
            }
            else
            {
                displayText = targetItem.ItemTypeId.ToString();
            }

            _desc.text = displayText;
        }
    }
}
