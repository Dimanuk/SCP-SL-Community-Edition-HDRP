using GameObjectPools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerRoles.FirstPersonControl.Thirdperson
{
    public class CharacterModel : PoolObject, IPoolResettable, IPoolSpawnable
    {
        private readonly struct RendererMaterialPair
        {
            public readonly Renderer Rend;
            public readonly Material Mat;

            public RendererMaterialPair(Renderer rend, Material mat)
            {
                Rend = rend;
                Mat = mat;
            }
        }

        private float _lastFade = 1f;
        private Material[] _fadeableMaterials;
        private RendererMaterialPair[] _originalMaterials;

        private static readonly int FadeHash = Shader.PropertyToID("_Fade");
        private static Material[] _copyMaterialsNonAlloc = new Material[16];

        [SerializeField]
        private Renderer[] _renderers;

        public HitboxIdentity[] Hitboxes;

        public bool IsVisible { get; private set; }
        public ReferenceHub OwnerHub { get; private set; }

        public ReadOnlySpan<Renderer> Renderers => _renderers;

        protected Transform CachedTransform { get; private set; }

        public virtual float Fade
        {
            get => Mathf.Clamp01(_lastFade);
            set
            {
                value = Mathf.Clamp01(value);
                if (Mathf.Approximately(_lastFade, value))
                    return;

                if (_fadeableMaterials == null)
                    InstantiateFadeableMaterials();

                for (int i = 0; i < _fadeableMaterials.Length; i++)
                {
                    _fadeableMaterials[i].SetFloat(FadeHash, value);
                }

                _lastFade = value;
                OnFadeChanged?.Invoke();
            }
        }

        public Material[] FadeableMaterials
        {
            get
            {
                if (_fadeableMaterials == null)
                    InstantiateFadeableMaterials();
                return _fadeableMaterials;
            }
        }

        public event Action OnVisibilityChanged;
        public event Action OnFadeChanged;

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            PlayerRoleLoader.OnLoaded = () =>
            {
                foreach (var role in PlayerRoleLoader.AllRoles)
                {
                    if (role.Value is IFpcRole fpcRole)
                    {
                        if (fpcRole.FpcModule.CharacterModelTemplate != null &&
                            fpcRole.FpcModule.CharacterModelTemplate.TryGetComponent<CharacterModel>(out var component))
                        {
                            PoolManager.Singleton.TryAddPool(component);
                        }
                    }
                }
            };
        }

        private void InstantiateFadeableMaterials()
        {
            int count = _renderers.Length;
            if (_copyMaterialsNonAlloc.Length < count)
                _copyMaterialsNonAlloc = new Material[count];

            if (_originalMaterials == null)
                SetOriginalMaterials();

            int fadeableCount = 0;
            for (int i = 0; i < count; i++)
            {
                Renderer renderer = _renderers[i];
                if (renderer == null)
                    continue;

                Material sharedMat = renderer.sharedMaterial;

                if (sharedMat != null && sharedMat.HasFloat(FadeHash))
                {
                    Material instance = new Material(sharedMat);
                    renderer.material = instance;
                    _copyMaterialsNonAlloc[fadeableCount] = instance;
                    fadeableCount++;
                }
            }

            _fadeableMaterials = new Material[fadeableCount];
            Array.Copy(_copyMaterialsNonAlloc, _fadeableMaterials, fadeableCount);
        }

        protected virtual void Awake()
        {
            CachedTransform = transform;
        }

        protected virtual void OnValidate()
        {
            Hitboxes = GetComponentsInChildren<HitboxIdentity>();

            MeshRenderer[] meshRends = GetComponentsInChildren<MeshRenderer>();
            SkinnedMeshRenderer[] skinnedRends = GetComponentsInChildren<SkinnedMeshRenderer>();

            _renderers = new Renderer[meshRends.Length + skinnedRends.Length];
            Array.Copy(meshRends, _renderers, meshRends.Length);
            Array.Copy(skinnedRends, 0, _renderers, meshRends.Length, skinnedRends.Length);
        }

        public virtual void SetVisibility(bool newState)
        {
            IsVisible = newState;
            foreach (var rend in _renderers)
            {
                if (rend != null)
                    rend.enabled = newState;
            }
            OnVisibilityChanged?.Invoke();
        }

        public virtual void ResetObject()
        {
            Fade = 1f;

            foreach (var hitbox in Hitboxes)
            {
                HitboxIdentity.Instances.Remove(hitbox);
            }
        }

        public virtual void SpawnObject()
        {
            OwnerHub = ReferenceHub.GetHub(transform.root.gameObject);

            foreach (var hitbox in Hitboxes)
            {
                HitboxIdentity.Instances.Add(hitbox);
                hitbox.SetColliders(!OwnerHub.isLocalPlayer);
            }
        }

        public void SetOriginalMaterials()
        {
            int num = _renderers.Length;
            if (_copyMaterialsNonAlloc.Length < num)
                _copyMaterialsNonAlloc = new Material[num];

            _originalMaterials ??= new RendererMaterialPair[num];

            for (int i = 0; i < num; i++)
            {
                Renderer renderer = _renderers[i];
                if (renderer == null)
                    continue;

                Material sharedMaterial = renderer.sharedMaterial;
                _originalMaterials[i] = new RendererMaterialPair(renderer, sharedMaterial);
            }
        }

        public void RestoreOriginalMaterials()
        {
            if (_originalMaterials == null)
                return;

            foreach (var pair in _originalMaterials)
            {
                if (pair.Rend != null)
                    pair.Rend.sharedMaterial = pair.Mat;
            }

            _fadeableMaterials = null;
        }

        public virtual void OnTreadmillInitialized() { }
    }
}