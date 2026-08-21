using Mirror;
using UnityEngine;

namespace Hints
{
    public abstract class DisplayableObject<TData> : NetworkObject<TData>
    {
        public float DurationScalar { get; private set; }

        protected DisplayableObject(float durationScalar = 1f)
        {
            this.DurationScalar = durationScalar;
        }

        public override void Deserialize(NetworkReader reader)
        {
            this.DurationScalar = reader.ReadFloat();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.WriteFloat(this.DurationScalar);
        }

        public bool TryAwake(TData data, float rawTime)
        {
            float progress = this.UpdateProgress(rawTime);
            if (progress <= 0f || progress > 1f)
                return false;

            this.Data = data;
            this.Start();
            return true;
        }

        public float Update(float rawTime)
        {
            float progress = this.UpdateProgress(rawTime);
            if (progress > 0f && progress <= 1f)
                this.UpdateState(progress);
            return progress;
        }

        protected abstract float UpdateProgress(float rawTime);
        protected abstract void UpdateState(float progress);
    }
}
