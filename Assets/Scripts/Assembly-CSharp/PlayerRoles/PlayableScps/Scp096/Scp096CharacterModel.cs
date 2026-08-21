using System.Runtime.CompilerServices;
using PlayerRoles.FirstPersonControl.Thirdperson;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp096
{
    public class Scp096CharacterModel : AnimatedCharacterModel
    {
        private static readonly int AnimatorEnragingHash = Animator.StringToHash("Enraging");
        private static readonly int AnimatorEnragedHash = Animator.StringToHash("Enraged");
        private static readonly int AnimatorChargingHash= Animator.StringToHash("Charging");
        private static readonly int AnimatorTryNotToCryHash = Animator.StringToHash("TryNotToCry");
        private static readonly int AnimatorLeftAttackHash= Animator.StringToHash("LeftAttack");
        private static readonly int AnimatorPryGateHash = Animator.StringToHash("PryGate");
        private static readonly int AnimatorCalmingHash= Animator.StringToHash("Calming");
        private static readonly int AnimatorAttackHash = Animator.StringToHash("Attack");

        [field: SerializeField]
        public Transform Head { get; private set; }

        [SerializeField]
        private Animator _thirdPersonAnimator;

        [SerializeField]
        private ParticleSystem _shieldParticles;

        private Scp096Role _role;
        private Scp096AttackAbility _attackAbility;
        private Scp096RageManager _rageAbility;

        public override void SpawnObject()
        {
            base.SpawnObject();

            _role = base.OwnerHub.roleManager.CurrentRole as Scp096Role;
            _role.SubroutineModule.TryGetSubroutine(out _attackAbility);
            _role.SubroutineModule.TryGetSubroutine(out _rageAbility);
        }

        protected override void Update()
        {
            base.Update();

            if (_rageAbility == null)
                return;

            bool tryingNotToCry = _role.IsAbilityState(Scp096AbilityState.TryingNotToCry);

            _thirdPersonAnimator.SetBool(AnimatorEnragedHash, _rageAbility.IsEnraged);

            _thirdPersonAnimator.SetBool(AnimatorEnragingHash, _rageAbility.IsDistressed);

            _thirdPersonAnimator.SetBool(AnimatorChargingHash, _role.IsAbilityState(Scp096AbilityState.Charging));

            _thirdPersonAnimator.SetBool(AnimatorTryNotToCryHash, tryingNotToCry);

            _thirdPersonAnimator.SetBool(AnimatorAttackHash, _role.IsAbilityState(Scp096AbilityState.Attacking));

            _thirdPersonAnimator.SetBool(AnimatorLeftAttackHash, _attackAbility.LeftAttack);

            _thirdPersonAnimator.SetBool(AnimatorPryGateHash, _role.IsAbilityState(Scp096AbilityState.PryingGate));

            _thirdPersonAnimator.SetBool(AnimatorCalmingHash, _role.IsRageState(Scp096RageState.Calming));

            ParticleSystem.MainModule main = _shieldParticles.main;

            float humeParticleCount = (!_role.IsLocalPlayer 
                                       && _role.HumeShieldModule.HsCurrent > 0f 
                                       && !tryingNotToCry)
                ? (_role.HumeShieldModule.HsCurrent / _role.HumeShieldModule.HsMax * 40f)
                : 0f;

            main.maxParticles = (int)(humeParticleCount 
                + ((_rageAbility.IsEnraged && !_role.IsLocalPlayer) ? 10f : 0f));

            main.simulationSpeed = ((!_rageAbility.IsEnraged || _role.IsLocalPlayer) ? 1f : 2f);
        }
    }
}