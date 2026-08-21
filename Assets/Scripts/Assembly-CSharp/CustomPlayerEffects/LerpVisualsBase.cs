using PlayerRoles;
using PlayerRoles.Spectating;
using UnityEngine;

namespace CustomPlayerEffects
{
    public abstract class LerpVisualsBase : MonoBehaviour
    {
        private bool _enableInstant;
        private bool _disableInstant;
        private float _weight;

        [SerializeField]
        protected bool UpdateOnRoleChange = true;

        [SerializeField]
        protected float EnableSpeed = 1f;

        [SerializeField]
        protected float DisableSpeed = 1f;

        protected StatusEffectBase TargetEffect { get; private set; }

        protected float Weight
        {
            get => _weight;
            set
            {
                if (_weight <= 0f && value > 0f)
                {
                    OnActivated();
                }

                _weight = Mathf.Clamp01(value);
                OnWeightChanged(_weight);

                if (_weight >= 1f)
                {
                    enabled = false;
                }
                else if (_weight <= 0f)
                {
                    OnShutdown();
                    enabled = false;
                }
            }
        }

        protected abstract void OnWeightChanged(float weight);
        protected virtual void OnActivated() { }
        protected virtual void OnShutdown() { }

        protected virtual void Update()
        {
            if (TargetEffect == null || !TargetEffect.IsEnabled)
            {
                if (!_disableInstant)
                    Weight -= Time.deltaTime / DisableSpeed;
                else
                    Weight = 0f;
                return;
            }

            if (!_enableInstant)
                Weight += Time.deltaTime / EnableSpeed;
            else
                Weight = 1f;
        }

        protected virtual void Awake()
        {
            TargetEffect = GetComponent<StatusEffectBase>();

            if (TargetEffect != null)
            {
                StatusEffectBase.OnEnabled += UpdateState;
                StatusEffectBase.OnDisabled += UpdateState;
            }

            PlayerRoleManager.OnRoleChanged += UpdateRoleChange;
            SpectatorTargetTracker.OnTargetChanged += UpdateSpectator;

            // Speed <= 0 means instant snap (lerp with 0 speed never reaches target)
            _enableInstant = EnableSpeed <= 0f;
            _disableInstant = DisableSpeed <= 0f;
        }

        protected virtual void OnDestroy()
        {
            if (TargetEffect != null)
            {
                StatusEffectBase.OnEnabled -= UpdateState;
                StatusEffectBase.OnDisabled -= UpdateState;
            }

            PlayerRoleManager.OnRoleChanged -= UpdateRoleChange;
            SpectatorTargetTracker.OnTargetChanged -= UpdateSpectator;
        }

        private void UpdateState(StatusEffectBase triggeringEffect)
        {
            if (TargetEffect == null)
                return;

            bool shouldBeActive = TargetEffect.IsLocalPlayer || TargetEffect.IsSpectated;

            if (triggeringEffect == TargetEffect && shouldBeActive)
                enabled = true;
        }

        private void UpdateSpectator()
        {
            if (TargetEffect == null)
                return;

            if (TargetEffect.IsSpectated)
                Weight = TargetEffect.IsEnabled ? TargetEffect.Intensity / 255f : 0f;
        }

        private void UpdateRoleChange(ReferenceHub hub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
        {
            if (TargetEffect == null)
                return;

            if (hub == TargetEffect.Hub && UpdateOnRoleChange)
                Weight = 0f;
        }
    }
}
