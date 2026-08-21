using CustomPlayerEffects;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Ragdolls;
using UnityEngine;

namespace PlayerStatsSystem
{
    public abstract class StandardDamageHandler : DamageHandlerBase
    {
        public const float KillValue = -1f;

        public HitboxType Hitbox;
        protected Vector3 StartVelocity;

        private short _velX;
        private short _velY;
        private short _velZ;

        public abstract float Damage { get; protected set; }
        public float DealtHealthDamage { get; private set; }
        public float AbsorbedAhpDamage { get; private set; }
        public float AbsorbedHumeDamage { get; private set; }

        public override HandlerOutput ApplyDamage(ReferenceHub ply)
        {
            StartVelocity = FpcExtensionMethods.GetVelocity(ply);
            StartVelocity.y = Mathf.Max(StartVelocity.y, 0f);

            PlayerStats playerStats = ply.playerStats;
            if (ply.roleManager.CurrentRole is IHealthbarRole healthbarRole && healthbarRole.TargetStats != playerStats)
            {
                return HandlerOutput.Nothing;
            }

            AhpStat ahpStat = playerStats.GetModule<AhpStat>();
            HealthStat healthStat = playerStats.GetModule<HealthStat>();
            HumeShieldStat humeShieldStat = playerStats.GetModule<HumeShieldStat>();

            if (Damage == -1f)
            {
                ahpStat.CurValue = 0f;
                healthStat.CurValue = 0f;
                return HandlerOutput.Death;
            }

            ProcessDamage(ply);

            StatusEffectBase[] allEffects = ply.playerEffectsController.AllEffects;
            foreach (StatusEffectBase statusEffectBase in allEffects)
            {
                if (statusEffectBase.IsEnabled && statusEffectBase is IDamageModifierEffect damageModifierEffect)
                {
                    Damage *= damageModifierEffect.GetDamageModifier(Damage, this, Hitbox);
                }
            }

            if (Damage <= 0f)
            {
                return HandlerOutput.Nothing;
            }

            float healthBefore = healthStat.CurValue;
            float afterAhp = ahpStat.ServerProcessDamage(Damage);
            AbsorbedAhpDamage = Damage - afterAhp;
            AbsorbedHumeDamage = Mathf.Min(humeShieldStat.CurValue, afterAhp);
            float humeAfter = humeShieldStat.CurValue - afterAhp;
            if (humeAfter < 0f)
            {
                healthStat.CurValue += humeAfter;
            }
            humeShieldStat.CurValue = humeAfter;
            DealtHealthDamage = healthBefore - healthStat.CurValue;

            return healthStat.CurValue <= 0f ? HandlerOutput.Death : HandlerOutput.Damaged;
        }

        protected virtual void ProcessDamage(ReferenceHub ply) { }

        public override void WriteAdditionalData(NetworkWriter writer)
        {
            base.WriteAdditionalData(writer);
            _velX = (short)Mathf.RoundToInt(StartVelocity.x * 100f);
            _velY = (short)Mathf.RoundToInt(StartVelocity.y * 100f);
            _velZ = (short)Mathf.RoundToInt(StartVelocity.z * 100f);

            writer.WriteShort(_velX);
            writer.WriteShort(_velY);
            writer.WriteShort(_velZ);
        }

        public override void ReadAdditionalData(NetworkReader reader)
        {
            base.ReadAdditionalData(reader);
            _velX = reader.ReadShort();
            _velY = reader.ReadShort();
            _velZ = reader.ReadShort();
        }

        public override void ProcessRagdoll(BasicRagdoll ragdoll)
        {
            base.ProcessRagdoll(ragdoll);
            if (ragdoll is DynamicRagdoll dynamicRagdoll)
            {
                Vector3 velocity = new Vector3(_velX, _velY, _velZ) / 100f;
                Rigidbody[] linkedRigidbodies = dynamicRagdoll.LinkedRigidbodies;
                for (int i = 0; i < linkedRigidbodies.Length; i++)
                {
                    linkedRigidbodies[i].linearVelocity = velocity;
                }
            }
        }
    }
}