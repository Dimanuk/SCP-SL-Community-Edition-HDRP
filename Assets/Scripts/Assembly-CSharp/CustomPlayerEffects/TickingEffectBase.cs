namespace CustomPlayerEffects
{
    public abstract class TickingEffectBase : global::CustomPlayerEffects.StatusEffectBase
    {
        [global::UnityEngine.Tooltip("Used to track intervals/timers/etc without every effect needing to redefine a unique float.")]
        public float TimeBetweenTicks;

        private float _timeTillTick;

        protected abstract void OnTick();

        protected override void Enabled()
        {
            base.Enabled();
            _timeTillTick = TimeBetweenTicks;
        }

        protected override void OnEffectUpdate()
        {
            base.OnEffectUpdate();
            _timeTillTick -= global::UnityEngine.Time.deltaTime;
            if (!(_timeTillTick > 0f))
            {
                _timeTillTick += TimeBetweenTicks;
                OnTick();
            }
        }
    }
}
