using InventorySystem.Items;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Hotkeys
{
	public class SimpleHotkeyIcon : HotkeyIconBase
	{
		[SerializeField]
		private RawImage _icon;

		protected override void UpdateIcons(ushort serial, ItemBase ib)
		{
            _icon.texture = ib.Icon;
		}
	}
}
