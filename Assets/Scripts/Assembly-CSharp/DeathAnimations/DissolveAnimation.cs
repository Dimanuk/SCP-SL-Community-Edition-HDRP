using UnityEngine;

namespace DeathAnimations
{
    public class DissolveAnimation : DeathAnimation
    {
        private static readonly int StoneIntensityId = Shader.PropertyToID("_DissolveAmount");

        private static readonly int GlossId = Shader.PropertyToID("_GlossMapScale");

        [Range(0f, 1f)]
        public float StoneIntensity;

        private Renderer _renderer;

        private MaterialPropertyBlock _materialPropertyBlock;

        private float _glossStartValue;

        protected override void OnAnimationStarted()
        {
            enabled = true;

            _renderer = GetComponent<Renderer>();
            _materialPropertyBlock = new MaterialPropertyBlock();

            _renderer.GetPropertyBlock(_materialPropertyBlock);
            _glossStartValue = _materialPropertyBlock.GetFloat(GlossId);
        }

        private void Update()
        {
            float dissolveAmount = Mathf.Clamp01(_materialPropertyBlock.GetFloat(StoneIntensityId) + Time.deltaTime);

            _materialPropertyBlock.SetFloat(StoneIntensityId, dissolveAmount);
            _materialPropertyBlock.SetFloat(GlossId, Mathf.Lerp(_glossStartValue, 0f, dissolveAmount));

            _renderer.SetPropertyBlock(_materialPropertyBlock);

            if (dissolveAmount >= StoneIntensity)
            {
                Destroy(this);
            }
        }

        static DissolveAnimation()
        {
            StoneIntensityId = Shader.PropertyToID("_DissolveAmount");
            GlossId = Shader.PropertyToID("_GlossMapScale");
        }
    }
}