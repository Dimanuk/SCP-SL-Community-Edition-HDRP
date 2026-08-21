using System.Runtime.CompilerServices;

using Footprinting;
using UnityEngine;

namespace PlayerStatsSystem
{
	public class JailbirdDamageHandler : AttackerDamageHandler
	{
		private readonly Vector3 _moveDirection;

		private const float ZombieDamageMultiplier = 4f;

		private const float UpwardsForce = 0.02f;

		private const float HorizontalForce = 0.1f;

		public override float Damage { get; protected set; }

		public override Footprint Attacker { get; protected set; }

		public override bool AllowSelfDamage => false;
        public override string ServerLogsText => "Jailbirded by " + Attacker.Nickname;

        public override string RagdollInspectText => "Blunt force trauma.";

		public override string DeathScreenText
		{
			get
			{
				return string.Empty;
			}
		}

        public JailbirdDamageHandler()
        {
            Attacker = default(global::Footprinting.Footprint);
            Damage = 0f;
            _moveDirection = global::UnityEngine.Vector3.zero;
        }

        public JailbirdDamageHandler(ReferenceHub attacker, float damage, global::UnityEngine.Vector3 moveDirection)
        {
            Attacker = new global::Footprinting.Footprint(attacker);
            Damage = damage;
            _moveDirection = moveDirection;
        }

        public override global::PlayerStatsSystem.DamageHandlerBase.HandlerOutput ApplyDamage(ReferenceHub ply)
        {
            global::PlayerStatsSystem.HealthStat module = ply.playerStats.GetModule<global::PlayerStatsSystem.HealthStat>();
            ProcessDamage(ply);
            if (Damage <= 0f)
            {
                return global::PlayerStatsSystem.DamageHandlerBase.HandlerOutput.Nothing;
            }
            if (global::PlayerRoles.PlayerRolesUtils.GetRoleId(ply) == global::PlayerRoles.RoleTypeId.Scp0492)
            {
                Damage *= 4f;
            }
            module.CurValue -= Damage;
            StartVelocity += (_moveDirection.NormalizeIgnoreY() * 0.1f + global::UnityEngine.Vector3.up * 0.02f) * Damage;
            if (!(module.CurValue <= 0f))
            {
                return global::PlayerStatsSystem.DamageHandlerBase.HandlerOutput.Damaged;
            }
            return global::PlayerStatsSystem.DamageHandlerBase.HandlerOutput.Death;
        }
    }
}
