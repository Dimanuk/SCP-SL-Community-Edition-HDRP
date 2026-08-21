using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Footprinting;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049;
using PlayerRoles.Spectating;

namespace PlayerStatsSystem
{
    public class Scp049DamageHandler : AttackerDamageHandler
    {
        public enum AttackType : byte
        {
            Instakill = 0,
            CardiacArrest = 1,
            Scp0492 = 2
        }

        private static readonly Dictionary<AttackType, string> LogReasons;
        private readonly string _ragdollInspectText;

        public override float Damage { get; protected set; }
        public override string RagdollInspectText => _ragdollInspectText;
        public override string DeathScreenText => string.Empty;
        public override CassieAnnouncement CassieDeathAnnouncement => new CassieAnnouncement();
        public override bool AllowSelfDamage => false;
        
        [CompilerGenerated]
        public override Footprint Attacker { get; protected set; }
        
        public AttackType DamageSubType { get; private set; }

        public override string ServerLogsText
        {
            get
            {
                if (LogReasons.TryGetValue(DamageSubType, out string reason))
                    return $"SCP-049 ({reason})";
                return "SCP-049";
            }
        }

        static Scp049DamageHandler()
        {
            LogReasons = new Dictionary<AttackType, string>
            {
                { AttackType.Instakill, "Instakill" },
                { AttackType.CardiacArrest, "Cardiac Arrest" },
                { AttackType.Scp0492, "SCP-049-2" }
            };
        }

        public Scp049DamageHandler()
        {
            _ragdollInspectText = DeathTranslations.Scp049.RagdollTranslation;
        }

        public Scp049DamageHandler(ReferenceHub attacker, float damage, AttackType attackType) 
        {
            Damage = damage;
            Attacker = new Footprint(attacker);
            DamageSubType = attackType;
        }

        public Scp049DamageHandler(Footprint attacker, float damage, AttackType attackType)
        {
            Damage = damage;
            Attacker = attacker;
            DamageSubType = attackType;
        }

        public override HandlerOutput ApplyDamage(ReferenceHub ply)
        {
            HandlerOutput result = base.ApplyDamage(ply);

            if (!NetworkServer.active || result != HandlerOutput.Death)
            {
                return result;
            }

            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.roleManager.CurrentRole is Scp049Role scp049Role && scp049Role.SubroutineModule.TryGetSubroutine<Scp049SenseAbility>(out var subroutine))
                {
                    subroutine.ServerProcessKilledPlayer(ply);
                }
            }

            return result;
        }

        public override void WriteDeathScreen(NetworkWriter writer)
        {
            RoleTypeId role = (Attacker.Role != RoleTypeId.Scp0492) ? RoleTypeId.Scp049 : RoleTypeId.Scp0492;
            SpectatorSpawnReasonReaderWriter.WriteSpawnReason(writer, SpectatorSpawnReason.KilledByPlayer);
            writer.WriteString(Attacker.Nickname);
            writer.WriteRoleType(role);
        }

        public override void WriteAdditionalData(NetworkWriter writer)
        {
            base.WriteAdditionalData(writer);
            writer.WriteByte((byte)DamageSubType);
        }

        public override void ReadAdditionalData(NetworkReader reader)
        {
            base.ReadAdditionalData(reader);
            DamageSubType = (AttackType)reader.ReadByte();
        }
    }
}