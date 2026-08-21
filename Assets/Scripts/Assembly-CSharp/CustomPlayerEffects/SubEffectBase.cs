namespace CustomPlayerEffects
{
    public abstract class SubEffectBase : global::UnityEngine.MonoBehaviour
    {
        protected global::CustomPlayerEffects.StatusEffectBase MainEffect { get; private set; }

        protected ReferenceHub Hub => MainEffect.Hub;

        protected bool IsLocalPlayer => MainEffect.IsLocalPlayer;

        public virtual bool IsActive
        {
            get
            {
                if (base.gameObject.activeInHierarchy)
                {
                    return MainEffect.IsEnabled;
                }
                return false;
            }
        }

        public virtual void DisableEffect()
        {
        }

        internal virtual void Init(global::CustomPlayerEffects.StatusEffectBase mainEffect)
        {
            MainEffect = mainEffect;
        }

        internal virtual void UpdateEffect()
        {
        }
    }
}
