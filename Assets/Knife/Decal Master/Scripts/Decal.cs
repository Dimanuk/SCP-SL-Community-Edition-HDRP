using System.Collections.Generic;
using CustomCulling;
using GameObjectPools;
using MapGeneration;
using UnityEngine;

namespace Knife.DeferredDecals
{
    public class Decal : PoolObject, IPoolResettable
    {
        public DecalPoolType DecalPoolType;

        [SerializeField]
        private Material m_Material;

        public bool _linkToNearestCullable = true;
        public bool IgnoreFrustumCulling;

        private bool _initialized;
        private CullableBase _linkedCullableBase;
        private Vector3 _initialScale;
        private Bounds _decalBounds;
        private Vector3 _cachedSize;
        private bool _needRebuildBounds;
        private Transform _cachedTransform;

        public int MyGroupIndex { get; private set; }

        public int SortingOrder;

        public Color InstancedColor = Color.white;

        [Range(0f, 1f)]
        public float Fade = 1f;

        public Vector2 UVTiling = new Vector2(1, 1);
        public Vector2 UVOffset = new Vector2(0, 0);

        public Material DecalMaterial
        {
            get => m_Material;
            set
            {
                m_Material = value;
                MyGroupIndex = (value != null) ? DeferredDecalsSystem.GetDecalIndex(value) : -1;
            }
        }

        public Vector4 UV
        {
            get => new Vector4(UVTiling.x, UVTiling.y, UVOffset.x, UVOffset.y);
            set
            {
                UVTiling.x = value.x;
                UVTiling.y = value.y;
                UVOffset.x = value.z;
                UVOffset.y = value.w;
            }
        }

        internal Bounds Bounds
        {
            get => _decalBounds;
            set => _decalBounds = value;
        }

        public Transform CachedTransform => _cachedTransform;

        public static void SetTransformAlongSurface(Transform decal, RaycastHit hitSurface)
        {
            if (decal == null)
                return;

            decal.rotation = Quaternion.LookRotation(-hitSurface.normal);
            decal.position = hitSurface.point + hitSurface.normal * 0.5f * decal.localScale.y;
        }

        public void ResetObject()
        {
            _initialized = false;

            if (_cachedTransform != null)
                _cachedTransform.localScale = _initialScale;

            if (_linkedCullableBase != null)
            {
                _linkedCullableBase.Decals.Remove(this);
                _linkedCullableBase = null;
            }
        }

        private void Awake()
        {
            _cachedTransform = transform;
            _initialScale = _cachedTransform.localScale;

            if (m_Material != null)
                MyGroupIndex = DeferredDecalsSystem.GetDecalIndex(m_Material);
        }

        internal void SetupBounds()
        {
            if (!_needRebuildBounds)
                return;

            float sideX = 0.5f;
            float sideY = 0.5f;
            float sideZ = 0.5f;

            Vector3 p1 = _cachedTransform.TransformPoint(new Vector3(-sideX, -sideY * 2, -sideZ));
            Vector3 p2 = _cachedTransform.TransformPoint(new Vector3(-sideX, -sideY * 2, sideZ));
            Vector3 p3 = _cachedTransform.TransformPoint(new Vector3(sideX, -sideY * 2, sideZ));
            Vector3 p4 = _cachedTransform.TransformPoint(new Vector3(sideX, -sideY * 2, -sideZ));
            Vector3 p5 = _cachedTransform.TransformPoint(new Vector3(-sideX, 0, -sideZ));
            Vector3 p6 = _cachedTransform.TransformPoint(new Vector3(-sideX, 0, sideZ));
            Vector3 p7 = _cachedTransform.TransformPoint(new Vector3(sideX, 0, sideZ));
            Vector3 p8 = _cachedTransform.TransformPoint(new Vector3(sideX, 0, -sideZ));

            float minX = Mathf.Min(new float[] { p1.x, p2.x, p3.x, p4.x, p5.x, p6.x, p7.x, p8.x });
            float minY = Mathf.Min(new float[] { p1.y, p2.y, p3.y, p4.y, p5.y, p6.y, p7.y, p8.y });
            float minZ = Mathf.Min(new float[] { p1.z, p2.z, p3.z, p4.z, p5.z, p6.z, p7.z, p8.z });

            float maxX = Mathf.Max(new float[] { p1.x, p2.x, p3.x, p4.x, p5.x, p6.x, p7.x, p8.x });
            float maxY = Mathf.Max(new float[] { p1.y, p2.y, p3.y, p4.y, p5.y, p6.y, p7.y, p8.y });
            float maxZ = Mathf.Max(new float[] { p1.z, p2.z, p3.z, p4.z, p5.z, p6.z, p7.z, p8.z });

            _decalBounds.SetMinMax(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            _needRebuildBounds = false;
        }

        private void UpdateBoundsCenter()
        {
            Vector3 center = _cachedTransform.position - _cachedTransform.up * 0.5f * _cachedTransform.localScale.y;
            _decalBounds.center = center;
        }

        private void Update()
        {
            UpdateBoundsCenter();

            Vector3 currentScale = _cachedTransform.localScale;
            if (_cachedSize != currentScale)
            {
                _needRebuildBounds = true;
                _cachedSize = currentScale;
            }

            if (_linkToNearestCullable && !_initialized)
                UpdateRoom();
        }

        private void UpdateRoom()
        {
            CullableBase cullable = GetComponentInParent<CullableBase>();

            if (cullable == null)
            {
                Vector3 origin = _cachedTransform.position + Vector3.up;
                if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 100f))
                {
                    cullable = hit.collider.GetComponentInParent<CullableBase>();
                }
            }

            if (cullable == null)
            {
                RoomIdentifier room = RoomIdUtils.RoomAtPositionRaycasts(_cachedTransform.position);
                if (room != null)
                    cullable = room.GetComponent<CullableRoom>();
            }

            if (cullable != null)
            {
                _linkedCullableBase = cullable;
                _initialized = true;
                cullable.Decals.Add(this);
                enabled = cullable.CullEnabled;
            }
        }
    }
}
