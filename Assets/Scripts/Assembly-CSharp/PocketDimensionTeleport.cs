public class PocketDimensionTeleport : global::Mirror.NetworkBehaviour
{
    public enum PDTeleportType
    {
        Killer = 0,
        Exit = 1
    }

    public const float DisabledDuration = 10f;

    public static bool DebugBool;

    private const float MinAliveDuration = 1f;

    private PocketDimensionTeleport.PDTeleportType _type;

    public static bool RefreshExit;

    public void SetType(PocketDimensionTeleport.PDTeleportType t)
    {
        _type = t;
    }

    public PocketDimensionTeleport.PDTeleportType GetTeleportType()
    {
        return _type;
    }

    [global::Mirror.ServerCallback]
    private void OnTriggerEnter(global::UnityEngine.Collider other)
    {
        if (!global::Mirror.NetworkServer.active)
        {
            return;
        }
        global::Mirror.NetworkIdentity component = other.GetComponent<global::Mirror.NetworkIdentity>();
        if (component == null || !ReferenceHub.TryGetHubNetID(component.netId, out var hub) || hub.roleManager.CurrentRole.ActiveTime < 1f)
        {
            return;
        }
        if ((_type == PocketDimensionTeleport.PDTeleportType.Killer || AlphaWarheadController.Detonated) && !DebugBool)
        {
            hub.playerStats.DealDamage(new global::PlayerStatsSystem.UniversalDamageHandler(-1f, global::PlayerStatsSystem.DeathTranslations.PocketDecay));
        }
        else if ((_type == PocketDimensionTeleport.PDTeleportType.Exit || DebugBool) && hub.roleManager.CurrentRole is global::PlayerRoles.FirstPersonControl.IFpcRole fpcRole)
        {
            fpcRole.FpcModule.ServerOverridePosition(global::PlayerRoles.PlayableScps.Scp106.Scp106PocketExitFinder.GetBestExitPosition(fpcRole), global::UnityEngine.Vector3.zero);
            hub.playerEffectsController.EnableEffect<global::CustomPlayerEffects.Disabled>(10f, addDuration: true);
            hub.playerEffectsController.DisableEffect<global::CustomPlayerEffects.Corroding>();
            global::Achievements.AchievementHandlerBase.ServerAchieve(component.connectionToClient, global::Achievements.AchievementName.LarryFriend);
            global::MapGeneration.ImageGenerator.pocketDimensionGenerator.GenerateRandom();
        }
    }
}
