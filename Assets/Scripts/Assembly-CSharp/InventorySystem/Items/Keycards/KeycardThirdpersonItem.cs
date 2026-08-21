using InventorySystem.Items.Thirdperson;
using PlayerRoles.FirstPersonControl.Thirdperson;
using UnityEngine;

namespace InventorySystem.Items.Keycards
{
    public class KeycardThirdpersonItem : IdleThirdpersonItem
    {
        [SerializeField]
        private Renderer _targetRenderer;

        internal override void Initialize(HumanCharacterModel model, ItemIdentifier id)
        {
            base.Initialize(model, id);

            if (InventoryItemLoader.TryGetItem<KeycardItem>(id.TypeId, out var keycardItem))
            {
                if (keycardItem.PickupDropModel is KeycardPickup keycardPickup)
                {
                    Material material = keycardPickup.GetMaterialFromKeycardId(id.TypeId);

                    if (material != null && _targetRenderer != null)
                    {
                        _targetRenderer.sharedMaterial = material;
                    }
                }
            }
        }
    }
}