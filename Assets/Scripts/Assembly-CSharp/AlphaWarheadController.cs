using Achievements;
using Footprinting;
using GameCore;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Usables.Scp244;
using LightContainmentZoneDecontamination;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using Respawning;
using Subtitles;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Networking;
using Utils.NonAllocLINQ;

public class AlphaWarheadController : NetworkBehaviour
{
    [Serializable]
    private class DetonationScenario
    {
        public AudioClip Clip;

        public int TimeToDetonate;

        public int AdditionalTime;

        public int TotalTime => TimeToDetonate + AdditionalTime;
    }

    private AudioSource _alarmSource;

    private bool _doorsAlreadyOpen;

    private bool _blastDoorsShut;

    private bool _openDoors;

    private int _cooldown;

    private bool _isAutomatic;

    private bool _alreadyDetonated;

    private bool _fogEnabled;

    private float _autoDetonateTime;

    private bool _autoDetonate;

    private bool _autoDetonateLock;

    private Footprint _triggeringPlayer;

    private AlphaWarheadSyncInfo _prevInfo;

    [SerializeField]
    private DetonationScenario[] _startScenarios;

    [SerializeField]
    private DetonationScenario[] _resumeScenarios;

    [SerializeField]
    private AudioClip _cancelSound;

    [SerializeField]
    private int _defaultScenarioId;

    [SyncVar]
    public AlphaWarheadSyncInfo Info;

    [SyncVar]
    public double CooldownEndTime;

    internal static bool AutoWarheadBroadcastEnabled;

    internal static string WarheadBroadcastMessage;

    internal static string WarheadExplodedBroadcastMessage;

    internal static ushort WarheadBroadcastMessageTime;

    internal static ushort WarheadExplodedBroadcastMessageTime;

    internal static bool LockGatesOnCountdown;

    public const float FacilityDetectionThreshold = 900f;

    private const float DetonationTokenReward = 6f;

    private const int WarheadDetonationTeamRespawnTimer = 165;

    private DetonationScenario CurScenario => (Info.ResumeScenario ? _resumeScenarios : _startScenarios)[Info.ScenarioId];

    public int WarheadKills { get; private set; }

    public bool IsLocked { get; set; }

    public static AlphaWarheadController Singleton { get; private set; }

    public static bool SingletonSet { get; private set; }

    public static ReferenceHub WarheadTriggeredby
    {
        get
        {
            if (!SingletonSet)
            {
                return null;
            }
            return Singleton._triggeringPlayer.Hub;
        }
    }

    public static bool Detonated
    {
        get
        {
            if (InProgress)
            {
                return TimeUntilDetonation == 0f;
            }
            return false;
        }
    }

    public static bool InProgress
    {
        get
        {
            if (SingletonSet)
            {
                return Singleton.Info.InProgress;
            }
            return false;
        }
    }

    public static float TimeUntilDetonation => Mathf.Max(0f, (float)(Singleton.Info.StartTime + (double)Singleton.CurScenario.TotalTime - NetworkTime.time));

    public static event Action<bool> OnProgressChanged;

    public static event Action OnDetonated;

    private void Start()
    {
        Singleton = this;
        SingletonSet = true;
        _alarmSource = GetComponent<AudioSource>();
        if (!NetworkServer.active)
        {
            return;
        }
        CooldownEndTime = 0.0;
        _autoDetonateTime = ConfigFile.ServerConfig.GetFloat("auto_warhead_start_minutes") * 60f;
        _autoDetonate = _autoDetonateTime > 0f;
        _autoDetonateLock = ConfigFile.ServerConfig.GetBool("auto_warhead_lock");
        _openDoors = ConfigFile.ServerConfig.GetBool("open_doors_on_countdown", def: true);
        _cooldown = ConfigFile.ServerConfig.GetInt("warhead_cooldown", 40);
        AlphaWarheadSyncInfo networkInfo = default;
        int num = ConfigFile.ServerConfig.GetInt("warhead_tminus_start_duration", 90);
        networkInfo.ScenarioId = _defaultScenarioId;
        for (byte b = 0; b < _startScenarios.Length; b++)
        {
            if (_startScenarios[b].TimeToDetonate == num)
            {
                networkInfo.ScenarioId = b;
            }
        }
        Info = networkInfo;
        ConfigFile.OnConfigReloaded = (Action)Delegate.Combine(ConfigFile.OnConfigReloaded, new Action(OnConfigReloaded));
    }

    private void OnDestroy()
    {
        SingletonSet = false;
        ConfigFile.OnConfigReloaded = (Action)Delegate.Remove(ConfigFile.OnConfigReloaded, new Action(OnConfigReloaded));
    }
    private void Update()
    {
        if (Info != _prevInfo)
        {
            OnInfoUpdated();
            _prevInfo = Info;
        }
        FlickerableLightController.WarheadEnabled = Info.InProgress;
        UpdateFog();
        ServerUpdateDetonationTime();
        ServerUpdateAutonuke();
    }

    private bool TryGetBroadcaster(out Broadcast broadcaster)
    {
        broadcaster = null;
        if (ReferenceHub.TryGetLocalHub(out var hub))
        {
            return hub.TryGetComponent<Broadcast>(out broadcaster);
        }
        return false;
    }

    private void OnInfoUpdated()
    {
        bool inProgress = Info.InProgress;
        if (inProgress != _prevInfo.InProgress)
        {
            AlphaWarheadController.OnProgressChanged?.Invoke(inProgress);
        }
        _alarmSource.Stop();
        if (!inProgress)
        {
            _alarmSource.PlayOneShot(_cancelSound);
            return;
        }
        _alarmSource.volume = 1f;
        _alarmSource.clip = CurScenario.Clip;
        float num = (float)(NetworkTime.time - Info.StartTime);
        if (num < 0f)
        {
            _alarmSource.PlayDelayed(0f - num);
        }
        else if (num < _alarmSource.clip.length)
        {
            _alarmSource.Play();
            _alarmSource.time = num;
        }
    }

    public void ForceTime(float remaining)
    {
        InstantPrepare();
        StartDetonation(isAutomatic: false, suppressSubtitles: true);
        AlphaWarheadSyncInfo info = Info;
        remaining -= (float)CurScenario.TotalTime;
        info.StartTime = NetworkTime.time + (double)remaining;
        Info = info;
    }

    public void InstantPrepare()
    {
        AlphaWarheadSyncInfo info = Info;
        info.StartTime = 0.0;
        Info = info;
        CooldownEndTime = 0.0;
    }

    public void StartDetonation(bool isAutomatic = false, bool suppressSubtitles = false, ReferenceHub trigger = null)
    {
        if (Info.InProgress || CooldownEndTime > NetworkTime.time || IsLocked)
        {
            return;
        }
        _isAutomatic = isAutomatic;
        _alreadyDetonated = false;
        if (isAutomatic)
        {
            IsLocked |= _autoDetonateLock;
            if (!_alreadyDetonated && !Info.InProgress && AutoWarheadBroadcastEnabled && TryGetBroadcaster(out var broadcaster))
            {
                broadcaster.RpcAddElement(WarheadBroadcastMessage, WarheadBroadcastMessageTime, Broadcast.BroadcastFlags.Normal);
            }
            _autoDetonate = false;
        }
        _doorsAlreadyOpen = false;
        ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Countdown started.", ServerLogs.ServerLogType.GameEvent);
        _triggeringPlayer = new Footprint(trigger);
        AlphaWarheadSyncInfo info = Info;
        info.StartTime = NetworkTime.time;
        Info = info;
        if (!suppressSubtitles)
        {
            SubtitleType subtitle = (Info.ResumeScenario ? SubtitleType.AlphaWarheadResumed : SubtitleType.AlphaWarheadEngage);
            new SubtitleMessage(new SubtitlePart(subtitle, CurScenario.TimeToDetonate.ToString())).SendToAuthenticated();
        }
    }


    public void CancelDetonation()
    {
        CancelDetonation(null);
    }

    public void CancelDetonation(ReferenceHub disabler)
    {
        if (!Info.InProgress || TimeUntilDetonation <= 10f || IsLocked)
        {
            return;
        }
        ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Detonation cancelled.", ServerLogs.ServerLogType.GameEvent);
        if (TimeUntilDetonation <= 15f && disabler != null)
        {
            AchievementHandlerBase.ServerAchieve(disabler.connectionToClient, AchievementName.ThatWasClose);
        }
        AlphaWarheadSyncInfo info = Info;
        info.StartTime = 0.0;
        int num = (int)Mathf.Min(TimeUntilDetonation, CurScenario.TimeToDetonate);
        int num2 = int.MaxValue;
        info.ResumeScenario = true;
        for (int i = 0; i < _resumeScenarios.Length; i++)
        {
            int num3 = _resumeScenarios[i].TimeToDetonate - num;
            if (num3 >= 0 && num3 <= num2)
            {
                num2 = num3;
                info.ScenarioId = i;
            }
        }
        Info = info;
        CooldownEndTime = NetworkTime.time + (double)_cooldown;
        DoorEventOpenerExtension.TriggerAction(DoorEventOpenerExtension.OpenerEventType.WarheadCancel);
        if (NetworkServer.active)
        {
            _isAutomatic = false;
            new SubtitleMessage(new SubtitlePart(SubtitleType.AlphaWarheadCancelled, (string[])null)).SendToAuthenticated();
        }
    }

    private void Detonate()
    {
        AlphaWarheadController.OnDetonated?.Invoke();
        if (_isAutomatic && !_alreadyDetonated && !Info.InProgress && AutoWarheadBroadcastEnabled && TryGetBroadcaster(out var broadcaster))
        {
            broadcaster.RpcAddElement(WarheadExplodedBroadcastMessage, WarheadExplodedBroadcastMessageTime, Broadcast.BroadcastFlags.Normal);
        }
        ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Warhead detonated.", ServerLogs.ServerLogType.GameEvent);
        if (DecontaminationController.Singleton.DecontaminationOverride != DecontaminationController.DecontaminationStatus.Disabled)
        {
            ServerLogs.AddLog(ServerLogs.Modules.Administrative, "LCZ decontamination has been disabled by detonation of the Alpha Warhead.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
            DecontaminationController.Singleton.DecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;
        }
        _alreadyDetonated = true;
        RespawnManager.Singleton.TimeTillRespawn = 165;
        HashSet<Team> hashSet = new();
        foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
        {
            PlayerRoleBase currentRole = allHub.roleManager.CurrentRole;
            if (allHub.IsAlive() && (currentRole is not IFpcRole fpcRole || CanBeDetonated(fpcRole.FpcModule.Position)))
            {
                hashSet.Add(allHub.GetTeam());
                allHub.playerStats.DealDamage(new WarheadDamageHandler());
                WarheadKills++;
            }
        }
        foreach (Scp244DeployablePickup instance in Scp244DeployablePickup.Instances)
        {
            if (CanBeDetonated(instance.transform.position, includeOnlyLifts: true))
            {
                instance.DestroySelf();
            }
        }
        foreach (DoorVariant allDoor in DoorVariant.AllDoors)
        {
            if (allDoor is ElevatorDoor elevatorDoor)
            {
                elevatorDoor.ActiveLocks = (ushort)(elevatorDoor.ActiveLocks | 4);
            }
        }
        RpcShake(achieve: true);
        if (!_triggeringPlayer.IsSet)
        {
            return;
        }
        switch (_triggeringPlayer.Role.GetFaction())
        {
            case Faction.FoundationEnemy:
                if (!hashSet.Contains(Team.ClassD))
                {
                    RespawnTokensManager.GrantTokens(SpawnableTeamType.ChaosInsurgency, 6f);
                }
                break;
            case Faction.FoundationStaff:
                if (!hashSet.Contains(Team.Scientists))
                {
                    RespawnTokensManager.GrantTokens(SpawnableTeamType.NineTailedFox, 6f);
                }
                break;
        }
    }

    private static bool CanBeDetonated(Vector3 pos, bool includeOnlyLifts = false)
    {
        if (pos.y < 900f && !includeOnlyLifts)
        {
            return true;
        }
        foreach (List<ElevatorDoor> value in ElevatorDoor.AllElevatorDoors.Values)
        {
            if (value.Count != 0 && value[0].TargetPanel.AssignedChamber.WorldspaceBounds.Contains(pos))
            {
                return true;
            }
        }
        return false;
    }

    private void OnConfigReloaded()
    {
        if (!InProgress)
        {
            return;
        }
        foreach (DoorVariant allDoor in DoorVariant.AllDoors)
        {
            if (allDoor is PryableDoor pryableDoor)
            {
                if (LockGatesOnCountdown)
                {
                    pryableDoor.TargetState = true;
                    pryableDoor.ServerChangeLock(DoorLockReason.Warhead, newState: true);
                }
                else
                {
                    pryableDoor.ServerChangeLock(DoorLockReason.Warhead, newState: false);
                }
            }
        }
    }

    [ClientRpc]
    private void RpcShake(bool achieve)
    {
        ExplosionCameraShake.singleton?.Shake(1f);
        Transform currentCamera = MainCameraController.CurrentCamera;
        if (currentCamera == null)
        {
            return;
        }
        if (CanBeDetonated(currentCamera.position))
        {
            _fogEnabled = true;
            PostProcessing.FogController.EnableFogType(PostProcessing.FogType.Nuke, 0.6f);
            BloodEffectsSystem bloodSystem = BloodEffectsSystem.LocalPlayerSingleton;
            if (bloodSystem != null)
            {
                bloodSystem.DisableHurtEffect = true;
            }
        }
        else if (achieve && ReferenceHub.TryGetLocalHub(out var hub) && hub.IsAlive())
        {
            if (Achievements.AchievementManager.Achievements.TryGetValue(Achievements.AchievementName.TMinus, out var achievement))
            {
                achievement.Achieve();
            }
        }
    }

    private void UpdateFog()
    {
        if (!_fogEnabled)
        {
            return;
        }
        PostProcessing.FogController fogController = PostProcessing.FogController.Singleton;
        if (fogController == null)
        {
            return;
        }
        PostProcessing.FogSetting fogSetting = fogController.GetFogSetting(PostProcessing.FogType.Nuke);
        if (fogSetting == null)
        {
            return;
        }
        if (fogSetting.IsEnabled)
        {
            if (fogSetting.Weight >= 1f)
            {
                PostProcessing.FogController.DisableFogType(PostProcessing.FogType.Nuke, 15f);
            }
        }
        else
        {
            _fogEnabled = false;
            BloodEffectsSystem bloodSystem = BloodEffectsSystem.LocalPlayerSingleton;
            if (bloodSystem != null)
            {
                bloodSystem.DisableHurtEffect = false;
            }
        }
    }

    [ServerCallback]
    private void ServerUpdateAutonuke()
    {
        if (NetworkServer.active && NetworkServer.active && RoundStart.RoundStarted && _autoDetonate && !_alreadyDetonated && !Info.InProgress && !(RoundStart.RoundLength.TotalSeconds < (double)_autoDetonateTime))
        {
            StartDetonation(isAutomatic: true);
        }
    }

    [ServerCallback]
    private void ServerUpdateDetonationTime()
    {
        if (!NetworkServer.active || !NetworkServer.active || !Info.InProgress)
        {
            return;
        }
        if (!_blastDoorsShut && TimeUntilDetonation < 2f)
        {
            _blastDoorsShut = true;
            BlastDoor.Instances.ForEach(delegate (BlastDoor x)
            {
                x.SetClosed(prev: false, b: true);
            });
        }
        if (_openDoors && !_doorsAlreadyOpen && TimeUntilDetonation < (float)CurScenario.TimeToDetonate)
        {
            _doorsAlreadyOpen = true;
            DoorEventOpenerExtension.TriggerAction(DoorEventOpenerExtension.OpenerEventType.WarheadStart);
        }
        if (!_alreadyDetonated && !(TimeUntilDetonation > 0f))
        {
            Detonate();
        }
    }
}
