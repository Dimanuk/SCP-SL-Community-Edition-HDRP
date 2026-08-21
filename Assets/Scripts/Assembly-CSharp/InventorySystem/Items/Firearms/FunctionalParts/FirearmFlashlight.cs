using UnityEngine;

namespace InventorySystem.Items.Firearms.FunctionalParts
{
    public class FirearmFlashlight : FunctionalFirearmPart
    {
        [SerializeField] private Light _targetSource;
        [SerializeField] private Renderer[] _targetRenderers;
        [SerializeField] private Material _onMat;
        [SerializeField] private Material _offMat;

        private int _prevEnabled = 0;

        private void Update()
        {
            Firearm fa = Firearm;
            if (fa == null) return;

            bool isEnabled = fa.Status.Flags.HasFlag(FirearmStatusFlags.FlashlightEnabled);
            SetStatus(isEnabled);
        }

        public void SetStatus(bool isEnabled)
        {
            int newState = isEnabled ? 2 : 1;
            if (_prevEnabled == newState) return;

            _targetSource.enabled = isEnabled;
            _prevEnabled = newState;

            Material mat = isEnabled ? _onMat : _offMat;
            foreach (Renderer ren in _targetRenderers)
            {
                if (ren != null) ren.material = mat;
            }
        }

        private void OnEnable() => _prevEnabled = 0;

        private void OnDisable() => SetStatus(false);
    }
}