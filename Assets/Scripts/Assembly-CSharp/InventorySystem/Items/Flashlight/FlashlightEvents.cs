using UnityEngine;
using Mirror;
using InventorySystem.Items;

namespace InventorySystem.Items.Flashlight
{
    public class FlashlightEvents : MonoBehaviour
    {
        [SerializeField]
        private ItemViewmodelBase _ivb;

        private void Toggle()
        {
            if (_ivb == null || _ivb.ParentItem == null)
                return;

            ReferenceHub owner = _ivb.ParentItem.Owner;
            if (owner == null)
                return;

            ItemBase currentItem = owner.inventory.CurInstance;
            if (currentItem is FlashlightItem flashlightItem)
            {
                if (flashlightItem.Owner.isLocalPlayer)
                {
                    bool newLightState = !flashlightItem.IsEmittingLight;
                    flashlightItem.IsEmittingLight = newLightState;

                    NetworkClient.Send(new FlashlightNetworkHandler.FlashlightMessage(flashlightItem.ItemSerial, newLightState));
                }
            }
        }
    }
}