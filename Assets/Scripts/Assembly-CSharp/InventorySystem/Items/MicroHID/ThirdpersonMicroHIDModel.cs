using UnityEngine;
using InventorySystem.Items.MicroHID;

namespace InventorySystem.Items.MicroHID
{
    public class ThirdpersonMicroHIDModel : MonoBehaviour
    {
        [SerializeField]
        private ReferenceHub _hub;

        [SerializeField]
        private Transform _needle;

        private void Update()
        {
            if (_hub == null)
            {
                if (!ReferenceHub.TryGetHub(transform.root.gameObject, out _hub))
                    return;
            }

            if (_hub == null || _hub.inventory.CurItem.TypeId != ItemType.MicroHID)
                return;

            if (MicroHIDHandler.SyncEnergy.TryGetValue(_hub.inventory.CurItem.SerialNumber, out float energy))
            {
                float lerpAmount = Time.deltaTime * 4f;
                MicroHIDViewmodel.LerpGauge(_needle, energy, lerpAmount);
            }
        }
    }
}