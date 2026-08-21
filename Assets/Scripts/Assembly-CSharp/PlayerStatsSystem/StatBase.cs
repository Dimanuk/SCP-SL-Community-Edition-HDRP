namespace PlayerStatsSystem
{
    public abstract class StatBase
    {
        public abstract float CurValue { get; set; }

        public abstract float MinValue { get; }

        public abstract float MaxValue { get; }

        public float NormalizedValue
        {
            get
            {
                if (MinValue != MaxValue)
                {
                    return (CurValue - MinValue) / (MaxValue - MinValue);
                }
                return 0f;
            }
        }

        public ReferenceHub Hub { get; private set; }

        internal virtual void Init(ReferenceHub ply)
        {
            Hub = ply;
        }

        internal virtual void Update()
        {
        }

        internal virtual void ClassChanged()
        {
        }
    }
}
