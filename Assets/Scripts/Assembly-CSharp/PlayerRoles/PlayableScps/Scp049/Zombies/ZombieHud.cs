using PlayerRoles.PlayableScps.HUDs;
using UnityEngine;
using UnityEngine.UI;
using InventorySystem.Items;
using PlayerRoles.FirstPersonControl;

namespace PlayerRoles.PlayableScps.Scp049.Zombies
{
    public class ZombieHud : ScpHudBase, IViewmodelRole
    {
        [SerializeField]
        private AbilityHud _attackCooldownIcon;

        [SerializeField]
        private LoadingCircleHud _consumeCircle;

        [SerializeField]
        private Image _bloodlustBackground;

        [SerializeField]
        private Image _bloodlustFill;

        [SerializeField]
        private float _bloodlustAlphaSpeed;

        [SerializeField]
        private float _bloodlustTickDelay;

        [SerializeField]
        private float _bloodlustThreshold;

        [SerializeField]
        private Sprite _eyeOpen;

        [SerializeField]
        private Sprite _eyeSemiOpen;

        [SerializeField]
        private Sprite _eyeClosed;

        [SerializeField]
        private Animator _hands;

        [SerializeField]
        private GameObject _uiRoot;

        [SerializeField]
        private ScpWarningHud _warningHud;

        private ZombieConsumeAbility _consumeAbility;
        private ZombieAttackAbility _attackAbility;
        private ZombieMovementModule _fpcModule;

        private float _bloodlustTimer;
        private float _bloodlustAlpha;
        private float _bloodlustLerpValue;

        private const float HandsFov = 70f;

        private static readonly int AttackHash = Animator.StringToHash("Shoot");
        private static readonly int EatHash = Animator.StringToHash("Eat");

        protected override void Update()
        {
            base.Update();

            _consumeCircle.Apply(_consumeAbility.ProgressStatus, false);
            _hands.SetBool(EatHash, _consumeAbility.IsInProgress);
            _attackCooldownIcon.Update(false);

            if (_fpcModule != null)
            {
                float min = _fpcModule.NormalSpeed;
                float max = _fpcModule.BloodlustSpeed;
                float bloodlust = (_fpcModule.WalkSpeed - min) / (max - min);
                UpdateBloodlust(_bloodlustFill, _bloodlustBackground, bloodlust);
            }
        }

        private void PlayAttack()
        {
            _hands.SetTrigger(AttackHash);
        }

        private void ProcessError(byte err)
        {
            string defaultText = $"Operation refused, code #{err}";
            string text = TranslationReader.Get("SCP049_HUD", err, defaultText);
            _warningHud.SetText(text, 3.8f);
        }

        private void UpdateBloodlust(Image fill, Image background, float fillAmount)
        {
            _bloodlustTimer += Time.deltaTime;
            if (_bloodlustTimer >= _bloodlustTickDelay)
            {
                _bloodlustTimer = 0f;

                if (fillAmount <= _bloodlustThreshold)
                {
                    _bloodlustAlpha -= _bloodlustAlphaSpeed;
                    if (_bloodlustAlpha < 0f)
                        _bloodlustAlpha = 0f;
                }
                else
                {
                    _bloodlustAlpha += _bloodlustAlphaSpeed;
                    if (_bloodlustAlpha > 1f)
                        _bloodlustAlpha = 1f;
                }

                SetTransparency(fill, _bloodlustAlpha);
                SetTransparency(background, _bloodlustAlpha);
            }

            Sprite eye;
            if (fillAmount > 0.75f)
                eye = _eyeOpen;
            else if (fillAmount > 0.4f)
                eye = _eyeSemiOpen;
            else
                eye = _eyeClosed;

            fill.sprite = eye;
            if (background != null)
                background.sprite = eye;

            _bloodlustLerpValue = Mathf.Lerp(_bloodlustLerpValue, fillAmount, Time.deltaTime * 1.5f);
            fill.fillAmount = _bloodlustLerpValue;
        }

        private void SetBloodlustTransparency(Image fill, Image background, float fillAmount)
        {
            _bloodlustTimer += Time.deltaTime;
            if (_bloodlustTimer >= _bloodlustTickDelay)
            {
                _bloodlustTimer = 0f;

                if (fillAmount <= _bloodlustThreshold)
                {
                    _bloodlustAlpha -= _bloodlustAlphaSpeed;
                    if (_bloodlustAlpha < 0f)
                        _bloodlustAlpha = 0f;
                }
                else
                {
                    _bloodlustAlpha += _bloodlustAlphaSpeed;
                    if (_bloodlustAlpha > 1f)
                        _bloodlustAlpha = 1f;
                }

                SetTransparency(fill, _bloodlustAlpha);
                SetTransparency(background, _bloodlustAlpha);
            }
        }

        private void SetTransparency(Image image, float alpha)
        {
            if (image == null)
                return;

            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }

        protected override void OnDestroy()
        {
            if (_hands != null)
                Destroy(_hands.gameObject);

            if (_attackAbility != null)
                _attackAbility.OnTriggered -= PlayAttack;

            if (_consumeAbility != null)
                _consumeAbility.OnErrorReceived -= ProcessError;

            base.OnDestroy();
        }

        internal override void OnDied()
        {
            if (_uiRoot != null)
                _uiRoot.SetActive(false);
        }

        internal override void Init(ReferenceHub hub)
        {
            base.Init(hub);

            var currentRole = hub.roleManager.CurrentRole;
            (currentRole as ZombieRole).SubroutineModule.TryGetSubroutine(out _consumeAbility);
             (currentRole as ZombieRole).SubroutineModule.TryGetSubroutine(out _attackAbility);

            _fpcModule = (currentRole as IFpcRole)?.FpcModule as ZombieMovementModule;

        _attackCooldownIcon.Setup(_attackAbility.Cooldown, null);

            Vector3 localPos = _hands.transform.localPosition;
            _hands.transform.SetParent(SharedHandsController.Singleton.transform);
            _hands.transform.localPosition = localPos;
            _hands.transform.localRotation = Quaternion.identity;

            _attackAbility.OnTriggered += PlayAttack;
            _consumeAbility.OnErrorReceived += ProcessError;

            if (!hub.isLocalPlayer)
            {
                for (int i = 0; i < 5; i++)
                    _hands.Update(0.15f);
            }
        }

        public bool TryGetViewmodelFov(out float fov)
        {
            fov = HandsFov;
            return true;
        }
    }
}
