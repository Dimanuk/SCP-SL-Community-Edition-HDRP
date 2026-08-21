using Mirror;
using System;
using Utils.Networking;

namespace Hints
{
    public abstract class Hint : DisplayableObject<SharedHintData>
    {
        private Action<Hint> _destroyStrategy;
        private Action<Hint, float> _updateStateStrategy;
        private HintEffect[] _effects;

        public float StartTime { get; private set; }
        protected HintParameter[] Parameters { get; private set; }

        protected Hint(HintParameter[] parameters, HintEffect[] effects, float durationScalar = 1f)
            : base(durationScalar)
        {
            this._effects = effects;
            this.Parameters = parameters;
        }

        private static void DestroyParameters(Hint self)
        {
            if (self.Parameters == null) return;
            for (int i = 0; i < self.Parameters.Length; i++)
            {
                self.Parameters[i]?.Dispose();
            }
        }

        private static void DestroyEffects(Hint self)
        {
            if (self._effects == null) return;
            for (int i = 0; i < self._effects.Length; i++)
            {
                self._effects[i]?.Dispose();
            }
        }

        private static void UpdateParameters(Hint self, float progress)
        {
            if (self.Parameters == null) return;
            foreach (var parameter in self.Parameters)
            {
                parameter?.Update(progress);
            }
        }

        private static void UpdateEffects(Hint self, float progress)
        {
            if (self._effects == null) return;
            foreach (var effect in self._effects)
            {
                effect?.Update(progress);
            }
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            this._effects = reader.ReadHintEffectArray();
            this.Parameters = reader.ReadHintParameterArray();
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.WriteHintEffectArray(this._effects);
            writer.WriteHintParameterArray(this.Parameters);
        }

        protected abstract string UpdateContent(float progress);

        protected override void Start()
        {
            this.StartTime = base.Data.RawTime;

            this._destroyStrategy = (Action<Hint>)Delegate.Combine(this._destroyStrategy, new Action<Hint>(DestroyParameters));
            this._destroyStrategy = (Action<Hint>)Delegate.Combine(this._destroyStrategy, new Action<Hint>(DestroyEffects));

            this._updateStateStrategy = (Action<Hint, float>)Delegate.Combine(this._updateStateStrategy, new Action<Hint, float>(UpdateParameters));
            this._updateStateStrategy = (Action<Hint, float>)Delegate.Combine(this._updateStateStrategy, new Action<Hint, float>(UpdateEffects));

            if (this._effects != null)
            {
                foreach (var effect in this._effects)
                {
                    effect?.TryAwake(base.Data, this.StartTime);
                }
            }
        }

        protected override void Destroy()
        {
            this._destroyStrategy?.Invoke(this);

            this._destroyStrategy = null;
            this._updateStateStrategy = null;
            base.Destroy();
        }

        protected override float UpdateProgress(float rawTime)
        {
            if (base.DurationScalar <= 0) return 1f;
            return (rawTime - this.StartTime) / base.DurationScalar;
        }

        protected override void UpdateState(float progress)
        {
            this._updateStateStrategy?.Invoke(this, progress);
            string content = this.UpdateContent(progress);

            if (this.Data.Textbox != null)
            {
                this.Data.Textbox.text = content;
            }
        }
    }
}