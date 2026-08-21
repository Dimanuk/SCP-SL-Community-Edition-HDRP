using UnityEngine;

namespace CustomCulling
{
    public class AutoToggleLight : MonoBehaviour
    {
        private Light _light;
        private CullableRoom _room;

        private void Start()
        {
            _light = GetComponent<Light>();

            if (_light == null)
            {
                enabled = false;
                return;
            }

            _room = GetComponentInParent<CullableRoom>();

            if (_room == null)
                _light.enabled = false;
        }

        private void Update()
        {
            if (!CullingManager.Initialized)
                return;

            _light.enabled = _room != null
                && _room.CurrentUpdateType.UpdateLights
                && !CullingManager.AllLightsDisabled;
        }
    }
}