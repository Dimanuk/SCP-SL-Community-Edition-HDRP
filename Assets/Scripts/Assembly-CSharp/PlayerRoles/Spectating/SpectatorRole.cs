namespace PlayerRoles.Spectating
{
    public class SpectatorRole : global::PlayerRoles.PlayerRoleBase, global::PlayerRoles.SpawnData.IPrivateSpawnDataWriter, global::PlayerRoles.IHealthbarRole, global::PlayerRoles.SpawnData.ISpawnDataReader, global::PlayerRoles.IAdvancedCameraController, global::PlayerRoles.ICameraController, global::GameObjectPools.IPoolSpawnable, global::PlayerRoles.Voice.IVoiceRole, IViewmodelRole, global::PlayerRoles.IAmbientLightRole
    {
        public global::PlayerRoles.Spectating.SpectatorTargetTracker TrackerPrefab;

        public const float SpawnHeight = 6000f;

        private global::UnityEngine.Transform _transformToRestore;

        private global::PlayerStatsSystem.DamageHandlerBase _damageHandler;

        private bool _spawnDataHandledLocally;

        public override global::PlayerRoles.RoleTypeId RoleTypeId => global::PlayerRoles.RoleTypeId.Spectator;

        public override global::PlayerRoles.Team Team => global::PlayerRoles.Team.Dead;

        public override global::UnityEngine.Color RoleColor => global::UnityEngine.Color.white;

        public float VerticalRotation => global::PlayerRoles.Spectating.SpectatorTargetTracker.CurrentOffset.rotation.x;

        public float HorizontalRotation => global::PlayerRoles.Spectating.SpectatorTargetTracker.CurrentOffset.rotation.y;

        public float RollRotation => global::PlayerRoles.Spectating.SpectatorTargetTracker.CurrentOffset.rotation.z;

        public global::UnityEngine.Vector3 CameraPosition => global::PlayerRoles.Spectating.SpectatorTargetTracker.CurrentOffset.position;

        public virtual bool ReadyToRespawn
        {
            get
            {
                if (TryGetOwner(out var hub))
                {
                    return hub.characterClassManager.InstanceMode != ClientInstanceMode.DedicatedServer;
                }
                return false;
            }
        }

        [field: global::UnityEngine.SerializeField]
        public global::PlayerRoles.Voice.VoiceModuleBase VoiceModule { get; private set; }

        public global::RelativePositioning.RelativePosition DeathPosition { get; private set; }

        public uint SyncedSpectatedNetId { get; internal set; }

        public float MaxHealth => 0f;

        public global::PlayerStatsSystem.PlayerStats TargetStats
        {
            get
            {
                if (!global::PlayerRoles.Spectating.SpectatorTargetTracker.TryGetTrackedPlayer(out var hub))
                {
                    return null;
                }
                if (!(hub.roleManager.CurrentRole is global::PlayerRoles.IHealthbarRole healthbarRole))
                {
                    return null;
                }
                return healthbarRole.TargetStats;
            }
        }

        public global::UnityEngine.Color AmbientLight
        {
            get
            {
                if (!global::PlayerRoles.Spectating.SpectatorTargetTracker.TryGetTrackedPlayer(out var hub) || !(hub.roleManager.CurrentRole is global::PlayerRoles.IAmbientLightRole ambientLightRole))
                {
                    return global::CustomPlayerEffects.InsufficientLighting.DefaultColor;
                }
                return ambientLightRole.AmbientLight;
            }
        }

        public bool InsufficientLight => false;

        public override void DisableRole(global::PlayerRoles.RoleTypeId newRole)
        {
            base.DisableRole(newRole);
            _damageHandler = null;
            if (!(_transformToRestore == null))
            {
                _transformToRestore.position = DeathPosition.Position;
                _transformToRestore = null;
            }
        }

        public void SpawnObject()
        {
            if (!TryGetOwner(out var hub))
            {
                throw new global::System.InvalidOperationException("Spectator role failed to spawn - owner is null");
            }
            global::UnityEngine.Transform transform = hub.transform;
            DeathPosition = new global::RelativePositioning.RelativePosition(transform.position);
            transform.position = global::UnityEngine.Vector3.up * 6000f;
            SyncedSpectatedNetId = 0u;
            if (global::Mirror.NetworkServer.active || hub.isLocalPlayer)
            {
                _transformToRestore = transform;
            }

            // On a listen server, PlayerRoleManager never sends the host its own RoleSyncInfo
            // (it skips isLocalPlayer connections), so ReadSpawnData never runs for the host and
            // StartScreen never shows. Deferred one frame so a same-frame ServerSetData (real death)
            // still gets to run ReadSpawnData itself and take priority over this fallback.
            if (global::Mirror.NetworkServer.active && hub.isLocalPlayer)
            {
                _spawnDataHandledLocally = false;
                MEC.Timing.CallDelayed(0f, () =>
                {
                    if (!_spawnDataHandledLocally && TryGetOwner(out var owner) && owner.roleManager.CurrentRole == this)
                    {
                        global::StartScreen.Show(this);
                    }
                });
            }
        }

        public void WritePrivateSpawnData(global::Mirror.NetworkWriter writer)
        {
            if (_damageHandler == null)
            {
                writer.WriteSpawnReason(global::PlayerRoles.Spectating.SpectatorSpawnReason.None);
            }
            else
            {
                _damageHandler.WriteDeathScreen(writer);
            }
            _damageHandler = null;
        }

        public void ReadSpawnData(global::Mirror.NetworkReader reader)
        {
            if (!base.IsLocalPlayer)
                return;

            _spawnDataHandledLocally = true;

            switch (reader.ReadSpawnReason())
            {
                case SpectatorSpawnReason.None:
                    global::StartScreen.Show(this);
                    break;

                case SpectatorSpawnReason.KilledByPlayer:
                    string attackerName = global::Mirror.NetworkReaderExtensions.ReadString(reader);
                    global::PlayerRoles.RoleTypeId attackerRole = reader.ReadRoleType();
                    if (YouWereKilled.Singleton != null)
                        YouWereKilled.Singleton.PlayAttacker(attackerName, attackerRole);
                    break;

                case SpectatorSpawnReason.Other:
                    global::PlayerStatsSystem.DamageHandlerBase handler = global::PlayerStatsSystem.DamageHandlerReaderWriter.ReadDamageHandler(reader);
                    if (YouWereKilled.Singleton != null)
                        YouWereKilled.Singleton.PlayRegular(handler);
                    break;
            }
        }

        public void ServerSetData(global::PlayerStatsSystem.DamageHandlerBase dhb)
        {
            _damageHandler = dhb;

            if (!base.IsLocalPlayer)
                return;

            global::Mirror.NetworkWriter writer = new global::Mirror.NetworkWriter();
            WritePrivateSpawnData(writer);
            global::Mirror.NetworkReader reader = new global::Mirror.NetworkReader(writer.ToArraySegment());
            ReadSpawnData(reader);
        }

        public bool TryGetViewmodelFov(out float fov)
        {
            if (SpectatorTargetTracker.CurrentTarget is IViewmodelRole viewmodelRole)
                return viewmodelRole.TryGetViewmodelFov(out fov);

            fov = 0f;
            return false;
        }
    }
}
