using System.Collections.Generic;
using UnityEngine;

namespace Knife.DeferredDecals
{
    public class DeferredDecalsSystem : MonoBehaviour
    {
        public static DeferredDecalsSystem Singleton { get; private set; }

        public bool EnableDecals { get; private set; }

        public static IReadOnlyList<DecalsGroup> DecalGroup => _decalGroups;

        internal static List<DecalsGroup> _decalGroups = new List<DecalsGroup>();

        public bool UseExclusionMask;
        public LayerMask ExclusionMask;
        public bool FrustumCulling = true;

        public Mesh CubeMesh;

        [SerializeField]
        private Shader specularSmoothnessBlitterShader;

        private static Material _defaultDecalMaterial;

        public Material SpecularSmoothnessBlitterMaterial { get; private set; }

        public DecalCommandBuffer DefaultBuffer { get; private set; }

        // Editor compatibility — used by DecalSystemEditor and DecalPlacementTool
        public static bool DrawDecalGizmos = true;
        public TerrainDecalsType TerrainDecals;
        public int TerrainHeightMapSize = 1024;

        public enum TerrainDecalsType
        {
            None,
            OneTerrain,
            MultiTerrain
        }

        public void CopyHeightmaps()
        {
            // Terrain heightmap copy removed in game version — stub for editor compatibility
        }

        public static Material DefaultDecalMaterial
        {
            get
            {
                if (_defaultDecalMaterial == null)
                    _defaultDecalMaterial = Resources.Load<Material>("Knife/Deferred Decals/DefaultDecalMaterial");

                return _defaultDecalMaterial;
            }
        }

        public static int GetDecalIndex(Material key)
        {
            for (int i = 0; i < _decalGroups.Count; i++)
            {
                if (_decalGroups[i].Material.GetInstanceID() == key.GetInstanceID())
                    return i;
            }

            DecalsGroup newGroup = new DecalsGroup(key);
            _decalGroups.Add(newGroup);
            return _decalGroups.Count - 1;
        }

        private void Awake()
        {
            Singleton = this;

            EnableDecals = PlayerPrefsSl.Get("decals_enabled", true);

            specularSmoothnessBlitterShader = Resources.Load<Shader>("Knife/Deferred Decals/SpecSmoothnessBlitter");
            if (specularSmoothnessBlitterShader != null)
                SpecularSmoothnessBlitterMaterial = new Material(specularSmoothnessBlitterShader);

            DefaultBuffer = GetComponent<DecalCommandBuffer>();
        }
    }
}
