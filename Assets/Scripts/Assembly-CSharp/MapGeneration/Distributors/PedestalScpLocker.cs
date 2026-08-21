using InventorySystem.Items.Pickups;
using UnityEngine;

namespace MapGeneration.Distributors
{
    public class PedestalScpLocker : Locker
    {
        [Header("Pedestal settings")]
        [SerializeField]
        private Renderer _lightRenderer;

        [SerializeField]
        private int _lightMaterialId;

        [SerializeField]
        private Light _lightSource;

        [SerializeField]
        private float _lightIdleIntensity;

        [SerializeField]
        private float _lightSpeed;

        [SerializeField]
        private Transform _detectionOrigin;

        [SerializeField]
        private float _detectionRayLength;

        private Color _startColor;
        private float _startIntensity;
        private float _prevValue;

        private const int DetectionMask = 512;
        private const string EmissionColorName = "_EmissionColor";

        private float TargetIntensity
        {
            get
            {
                if (ContentDetected)
                {
                    return _lightIdleIntensity;
                }
                return 0f;
            }
        }

        private bool ContentDetected
        {
            get
            {
                if (_detectionOrigin == null) return false;

                RaycastHit hit;
                if (Physics.Raycast(_detectionOrigin.position, _detectionOrigin.forward, out hit, _detectionRayLength, DetectionMask))
                {
                    if (hit.rigidbody != null)
                    {
                        return hit.rigidbody.TryGetComponent<ItemPickupBase>(out _);
                    }
                }
                return false;
            }
        }

        private Material RendererMaterial
        {
            get
            {
                if (_lightRenderer == null) return null;
                return _lightRenderer.sharedMaterials[_lightMaterialId];
            }
            set
            {
                if (_lightRenderer == null) return;
                Material[] materials = _lightRenderer.materials;
                materials[_lightMaterialId] = value;
                _lightRenderer.materials = materials;
            }
        }

        protected override void Start()
        {
            base.Start();
            _prevValue = -1f;

            if (_lightRenderer != null)
            {
                Material instanceMaterial = new Material(RendererMaterial);
                RendererMaterial = instanceMaterial;

                _startColor = instanceMaterial.GetColor(EmissionColorName);
            }

            if (_lightSource != null)
            {
                _startIntensity = _lightSource.intensity;
            }
        }

        protected override void Update()
        {
            base.Update();

            float target = TargetIntensity;

            if (!Mathf.Approximately(_prevValue, target))
            {
                _prevValue = Mathf.MoveTowards(_prevValue, target, Time.deltaTime * _lightSpeed);
                float clamped = Mathf.Clamp01(_prevValue);

                if (RendererMaterial != null)
                {
                    RendererMaterial.SetColor(EmissionColorName, _startColor * clamped);
                }

                if (_lightSource != null)
                {
                    _lightSource.intensity = clamped * _startIntensity;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_detectionOrigin == null) return;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(_detectionOrigin.position, _detectionOrigin.position + _detectionOrigin.forward * _detectionRayLength);
        }
    }
}