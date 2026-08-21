using CameraShaking;
using Knife.DeferredDecals;
using PlayerRoles.FirstPersonControl.Thirdperson;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp106
{
    public class Scp106Model : AnimatedCharacterModel
    {
        private static readonly int SubmergeHash = Animator.StringToHash("IsSubmerged");
        private static readonly int SpeedHash = Animator.StringToHash("TransitionSpeed");

        private const float PortalRotationSpeed = 21f;

        [SerializeField]
        private AnimationCurve _submergeAnim;

        [SerializeField]
        private AnimationCurve _appearAnim;

        [SerializeField]
        private GameObject[] _hiddenObjects;

        [SerializeField]
        private Transform _balanceTransform;

        [SerializeField]
        private Transform _portalTransform;

        [SerializeField]
        private ParticleSystem _stalkParticles;

        [SerializeField]
        private AnimationCurve _portalScale;

        [field: SerializeField]
        public Transform StalkCameraTarget { get; private set; }

        private Decal[] _portalDecals;
        private Transform _tr;
        private Vector3 _defaultPos;
        private Scp106SinkholeController _sinkhole;
        private Scp106StalkAbility _stalkAbility;
        private Scp106MovementModule _fpc;

        protected override bool FootstepPlayable
        {
            get
            {
                if (base.FpcModule.IsGrounded && base.FpcModule.Motor.MovementDetected)
                {
                    return LandingFootstepPlayable;
                }
                return false;
            }
        }

        protected override bool LandingFootstepPlayable => _sinkhole != null && _sinkhole.NormalizedState == 0f;

        private void LateUpdate()
        {
            if (base.Pooled || _sinkhole == null)
                return;

            base.Animator.SetFloat(SpeedHash, _sinkhole.SpeedMultiplier);

            bool isHidden = _sinkhole.IsHidden;
            GameObject[] hiddenObjects = _hiddenObjects;
            for (int i = 0; i < hiddenObjects.Length; i++)
            {
                hiddenObjects[i].SetActive(!isHidden);
            }

            if (base.IsTracked)
            {
                SetVisibility(_sinkhole.IsDuringAnimation);
            }

            base.Animator.SetBool(SubmergeHash, _sinkhole.State);

            float normalizedState = _sinkhole.NormalizedState;
            AnimationCurve animationCurve = _sinkhole.State ? _submergeAnim : _appearAnim;
            Vector3 vector = Vector3.up * animationCurve.Evaluate(normalizedState);
            _tr.localPosition = _defaultPos + vector;

            float decalRotationSpeed = (1f - normalizedState) * PortalRotationSpeed;

            float portalScale = _portalScale.Evaluate(normalizedState);
            _portalTransform.localScale = new Vector3(portalScale, 1f, portalScale);

            _balanceTransform.localPosition = -vector / _tr.localScale.y;

            ParticleSystem.EmissionModule emission = _stalkParticles.emission;
            emission.enabled = ReferenceHub.TryGetLocalHub(out ReferenceHub localHub)
                && !localHub.IsHuman()
                && _stalkAbility.IsActive
                && _sinkhole.IsHidden;

            if (_sinkhole.IsDuringAnimation)
            {
                for (int i = 0; i < _portalDecals.Length; i++)
                {
                    Vector3 axis = (i == 1) ? Vector3.up : Vector3.down;
                    _portalDecals[i].transform.Rotate(axis * (decalRotationSpeed * Time.deltaTime));
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            if (_sinkhole != null && _sinkhole.NormalizedState > 0f)
            {
                base.HeadBobPosition = Vector3.zero;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _tr = base.transform;
            _portalDecals = GetComponentsInChildren<Decal>();
        }

        public override void SpawnObject()
        {
            base.SpawnObject();

            if (!(base.OwnerHub.roleManager.CurrentRole is Scp106Role scp106Role))
                return;

            scp106Role.SubroutineModule.TryGetSubroutine(out _stalkAbility);
            _fpc = scp106Role.FpcModule as Scp106MovementModule;
            _defaultPos = _fpc.CharacterModelTemplate.transform.localPosition;
            _sinkhole = scp106Role.Sinkhole;

            if (base.IsTracked)
            {
                CameraShakeController.AddEffect(new Scp106PortalShake(scp106Role, this));
            }
        }
    }
}
