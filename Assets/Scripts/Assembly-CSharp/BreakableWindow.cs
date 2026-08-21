using Footprinting;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BreakableWindow : NetworkBehaviour, IDestructible
{
    public struct BreakableWindowStatus : IEquatable<BreakableWindowStatus>
    {
        public Vector3 position;

        public Quaternion rotation;

        public bool broken;

        public readonly bool IsEqual(BreakableWindowStatus stat)
        {
            if (position == stat.position && rotation == stat.rotation)
            {
                return broken == stat.broken;
            }
            return false;
        }

        public readonly bool Equals(BreakableWindowStatus other)
        {
            if (position == other.position && rotation == other.rotation)
            {
                return broken == other.broken;
            }
            return false;
        }

        public override readonly bool Equals(object obj)
        {
            if (obj is BreakableWindowStatus other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (((position.GetHashCode() * 397) ^ rotation.GetHashCode()) * 397) ^ broken.GetHashCode();
        }

        public static bool operator ==(BreakableWindowStatus left, BreakableWindowStatus right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BreakableWindowStatus left, BreakableWindowStatus right)
        {
            return !left.Equals(right);
        }
    }

    public GameObject template;

    public Transform parent;

    public Vector3 size;

    [SerializeField]
    private bool _preventScpDamage;

    public Footprint LastAttacker;

    private BreakableWindowStatus prevStatus;

    [SyncVar]
    public BreakableWindowStatus syncStatus;

    public float health = 30f;

    public bool isBroken;

    private readonly List<MeshRenderer> meshRenderers = new();

    private Transform _transform;

    public uint NetworkId => base.netId;

    public Vector3 CenterOfMass => base.transform.position;

    [ServerCallback]
    private void UpdateStatus(BreakableWindowStatus s)
    {
        if (NetworkServer.active)
        {
            syncStatus = s;
        }
    }

    [ServerCallback]
    private void ServerDamageWindow(float damage)
    {
        // Guard against re-breaking: when several projectiles land in the same
        // frame (shotgun/disruptor/full-auto) every hit after health hits 0 would
        // otherwise start another BreakWindow coroutine, each spawning a full set
        // of shard rigidbodies — tanking the framerate and breaking the glass
        // "once per bullet". BreakWindow sets isBroken=true synchronously (it runs
        // up to its first yield inside StartCoroutine), so this guard blocks any
        // follow-up hits the same frame.
        if (NetworkServer.active && !isBroken)
        {
            health -= damage;
            if (health <= 0f)
            {
                StartCoroutine(BreakWindow());
            }
        }
    }

    private void Awake()
    {
        meshRenderers.AddRange(GetComponentsInChildren<MeshRenderer>());
        _transform = base.transform;
        GetComponent<Collider>().enabled = false;
        Invoke(nameof(EnableColliders), 1f);
    }

    private void EnableColliders()
    {
        GetComponent<Collider>().enabled = true;
    }


    private void Update()
    {
        _transform.GetPositionAndRotation(out Vector3 position, out Quaternion rotation);
        if (position == syncStatus.position && rotation == syncStatus.rotation && isBroken == syncStatus.broken)
        {
            return;
        }
        if (NetworkServer.active)
        {
            BreakableWindowStatus s = new()
            {
                position = position,
                rotation = rotation,
                broken = isBroken
            };
            UpdateStatus(s);
            return;
        }
        if (!isBroken && syncStatus.broken)
        {
            StartCoroutine(BreakWindow());
        }
        _transform.SetPositionAndRotation(syncStatus.position, syncStatus.rotation);
        isBroken = syncStatus.broken;
    }

    private void LateUpdate()
    {
        for (int num = meshRenderers.Count - 1; num >= 0; num--)
        {
            MeshRenderer meshRenderer = meshRenderers[num];
            meshRenderer.shadowCastingMode = (isBroken ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.Off);
            if (isBroken)
            {
                meshRenderers.RemoveAt(num);
                UnityEngine.Object.Destroy(meshRenderer);
            }
            meshRenderer.gameObject.layer = (isBroken ? 28 : 14);
        }
    }

    private IEnumerator BreakWindow()
    {
        isBroken = true;
        if (ServerStatic.IsDedicated)
        {
            yield break;
        }
        Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            componentsInChildren[i].enabled = false;
        }
        GameObject gameObject = Instantiate(template, parent);
        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        Rigidbody[] rbs = gameObject.GetComponentsInChildren<Rigidbody>();
        List<Vector3> scales = NorthwoodLib.Pools.ListPool<Vector3>.Shared.Rent();
        Rigidbody[] array = rbs;
        foreach (Rigidbody rigidbody in array)
        {
            rigidbody.angularVelocity = new Vector3(UnityEngine.Random.Range(-360, 360), UnityEngine.Random.Range(-360, 360), UnityEngine.Random.Range(-360, 360));
            rigidbody.linearVelocity = new Vector3(UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-2, 2));
            scales.Add(rigidbody.transform.localScale);
        }
        for (int j = 0; j < 250; j++)
        {
            for (int k = 0; k < scales.Count; k++)
            {
                rbs[k].transform.localScale = Vector3.Lerp(scales[k], scales[k] / 2f, (float)j / 75f);
            }
            yield return null;
        }
        for (float i2 = 0f; i2 < 150f; i2 += 1f)
        {
            for (int l = 0; l < scales.Count; l++)
            {
                rbs[l].transform.localScale = Vector3.Lerp(scales[l] / 2f, Vector3.zero, i2 / 150f);
            }
            yield return null;
        }
        NorthwoodLib.Pools.ListPool<Vector3>.Shared.Return(scales);
        array = rbs;
        for (int i = 0; i < array.Length; i++)
        {
            Destroy(array[i].gameObject, 1f);
        }
    }

    private bool CheckDamagePerms(RoleTypeId roleType)
    {
        if (!_preventScpDamage)
        {
            return true;
        }
        if (!PlayerRoleLoader.TryGetRoleTemplate<PlayerRoleBase>(roleType, out var result))
        {
            return false;
        }
        return result.Team != Team.SCPs;
    }

    public bool Damage(float damage, DamageHandlerBase handler, Vector3 pos)
    {
        if (handler is AttackerDamageHandler attackerDamageHandler)
        {
            if (!CheckDamagePerms(attackerDamageHandler.Attacker.Role))
            {
                return false;
            }
            LastAttacker = attackerDamageHandler.Attacker;
        }
        ServerDamageWindow(damage);
        return true;
    }

}
