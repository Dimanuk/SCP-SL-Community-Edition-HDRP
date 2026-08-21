using InventorySystem.Items;
using InventorySystem.Items.Usables.Scp330;
using NorthwoodLib.Pools;
using System.Text;
using TMPro;
using UnityEngine;

namespace InventorySystem.GUI.Descriptions
{
	public class Scp330DescriptionGui : RadialDescriptionBase
	{
		[SerializeField]
		private TextMeshProUGUI _title;

		[SerializeField]
		private TextMeshProUGUI _desc;

		[SerializeField]
		private TextMeshProUGUI _candies;

        public override void UpdateInfo(ItemBase targetItem, Color roleColor)
        {
            Scp330Bag bag = targetItem as Scp330Bag;
            if (bag == null)
                return;

            _title.text = bag.Name;

            string description;
            if (TranslationReader.TryGet("InventoryGUI", 15, out description))
            {
                _desc.text = description;
            }

            StringBuilder sb = StringBuilderPool.Shared.Rent();

            foreach (CandyKindID candyId in bag.Candies)
            {
                string candyName = candyId.ToString();
                if (sb.Length > 0)
                    sb.Append("\n");

                sb.Append("- ").Append(candyName);
            }

            _candies.text = sb.ToString();
            StringBuilderPool.Shared.Return(sb);
        }
    }
}
