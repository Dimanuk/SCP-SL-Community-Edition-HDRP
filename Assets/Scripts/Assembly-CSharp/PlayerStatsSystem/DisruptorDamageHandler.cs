using System.Runtime.CompilerServices;

using Footprinting;

namespace PlayerStatsSystem
{
	public class DisruptorDamageHandler : AttackerDamageHandler
	{
        public override float Damage { get; protected set; }

        public override global::Footprinting.Footprint Attacker { get; protected set; }

        public override bool AllowSelfDamage => false;

        public override string ServerLogsText => "Molecularly disrupted by " + Attacker.Nickname;

        public override string RagdollInspectText => "Molecularly disrupted.";

		public override string DeathScreenText => string.Empty;

        public DisruptorDamageHandler(global::Footprinting.Footprint attacker, float damage)
        {
            Attacker = attacker;
            Damage = damage;
        }

        public override global::PlayerStatsSystem.DamageHandlerBase.HandlerOutput ApplyDamage(ReferenceHub ply)
        {
            global::PlayerStatsSystem.HealthStat module = ply.playerStats.GetModule<global::PlayerStatsSystem.HealthStat>();
            ProcessDamage(ply);
            if (Damage <= 0f)
            {
                return global::PlayerStatsSystem.DamageHandlerBase.HandlerOutput.Nothing;
            }
            module.CurValue -= Damage;
            if (!(module.CurValue <= 0f))
            {
                return global::PlayerStatsSystem.DamageHandlerBase.HandlerOutput.Damaged;
            }
            return global::PlayerStatsSystem.DamageHandlerBase.HandlerOutput.Death;
        }

        public override void ProcessRagdoll(BasicRagdoll _)
        {
            StartVelocity = global::UnityEngine.Vector3.zero;
        }
    }
}
