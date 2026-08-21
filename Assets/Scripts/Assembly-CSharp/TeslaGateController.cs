using System.Collections.Generic;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using Subtitles;
using UnityEngine;

public class TeslaGateController : NetworkBehaviour
{
    public List<TeslaGate> TeslaGates;

    public static TeslaGateController Singleton { get; private set; }

    private static void ServerReceiveMessage(NetworkConnection conn, TeslaHitMsg msg)
    {
        if (ReferenceHub.TryGetHubNetID(conn.identity.netId, out var hub))
        {
            if (msg.TeslaGateId < 0 || msg.TeslaGateId >= Singleton.TeslaGates.Count)
            {
                hub.gameConsoleTransmission.SendToClient($"Received invalid tesla gate id {msg.TeslaGateId}!", "red");
                return;
            }
            TeslaGate teslaGate = Singleton.TeslaGates[msg.TeslaGateId];
            if (Vector3.Distance(teslaGate.transform.position, hub.transform.position) > teslaGate.sizeOfTrigger * 2.2f)
            {
                hub.gameConsoleTransmission.SendToClient("You are too far from a tesla gate!", "red");
                return;
            }
            // Hardening (deviation from v12): only accept a hit while the gate is actually
            // discharging. Tesla damage is client-authoritative and can only ever hit the
            // sender, so the exploit ceiling is self-termination near a gate; gating on
            // InProgress rejects hits sent outside a real shock without touching the
            // client-side hurt-window (which stays authoritative to avoid rejecting valid hits).
            if (!teslaGate.InProgress)
            {
                return;
            }
            DamageHandlerBase.CassieAnnouncement cassieAnnouncement = new DamageHandlerBase.CassieAnnouncement();
            cassieAnnouncement.Announcement = "SUCCESSFULLY TERMINATED BY AUTOMATIC SECURITY SYSTEM";
            cassieAnnouncement.SubtitleParts = new SubtitlePart[1]
            {
                new(SubtitleType.TerminatedBySecuritySystem, (string[])null)
            };
            DamageHandlerBase.CassieAnnouncement cassieAnnouncement2 = cassieAnnouncement;
            hub.playerStats.DealDamage(new UniversalDamageHandler(Random.Range(200, 300), DeathTranslations.Tesla, cassieAnnouncement2));
        }
    }

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        Timing.RunCoroutine(DelayedStopIdleParticles());
        NetworkServer.ReplaceHandler<TeslaHitMsg>(ServerReceiveMessage);
    }

    private IEnumerator<float> DelayedStopIdleParticles()
    {
        for (int i = 0; i < 15; i++)
        {
            yield return float.NegativeInfinity;
        }
        foreach (TeslaGate teslaGate in TeslaGates)
        {
            if (teslaGate == null || teslaGate.windupParticles == null)
            {
                continue;
            }
            ParticleSystem[] windupParticles = teslaGate.windupParticles;
            foreach (ParticleSystem particleSystem in windupParticles)
            {
                if (!(particleSystem == null))
                {
                    particleSystem.Stop();
                }
            }
        }
    }

    public void FixedUpdate()
    {
        if (NetworkServer.active)
        {
            foreach (TeslaGate teslaGate in TeslaGates)
            {
                if (teslaGate.isActiveAndEnabled)
                {
                    if (teslaGate.InactiveTime > 0f)
                    {
                        teslaGate.InactiveTime = Mathf.Max(0f, teslaGate.InactiveTime - Time.fixedDeltaTime);
                    }
                    else
                    {
                        bool flag = false;
                        bool flag2 = false;
                        foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
                        {
                            if (allHub.IsAlive())
                            {
                                if (!flag)
                                {
                                    flag = teslaGate.PlayerInIdleRange(allHub);
                                }
                                if (!flag2 && teslaGate.PlayerInRange(allHub) && !teslaGate.InProgress)
                                {
                                    flag2 = true;
                                }
                            }
                        }
                        if (flag2)
                        {
                            teslaGate.ServerSideCode();
                        }
                        if (flag != teslaGate.isIdling)
                        {
                            teslaGate.ServerSideIdle(flag);
                        }
                    }
                }
            }
            return;
        }
        foreach (TeslaGate teslaGate2 in TeslaGates)
        {
            teslaGate2.ClientSideCode();
        }
    }
}
