using UnityEngine;

namespace InventorySystem.Items.Flashlight
{
    public class FlashlightViewmodel : StandardAnimatedViemodel
    {
        private static readonly int ToggleHash;

        private Light _light;

        static FlashlightViewmodel()
        {
            ToggleHash = Animator.StringToHash("Toggle");
        }

        public void PlayAnimation()
        {
            AnimatorSetTrigger(ToggleHash);
        }

        public override void InitSpectator(ReferenceHub ply, ItemIdentifier id, bool wasEquipped)
        {
            base.InitSpectator(ply, id, wasEquipped);

            _light = GetComponentInChildren<Light>(true);

            FlashlightNetworkHandler.OnStatusReceived += OnStatusReceived;

            if (FlashlightNetworkHandler.ReceivedStatuses.TryGetValue(id.SerialNumber, out bool newState))
            {
                if (_light != null)
                {
                    _light.enabled = newState;
                }
            }
        }

        private void OnStatusReceived(FlashlightNetworkHandler.FlashlightMessage msg)
        {
            if (msg.Serial == ItemId.SerialNumber)
            {
                if (_light != null)
                {
                    _light.enabled = msg.NewState;
                }
            }
        }

        private void OnDestroy()
        {
            FlashlightNetworkHandler.OnStatusReceived -= OnStatusReceived;
        }
    }
}