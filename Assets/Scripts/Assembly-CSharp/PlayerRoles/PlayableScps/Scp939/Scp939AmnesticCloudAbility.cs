namespace PlayerRoles.PlayableScps.Scp939
{
    public class Scp939AmnesticCloudAbility : global::PlayerRoles.PlayableScps.Subroutines.ScpKeySubroutine<global::PlayerRoles.PlayableScps.Scp939.Scp939Role>
    {
        private static readonly global::System.Collections.Generic.Dictionary<global::MapGeneration.RoomName, float> WhitelistedFloors = new global::System.Collections.Generic.Dictionary<global::MapGeneration.RoomName, float>
        {
            [global::MapGeneration.RoomName.EzOfficeSmall] = -0.527f,
            [global::MapGeneration.RoomName.EzOfficeStoried] = 3.767f,
            [global::MapGeneration.RoomName.Hcz106] = 1.19f,
            [global::MapGeneration.RoomName.Hcz079] = -4.334f
        };

        private const float FloorTolerance = 0.2f;

        public bool _targetState;

        private float _sendDuration;

        private global::PlayerRoles.PlayableScps.Scp939.Scp939FocusAbility _focusAbility;

        private readonly global::System.Diagnostics.Stopwatch _beginHeldSw = global::System.Diagnostics.Stopwatch.StartNew();

        [global::UnityEngine.SerializeField]
        private global::PlayerRoles.PlayableScps.Scp939.Scp939AmnesticCloudInstance _instancePrefab;

        [global::UnityEngine.SerializeField]
        private float _failedCooldown;

        [global::UnityEngine.SerializeField]
        private float _placedCooldown;

        public readonly global::PlayerRoles.PlayableScps.Subroutines.AbilityCooldown Duration = new global::PlayerRoles.PlayableScps.Subroutines.AbilityCooldown();

        public readonly global::PlayerRoles.PlayableScps.Subroutines.AbilityCooldown Cooldown = new global::PlayerRoles.PlayableScps.Subroutines.AbilityCooldown();

        public readonly global::PlayerRoles.PlayableScps.Subroutines.AbilityCooldown HudIndicatorMin = new global::PlayerRoles.PlayableScps.Subroutines.AbilityCooldown();

        public readonly global::PlayerRoles.PlayableScps.Subroutines.AbilityCooldown HudIndicatorMax = new global::PlayerRoles.PlayableScps.Subroutines.AbilityCooldown();

        protected override ActionName TargetKey => ActionName.ToggleFlashlight;

        protected override bool KeyPressable
        {
            get
            {
                if (base.KeyPressable)
                {
                    return _focusAbility.State == 0f;
                }
                return false;
            }
        }

        public float HoldDuration => (float)_beginHeldSw.Elapsed.TotalSeconds;

        public bool TargetState
        {
            get
            {
                return _targetState;
            }
            set
            {
                if (_targetState != value)
                {
                    _targetState = value;
                    if (value)
                    {
                        OnStateEnabled();
                    }
                    else
                    {
                        OnStateDisabled();
                    }
                }
            }
        }

        public event global::System.Action<global::PlayerRoles.PlayableScps.Scp939.Scp939HudTranslation> OnDeployFailed;

        private void OnStateEnabled()
        {
            _beginHeldSw.Restart();
            HudIndicatorMax.Clear();
            HudIndicatorMin.Trigger(_instancePrefab.MinMaxTime.y);
            if (global::Mirror.NetworkServer.active)
            {
                global::UnityEngine.Object.Instantiate(_instancePrefab).ServerSetup(base.Owner);
            }
        }

        private void OnStateDisabled()
        {
            _beginHeldSw.Reset();
        }

        protected override void Awake()
        {
            base.Awake();
            GetSubroutine<global::PlayerRoles.PlayableScps.Scp939.Scp939FocusAbility>(out _focusAbility);
        }

        protected override void Update()
        {
            base.Update();
            if ((base.Owner.isLocalPlayer || global::PlayerRoles.Spectating.SpectatorNetworking.IsLocallySpectated(base.Owner)) && TargetState && HudIndicatorMax.IsReady)
            {
                global::UnityEngine.Vector2 minMaxTime = _instancePrefab.MinMaxTime;
                if (!(HoldDuration < minMaxTime.x))
                {
                    HudIndicatorMax.NextUse = HudIndicatorMin.NextUse;
                    HudIndicatorMax.InitialTime = HudIndicatorMin.InitialTime;
                }
            }
        }

        protected override void OnKeyDown()
        {
            base.OnKeyDown();
            if (Cooldown.IsReady)
            {
                if (!ValidateFloor())
                {
                    ClientCancel(global::PlayerRoles.PlayableScps.Scp939.Scp939HudTranslation.CloudFailedPositionInvalid);
                    return;
                }
                TargetState = true;
                ClientSendCmd();
            }
        }

        protected override void OnKeyUp()
        {
            base.OnKeyUp();
            if (TargetState)
            {
                ClientCancel(global::PlayerRoles.PlayableScps.Scp939.Scp939HudTranslation.PressKeyToLunge);
            }
        }

        public bool ValidateFloor()
        {
            global::UnityEngine.Vector3 pos = base.ScpRole.FpcModule.Position;
            global::MapGeneration.RoomIdentifier roomIdentifier = global::MapGeneration.RoomIdUtils.RoomAtPositionRaycasts(pos);
            if (roomIdentifier == null)
            {
                return false;
            }
            if (WhitelistedFloors.TryGetValue(roomIdentifier.Name, out var value) && global::UnityEngine.Mathf.Abs(roomIdentifier.transform.position.y - pos.y + value) < 0.2f)
            {
                return true;
            }
            if (!global::Interactables.Interobjects.DoorUtils.DoorVariant.DoorsByRoom.TryGetValue(roomIdentifier, out var value2))
            {
                return false;
            }
            float halfHeight = base.ScpRole.FpcModule.CharController.height / 2f;
            return global::Utils.NonAllocLINQ.HashsetExtensions.Any(value2, (global::Interactables.Interobjects.DoorUtils.DoorVariant x) => global::UnityEngine.Mathf.Abs(x.transform.position.y - pos.y + halfHeight) < 0.2f);
        }

        public void ClientCancel(global::PlayerRoles.PlayableScps.Scp939.Scp939HudTranslation reason)
        {
            if ((uint)(reason - 12) <= 1u)
            {
                this.OnDeployFailed?.Invoke(reason);
            }
            TargetState = false;
            ClientSendCmd();
        }

        public void ServerConfirmPlacement(float duration)
        {
            _sendDuration = duration;
            Cooldown.Trigger(_placedCooldown);
            ServerSendRpc(toAll: true);
        }

        public void ServerFailPlacement()
        {
            _sendDuration = -128f;
            Cooldown.Trigger(_failedCooldown);
            ServerSendRpc(toAll: true);
        }

        public override void ClientWriteCmd(global::Mirror.NetworkWriter writer)
        {
            base.ClientWriteCmd(writer);
            global::Mirror.NetworkWriterExtensions.WriteBool(writer, TargetState);
        }

        public override void ResetObject()
        {
            base.ResetObject();
            Cooldown.Clear();
            Duration.Clear();
            HudIndicatorMin.Clear();
            HudIndicatorMax.Clear();
        }

        public override void ServerProcessCmd(global::Mirror.NetworkReader reader)
        {
            base.ServerProcessCmd(reader);
            bool flag = global::Mirror.NetworkReaderExtensions.ReadBool(reader);
            bool toAll = flag != TargetState;
            if (flag)
            {
                if (Cooldown.IsReady)
                {
                    TargetState = flag;
                    Cooldown.Trigger(_failedCooldown);
                }
            }
            else
            {
                TargetState = false;
            }
            ServerSendRpc(toAll);
        }

        public override void ServerWriteRpc(global::Mirror.NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            global::Mirror.NetworkWriterExtensions.WriteBool(writer, TargetState);
            if (!TargetState)
            {
                Cooldown.WriteCooldown(writer);
                global::Mirror.NetworkWriterExtensions.WriteFloat(writer, _sendDuration);
            }
        }

        public override void ClientProcessRpc(global::Mirror.NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            TargetState = global::Mirror.NetworkReaderExtensions.ReadBool(reader);
            if (!TargetState)
            {
                Cooldown.ReadCooldown(reader);
                float num = global::Mirror.NetworkReaderExtensions.ReadFloat(reader);
                if (num <= 0f)
                {
                    Duration.Clear();
                    return;
                }
                Duration.Trigger(num);
                Cooldown.InitialTime += num;
            }
        }

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientStarted += delegate
            {
                if (!global::PlayerRoles.PlayerRoleLoader.TryGetRoleTemplate<global::PlayerRoles.PlayableScps.Scp939.Scp939Role>(global::PlayerRoles.RoleTypeId.Scp939, out var result))
                {
                    throw new global::System.InvalidOperationException("Cannot register amnestic cloud. SCP-939 role template not found.");
                }
                if (!result.SubroutineModule.TryGetSubroutine<global::PlayerRoles.PlayableScps.Scp939.Scp939AmnesticCloudAbility>(out var subroutine))
                {
                    throw new global::System.InvalidOperationException("Cannot register amnestic cloud. Ability not found.");
                }
                global::Mirror.NetworkClient.RegisterPrefab(subroutine._instancePrefab.gameObject);
            };
        }
    }
}
