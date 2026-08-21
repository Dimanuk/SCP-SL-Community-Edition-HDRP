namespace CustomPlayerEffects
{
    public abstract class ParentEffectBase<T> : global::CustomPlayerEffects.StatusEffectBase where T : global::CustomPlayerEffects.SubEffectBase
    {
        public T[] SubEffects { get; private set; }

        internal override void OnRoleChanged(global::PlayerRoles.PlayerRoleBase previousRole, global::PlayerRoles.PlayerRoleBase newRole)
        {
            base.OnRoleChanged(previousRole, newRole);
            if (SubEffects == null) return;

            for (int i = 0; i < SubEffects.Length; i++)
            {
                SubEffects[i]?.DisableEffect();
            }
        }

        public override void OnStopSpectating()
        {
            base.OnStopSpectating();
            if (SubEffects == null) return;

            for (int i = 0; i < SubEffects.Length; i++)
            {
                SubEffects[i]?.DisableEffect();
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            if (SubEffects == null || SubEffects.Length == 0)
            {
                SubEffects = GetComponentsInChildren<T>(true);
            }

            if (SubEffects == null) return;

            for (int i = 0; i < SubEffects.Length; i++)
            {
                SubEffects[i]?.Init(this);
            }
        }

        protected virtual void UpdateSubEffects()
        {
            if (SubEffects == null) return;

            for (int i = 0; i < SubEffects.Length; i++)
            {
                SubEffects[i]?.UpdateEffect();
            }
        }
    }
}