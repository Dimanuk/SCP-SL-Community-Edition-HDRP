using Interactables.Verification;
using MapGeneration;
using Mirror;
using PlayerRoles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables.Interobjects.DoorUtils
{
    public abstract class DoorVariant : NetworkBehaviour, IServerInteractable, IInteractable
    {
        [Flags]
        private enum CollisionsDisablingReasons : byte
        {
            DoorClosing = 1,
            Scp106 = 2
        }

        public static readonly HashSet<DoorVariant> AllDoors = new HashSet<DoorVariant>();

        public static readonly Dictionary<RoomIdentifier, HashSet<DoorVariant>> DoorsByRoom
            = new Dictionary<RoomIdentifier, HashSet<DoorVariant>>();

        [SyncVar]
        public bool TargetState;

        [SyncVar]
        public ushort ActiveLocks;

        [SyncVar]
        public byte DoorId;

        public bool CanSeeThrough;

        public DoorPermissions RequiredPermissions;

        private bool _prevState;

        private ushort _prevLock;

        private byte _existenceCooldown = byte.MaxValue;

        private ReferenceHub _triggerPlayer;

        private CollisionsDisablingReasons _collidersStatus;

        private BoxCollider[] _colliders;

        private bool _collidersActivationPending;

        private const int WorldDirCount = 4;

        private static int _serverDoorIdClock;

        private static readonly RoomIdentifier[] RoomsNonAlloc = new RoomIdentifier[4];

        private static readonly Vector3[] WorldDirections = new Vector3[4]
        {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right
        };

        public IVerificationRule VerificationRule => StandardDistanceVerification.Default;

        public RoomIdentifier[] Rooms { get; private set; }

        [Server]
        public void ServerInteract(ReferenceHub ply, byte colliderId)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void Interactables.Interobjects.DoorUtils.DoorVariant::ServerInteract(ReferenceHub,System.Byte)' called when server was not active");
                return;
            }

            if (ActiveLocks > 0 && !ply.serverRoles.BypassMode)
            {
                DoorLockMode mode = DoorLockUtils.GetMode((DoorLockReason)ActiveLocks);
                if ((!mode.HasFlagFast(DoorLockMode.CanClose) || !mode.HasFlagFast(DoorLockMode.CanOpen))
                    && (!mode.HasFlagFast(DoorLockMode.ScpOverride) || !PlayerRolesUtils.IsSCP(ply))
                    && (mode == DoorLockMode.FullLock
                        || (TargetState && !mode.HasFlagFast(DoorLockMode.CanClose))
                        || (!TargetState && !mode.HasFlagFast(DoorLockMode.CanOpen))))
                {
                    LockBypassDenied(ply, colliderId);
                    return;
                }
            }

            if (!AllowInteracting(ply, colliderId))
                return;

            bool hasPermission = PlayerRolesUtils.GetRoleId(ply) == RoleTypeId.Scp079
                || RequiredPermissions.CheckPermissions(ply.inventory.CurInstance, ply);

            if (hasPermission)
            {
                TargetState = !TargetState;
                _triggerPlayer = ply;
            }
            else
            {
                PermissionsDenied(ply, colliderId);
                DoorEvents.TriggerAction(this, DoorAction.AccessDenied, ply);
            }
        }

        [Server]
        public void ServerChangeLock(DoorLockReason reason, bool newState)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void Interactables.Interobjects.DoorUtils.DoorVariant::ServerChangeLock(Interactables.Interobjects.DoorUtils.DoorLockReason,System.Boolean)' called when server was not active");
                return;
            }

            DoorLockReason activeLocks = (DoorLockReason)ActiveLocks;
            activeLocks = newState
                ? (activeLocks | reason)
                : (activeLocks & ~reason);

            if ((uint)ActiveLocks != (uint)activeLocks)
            {
                if (newState)
                {
                    if (ActiveLocks == 0)
                        DoorEvents.TriggerAction(this, DoorAction.Locked, null);
                }
                else
                {
                    if (ActiveLocks != 0)
                        DoorEvents.TriggerAction(this, DoorAction.Unlocked, null);
                }
            }

            ActiveLocks = (ushort)activeLocks;
        }

        public abstract void LockBypassDenied(ReferenceHub ply, byte colliderId);
        public abstract bool AnticheatPassageApproved();
        public abstract void PermissionsDenied(ReferenceHub ply, byte colliderId);
        public abstract bool AllowInteracting(ReferenceHub ply, byte colliderId);
        public abstract float GetExactState();
        public abstract bool IsConsideredOpen();

        protected virtual void LockChanged(ushort prevValue) { }
        internal virtual void TargetStateChanged() { }

        protected virtual void Start()
        {
            if (NetworkServer.active)
            {
                _colliders = GetComponentsInChildren<BoxCollider>();
                _serverDoorIdClock++;
                if (_serverDoorIdClock > 255)
                    _serverDoorIdClock = 1;
                DoorId = (byte)_serverDoorIdClock;
            }

            AllDoors.Add(this);

            if (SeedSynchronizer.MapGenerated)
                RegisterRooms();
        }

        protected virtual void OnDestroy()
        {
            AllDoors.Remove(this);
            if (Rooms == null)
                return;

            foreach (RoomIdentifier room in Rooms)
            {
                if (DoorsByRoom.TryGetValue(room, out var set))
                    set.Remove(this);
            }
        }

        protected virtual void Update()
        {
            if (_existenceCooldown == 0)
            {
                if (_prevLock != ActiveLocks)
                {
                    LockChanged(_prevLock);
                    _prevLock = ActiveLocks;
                }

                if (_prevState != TargetState)
                {
                    TargetStateChanged();
                    DoorEvents.TriggerAction(
                        this,
                        TargetState ? DoorAction.Opened : DoorAction.Closed,
                        _triggerPlayer);

                    _triggerPlayer = null;

                    if (NetworkServer.active)
                    {
                        if (TargetState)
                        {
                            _collidersStatus &= ~CollisionsDisablingReasons.DoorClosing;
                        }
                        else
                        {
                            _collidersStatus |= CollisionsDisablingReasons.DoorClosing;
                            _collidersActivationPending = true;
                        }
                        SetColliders();
                    }

                    _prevState = TargetState;
                }
            }
            else
            {
                _existenceCooldown--;
            }

            if (_collidersActivationPending && !AnticheatPassageApproved())
            {
                _collidersActivationPending = false;
                _collidersStatus &= ~CollisionsDisablingReasons.DoorClosing;
                SetColliders();
            }
        }

        private void SetColliders()
        {
            foreach (BoxCollider col in _colliders)
            {
                col.isTrigger = _collidersStatus != 0;
            }
        }

        private void RegisterRooms()
        {
            Vector3 position = transform.position;
            int count = 0;

            for (int i = 0; i < 4; i++)
            {
                Vector3Int coords = RoomIdUtils.PositionToCoords(position + WorldDirections[i]);
                if (RoomIdentifier.RoomsByCoordinates.TryGetValue(coords, out var room)
                    && DoorsByRoom.GetOrAdd(room, () => new HashSet<DoorVariant>()).Add(this))
                {
                    RoomsNonAlloc[count] = room;
                    count++;
                }
            }

            Rooms = new RoomIdentifier[count];
            Array.Copy(RoomsNonAlloc, Rooms, count);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            SeedSynchronizer.OnMapGenerated += () =>
            {
                foreach (var door in AllDoors)
                    door.RegisterRooms();
            };

            RoomIdentifier.OnRemoved += room =>
            {
                DoorsByRoom.Remove(room);
            };
        }
    }
}