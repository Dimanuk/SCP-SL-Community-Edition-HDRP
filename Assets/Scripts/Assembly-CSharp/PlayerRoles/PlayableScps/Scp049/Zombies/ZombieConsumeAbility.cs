using System.Collections.Generic;
using PlayerStatsSystem;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp049.Zombies
{
    public class ZombieConsumeAbility : RagdollAbilityBase<ZombieRole>
    {
        private enum ConsumeError : byte
        {
            None = 0,
            CannotCancel = 1,
            AlreadyConsumed = 2,
            TargetNotValid = 3,
            FullHealth = 8,
            BeingConsumed = 9
        }

        private const float ConsumeHeal = 100f;

        private static readonly HashSet<ZombieConsumeAbility> AllAbilities = new HashSet<ZombieConsumeAbility>();
        public static readonly HashSet<BasicRagdoll> ConsumedRagdolls = new HashSet<BasicRagdoll>();

        [SerializeField]
        private AnimationCurve _eatAnimRotationFade;

        [SerializeField]
        private AnimationCurve _eatAnimPositionFade;

        private ZombieAttackAbility _attackAbility;
        private Transform _headTransform;
        private bool _headRotationDirty;
        private Vector3 _headRotation;

        protected override float Duration => 7f;
        protected override float RangeSqr => 3.3f;

        protected override void OnKeyDown()
        {
            base.OnKeyDown();
            if (_attackAbility != null && _attackAbility.Cooldown.IsReady)
            {
                ClientTryStart();
            }
        }

        protected override byte ServerValidateCancel()
        {
            return (byte)ConsumeError.CannotCancel;
        }

        protected override byte ServerValidateBegin(BasicRagdoll ragdoll)
        {
            if (ConsumedRagdolls.Contains(ragdoll))
            {
                return (byte)ConsumeError.AlreadyConsumed;
            }

            if (ragdoll == null || !ragdoll.Info.RoleType.IsHuman())
            {
                return (byte)ConsumeError.TargetNotValid;
            }

            if (!ServerValidateAny())
            {
                return (byte)ConsumeError.TargetNotValid;
            }

            HealthStat healthStat = base.Owner.playerStats.GetModule<HealthStat>();
            if (healthStat != null && healthStat.NormalizedValue >= 1f)
            {
                return (byte)ConsumeError.FullHealth;
            }

            foreach (ZombieConsumeAbility ability in AllAbilities)
            {
                if (ability != null && ability.IsInProgress && ability.CurRagdoll == ragdoll)
                {
                    return (byte)ConsumeError.BeingConsumed;
                }
            }

            return (byte)ConsumeError.None;
        }

        protected override bool ServerValidateAny()
        {
            return true;
        }

        protected override void Awake()
        {
            base.Awake();
            GetSubroutine(out _attackAbility);
        }

        protected override void Update()
        {
            base.Update();
            _headRotationDirty = true;
        }

        protected override void ServerComplete()
        {
            if (base.CurRagdoll != null && !ConsumedRagdolls.Contains(base.CurRagdoll))
            {
                ConsumedRagdolls.Add(base.CurRagdoll);
                
                HealthStat healthStat = base.Owner.playerStats.GetModule<HealthStat>();
                if (healthStat != null)
                {
                    healthStat.ServerHeal(ConsumeHeal);
                }
            }
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            AllAbilities.Add(this);

            ZombieModel zombieModel = base.ScpRole.FpcModule.CharacterModelInstance as ZombieModel;
            if (zombieModel != null)
            {
                _headTransform = zombieModel.HeadObject;
            }
        }

        public override void ResetObject()
        {
            base.ResetObject();
            AllAbilities.Remove(this);
        }

        public Vector3 ProcessCamPos(Vector3 original)
        {
            if (_headTransform == null || _eatAnimPositionFade == null)
                return original;

            return Vector3.Lerp(
                original,
                _headTransform.position,
                _eatAnimPositionFade.Evaluate(base.ProgressStatus)
            );
        }

        public Vector3 ProcessRotation()
        {
            if (!_headRotationDirty)
                return _headRotation;

            if (_headTransform == null || _eatAnimRotationFade == null || base.Owner.PlayerCameraReference == null)
            {
                _headRotationDirty = false;
                return _headRotation;
            }

            Quaternion targetRotation = Quaternion.Lerp(
                base.Owner.PlayerCameraReference.rotation,
                _headTransform.rotation,
                _eatAnimRotationFade.Evaluate(base.ProgressStatus)
            );

            _headRotation = targetRotation.eulerAngles;
            _headRotationDirty = false;

            return _headRotation;
        }
    }
}