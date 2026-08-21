using UnityEngine;
using PlayerRoles.FirstPersonControl.Thirdperson;

namespace PlayerRoles.PlayableScps.Scp049.Zombies
{
    public class ZombieModel : HumanCharacterModel
    {
        private const int StrafeLayer = 6;
        private const int ConsumeLayer = 8;
        private const float ConsumeTransitionSpeed = 10f;

        
        private static readonly int StrafeHash;
        private static readonly int AttackHash;
        private static readonly int ConsumeHash;
        private ZombieAttackAbility _attackAbility;
        private ZombieConsumeAbility _consumeAbility;

        private float _prevConsume;

        [field: SerializeField]
        public Transform HeadObject { get; private set; }

        private void OnAttack()
        {
            Animator.SetTrigger(AttackHash);
        }

        protected override void Update()
        {
            base.Update();

            float strafeValue = Mathf.Abs(Animator.GetFloat(StrafeHash));
            Animator.SetLayerWeight(StrafeLayer, Mathf.Clamp01(strafeValue));

            float consumeDelta = Time.deltaTime * ConsumeTransitionSpeed;
            int direction = _consumeAbility.IsInProgress ? 1 : -1;
            float newConsume = Mathf.Clamp01(_prevConsume + consumeDelta * direction);

            if (_prevConsume != newConsume)
            {
                if (_prevConsume == 0f)
                    Animator.SetTrigger(ConsumeHash);

                Animator.SetLayerWeight(ConsumeLayer, newConsume);
                _prevConsume = newConsume;
            }
        }

        public override void SpawnObject()
        {
            base.SpawnObject();

            if (OwnerHub.roleManager.CurrentRole is not ZombieRole zombieRole)
                return;

            zombieRole.SubroutineModule.TryGetSubroutine(out _consumeAbility);
            zombieRole.SubroutineModule.TryGetSubroutine(out _attackAbility);

            _attackAbility.OnTriggered += OnAttack;
        }
        public override void ResetObject()
        {
            base.ResetObject();

            if (_attackAbility != null)
                _attackAbility.OnTriggered -= OnAttack;

            _prevConsume = 0f;
        }
        static ZombieModel()
        {
            StrafeHash = Animator.StringToHash("Strafe");
            AttackHash = Animator.StringToHash("Attack");
            ConsumeHash = Animator.StringToHash("Eat");
        }
    }
}