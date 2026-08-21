using System.Collections.Generic;
using System.Diagnostics;
using Interactables;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using MapGeneration.Distributors;
using Mirror;
using PlayerStatsSystem;
using Subtitles;
using UnityEngine;
using Utils.Networking;
using Utils.NonAllocLINQ;

namespace PlayerRoles.PlayableScps.Scp079
{
    public class Scp079Recontainer : MonoBehaviour
    {
        public static readonly HashSet<Scp079Generator> AllGenerators = new HashSet<Scp079Generator>();

        [SerializeField]
        private DoorVariant[] _containmentGates;

        [SerializeField]
        private float _activationDelay;

        [SerializeField]
        private float _lockdownDuration;

        [SerializeField]
        private Transform _activatorButton;

        [SerializeField]
        private BreakableWindow _activatorGlass;

        [SerializeField]
        private Vector3 _activatorPos;

        [SerializeField]
        private float _activatorLerpSpeed;

        [SerializeField]
        private string _announcementProgress;

        [SerializeField]
        private string _announcementAllActivated;

        [SerializeField]
        private string _announcementCountdown;

        [SerializeField]
        private string _announcementSuccess;

        [SerializeField]
        private string _announcementFailure;

        private const float AnnouncementGlitchChance = 0.035f;

        private const float AnnouncementJamChance = 0.03f;

        private bool _alreadyRecontained;

        private bool _success;

        private int _prevEngaged;

        private float _recontainLater;

        private readonly Stopwatch _delayStopwatch = new Stopwatch();

        private readonly Stopwatch _unlockStopwatch = new Stopwatch();

        private readonly HashSet<DoorVariant> _lockedDoors = new HashSet<DoorVariant>();

        private bool CassieBusy => NineTailedFoxAnnouncer.singleton.queue.Count > 0;

        private void Start()
        {
            SetContainmentDoors(opened: false, locked: true);
            PlayerRoleManager.OnServerRoleSet += OnServerRoleChanged;
        }

        private void OnDestroy()
        {
            PlayerRoleManager.OnServerRoleSet -= OnServerRoleChanged;
        }

        private void Update()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            RefreshActivator();
            RefreshAmount();
            if (_unlockStopwatch.IsRunning && _unlockStopwatch.Elapsed.TotalSeconds > (double)_lockdownDuration)
            {
                EndOvercharge();
                _unlockStopwatch.Stop();
            }
            if (_recontainLater > 0f)
            {
                _delayStopwatch.Stop();
                if (!CassieBusy)
                {
                    _recontainLater -= Time.deltaTime;
                }
                if (_recontainLater <= 0f)
                {
                    Recontain();
                }
            }
        }

        private void OnServerRoleChanged(ReferenceHub hub, RoleTypeId newRole, RoleChangeReason reason)
        {
            if (newRole != RoleTypeId.Spectator || !IsScpButNot079(hub.roleManager.CurrentRole) || Scp079Role.ActiveInstances.Count == 0 || ReferenceHub.AllHubs.Count((ReferenceHub x) => x != hub && IsScpButNot079(x.roleManager.CurrentRole)) > 0)
            {
                return;
            }
            SetContainmentDoors(opened: true, locked: true);
            _alreadyRecontained = true;
            _recontainLater = 3f;
            foreach (Scp079Generator allGenerator in AllGenerators)
            {
                allGenerator.Engaged = true;
            }
        }

        private bool IsScpButNot079(PlayerRoleBase prb)
        {
            if (prb.Team == Team.SCPs)
            {
                return prb.RoleTypeId != RoleTypeId.Scp079;
            }
            return false;
        }

        private void RefreshActivator()
        {
            if (_delayStopwatch.Elapsed.TotalSeconds > (double)_activationDelay)
            {
                if (_delayStopwatch.IsRunning)
                {
                    BeginOvercharge();
                    _delayStopwatch.Stop();
                    _unlockStopwatch.Start();
                }
            }
            else if (_activatorGlass.isBroken)
            {
                _activatorButton.transform.localPosition = Vector3.Lerp(_activatorButton.transform.localPosition, _activatorPos, _activatorLerpSpeed * Time.deltaTime);
                if (!_alreadyRecontained && !CassieBusy)
                {
                    Recontain();
                }
            }
        }

        private void Recontain()
        {
            _delayStopwatch.Restart();
            PlayAnnouncement(_announcementCountdown, 0f);
            new SubtitleMessage(new SubtitlePart(SubtitleType.OverchargeIn, (string[])null)).SendToAuthenticated();
            _alreadyRecontained = true;
        }

        private void RefreshAmount()
        {
            if (_alreadyRecontained)
            {
                return;
            }
            int num = 0;
            foreach (Scp079Generator allGenerator in AllGenerators)
            {
                if (allGenerator.Engaged)
                {
                    num++;
                }
            }
            if (num > _prevEngaged)
            {
                UpdateStatus(num);
                _prevEngaged = num;
            }
        }

        private void SetContainmentDoors(bool opened, bool locked)
        {
            if (NetworkServer.active)
            {
                DoorVariant[] containmentGates = _containmentGates;
                foreach (DoorVariant obj in containmentGates)
                {
                    obj.TargetState = opened;
                    obj.ServerChangeLock(DoorLockReason.SpecialDoorFeature, locked);
                }
            }
        }

        private void UpdateStatus(int engagedGenerators)
        {
            int count = AllGenerators.Count;
            string text = string.Format(_announcementProgress, engagedGenerators, count);
            global::System.Collections.Generic.List<global::Subtitles.SubtitlePart> list = new global::System.Collections.Generic.List<global::Subtitles.SubtitlePart>();
            list.Add(new global::Subtitles.SubtitlePart(global::Subtitles.SubtitleType.GeneratorsActivated, engagedGenerators.ToString(), count.ToString()));
            global::System.Collections.Generic.List<global::Subtitles.SubtitlePart> list2 = list;
            if (engagedGenerators >= count)
            {
                text += _announcementAllActivated;
                SetContainmentDoors(opened: true, global::PlayerRoles.PlayableScps.Scp079.Scp079Role.ActiveInstances.Count > 0);
                list2.Add(new global::Subtitles.SubtitlePart(global::Subtitles.SubtitleType.AllGeneratorsEngaged, (string[])null));
            }
            global::Utils.Networking.NetworkUtils.SendToAuthenticated(new global::Subtitles.SubtitleMessage(list2.ToArray()));
            PlayAnnouncement(text, 1f);
        }

        private void BeginOvercharge()
        {
            _success = TryKill079();
            bool inProgress = AlphaWarheadController.InProgress;
            foreach (global::System.Collections.Generic.KeyValuePair<global::Interactables.IInteractable, global::System.Collections.Generic.Dictionary<byte, global::Interactables.InteractableCollider>> allInstance in global::Interactables.InteractableCollider.AllInstances)
            {
                if (allInstance.Key is global::Interactables.Interobjects.BasicDoor basicDoor && !(basicDoor == null) && basicDoor.RequiredPermissions.RequiredPermissions == global::Interactables.Interobjects.DoorUtils.KeycardPermissions.None && global::MapGeneration.RoomIdentifier.RoomsByCoordinates.TryGetValue(global::MapGeneration.RoomIdUtils.PositionToCoords(basicDoor.transform.position), out var value) && value.Zone == global::MapGeneration.FacilityZone.HeavyContainment)
                {
                    basicDoor.TargetState = basicDoor.TargetState && inProgress;
                    basicDoor.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.NoPower, newState: true);
                    _lockedDoors.Add(basicDoor);
                }
            }
            foreach (FlickerableLightController instance in FlickerableLightController.Instances)
            {
                if (global::MapGeneration.RoomIdentifier.RoomsByCoordinates.TryGetValue(global::MapGeneration.RoomIdUtils.PositionToCoords(instance.transform.position), out var value2) && value2.Zone == global::MapGeneration.FacilityZone.HeavyContainment)
                {
                    instance.ServerFlickerLights(_lockdownDuration);
                }
            }
            SetContainmentDoors(opened: true, locked: false);
        }

        private void EndOvercharge()
        {
            if (!_success)
            {
                PlayAnnouncement(_announcementFailure, 1f);
                new SubtitleMessage(new SubtitlePart(SubtitleType.OperationalMode, (string[])null)).SendToAuthenticated();
            }
            foreach (DoorVariant lockedDoor in _lockedDoors)
            {
                lockedDoor.ServerChangeLock(DoorLockReason.NoPower, newState: false);
            }
        }

        private bool TryKill079()
        {
            bool result = false;
            HashSet<ReferenceHub> hashSet = new HashSet<ReferenceHub>();
            foreach (Scp079Role activeInstance in Scp079Role.ActiveInstances)
            {
                if (activeInstance.TryGetOwner(out var hub))
                {
                    hashSet.Add(hub);
                }
            }
            foreach (ReferenceHub item in hashSet)
            {
                result = true;
                if (_activatorGlass.LastAttacker.IsSet)
                {
                    item.playerStats.DealDamage(new RecontainmentDamageHandler(_activatorGlass.LastAttacker));
                }
                else
                {
                    item.playerStats.DealDamage(new UniversalDamageHandler(-1f, DeathTranslations.Recontained));
                }
            }
            return result;
        }

        private void PlayAnnouncement(string annc, float glitchyMultiplier)
        {
            NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(annc, 0.035f * glitchyMultiplier, 0.03f * glitchyMultiplier);
        }
    }
}
