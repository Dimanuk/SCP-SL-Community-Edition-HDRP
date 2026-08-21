using System;
using System.Runtime.CompilerServices;
using CursorManagement;
using GameObjectPools;
using Interactables.Interobjects;
using Mirror;
using PlayerRoles.FirstPersonControl.NetworkMessages;
using PlayerRoles.FirstPersonControl.Thirdperson;
using RelativePositioning;
using UnityEngine;

namespace PlayerRoles.FirstPersonControl
{
    public class FirstPersonMovementModule : MonoBehaviour, IPoolSpawnable, IPoolResettable, ICursorOverride
    {
        public Action OnServerPositionOverwritten;

        public Action OnGrounded;

        public GameObject CharacterModelTemplate;

        public float CrouchSpeed;

        public float SneakSpeed;

        public float WalkSpeed;

        public float SprintSpeed;

        public float JumpSpeed;

        public CharacterControllerSettingsPreset CharacterControllerSettings;

        public float CrouchHeightRatio;

        private Transform _transform;

        private PlayerMovementState _speedState;

        private bool _syncGrounded;

        private Vector3 _cachedPosition;

        private static Action _activeUpdates;

        public virtual CursorOverrideMode CursorOverride => CursorOverrideMode.Centered;

        public virtual bool LockMovement => false;

        public CharacterController CharController { get; private set; }

        public bool CharControllerSet { get; private set; }

        public bool ModuleReady { get; private set; }

        public FpcMotor Motor { get; protected set; }

        public FpcNoclip Noclip { get; protected set; }

        public FpcMouseLook MouseLook { get; protected set; }

        public FpcStateProcessor StateProcessor { get; protected set; }

        public MovementTracer Tracer { get; private set; }

        public CharacterModel CharacterModelInstance { get; protected set; }

        public FpcSyncData LastSentData { get; internal set; }

        public float MaxMovementSpeed => VelocityForState(ValidateMovementState(_speedState), applyCrouch: true);

        public PlayerMovementState CurrentMovementState
        {
            get
            {
                return ValidateMovementState(SyncMovementState);
            }
            set
            {
                SyncMovementState = value;
            }
        }

        public PlayerMovementState SyncMovementState { get; private set; }

        public bool IsGrounded
        {
            get
            {
                if (CharControllerSet ? CharController.isGrounded : _syncGrounded)
                {
                    return !Noclip.IsActive;
                }
                return false;
            }
            set
            {
                if (_syncGrounded != value)
                {
                    _syncGrounded = value;
                    if (value)
                    {
                        OnGrounded?.Invoke();
                    }
                }
            }
        }

        public Vector3 Position
        {
            get
            {
                return _cachedPosition;
            }
            set
            {
                _transform.position = value;
                _cachedPosition = value;
            }
        }

        protected ReferenceHub Hub { get; private set; }

        protected PlayerRoleBase Role { get; private set; }

        protected virtual FpcMotor NewMotor => new FpcMotor(Hub, this, !Hub.IsSCP());

        protected virtual FpcNoclip NewNoclip => new FpcNoclip(Hub, this);

        protected virtual FpcMouseLook NewMouseLook => new FpcMouseLook(Hub, this);

        protected virtual FpcStateProcessor NewStateProcessor => new FpcStateProcessor(Hub, this);

        public static event Action OnPositionUpdated;

        protected virtual void UpdateMovement()
        {
            SyncMovementState = StateProcessor.UpdateMovementState(CurrentMovementState);
            Motor.UpdatePosition(out var sendJump);
            Noclip.UpdateNoclip();
            MouseLook.UpdateRotation();
            if (SyncMovementState != PlayerMovementState.Crouching)
            {
                _speedState = SyncMovementState;
            }
            if (Hub.isLocalPlayer)
            {
                float walkSpeed = VelocityForState(PlayerMovementState.Walking, applyCrouch: false);
                StateProcessor.ClientUpdateInput(this, walkSpeed, out var valueToSend);
                Motor.ReceivedPosition = new RelativePosition(Position);
                NetworkClient.Send(new FpcFromClientMessage(Motor.ReceivedPosition, valueToSend, sendJump, MouseLook));
            }
        }

        private void FixedUpdate()
        {
            if (NetworkServer.active && ModuleReady && Tracer != null)
            {
                Tracer.Record(Position);
            }
        }

        private void OnRoleDisabled(RoleTypeId rid)
        {
            CharControllerSet = false;
            CharacterModelInstance.ReturnToPool();
            Noclip.ShutdownModule();
        }

        protected virtual PlayerMovementState ValidateMovementState(PlayerMovementState state)
        {
            switch (state)
            {
                case PlayerMovementState.Crouching:
                    if (CrouchSpeed != 0f)
                    {
                        break;
                    }
                    goto IL_0045;
                case PlayerMovementState.Sneaking:
                    if (SneakSpeed != 0f)
                    {
                        break;
                    }
                    goto IL_0045;
                case PlayerMovementState.Sprinting:
                    {
                        if (SprintSpeed != 0f)
                        {
                            break;
                        }
                        goto IL_0045;
                    }
                IL_0045:
                    return PlayerMovementState.Walking;
            }
            return state;
        }

        public void ServerOverridePosition(Vector3 position, Vector3 deltaRotation)
        {
            Position = position;
            Hub.connectionToClient.Send(new FpcOverrideMessage(position, deltaRotation.y));
            OnServerPositionOverwritten();
        }

        public virtual float VelocityForState(PlayerMovementState state, bool applyCrouch)
        {
            float num = 0f;
            switch (state)
            {
                case PlayerMovementState.Crouching:
                    num = CrouchSpeed;
                    break;
                case PlayerMovementState.Sneaking:
                    num = SneakSpeed;
                    break;
                case PlayerMovementState.Sprinting:
                    num = SprintSpeed;
                    break;
                case PlayerMovementState.Walking:
                    num = WalkSpeed;
                    break;
            }
            if (applyCrouch)
            {
                num = Mathf.Lerp(num, CrouchSpeed, StateProcessor.CrouchPercent);
            }
            num *= Hub.inventory.MovementSpeedMultiplier;
            float num2 = Hub.inventory.MovementSpeedLimit;
            for (int i = 0; i < Hub.playerEffectsController.EffectsLength; i++)
            {
                if (Hub.playerEffectsController.AllEffects[i] is IMovementSpeedModifier { MovementModifierActive: not false } movementSpeedModifier)
                {
                    num2 = Mathf.Min(num2, movementSpeedModifier.MovementSpeedLimit);
                    num *= movementSpeedModifier.MovementSpeedMultiplier;
                }
            }
            return Mathf.Min(num, num2);
        }


        public virtual void SpawnObject()
        {
            if (!TryGetComponent<PlayerRoleBase>(out var component) || !component.TryGetOwner(out var hub))
            {
                throw new InvalidOperationException("Movement module failed to initiate. Unable to find owner of the role.");
            }
            _activeUpdates = (Action)Delegate.Combine(_activeUpdates, new Action(UpdateMovement));
            Hub = hub;
            Role = component;
            _transform = Hub.transform;
            _speedState = PlayerMovementState.Walking;
            SyncMovementState = PlayerMovementState.Walking;
            if (NetworkServer.active || Hub.isLocalPlayer)
            {
                CharController = Hub.GetComponent<CharacterController>();
                CharacterControllerSettings.Apply(CharController);
                CharControllerSet = true;
                if (NetworkServer.active)
                {
                    Tracer = new MovementTracer(15, 3, 50f);
                }
                if (Hub.isLocalPlayer)
                {
                    CursorManager.Register(this);
                }
            }
            else
            {
                CharControllerSet = false;
            }
            Motor = NewMotor;
            Noclip = NewNoclip;
            MouseLook = NewMouseLook;
            StateProcessor = NewStateProcessor;
            PlayerRoleBase role = Role;
            role.OnRoleDisabled = (Action<RoleTypeId>)Delegate.Combine(role.OnRoleDisabled, new Action<RoleTypeId>(OnRoleDisabled));
            ElevatorChamber.OnElevatorMoved += Motor.OnElevatorMoved;
            SetModel(CharacterModelTemplate);
            ModuleReady = true;
        }

        protected virtual void SetModel(GameObject template)
        {
            if (PoolManager.Singleton.TryGetPoolObject(template, _transform, out var poolObject) && poolObject is CharacterModel characterModel)
            {
                CharacterModelInstance = characterModel;
                Transform transform = CharacterModelTemplate.transform;
                Transform obj = characterModel.transform;
                obj.localPosition = transform.position;
                obj.localScale = transform.localScale;
                obj.localRotation = transform.rotation;
                characterModel?.SetVisibility(!Hub.isLocalPlayer);
            }
            else
            {
                Debug.LogError("Can't spawn '" + template.name + "' - FPC models must derive from CharacterModel.");
            }
        }

        public virtual void ResetObject()
        {
            CursorManager.Unregister(this);
            _activeUpdates = (Action)Delegate.Remove(_activeUpdates, new Action(UpdateMovement));
            PlayerRoleBase role = Role;
            role.OnRoleDisabled = (Action<RoleTypeId>)Delegate.Remove(role.OnRoleDisabled, new Action<RoleTypeId>(OnRoleDisabled));
            ElevatorChamber.OnElevatorMoved -= Motor.OnElevatorMoved;
            ModuleReady = false;
        }

        private void OnDestroy()
        {
            // Safety net. Pooled role modules are normally removed from the static
            // update delegate via ResetObject() on role change / Mirror OnStopClient.
            // But when the owner Hub's GameObject is destroyed directly by Unity
            // (e.g. a scene reload on round restart) ResetObject never runs, and since
            // _activeUpdates is static it survives the scene reload — the dead module
            // would keep ticking against a destroyed _transform every frame
            // (MissingReferenceException). Unregister here on actual destruction.
            // Note: pooled objects are only deactivated (not destroyed) during normal
            // play, so this does not interfere with pooling.
            _activeUpdates = (Action)Delegate.Remove(_activeUpdates, new Action(UpdateMovement));
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            StaticUnityMethods.OnUpdate += delegate
            {
                if (_activeUpdates != null)
                {
                    _activeUpdates();
                    FirstPersonMovementModule.OnPositionUpdated?.Invoke();
                }
            };
        }
    }
}
