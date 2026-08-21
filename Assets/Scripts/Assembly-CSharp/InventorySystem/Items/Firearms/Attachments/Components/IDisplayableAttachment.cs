using UnityEngine;

namespace InventorySystem.Items.Firearms.Attachments.Components
{
	public interface IDisplayableAttachment
	{
		Texture Icon { get; }

		Vector2 IconOffset { get; }

		int ParentId { get; }

		Vector2 ParentOffset { get; }
	}
}
