using Mirror;
using PlayerStatsSystem;
using UnityEngine;
using static PlayerStatsSystem.DamageHandlerBase;

public class CheckpointKiller : MonoBehaviour
{
    private const float MinAliveDuration = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkServer.active)
            return;

        Transform rootTransform = other.transform.root;
        GameObject rootObject = rootTransform.gameObject;

        if (!ReferenceHub.TryGetHub(rootObject, out ReferenceHub hub))
            return;

        if (hub.roleManager.CurrentRole.ActiveTime < MinAliveDuration)
            return;

        UniversalDamageHandler damageHandler = new UniversalDamageHandler(
            -1f,                                
            DeathTranslations.Crushed,             
            CassieAnnouncement.Default);            

        hub.playerStats.DealDamage(damageHandler);
    }
}