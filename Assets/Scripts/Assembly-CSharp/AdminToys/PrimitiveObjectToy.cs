using System;
using Mirror;
using UnityEngine;

namespace AdminToys
{
    public class PrimitiveObjectToy : AdminToyBase
    {
        [SerializeField]
        private Material _regularMatTemplate;

        [SerializeField]
        private Material _transparentMatTemplate;

        private GameObject _spawnedPrimitve;

        private MeshRenderer _renderer;

        private Material _sharedRegular;

        private Material _sharedTransparent;

        private bool _materialsSet;

        [SyncVar(hook = nameof(SetPrimitive))]
        public PrimitiveType PrimitiveType;

        [SyncVar(hook = nameof(SetColor))]
        public Color MaterialColor;

        public override string CommandName => "PrimitiveObject";

        public override void OnSpawned(ReferenceHub admin, ArraySegment<string> arguments)
        {
            string[] array = arguments.Array;
            PrimitiveType = ((array.Length > 2 && Enum.TryParse<PrimitiveType>(array[2], ignoreCase: true, out var result)) ? result : PrimitiveType.Sphere);
            MaterialColor = ((array.Length > 3 && ColorUtility.TryParseHtmlString(array[3], out var color)) ? color : Color.gray);
            float num = ((array.Length > 4 && float.TryParse(array[4], out var result2)) ? result2 : 1f);
            base.transform.SetPositionAndRotation(admin.PlayerCameraReference.position, admin.PlayerCameraReference.rotation);
            base.transform.localScale = Vector3.one * num;
            base.Scale = base.transform.localScale;
            base.OnSpawned(admin, arguments);
        }

        private void Start()
        {
            SetPrimitive(default, PrimitiveType);
        }

        private void SetPrimitive(PrimitiveType oldPrim, PrimitiveType newPrim)
        {
            if (_spawnedPrimitve != null)
            {
                UnityEngine.Object.Destroy(_spawnedPrimitve);
            }

            _spawnedPrimitve = GameObject.CreatePrimitive(newPrim);
            _renderer = _spawnedPrimitve.GetComponent<MeshRenderer>();

            Transform primitiveTransform = _spawnedPrimitve.transform;
            primitiveTransform.SetParent(base.transform);
            primitiveTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            primitiveTransform.localScale = Vector3.one;

            UnityEngine.Object.Destroy(_spawnedPrimitve.GetComponent<Collider>());

            if (base.Scale.x > 0f || base.Scale.y > 0f || base.Scale.z > 0f)
            {
                bool convex = newPrim != PrimitiveType.Plane && newPrim != PrimitiveType.Quad;
                MeshCollider meshCollider = _spawnedPrimitve.AddComponent<MeshCollider>();
                if (meshCollider != null)
                {
                    meshCollider.convex = convex;
                }
            }

            SetColor(Color.clear, MaterialColor);
        }

        private void SetColor(Color oldColor, Color newColor)
        {
            if (_spawnedPrimitve == null)
            {
                return;
            }

            if (!_materialsSet)
            {
                _sharedRegular = new Material(_regularMatTemplate);
                _sharedTransparent = new Material(_transparentMatTemplate);
                _materialsSet = true;
            }

            _renderer.sharedMaterial = ((newColor.a >= 1f) ? _sharedRegular : _sharedTransparent);

            Material sharedMaterial = _renderer.sharedMaterial;
            if (sharedMaterial != null)
            {
                sharedMaterial.SetColor("_Color", newColor);
            }
        }
    }
}
