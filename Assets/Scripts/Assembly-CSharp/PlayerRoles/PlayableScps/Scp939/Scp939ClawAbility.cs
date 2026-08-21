using Mirror;
using PlayerRoles.PlayableScps.Scp939.Mimicry;
using PlayerStatsSystem;

namespace PlayerRoles.PlayableScps.Scp939
{
    public class Scp939ClawAbility : ScpAttackAbilityBase<Scp939Role>
    {
        public const float BaseDamage = 40f;
        public const int DamagePenetration = 75;

        private Scp939FocusAbility _focusAbility;
        private Scp939AmnesticCloudAbility _cloudAbility;

        public override float DamageAmount => BaseDamage;

        protected override float AttackDelay => 0.2f;
        protected override bool KeyPressable
        {
            get
            {
                if (!base.KeyPressable)
                    return false;
                return MimicryMenuController.ReadyForInteraction;
            }
        }

        protected override float BaseCooldown => 0.6f;

        protected override bool CanTriggerAbility
        {
            get
            {
                if (!base.CanTriggerAbility)
                    return false;

                if (_focusAbility.State != 0f)
                    return false;

                return !_cloudAbility.TargetState;
            }
        }

        protected override DamageHandlerBase DamageHandler => 
            new Scp939DamageHandler(base.ScpRole, Scp939DamageType.Claw);

        public override void ServerProcessCmd(NetworkReader reader)
        {
            if (_focusAbility.State == 0f)
                base.ServerProcessCmd(reader);
        }

        protected override void Awake()
        {
            base.Awake();
            GetSubroutine<Scp939FocusAbility>(out _focusAbility);
            GetSubroutine<Scp939AmnesticCloudAbility>(out _cloudAbility);
        }
    }
}