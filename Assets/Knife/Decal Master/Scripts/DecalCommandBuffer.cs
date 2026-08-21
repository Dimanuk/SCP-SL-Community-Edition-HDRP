using System.Collections.Generic;
using CustomCulling;
using UnityEngine;
using UnityEngine.Rendering;

namespace Knife.DeferredDecals
{
    public class DecalCommandBuffer : MonoBehaviour
    {
        public static readonly int NormalsID;
        public static readonly int SpecularID;
        public static readonly int SmoothnessID;
        public static readonly int EmissionID;
        public static readonly int NotInstancedUVID;
        public static readonly int NotInstancedColorID;
        public static readonly int SelectionTimeID;
        public static readonly int TintID;
        public static readonly int UVID;
        public static readonly int AlphaID;

        private List<Decal> _currentDecalsVisibility = new List<Decal>();
        private Plane[] _cameraPlanes;
        private Mesh _cubeMeshCopy;
        private MaterialPropertyBlock _instancedBlock;
        private Matrix4x4[] _matrices = new Matrix4x4[1023];
        private Vector4[] _tintArray = new Vector4[1023];
        private Vector4[] _uvArray = new Vector4[1023];
        private Vector3 _cachedCameraPosition;
        private Color _cachedTint;
        private Vector4 _cachedUV;
        private Vector4 _cachedUVOne;

        public CommandBuffer DefaultCommandBuffer { get; private set; }

        public bool UseExclusionMask => DeferredDecalsSystem.Singleton.UseExclusionMask;

        public bool FrustumCulling => DeferredDecalsSystem.Singleton.FrustumCulling;

        private Mesh CubeMesh => _cubeMeshCopy;

        private Material specularSmoothnessBlitterMaterial => DeferredDecalsSystem.Singleton.SpecularSmoothnessBlitterMaterial;

        static DecalCommandBuffer()
        {
            NormalsID = Shader.PropertyToID("_NormalsCopy");
            SpecularID = Shader.PropertyToID("_SpecularTarget");
            SmoothnessID = Shader.PropertyToID("_SmoothnessTarget");
            EmissionID = Shader.PropertyToID("_CameraTargetCopy");
            NotInstancedUVID = Shader.PropertyToID("_NotInstancedUV");
            NotInstancedColorID = Shader.PropertyToID("_NotInstancedColor");
            SelectionTimeID = Shader.PropertyToID("SelectionTime");
            TintID = Shader.PropertyToID("_Tint");
            UVID = Shader.PropertyToID("_UV");
            AlphaID = Shader.PropertyToID("_Alpha");
        }

        private void OnEnable()
        {
            if (_cubeMeshCopy == null)
                CreateCubeMeshCopy();

            _instancedBlock = new MaterialPropertyBlock();

            CommandBuffer buffer = new CommandBuffer();
            buffer.name = "Decals_" + gameObject.name;
            DefaultCommandBuffer = buffer;
        }

        private void CreateCubeMeshCopy()
        {
            DeferredDecalsSystem system = GetComponent<DeferredDecalsSystem>();
            if (system == null || system.CubeMesh == null)
                return;

            _cubeMeshCopy = Instantiate(system.CubeMesh);

            // Shift the projection volume DOWN so it spans local y ∈ [-1, 0] — the decal origin sits
            // 0.5·scale above the surface (SetTransformAlongSurface) and projects down through it.
            // Bounds (SetupBounds), IsPointInsideDecal and the camera-inside passes all assume this range.
            Vector3[] verts = _cubeMeshCopy.vertices;
            for (int i = 0; i < verts.Length; i++)
                verts[i] = verts[i] + Vector3.up * -0.5f;

            _cubeMeshCopy.vertices = verts;
            _cubeMeshCopy.UploadMeshData(true);
        }

        internal void Render(Camera cam)
        {
            Render(cam, DefaultCommandBuffer);
        }

        private void Render(Camera cam, CommandBuffer buffer)
        {
            if (buffer == null || cam == null)
                return;

            DeferredDecalsSystem system = DeferredDecalsSystem.Singleton;
            if (system == null)
                return;

            buffer.Clear();

            if (!system.EnableDecals)
                return;

            if (UseExclusionMask)
                buffer.EnableShaderKeyword("EXCLUSIONMASK");
            else
                buffer.DisableShaderKeyword("EXCLUSIONMASK");

            _cameraPlanes = GeometryUtility.CalculateFrustumPlanes(cam);
            _currentDecalsVisibility.Clear();

            // Collect decals from enabled culling rooms and from dynamic cullables
            // attached to them (e.g. the tantrum puddle's DynamicCullableBase).
            List<CullableRoom> enabledRooms = CullingCamera.EnabledCullableRooms;
            if (enabledRooms != null)
            {
                if (FrustumCulling)
                {
                    foreach (CullableRoom room in enabledRooms)
                    {
                        if (room == null)
                            continue;

                        CacheVisibility(room);

                        List<CullableBase> cullableBases = room.CullableBases;
                        if (cullableBases == null)
                            continue;

                        foreach (CullableBase cullableBase in cullableBases)
                            CacheVisibility(cullableBase);
                    }
                }
                else
                {
                    foreach (CullableRoom room in enabledRooms)
                    {
                        if (room == null || room.Decals == null)
                            continue;

                        foreach (Decal decal in room.Decals)
                        {
                            if (decal != null)
                                _currentDecalsVisibility.Add(decal);
                        }
                    }
                }
            }

            // Setup GBuffer copies
            bool hdr = cam.allowHDR;
            BuiltinRenderTextureType emissionTexture = hdr ? BuiltinRenderTextureType.CameraTarget : BuiltinRenderTextureType.GBuffer3;

            buffer.GetTemporaryRT(NormalsID, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            buffer.Blit(BuiltinRenderTextureType.GBuffer2, NormalsID);

            buffer.GetTemporaryRT(SpecularID, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            buffer.Blit(BuiltinRenderTextureType.GBuffer1, SpecularID);

            buffer.GetTemporaryRT(SmoothnessID, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            buffer.Blit(BuiltinRenderTextureType.GBuffer1, SmoothnessID, specularSmoothnessBlitterMaterial, 0);

            buffer.GetTemporaryRT(EmissionID, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            buffer.Blit(emissionTexture, EmissionID);

            _instancedBlock.Clear();
            _cachedCameraPosition = cam.transform.position;

            // Sort decals into groups
            IReadOnlyList<DecalsGroup> decalGroups = DeferredDecalsSystem.DecalGroup;
            if (decalGroups != null)
            {
                foreach (DecalsGroup group in decalGroups)
                {
                    group.CameraInsideDecalList.Clear();
                    group.CameraOutsideDecalList.Clear();
                }
            }

            foreach (Decal decal in _currentDecalsVisibility)
            {
                if (decal == null || decal.DecalMaterial == null)
                    continue;

                SetupBounds(decal);

                int groupIndex = decal.MyGroupIndex;
                if (groupIndex < 0 || groupIndex >= DeferredDecalsSystem._decalGroups.Count)
                    continue;

                DecalsGroup group = DeferredDecalsSystem._decalGroups[groupIndex];

                if (IsPointInsideDecal(decal.CachedTransform, _cachedCameraPosition))
                    group.CameraInsideDecalList.Add(decal);
                else
                    group.CameraOutsideDecalList.Add(decal);
            }

            // First pass: Diffuse + Normals + Emission
            RenderTargetIdentifier[] mrtDifNormEmission = new RenderTargetIdentifier[]
            {
                BuiltinRenderTextureType.GBuffer0,
                BuiltinRenderTextureType.GBuffer2,
                emissionTexture
            };
            buffer.SetRenderTarget(mrtDifNormEmission, BuiltinRenderTextureType.CameraTarget);

            _cachedUVOne = new Vector4(1, 1, 0, 0);

            if (decalGroups != null)
            {
                foreach (DecalsGroup group in decalGroups)
                {
                    if (group.Material == null)
                        continue;

                    if (group.Instancing)
                    {
                        RenderInstanced(buffer, group.Material, group.CameraInsideDecalList, 3, 0);
                        RenderInstanced(buffer, group.Material, group.CameraOutsideDecalList, 0, 0);
                    }
                    else
                    {
                        RenderNonInstanced(buffer, group.CameraInsideDecalList, 3, 0);
                        RenderNonInstanced(buffer, group.CameraOutsideDecalList, 0, 0);
                    }
                }
            }

            // Second pass: Specular + Smoothness
            RenderTargetIdentifier[] mrtSpecSmooth = new RenderTargetIdentifier[]
            {
                (RenderTargetIdentifier)SpecularID,
                (RenderTargetIdentifier)SmoothnessID
            };
            buffer.SetRenderTarget(mrtSpecSmooth, BuiltinRenderTextureType.CameraTarget);

            if (decalGroups != null)
            {
                foreach (DecalsGroup group in decalGroups)
                {
                    if (group.Material == null)
                        continue;

                    if (group.Instancing)
                    {
                        RenderInstanced(buffer, group.Material, group.CameraInsideDecalList, 3, 2);
                        RenderInstanced(buffer, group.Material, group.CameraOutsideDecalList, 0, 2);
                    }
                    else
                    {
                        RenderNonInstanced(buffer, group.CameraInsideDecalList, 3, 2);
                        RenderNonInstanced(buffer, group.CameraOutsideDecalList, 0, 2);
                    }
                }
            }

            // Final blit: merge specular + smoothness back
            buffer.SetGlobalTexture(AlphaID, SmoothnessID);
            buffer.Blit(SpecularID, BuiltinRenderTextureType.GBuffer1, specularSmoothnessBlitterMaterial, 1);

            // Release temp RTs
            buffer.ReleaseTemporaryRT(NormalsID);
            buffer.ReleaseTemporaryRT(SpecularID);
            buffer.ReleaseTemporaryRT(SmoothnessID);
            buffer.ReleaseTemporaryRT(EmissionID);
        }

        private void CacheVisibility(CullableBase cullableBase)
        {
            if (cullableBase == null)
                return;

            List<Decal> decals = cullableBase.Decals;
            if (decals == null)
                return;

            foreach (Decal decal in decals)
            {
                if (decal == null)
                    continue;

                if (decal.IgnoreFrustumCulling)
                {
                    _currentDecalsVisibility.Add(decal);
                    continue;
                }

                SetupBounds(decal);

                if (IsBoundsVisible(_cameraPlanes, decal.Bounds))
                    _currentDecalsVisibility.Add(decal);
            }
        }

        private void RenderNonInstanced(CommandBuffer buffer, List<Decal> decalsList, int cameraInsideOffset, int specularOffset)
        {
            if (decalsList == null || decalsList.Count == 0)
                return;

            Mesh cube = CubeMesh;
            if (cube == null)
                return;

            foreach (Decal decal in decalsList)
            {
                if (decal == null || decal.DecalMaterial == null)
                    continue;

                _cachedTint = decal.InstancedColor;
                _cachedTint.a *= decal.Fade;
                buffer.SetGlobalColor(NotInstancedColorID, _cachedTint);

                _cachedUV = decal.UV;
                buffer.SetGlobalVector(NotInstancedUVID, _cachedUV);

                Matrix4x4 matrix = decal.CachedTransform.localToWorldMatrix;
                buffer.DrawMesh(cube, matrix, decal.DecalMaterial, 0, specularOffset + cameraInsideOffset);
            }
        }

        private void RenderInstanced(CommandBuffer buffer, Material mat, List<Decal> decalsList, int cameraInsideOffset, int specularOffset)
        {
            if (decalsList == null || decalsList.Count == 0 || mat == null)
                return;

            Mesh cube = CubeMesh;
            if (cube == null)
                return;

            buffer.SetGlobalColor(NotInstancedColorID, Color.white);
            buffer.SetGlobalVector(NotInstancedUVID, _cachedUVOne);

            int total = decalsList.Count;
            int batchCount = 1 + Mathf.FloorToInt(total / 1024f);

            int index = 0;
            for (int batch = 0; batch < batchCount; batch++)
            {
                int remaining = total - index;
                int drawCount = Mathf.Min(1023, remaining);

                if (drawCount <= 0)
                    break;

                for (int j = 0; j < drawCount; j++)
                {
                    Decal decal = decalsList[index + j];

                    Vector4 tint = decal.InstancedColor;
                    tint.w *= decal.Fade;
                    _tintArray[j] = tint;

                    _uvArray[j] = decal.UV;
                    _matrices[j] = decal.CachedTransform.localToWorldMatrix;
                }

                _instancedBlock.SetVectorArray(TintID, _tintArray);
                _instancedBlock.SetVectorArray(UVID, _uvArray);
                buffer.DrawMeshInstanced(cube, 0, mat, specularOffset + cameraInsideOffset, _matrices, drawCount, _instancedBlock);

                index += drawCount;
            }
        }

        private void SetupBounds(Decal decal)
        {
            if (decal != null)
                decal.SetupBounds();
        }

        private bool IsPointInsideDecal(Transform decalTransform, Vector3 point)
        {
            if (decalTransform == null)
                return false;

            Vector3 local = decalTransform.InverseTransformPoint(point);
            return local.x >= -0.5f && local.x <= 0.5f
                && local.y >= -1.0f && local.y <= 0.0f
                && local.z >= -0.5f && local.z <= 0.5f;
        }

        private bool IsBoundsVisible(Plane[] planes, Bounds bounds)
        {
            return GeometryUtility.TestPlanesAABB(planes, bounds);
        }
    }
}
