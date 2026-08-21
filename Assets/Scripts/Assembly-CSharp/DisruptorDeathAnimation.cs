using System.Collections.Generic;

using DeathAnimations;
using UnityEngine;

public class DisruptorDeathAnimation : DeathAnimation
{
    private const float DisolveSpeed = 1.5f;

    private const int TargetLayerMask = 1;

    private const float FlySpeed = 2f;

    private static readonly int DisintegrateId;

    private static readonly int BurnRampId;

    private static readonly int DissolveId;

    [SerializeField]
    private Material _templateMaterial;

    private List<Material> _materials;

    private bool _initialize;

    private float _timer;

    static DisruptorDeathAnimation()
    {
        DisintegrateId = Shader.PropertyToID("_Disintegrate");
        BurnRampId     = Shader.PropertyToID("_BurnRamp");
        DissolveId     = Shader.PropertyToID("_DisolveGuide");
    }

    protected override void OnAnimationStarted()
    {
        PlayerStatsSystem.DamageHandlerBase handler = TargetRagdoll.Info.Handler;

        if (!(handler is PlayerStatsSystem.DisruptorDamageHandler))
        {
            enabled = false;
            return;
        }

        _materials = new List<Material>();

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null)
                continue;

            // Particle systems don't use the dissolve shader; the original just
            // hides them immediately instead of swapping their materials.
            if (renderer is ParticleSystemRenderer)
            {
                renderer.enabled = false;
                continue;
            }

            Material[] sharedMaterials = renderer.sharedMaterials;

            renderer.gameObject.layer = TargetLayerMask;

            Material[] newMaterials = new Material[sharedMaterials.Length];
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                // Rebuilt ragdoll prefabs can have empty material slots; the original
                // assets never did, so guard against a null source/template instead of
                // throwing ArgumentNullException("source") in the Material copy ctor.
                Material source = sharedMaterials[i];
                if (source == null)
                {
                    newMaterials[i] = null;
                    continue;
                }

                Material mat = new Material(source);
                if (_templateMaterial != null)
                {
                    mat.shader = _templateMaterial.shader;
                    mat.SetTexture(BurnRampId,  _templateMaterial.GetTexture(BurnRampId));
                    mat.SetTexture(DissolveId,  _templateMaterial.GetTexture(DissolveId));
                }
                newMaterials[i] = mat;
                _materials.Add(mat);
            }

            renderer.materials = newMaterials;
            // non-null mats were already collected into _materials above; this keeps
            // _materials free of nulls so Update()'s SetFloat loop won't NRE.
            // Renderer stays enabled — the dissolve shader (_Disintegrate) is what
            // fades it out over DisolveSpeed seconds in Update(), not an instant hide.
        }

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.useGravity = false;
            rb.linearVelocity   = rb.linearVelocity / FlySpeed + Vector3.up * Random.value;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        _initialize = true;
    }

    private void Update()
    {
        if (!_initialize)
            return;

        _timer += Time.deltaTime;

        float dissolve = Mathf.Clamp01(_timer / DisolveSpeed);
        foreach (Material mat in _materials)
        {
            mat.SetFloat(DisintegrateId, dissolve);
        }

        if (_timer > DisolveSpeed)
        {
            enabled = false;
        }
    }
}