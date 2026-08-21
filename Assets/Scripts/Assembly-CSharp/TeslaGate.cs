using System;
using System.Collections.Generic;
using Hazards;
using MapGeneration;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

public class TeslaGate : NetworkBehaviour
{
    public delegate void BurstComplete(ReferenceHub hub, TeslaGate teslaGate);

    public Vector3 localPosition;
    public Vector3 localRotation;
    public GameObject[] killers;
    public LayerMask killerMask;

    public bool InProgress;

    public Animator ledLights;
    public ParticleSystem[] windupParticles;
    public ParticleSystem[] shockParticles;
    public ParticleSystem[] smokeParticles;
    public RoomIdentifier Room;

    private Vector3 _position;

    [Header("Parameters")]
    public Vector3 sizeOfKiller;
    public float sizeOfTrigger;
    public float distanceToIdle;
    public float windupTime;
    public float cooldownTime;

    [Header("Idle Loop")]
    public bool isIdling;
    public AudioSource loopSource;
    public AudioClip idleStart;
    public AudioClip idleLoop;
    public AudioClip idleEnd;

    [Header("Audio")]
    public AudioSource source;
    public AudioClip[] clipsWarmup;
    public AudioClip[] clipsShock;

    public bool showGizmos;

    private sbyte _gateId = -1;

    [SyncVar]
    public float InactiveTime;

    public List<TantrumEnvironmentalHazard> TantrumsToBeDestroyed = new();

    private bool next079burst;

    private static readonly int _animatorShockHash = Animator.StringToHash("ShockActive");
    private static readonly int _animatorIdleHash = Animator.StringToHash("IdleActive");

    private Collider[] _colls = new Collider[70];

    public Vector3 Position
    {
        get
        {
            if (_position == Vector3.zero)
            {
                _position = base.transform.position;
            }
            return _position;
        }
    }

    public static event BurstComplete OnBurstComplete;
    public static event Action<TeslaGate> OnBursted;

    public void ServerSideCode()
    {
        if (!InProgress)
        {
            Timing.RunCoroutine(ServerSideWaitForAnimation());
            RpcPlayAnimation();
        }
    }
    private IEnumerator<float> ServerSideWaitForAnimation()
    {
        InProgress = true;
        yield return Timing.WaitForSeconds(windupTime);

        if (TantrumsToBeDestroyed.Count > 0)
        {
            foreach (var tantrum in TantrumsToBeDestroyed)
            {
                if (tantrum != null)
                {
                    tantrum.PlaySizzle = true;
                    tantrum.ServerDestroy();
                }
            }
            TantrumsToBeDestroyed.Clear();
        }

        yield return Timing.WaitForSeconds(cooldownTime);
        InProgress = false;
    }

    public void ServerSideIdle(bool shouldIdle)
    {
        if (shouldIdle)
            RpcDoIdle();
        else
            RpcDoneIdling();
    }

    private void Update()
    {
        if (ledLights != null)
        {
            ledLights.SetBool(_animatorShockHash, InProgress);
            ledLights.SetBool(_animatorIdleHash, isIdling);
        }
    }

    private void Start()
    {
        ObtainId();
    }

    public void ClientSideCode()
    {
        base.transform.localPosition = localPosition;
        base.transform.localRotation = Quaternion.Euler(localRotation);
        if (ledLights != null)
        {
            ledLights.SetBool(_animatorShockHash, InProgress);
            ledLights.SetBool(_animatorIdleHash, isIdling);
        }
    }

    [ClientRpc]
    private void RpcDoIdle()
    {
        if (!isIdling)
        {
            isIdling = true;
            loopSource.PlayOneShot(idleStart);
            loopSource.PlayDelayed(idleStart.length);
        }
        foreach (var particle in windupParticles)
        {
            if (!particle.isPlaying) particle.Play();
        }
    }

    [ClientRpc]
    private void RpcDoneIdling()
    {
        if (isIdling)
        {
            isIdling = false;
            loopSource.Stop();
            loopSource.PlayOneShot(idleEnd);
            foreach (var particle in windupParticles)
            {
                particle.Stop();
            }
        }
    }

    [ClientRpc]
    private void RpcPlayAnimation()
    {
        Timing.RunCoroutine(_PlayAnimation(), Segment.FixedUpdate);
    }

    [ClientRpc]
    public void RpcInstantBurst()
    {
        next079burst = true;
        Timing.RunCoroutine(_PlayAnimation(), Segment.FixedUpdate);
    }

    private IEnumerator<float> _DoShock()
    {
        OnBursted?.Invoke(this);

        source.Stop();
        foreach (var clip in clipsShock)
        {
            if (clip != null) source.PlayOneShot(clip);
        }

        foreach (var p in windupParticles) if (p != null) p.Play();
        foreach (var p in shockParticles) if (p != null) p.Play();

        ReferenceHub hub;
        while (!ReferenceHub.TryGetLocalHub(out hub))
        {
            yield return float.NegativeInfinity;
        }

        if (PlayerInHurtRange(hub.gameObject))
        {
            ElectrocutePlayer();
        }

        yield return Timing.WaitForSeconds(0.25f);

        if (PlayerInHurtRange(hub.gameObject))
        {
            ElectrocutePlayer();
        }

        yield return Timing.WaitForSeconds(0.25f);

        float smokeDuration = 0f;
        foreach (var p in smokeParticles)
        {
            if (p != null)
            {
                if (p.IsAlive() && smokeDuration <= 0f)
                {
                    smokeDuration = p.main.duration;
                }
                p.Play();
            }
        }

        if (!isIdling)
        {
            foreach (var p in windupParticles)
            {
                if (p != null) p.Stop();
            }
        }

        yield return Timing.WaitForSeconds(smokeDuration);
    }

    private IEnumerator<float> _PlayAnimation()
    {
        bool is079 = next079burst;
        next079burst = false;

        if (!is079)
        {
            foreach (var clip in clipsWarmup) source.PlayOneShot(clip);
            foreach (var p in windupParticles) p.Play();
            yield return Timing.WaitForSeconds(windupTime);
        }

        Timing.RunCoroutine(_DoShock());

        yield return Timing.WaitForSeconds(is079 ? 0.5f : cooldownTime);
    }


    public bool PlayerInRange(ReferenceHub player)
    {
        if (player.roleManager.CurrentRole is ITeslaControllerRole { CanActivateShock: false })
        {
            return false;
        }
        if (!(player.roleManager.CurrentRole is IFpcRole fpcRole))
        {
            return false;
        }
        return InRange(fpcRole.FpcModule.Position);
    }

    private bool InRange(Vector3 position)
    {
        return Vector3.Distance(Position, position) < sizeOfTrigger;
    }

    public bool PlayerInIdleRange(ReferenceHub player)
    {
        if (!player.IsAlive())
        {
            return false;
        }
        if (player.roleManager.CurrentRole is ITeslaControllerRole { CanActivateIdle: false })
        {
            return false;
        }
        if (player.roleManager.CurrentRole is IFpcRole fpcRole)
        {
            return Vector3.Distance(Position, fpcRole.FpcModule.Position) < distanceToIdle;
        }
        if (player.roleManager.CurrentRole is PlayerRoles.PlayableScps.Scp079.Scp079Role scp079Role)
        {
            return RoomIdUtils.IsTheSameRoom(scp079Role.CameraPosition, Position);
        }
        return false;
    }

    public bool PlayerInHurtRange(GameObject player)
    {
        for (int i = 0; i < killers.Length; i++)
        {
            GameObject killer = killers[i];
            if (killer != null)
            {
                int hitCount = Physics.OverlapBoxNonAlloc(killer.transform.position + Vector3.up * (sizeOfKiller.y / 2f), sizeOfKiller / 2f, _colls, Quaternion.identity, killerMask);

                for (int j = 0; j < hitCount; j++)
                {
                    if (_colls[j].gameObject == player)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void ElectrocutePlayer()
    {
        if (_gateId == -1)
        {
            ObtainId();
        }
        NetworkClient.Send(new TeslaHitMsg(_gateId));
    }

    private void ObtainId()
    {
        if (TeslaGateController.Singleton != null)
        {
            List<TeslaGate> gates = TeslaGateController.Singleton.TeslaGates;
            for (int i = 0; i < gates.Count; i++)
            {
                if (gates[i] == this)
                {
                    _gateId = (sbyte)i;
                    return;
                }
            }
        }
        GameCore.Console.AddLog("Tesla Gate could not obtain its ID!", Color.red);
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            foreach (var killer in killers)
            {
                if (killer != null)
                    Gizmos.DrawCube(killer.transform.position + Vector3.up * (sizeOfKiller.y / 2f), sizeOfKiller);
            }
            Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
            Gizmos.DrawSphere(Position, sizeOfTrigger);
        }
    }
}