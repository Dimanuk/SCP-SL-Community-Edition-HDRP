using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.Spectating;
using Subtitles;
using UnityEngine;
using UnityEngine.UI;
using MEC;
using CustomPlayerEffects;
using GameCore;
using System.Text;
using static Broadcast;

namespace LightContainmentZoneDecontamination
{
    public class DecontaminationController : NetworkBehaviour
    {
        public enum DecontaminationStatus : byte
        {
            None = 0,
            Disabled = 1,
            Forced = 2
        }

        [Serializable]
        public struct DecontaminationPhase
        {
            public enum PhaseFunction : byte
            {
                None = 0,
                GloballyAudible = 1,
                OpenCheckpoints = 2,
                Final = 3
            }

            public float TimeTrigger;
            public float GameTime;
            public AudioClip AnnouncementLine;
            public PhaseFunction Function;
        }

        public static DecontaminationController Singleton;

        public float TimeOffset;

        public DecontaminationPhase[] DecontaminationPhases;

        public DecontaminationClientTimer ClientTimer;

        public ImageGenerator LczGenerator;

        public AudioSource AnnouncementAudioSource;

        [SyncVar]
        public double RoundStartTime;

        [SyncVar(hook = nameof(OnChangeDisableDecontamination))]
        public DecontaminationStatus DecontaminationOverride;

        private DecontaminationPhase.PhaseFunction _curFunction;

        private int _nextPhase;

        private bool _stopUpdating;

        private bool _elevatorsDirty;

        private bool _decontaminationBegun;

        private float _justJoinedCooldown;

        private string _elevatorsLockedText = "ELEVATOR SYSTEM <color=#e00>DISABLED</color>";

        [SerializeField]
        private Text _checkpointHczA;

        [SerializeField]
        private Text _checkpointHczB;

        private const float LowerBoundLCZ = -200f;
        private const float UpperBoundLCZ = 200f;

        internal static bool AutoDeconBroadcastEnabled;
        internal static string DeconBroadcastDeconMessage;
        internal static ushort DeconBroadcastDeconMessageTime;

        // SyncVar dirty bits: 1 = RoundStartTime, 2 = DecontaminationOverride

        public static double GetServerTime
        {
            get
            {
                if (Singleton == null)
                    throw new NullReferenceException();
                
                return NetworkTime.time - Singleton.RoundStartTime + Singleton.TimeOffset;
            }
        }

        private bool IsAnnouncementHearable
        {
            get
            {
                if (!ReferenceHub.TryGetLocalHub(out var hub))
                    return false;

                if (_curFunction == DecontaminationPhase.PhaseFunction.Final ||
                    _curFunction == DecontaminationPhase.PhaseFunction.GloballyAudible)
                {
                    return true;
                }

                float y;
                if (hub.roleManager.CurrentRole is ICameraController cameraController)
                {
                    y = cameraController.CameraPosition.y;
                }
                else
                {
                    y = hub.transform.position.y;
                }

                return y > LowerBoundLCZ && y < UpperBoundLCZ;
            }
        }

        public bool IsDecontaminating
        {
            get
            {
                if (!NetworkServer.active)
                    return false;
                return _decontaminationBegun;
            }
        }

        private void Awake()
        {
            Singleton = this;
        }

        public void OnChangeDisableDecontamination(DecontaminationStatus oldValue, DecontaminationStatus newValue)
        {
            if (oldValue == newValue)
                return;

            switch (newValue)
            {
                case DecontaminationStatus.None:
                    DecontaminationGas.TurnedOn = false;
                    if (_checkpointHczA != null)
                        _checkpointHczA.text = string.Empty;
                    if (_checkpointHczB != null)
                        _checkpointHczB.text = string.Empty;
                    break;

                case DecontaminationStatus.Disabled:
                    DoorEventOpenerExtension.TriggerAction(DoorEventOpenerExtension.OpenerEventType.DeconReset);
                    DecontaminationGas.TurnedOn = false;
                    if (_checkpointHczA != null)
                        _checkpointHczA.text = _elevatorsLockedText;
                    if (_checkpointHczB != null)
                        _checkpointHczB.text = _elevatorsLockedText;
                    break;

                case DecontaminationStatus.Forced:
                    DecontaminationClientTimer.RemainingTimeInSeconds = 0f;
                    DecontaminationGas.TurnedOn = true;
                    if (_checkpointHczA != null)
                        _checkpointHczA.text = _elevatorsLockedText;
                    if (_checkpointHczB != null)
                        _checkpointHczB.text = _elevatorsLockedText;
                    
                    StringBuilder sb = StringBuilderPool.Shared.Rent(8);
                    DecontaminationClientTimer.AppendDigits(sb, 0);
                    DecontaminationClientTimer.AppendColon(sb);
                    DecontaminationClientTimer.AppendDigits(sb, 0);
                    DecontaminationClientTimer.AppendDot(sb);
                    DecontaminationClientTimer.AppendDigits(sb, 0);
                    DecontaminationClientTimer.ScreenTimeString = sb.ToString();
                    StringBuilderPool.Shared.Return(sb);
                    break;
            }
        }

        public void ForceDecontamination()
        {
            DecontaminationOverride = DecontaminationStatus.Forced;
            FinishDecontamination();
        }

        private void Start()
        {
            if (NetworkServer.active && ConfigFile.ServerConfig.GetBool("disable_decontamination", false))
            {
                DecontaminationOverride = DecontaminationStatus.Disabled;
            }

            if (DecontaminationPhases != null)
            {
                for (int i = 0; i < DecontaminationPhases.Length; i++)
                {
                    DecontaminationPhases[i].TimeTrigger *= 60f;
                }
            }
        }

        private IEnumerator<float> KillPlayers()
        {
            float timer = 1f;
            
            while (Singleton != null && _decontaminationBegun && 
                   DecontaminationOverride != DecontaminationStatus.Disabled)
            {
                timer -= Time.deltaTime;
                yield return float.NegativeInfinity;
                
                if (timer > 0f)
                    continue;
                
                timer = 1f;
                
                foreach (ReferenceHub hub in ReferenceHub.AllHubs)
                {
                    if (!PlayerRolesUtils.IsAlive(hub))
                        continue;

                    float y = hub.transform.position.y;
                    bool inLcz = y < UpperBoundLCZ && y > LowerBoundLCZ;
                    
                    Decontaminating effect = hub.playerEffectsController.GetEffect<Decontaminating>();
                    
                    if (!effect.IsEnabled && inLcz)
                    {
                        hub.playerEffectsController.EnableEffect<Decontaminating>();
                    }
                    else if (effect.IsEnabled && !inLcz)
                    {
                        hub.playerEffectsController.DisableEffect<Decontaminating>();
                    }
                }
            }
        }

        private void FinishDecontamination()
        {
            if (!NetworkServer.active)
                return;

            DoorEventOpenerExtension.TriggerAction(DoorEventOpenerExtension.OpenerEventType.DeconFinish);
            DisableElevators();

            if (AutoDeconBroadcastEnabled && !_decontaminationBegun)
            {
                Broadcast.Singleton?.RpcAddElement(
                    DeconBroadcastDeconMessage, 
                    DeconBroadcastDeconMessageTime, 
                    BroadcastFlags.Normal);
            }

            _decontaminationBegun = true;
            Timing.RunCoroutine(KillPlayers());
            DecontaminationGas.TurnedOn = true;

            if (_checkpointHczA != null)
                _checkpointHczA.text = _elevatorsLockedText;
            if (_checkpointHczB != null)
                _checkpointHczB.text = _elevatorsLockedText;
        }

        private void DisableElevators()
        {
            bool failedToLock = false;
            
            foreach (DoorVariant door in DoorVariant.AllDoors)
            {
                if (door is not ElevatorDoor elevatorDoor)
                    continue;

                if (elevatorDoor.Rooms == null || elevatorDoor.Rooms.Length == 0)
                    continue;

                if (elevatorDoor.Rooms[0].Zone != FacilityZone.LightContainment)
                    continue;

                elevatorDoor.ActiveLocks |= 0x10;

                if (!door.TargetState && !ElevatorManager.TrySetDestination(elevatorDoor.Group, 1))
                {
                    failedToLock = true;
                }
            }

            if (!failedToLock)
            {
                _elevatorsDirty = false;
            }
        }

        private void Update()
        {
            if (_elevatorsDirty)
            {
                DisableElevators();
            }

            if (_stopUpdating)
                return;

            if (NetworkServer.active)
            {
                ServersideSetup();
            }

            UpdateTime();
        }

        private void ServersideSetup()
        {
            if (DecontaminationOverride != DecontaminationStatus.None)
                return;

            if (RoundStartTime != 0.0)
                return;

            if (RoundStart.singleton == null || RoundStart.singleton.Timer != -1)
                return;

            RoundStartTime = NetworkTime.time;
        }

        private void UpdateTime()
        {
            if (DecontaminationOverride != DecontaminationStatus.None)
                return;

            if (RoundStartTime <= 0.0)
            {
                if (RoundStartTime == -1.0)
                {
                    _stopUpdating = true;
                }
                return;
            }

            if (_justJoinedCooldown < 10f)
            {
                _justJoinedCooldown += Time.deltaTime;
            }

            float serverTime = (float)GetServerTime;

            if (serverTime != -1f &&
                DecontaminationPhases != null &&
                _nextPhase < DecontaminationPhases.Length &&
                serverTime > DecontaminationPhases[_nextPhase].TimeTrigger)
            {
                if (DecontaminationPhases[_nextPhase].AnnouncementLine != null &&
                    _justJoinedCooldown >= 10f)
                {
                    _curFunction = DecontaminationPhases[_nextPhase].Function;
                    UpdateSpeaker(hard: true);
                    AnnouncementAudioSource?.PlayOneShot(DecontaminationPhases[_nextPhase].AnnouncementLine);

                    if (NetworkServer.active)
                    {
                        List<SubtitlePart> subtitles = new List<SubtitlePart>(1);

                        switch (_nextPhase)
                        {
                            case 0:
                                subtitles.Add(new SubtitlePart(SubtitleType.DecontaminationStart, null));
                                break;
                            case 1:
                                subtitles.Add(new SubtitlePart(SubtitleType.DecontaminationMinutes, new[] { "10" }));
                                break;
                            case 2:
                                subtitles.Add(new SubtitlePart(SubtitleType.DecontaminationMinutes, new[] { "5" }));
                                break;
                            case 3:
                                subtitles.Add(new SubtitlePart(SubtitleType.Decontamination1Minute, null));
                                break;
                            case 4:
                                subtitles.Add(new SubtitlePart(SubtitleType.DecontaminationCountdown, null));
                                break;
                            case 6:
                                subtitles.Add(new SubtitlePart(SubtitleType.DecontaminationLockdown, null));
                                break;
                        }

                        foreach (ReferenceHub hub in ReferenceHub.AllHubs)
                        {
                            if (IsAudibleForClient(hub))
                            {
                                SpectatorNetworking.SendToSpectatorsOf(
                                    new SubtitleMessage(subtitles.ToArray()),
                                    hub,
                                    includeTarget: true);
                            }
                        }
                    }
                }

                if (DecontaminationPhases[_nextPhase].Function == DecontaminationPhase.PhaseFunction.Final)
                {
                    FinishDecontamination();
                }

                if (NetworkServer.active &&
                    DecontaminationPhases[_nextPhase].Function == DecontaminationPhase.PhaseFunction.OpenCheckpoints)
                {
                    DoorEventOpenerExtension.TriggerAction(DoorEventOpenerExtension.OpenerEventType.DeconEvac);
                }

                if (_nextPhase >= DecontaminationPhases.Length - 1)
                {
                    _stopUpdating = true;
                }
                else
                {
                    _nextPhase++;
                }
            }

            UpdateSpeaker(hard: false);
            ClientTimer?.UpdateTimer(serverTime);
        }

        private bool IsAudibleForClient(ReferenceHub hub)
        {
            if (_curFunction == DecontaminationPhase.PhaseFunction.Final ||
                _curFunction == DecontaminationPhase.PhaseFunction.GloballyAudible)
            {
                return true;
            }

            PlayerRoleBase currentRole = hub.roleManager?.CurrentRole;
            
            if (currentRole == null)
                return false;

            if (currentRole is Scp079Role scp079)
            {
                return scp079.CurrentCamera?.Room?.Zone == FacilityZone.LightContainment;
            }

            if (currentRole is IFpcRole fpc)
            {
                RoomIdentifier room = RoomIdUtils.RoomAtPositionRaycasts(fpc.FpcModule.Position);
                return room != null && room.Zone == FacilityZone.LightContainment;
            }

            return false;
        }

        private void UpdateSpeaker(bool hard)
        {
            bool hearable = IsAnnouncementHearable;
            float targetVolume = hearable ? 1f : 0f;
            float t = hard ? 1f : Time.deltaTime * 4f;

            if (AnnouncementAudioSource != null)
            {
                AnnouncementAudioSource.volume = Mathf.Lerp(AnnouncementAudioSource.volume, targetVolume, t);
            }
        }
    }
}
