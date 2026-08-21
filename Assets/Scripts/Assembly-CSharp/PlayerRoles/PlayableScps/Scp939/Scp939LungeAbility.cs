namespace PlayerRoles.PlayableScps.Scp939
{
    public class Scp939LungeAbility : global::PlayerRoles.PlayableScps.Subroutines.ScpKeySubroutine<global::PlayerRoles.PlayableScps.Scp939.Scp939Role>
    {
        [global::UnityEngine.SerializeField]
        private float _harshLandingHeight;

        [global::UnityEngine.SerializeField]
        private float _lungeAngleLimit;

        [global::UnityEngine.SerializeField]
        private float _overallTolerance;

        [global::UnityEngine.SerializeField]
        private float _bottomTolerance;

        [global::UnityEngine.SerializeField]
        private float _secondaryRangeSqr;

        [global::UnityEngine.SerializeField]
        private global::PlayerRoles.PlayableScps.Scp939.Scp939LungeAudio _audio;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.AnimationCurve _jumpSpeedOverPitch;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.AnimationCurve _forwardSpeedOverPitch;

        private global::PlayerRoles.PlayableScps.Scp939.Scp939FocusAbility _focus;

        private global::PlayerRoles.PlayableScps.Scp939.Scp939MovementModule _movementModule;

        private global::PlayerRoles.HumanRole _playerToHit;

        private global::PlayerRoles.PlayableScps.Scp939.Scp939LungeState _state;

        private float _lungePitch;

        private const float MainHitmarkerSize = 1f;

        private const float SecondaryHitmarkerSize = 0.6f;

        public const float LungeDamage = 120f;

        public const float SecondaryDamage = 30f;

        public bool IsReady
        {
            get
            {
                if (_focus.State == 1f && State == global::PlayerRoles.PlayableScps.Scp939.Scp939LungeState.None && (!base.Owner.isLocalPlayer || !global::UnityEngine.Cursor.visible))
                {
                    return global::UnityEngine.Mathf.Abs(_focus.AngularDeviation) < _lungeAngleLimit;
                }
                return false;
            }
        }

        public bool LungeRequested { get; private set; }

        public global::RelativePositioning.RelativePosition TriggerPos { get; private set; }

        public float LungeForwardSpeed => _forwardSpeedOverPitch.Evaluate(_lungePitch);

        public float LungeJumpSpeed => _jumpSpeedOverPitch.Evaluate(_lungePitch);

        [field: global::UnityEngine.SerializeField]
        public RagdollAnimationTemplate LungeDeathAnim { get; private set; }

        public global::PlayerRoles.PlayableScps.Scp939.Scp939LungeState State
        {
            get
            {
                return _state;
            }
            private set
            {
                _state = value;
                this.OnStateChanged?.Invoke(value);
                if (!base.Owner.isLocalPlayer)
                {
                    _movementModule.MouseLook.UpdateRotation();
                }
                _lungePitch = _movementModule.MouseLook.CurrentVertical;
                ServerSendRpc(toAll: true);
            }
        }

        protected override ActionName TargetKey => ActionName.Shoot;

        private bool HasAuthority
        {
            get
            {
                if (!global::Mirror.NetworkServer.active)
                {
                    return base.Owner.isLocalPlayer;
                }
                return true;
            }
        }

        private global::RelativePositioning.RelativePosition CurPos => new global::RelativePositioning.RelativePosition(base.ScpRole.FpcModule.Position);

        public event global::System.Action<global::PlayerRoles.PlayableScps.Scp939.Scp939LungeState> OnStateChanged;

        private void OnGrounded()
        {
            if (HasAuthority && State == global::PlayerRoles.PlayableScps.Scp939.Scp939LungeState.Triggered)
            {
                bool flag = CurPos.Position.y < TriggerPos.Position.y - _harshLandingHeight;
                State = (flag ? global::PlayerRoles.PlayableScps.Scp939.Scp939LungeState.LandHarsh : global::PlayerRoles.PlayableScps.Scp939.Scp939LungeState.LandRegular);
            }
        }

        private void OnFocusStateChanged()
        {
            if (!(_focus.State > 0f))
            {
                State = global::PlayerRoles.PlayableScps.Scp939.Scp939LungeState.None;
            }
        }

        protected override void OnKeyDown()
        {
            base.OnKeyDown();
            LungeRequested = true;
        }

        protected override void Awake()
        {
            base.Awake();
            GetSubroutine<global::PlayerRoles.PlayableScps.Scp939.Scp939FocusAbility>(out _focus);
            _movementModule = base.ScpRole.FpcModule as global::PlayerRoles.PlayableScps.Scp939.Scp939MovementModule;
            _focus.OnStateChanged += OnFocusStateChanged;
            _audio.Init(this);
        }

        protected override void Update()
        {
            LungeRequested = false;
            base.Update();
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            global::PlayerRoles.PlayableScps.Scp939.Scp939MovementModule movementModule = _movementModule;
            movementModule.OnGrounded = (global::System.Action)global::System.Delegate.Combine(movementModule.OnGrounded, new global::System.Action(OnGrounded));
        }

        public override void ResetObject()
        {
            LungeRequested = false;
            State = global::PlayerRoles.PlayableScps.Scp939.Scp939LungeState.None;
            global::PlayerRoles.PlayableScps.Scp939.Scp939MovementModule movementModule = _movementModule;
            movementModule.OnGrounded = (global::System.Action)global::System.Delegate.Remove(movementModule.OnGrounded, new global::System.Action(OnGrounded));
        }

        public void TriggerLunge()
        {
            TriggerPos = CurPos;
            State = global::PlayerRoles.PlayableScps.Scp939.Scp939LungeState.Triggered;
        }

        public void ClientSendHit(global::PlayerRoles.HumanRole human)
        {
            _playerToHit = human;
            State = global::PlayerRoles.PlayableScps.Scp939.Scp939LungeState.LandHit;
            ClientSendCmd();
        }

        public override void ClientWriteCmd(global::Mirror.NetworkWriter writer)
        {
            base.ClientWriteCmd(writer);
            global::RelativePositioning.RelativePositionSerialization.WriteRelativePosition(writer, new global::RelativePositioning.RelativePosition(base.ScpRole.FpcModule.Position));
            global::Utils.Networking.ReferenceHubReaderWriter.WriteReferenceHub(writer, _playerToHit.TryGetOwner(out var hub) ? hub : null);
            global::RelativePositioning.RelativePositionSerialization.WriteRelativePosition(writer, new global::RelativePositioning.RelativePosition(_playerToHit.FpcModule.Position));
        }

        public override void ServerProcessCmd(global::Mirror.NetworkReader reader)
        {
            base.ServerProcessCmd(reader);
            global::UnityEngine.Vector3 position = global::RelativePositioning.RelativePositionSerialization.ReadRelativePosition(reader).Position;
            ReferenceHub referenceHub = global::Utils.Networking.ReferenceHubReaderWriter.ReadReferenceHub(reader);
            global::RelativePositioning.RelativePosition relativePosition = global::RelativePositioning.RelativePositionSerialization.ReadRelativePosition(reader);
            if (State != global::PlayerRoles.PlayableScps.Scp939.Scp939LungeState.Triggered)
            {
                if (!IsReady)
                {
                    return;
                }
                TriggerLunge();
            }
            if (referenceHub == null || !(referenceHub.roleManager.CurrentRole is global::PlayerRoles.HumanRole humanRole))
            {
                return;
            }
            global::PlayerRoles.FirstPersonControl.FirstPersonMovementModule fpcModule = humanRole.FpcModule;
            using (new global::PlayerRoles.FirstPersonControl.FpcBacktracker(referenceHub, relativePosition.Position))
            {
                using (new global::PlayerRoles.FirstPersonControl.FpcBacktracker(base.Owner, fpcModule.Position, global::UnityEngine.Quaternion.identity))
                {
                    global::UnityEngine.Vector3 v = fpcModule.Position - base.ScpRole.FpcModule.Position;
                    if (v.SqrMagnitudeIgnoreY() > _overallTolerance * _overallTolerance || v.y > _overallTolerance || v.y < 0f - _bottomTolerance)
                    {
                        return;
                    }
                }
            }
            using (new global::PlayerRoles.FirstPersonControl.FpcBacktracker(base.Owner, position, global::UnityEngine.Quaternion.identity))
            {
                position = base.ScpRole.FpcModule.Position;
            }
            global::UnityEngine.Transform transform = referenceHub.transform;
            global::UnityEngine.Vector3 position2 = fpcModule.Position;
            global::UnityEngine.Quaternion rotation = transform.rotation;
            global::UnityEngine.Vector3 vector = new global::UnityEngine.Vector3(position.x, position2.y, position.z);
            transform.forward = -base.Owner.transform.forward;
            fpcModule.Position = vector;
            bool flag = referenceHub.playerStats.DealDamage(new global::PlayerRoles.PlayableScps.Scp939.Scp939DamageHandler(base.ScpRole, global::PlayerRoles.PlayableScps.Scp939.Scp939DamageType.LungeTarget));
            float num = (flag ? 1f : 0f);
            if (!flag || referenceHub.IsAlive())
            {
                fpcModule.Position = position2;
                transform.rotation = rotation;
            }
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (!(allHub == referenceHub) && allHub.roleManager.CurrentRole is global::PlayerRoles.HumanRole humanRole2 && !((humanRole2.FpcModule.Position - vector).sqrMagnitude > _secondaryRangeSqr) && allHub.playerStats.DealDamage(new global::PlayerRoles.PlayableScps.Scp939.Scp939DamageHandler(base.ScpRole, global::PlayerRoles.PlayableScps.Scp939.Scp939DamageType.LungeSecondary)))
                {
                    flag = true;
                    num = global::UnityEngine.Mathf.Max(num, 0.6f);
                }
            }
            if (flag)
            {
                Hitmarker.SendHitmarker(base.Owner, num);
            }
            State = global::PlayerRoles.PlayableScps.Scp939.Scp939LungeState.LandHit;
        }

        public override void ServerWriteRpc(global::Mirror.NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            writer.WriteByte((byte)State);
        }

        public override void ClientProcessRpc(global::Mirror.NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            if (!HasAuthority)
            {
                State = (global::PlayerRoles.PlayableScps.Scp939.Scp939LungeState)reader.ReadByte();
            }
        }
    }
}
