using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items;
using InventorySystem.Items.Keycards;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.GUI.Descriptions
{
	public class KeycardDescriptionGui : RadialDescriptionBase
	{
        [Serializable]
        private struct PermissionNode
        {
            [SerializeField]
            private Image _node;

            [SerializeField]
            private KeycardPermissions _permission;

            public void SetColor(KeycardPermissions keycardPerms, Color roleColor)
            {
                bool hasPerm = keycardPerms.HasFlag(_permission);
                _node.color = (hasPerm ? roleColor : new Color(1f, 1f, 1f, 0.051f));
            }
        }

        [SerializeField]
		private TextMeshProUGUI _title;

		[SerializeField]
		private PermissionNode[] _nodes;

        public override void UpdateInfo(ItemBase targetItem, Color roleColor)
        {
            KeycardItem keycard = targetItem as KeycardItem;
            if (keycard == null)
                return;

            // Use the translated item name (IItemNametag), not ToString() which prints
            // "KeycardItem(Clone) (InventorySystem.Items.Keycards.KeycardItem)".
            _title.text = keycard.Name;

            for (int i = 0; i < _nodes.Length; i++)
            {
                _nodes[i].SetColor(keycard.Permissions, roleColor);
            }
        }
    }
}
