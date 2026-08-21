using Footprinting;
using InventorySystem.Items.Armor;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp939;
using PlayerStatsSystem;
using RelativePositioning;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939
{
    public class Scp939DamageHandler : AttackerDamageHandler
    {
        private const float LungeUpwardsSpeed = 3.5f;
        private const float LungeTotalSpeed = 5.5f;

        private string _ragdollInspect;
        private Scp939DamageType _damageType;
        private RagdollAnimationTemplate _lungeTemplate;
        private RelativePosition _hitPos;
        private bool _lungeTemplateSet;

        public override bool AllowSelfDamage => false;
        public override float Damage { get; protected set; }
        public override Footprint Attacker { get; protected set; }
        public override string ServerLogsText => string.Format("Killed by SCP-939 ({0}).", _damageType);
        public override string RagdollInspectText => _ragdollInspect;
        public override string DeathScreenText => string.Empty;

        private RagdollAnimationTemplate LungeTemplate
        {
            get
            {
                if (!_lungeTemplateSet)
                {
                    if (PlayerRoleLoader.TryGetRoleTemplate<Scp939Role>(RoleTypeId.Scp939, out Scp939Role roleTemplate))
                    {
                        // if (roleTemplate.TryGetSubroutine<Scp939LungeAbility>(out Scp939LungeAbility lungeAbility))
                        // {
                        //     _lungeTemplate = lungeAbility.LungeAnimationTemplate;
                        //     _lungeTemplateSet = true;
                        // }
                    }
                }
                return _lungeTemplate;
            }
        }

        public const float ClawDamage = 40f;
        public const float LungeTargetDamage = 120f;
        public const float LungeSecondaryDamage = 30f;

        public Scp939DamageHandler(Scp939Role scp939, Scp939DamageType type)
        {
            _damageType = type;

            switch (type)
            {
                case Scp939DamageType.Claw:
                    Damage = ClawDamage;
                    break;
                case Scp939DamageType.LungeTarget:
                    Damage = LungeTargetDamage;
                    break;
                case Scp939DamageType.LungeSecondary:
                    Damage = LungeSecondaryDamage;
                    break;
            }

            if (scp939 != null)
            {
                ReferenceHub hub = scp939.GetComponentInParent<ReferenceHub>();
                if (hub != null)
                {
                    Attacker = new Footprint(scp939.GetComponentInParent<ReferenceHub>());
                }
                _hitPos = new RelativePosition(scp939.FpcModule.Position);
            }
        }

        protected override void ProcessDamage(ReferenceHub ply)
        {
            if (!ply.roleManager._anySet)
            {
                ply.roleManager.InitializeNewRole(RoleTypeId.None, RoleChangeReason.Died, RoleSpawnFlags.None, null);
            }

            if (_damageType == Scp939DamageType.LungeTarget && ply.roleManager.CurrentRole != null)
            {
                int armorEfficacy = 1;
                _ = BodyArmorUtils.ProcessDamage((int)Damage, armorEfficacy, (int)75f);
            }

            base.ProcessDamage(ply);
        }

        public override void WriteAdditionalData(NetworkWriter writer)
        {
            base.WriteAdditionalData(writer);
            writer.WriteByte((byte)_damageType);

            if (_damageType == Scp939DamageType.LungeSecondary)
            {
                RelativePositionSerialization.WriteRelativePosition(writer, _hitPos);
            }
        }

        public override void ReadAdditionalData(NetworkReader reader)
        {
            base.ReadAdditionalData(reader);
            _damageType = (Scp939DamageType)reader.ReadByte();

            if (_damageType == Scp939DamageType.LungeSecondary)
            {
                _hitPos = RelativePositionSerialization.ReadRelativePosition(reader);
            }

            if (_damageType == Scp939DamageType.LungeSecondary)
                _ragdollInspect = DeathTranslations.Scp939Lunge.RagdollTranslation;
            else
                _ragdollInspect = DeathTranslations.Scp939Other.RagdollTranslation;
        }

        public override void ProcessRagdoll(BasicRagdoll ragdoll)
        {
            if (ragdoll == null) return;

            if (_damageType == Scp939DamageType.LungeSecondary)
            {
                LungeTemplate?.ProcessRagdoll(ragdoll);
            }
            else if (_damageType == Scp939DamageType.None)
            {
                Vector3 dir = ragdoll.transform.position - _hitPos.Position;
                dir.y = 0f;
                dir.Normalize();
                // ragdoll.Rigidbody.velocity = dir * LungeTotalSpeed + Vector3.up * LungeUpwardsSpeed;
            }

            base.ProcessRagdoll(ragdoll);
        }
    }
}