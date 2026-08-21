namespace PlayerRoles.PlayableScps.Scp079
{
    public class Scp079Role : global::PlayerRoles.PlayerRoleBase, global::PlayerRoles.PlayableScps.Subroutines.ISubroutinedScpRole, global::PlayerRoles.Spectating.ISpectatableRole, global::PlayerRoles.PlayableScps.ISpawnableScp, global::PlayerRoles.IAdvancedCameraController, global::PlayerRoles.ICameraController, global::GameObjectPools.IPoolResettable, global::GameObjectPools.IPoolSpawnable, global::PlayerRoles.Voice.IVoiceRole, global::PlayerRoles.IAvatarRole, global::PlayerRoles.PlayableScps.HUDs.IHudScp, global::PlayerRoles.IAmbientLightRole, global::PlayerRoles.IAFKRole
    {
        public static readonly global::System.Collections.Generic.HashSet<global::PlayerRoles.PlayableScps.Scp079.Scp079Role> ActiveInstances = new global::System.Collections.Generic.HashSet<global::PlayerRoles.PlayableScps.Scp079.Scp079Role>();

        private global::PlayerRoles.PlayableScps.Scp079.Cameras.Scp079CurrentCameraSync _curCamSync;

        private global::UnityEngine.Vector3 _lastCamPos;

        public static global::PlayerRoles.PlayableScps.Scp079.Scp079Role LocalInstance { get; private set; }

        public static bool LocalInstanceActive { get; private set; }

        [field: global::UnityEngine.SerializeField]
        public global::PlayerRoles.PlayableScps.HUDs.ScpHudBase HudPrefab { get; private set; }

        [field: global::UnityEngine.SerializeField]
        public global::PlayerRoles.PlayableScps.Subroutines.SubroutineManagerModule SubroutineModule { get; private set; }

        [field: global::UnityEngine.SerializeField]
        public global::PlayerRoles.Voice.VoiceModuleBase VoiceModule { get; private set; }

        [field: global::UnityEngine.SerializeField]
        public global::UnityEngine.Texture RoleAvatar { get; private set; }

        [field: global::UnityEngine.SerializeField]
        public global::PlayerRoles.Spectating.SpectatableModuleBase SpectatorModule { get; private set; }

        public override global::PlayerRoles.RoleTypeId RoleTypeId => global::PlayerRoles.RoleTypeId.Scp079;

        public override global::PlayerRoles.Team Team => global::PlayerRoles.Team.SCPs;

        public override global::UnityEngine.Color RoleColor => global::UnityEngine.Color.red;

        public global::UnityEngine.Vector3 CameraPosition => CurrentCamera.CameraPosition;

        public float VerticalRotation => CurrentCamera.VerticalRotation;

        public float HorizontalRotation => CurrentCamera.HorizontalRotation;

        public float RollRotation => CurrentCamera.RollRotation;

        public global::PlayerRoles.PlayableScps.Scp079.Cameras.Scp079Camera CurrentCamera => _curCamSync.CurrentCamera;

        public bool IsSpectated
        {
            get
            {
                if (global::PlayerRoles.Spectating.SpectatorTargetTracker.TryGetTrackedPlayer(out var hub))
                {
                    return hub.roleManager.CurrentRole == this;
                }
                return false;
            }
        }

        public global::UnityEngine.Color AmbientLight
        {
            get
            {
                if (!VeryHighPerformance.LightsOff)
                {
                    return new global::UnityEngine.Color(0.07f, 0.07f, 0.07f, 1f);
                }
                return VeryHighPerformance.VHColor;
            }
        }

        public bool InsufficientLight => false;

        public bool IsAFK
        {
            get
            {
                if (_lastCamPos == global::UnityEngine.Vector3.zero)
                {
                    _lastCamPos = CurrentCamera.CameraPosition;
                }
                global::UnityEngine.Vector3 cameraPosition = CurrentCamera.CameraPosition;
                if (cameraPosition == _lastCamPos)
                {
                    return true;
                }
                _lastCamPos = cameraPosition;
                return false;
            }
        }

        public void ResetObject()
        {
            ActiveInstances.Remove(this);
            _lastCamPos = global::UnityEngine.Vector3.zero;
        }

        public void SpawnObject()
        {
            ActiveInstances.Add(this);
            if (!TryGetOwner(out var hub))
            {
                throw new global::System.InvalidOperationException("SCP-079 role failed to spawn - owner is null");
            }
            float num = 6000f;
            hub.transform.position = global::UnityEngine.Vector3.up * num;
            _lastCamPos = global::UnityEngine.Vector3.zero;
            // Карточка класса (имя роли + описание) при спавне. В v12 её у 079 не было
            // (StartScreen вызывали только FpcStandardRoleBase/SpectatorRole) — добавлено
            // по просьбе пользователя, как в современной SL. Player Canvas — overlay,
            // поэтому карточка рисуется поверх камерной цепочки 079 HUD.
            if (IsLocalPlayer)
            {
                global::StartScreen.Show(this);
            }
        }

        private void Awake()
        {
            SubroutineModule.TryGetSubroutine<global::PlayerRoles.PlayableScps.Scp079.Cameras.Scp079CurrentCameraSync>(out _curCamSync);
        }

        private void OnDestroy()
        {
            ActiveInstances.Remove(this);
        }

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            global::PlayerRoles.PlayerRoleManager.OnRoleChanged += delegate (ReferenceHub x, global::PlayerRoles.PlayerRoleBase y, global::PlayerRoles.PlayerRoleBase z)
            {
                if (x.isLocalPlayer)
                {
                    if (z is global::PlayerRoles.PlayableScps.Scp079.Scp079Role localInstance)
                    {
                        LocalInstance = localInstance;
                        LocalInstanceActive = true;
                    }
                    else
                    {
                        LocalInstanceActive = false;
                    }
                }
            };
        }

        public float GetSpawnChance(global::System.Collections.Generic.List<global::PlayerRoles.RoleTypeId> alreadySpawned)
        {
            int count = alreadySpawned.Count;
            return (count != 0 && !alreadySpawned.Contains(global::PlayerRoles.RoleTypeId.Scp096)) ? count : 0;
        }
    }
}
