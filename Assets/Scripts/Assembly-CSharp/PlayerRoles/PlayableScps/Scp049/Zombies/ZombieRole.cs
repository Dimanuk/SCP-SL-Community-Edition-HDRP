using System;
using System.Runtime.CompilerServices;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.HUDs;
using PlayerRoles.PlayableScps.HumeShield;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.PlayableScps.Scp049;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace PlayerRoles.PlayableScps.Scp049.Zombies
{
    public class ZombieRole : FpcStandardScp,
        ISubroutinedScpRole,
        IHumeShieldedRole,
        IHudScp,
        IAdvancedCameraController,
        ICameraController
    {
        [SerializeField]
        private GameObject _confirmBoxPrefab;

        [SerializeField]
        private ushort _specialMaxHp = 600;
        [SerializeField]
        private float _revivesModifier = 0.9f;

        private ushort _syncMaxHealth;
        private ZombieConsumeAbility _consumeAbility;

        public override float MaxHealth => (int)_syncMaxHealth;

        public override Vector3 CameraPosition => _consumeAbility.ProcessCamPos(base.CameraPosition);

        public override float HorizontalRotation => _consumeAbility.ProcessRotation().y;
        public override float VerticalRotation   => _consumeAbility.ProcessRotation().x;
        public float RollRotation                => _consumeAbility.ProcessRotation().z;

        [field: SerializeField]
        public HumeShieldModuleBase HumeShieldModule { get; private set; }

        [field: SerializeField]
        public SubroutineManagerModule SubroutineModule { get; private set; }

        [field: SerializeField]
        public ScpHudBase HudPrefab { get; private set; }

        private void Awake()
        {
            SubroutineModule.TryGetSubroutine(out _consumeAbility);
        }

        public override void WritePublicSpawnData(NetworkWriter writer)
        {
            writer.WriteUShort(_syncMaxHealth);
            base.WritePublicSpawnData(writer);
        }

        public override void ReadSpawnData(NetworkReader reader)
        {
            _syncMaxHealth = reader.ReadUShort();
            base.ReadSpawnData(reader);
        }

        public override void SpawnObject()
        {
            base.SpawnObject();

            if (!TryGetOwner(out var owner) || !NetworkServer.active)
                return;

            if (HashsetExtensions.Any(ReferenceHub.AllHubs, x =>
            {
                if (x.roleManager.CurrentRole is Scp049Role scp049Role &&
                    scp049Role.SubroutineModule.TryGetSubroutine(out Scp049SenseAbility sense) &&
                    sense.DeadTargets.Contains(owner))
                {
                    return true;
                }
                return false;
            }))
            {
                _syncMaxHealth = _specialMaxHp;
            }
            else
            {
                int resurrectionsNumber = Scp049ResurrectAbility.GetResurrectionsNumber(owner);
                float health = base.MaxHealth;
                for (int i = 0; i < resurrectionsNumber; i++)
                {
                    health *= _revivesModifier;
                }
                _syncMaxHealth = (ushort)(Mathf.RoundToInt(health / 10f) * 10);
            }

            Scp049ResurrectAbility.RegisterPlayerResurrection(owner);
        }

        public override void DisableRole(RoleTypeId newRole)
        {
            bool isLocalPlayer = base.IsLocalPlayer;
            base.DisableRole(newRole);
            _syncMaxHealth = 0;

            if (isLocalPlayer && newRole == RoleTypeId.Spectator &&
                HashsetExtensions.Any(ReferenceHub.AllHubs, x => x.roleManager.CurrentRole is Scp049Role))
            {
                Instantiate(_confirmBoxPrefab);
            }
        }
    }
}