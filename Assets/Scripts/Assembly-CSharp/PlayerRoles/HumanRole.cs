using InventorySystem;
using InventorySystem.Items.Armor;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Spawnpoints;
using Respawning;
using Respawning.NamingRules;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerRoles
{
    public class HumanRole : FpcStandardRoleBase, IArmoredRole
    {
        private static readonly Dictionary<RoleTypeId, RoomRoleSpawnpoint[]> SpawnpointsForRoles = new ();

        [SerializeField]
        private RoleTypeId _roleId;

        [SerializeField]
        private Team _team;

        [SerializeField]
        private Color _roleColor;

        [SerializeField]
        private RoomRoleSpawnpoint[] _spawnpoints;

        [field: SerializeField]
        public SpawnableTeamType AssignedSpawnableTeam { get; private set; }

        public SpawnableTeamType UnitNameTeam => AssignedSpawnableTeam;

        public override RoleTypeId RoleTypeId => _roleId;

        public override Team Team => _team;

        public override Color RoleColor => _roleColor;

        public override float MaxHealth => 100f;

        public byte UnitNameId { get; private set; }

        public int UnitNameIndex => UnitNameId;

        public bool UsesUnitNames
        {
            get
            {
                return UnitNamingRule.TryGetNamingRule(AssignedSpawnableTeam, out _);
            }
        }

        public string UnitName
        {
            get
            {
                if (!UsesUnitNames)
                    return string.Empty;

                if (UnitNameMessageHandler.ReceivedNames.TryGetValue(AssignedSpawnableTeam, out var names) &&
                    UnitNameId < names.Count)
                {
                    return names[UnitNameId];
                }

                return string.Empty;
            }
        }

        public override ISpawnpointHandler SpawnpointHandler
        {
            get
            {
                if (SpawnpointsForRoles.TryGetValue(_roleId, out var cached))
                {
                    return cached?.RandomItem();
                }

                int count = _spawnpoints.Length;
                if (count == 0)
                {
                    SpawnpointsForRoles[_roleId] = null;
                    return null;
                }

                var spawnpoints = new RoomRoleSpawnpoint[count];
                for (int i = 0; i < count; i++)
                {
                    spawnpoints[i] = new RoomRoleSpawnpoint(_spawnpoints[i]);
                }

                SpawnpointsForRoles[_roleId] = spawnpoints;
                return spawnpoints.RandomItem();
            }
        }

        public override void WritePublicSpawnData(NetworkWriter writer)
        {
            if (UsesUnitNames)
            {
                writer.WriteByte(UnitNameId);
            }

            base.WritePublicSpawnData(writer);
        }

        public override void ReadSpawnData(NetworkReader reader)
        {
            if (UsesUnitNames)
            {
                UnitNameId = reader.ReadByte();
            }

            base.ReadSpawnData(reader);
        }

        internal override void Init(ReferenceHub hub, RoleChangeReason spawnReason, RoleSpawnFlags spawnFlags)
        {
            base.Init(hub, spawnReason, spawnFlags);

            if (NetworkServer.active && UsesUnitNames)
            {
                UnitNameId = (byte)(UnitNameMessageHandler.ReceivedNames.TryGetValue(AssignedSpawnableTeam, out var list)
                    ? list.Count
                    : 0);

                if (UnitNameId != 0 && spawnReason != RoleChangeReason.Respawn)
                {
                    UnitNameId--;
                }
            }
        }

        public int GetArmorEfficacy(HitboxType hitbox)
        {
            if (!TryGetOwner(out ReferenceHub hub) ||
                !hub.inventory.TryGetBodyArmor(out BodyArmor bodyArmor))
            {
                return 0;
            }

            return hitbox switch
            {
                HitboxType.Headshot => bodyArmor.HelmetEfficacy,
                HitboxType.Body => bodyArmor.VestEfficacy,
                _ => 0
            };
        }
    }
}