using PlayerStatsSystem;

namespace PlayerRoles.PlayableScps.Scp049.Zombies
{
    public class ZombieAttackAbility : ScpAttackAbilityBase<ZombieRole>
    {
        public override float DamageAmount => 40f;

        protected override float AttackDelay => 0f;

        protected override float BaseCooldown => 1.3f;

        protected override bool CanTriggerAbility => base.Cooldown.IsReady;

        protected override bool SelfRepeating => false;

        protected override DamageHandlerBase DamageHandler => new Scp049DamageHandler(
            base.Owner, 
            DamageAmount, 
            Scp049DamageHandler.AttackType.Scp0492
        );
    }
}