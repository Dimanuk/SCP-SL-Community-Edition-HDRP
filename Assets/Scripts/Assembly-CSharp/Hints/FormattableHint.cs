using System;

namespace Hints
{
    public abstract class FormattableHint<THint> : Hint where THint : FormattableHint<THint>
    {
        private THint _genericThis;
        private Func<THint, float, string> _formattedTextStrategy;

        protected abstract Func<THint, float, string> FormattedTextStrategy { get; }

        protected FormattableHint(HintParameter[] parameters, HintEffect[] effects, float durationScalar = 1f)
            : base(parameters, effects, durationScalar)
        {
        }

        protected override void Start()
        {
            _genericThis = this as THint;

            if (_genericThis == null)
            {
                throw new InvalidOperationException("The generic argument of FormattableHint must be itself.");
            }

            this._formattedTextStrategy = this.FormattedTextStrategy;
            base.Start();
        }

        protected override void Destroy()
        {
            base.Destroy();
            this._formattedTextStrategy = null;
            this._genericThis = null;
        }

        protected override string UpdateContent(float progress)
        {
            if (this._formattedTextStrategy != null)
            {
                return this._formattedTextStrategy(this._genericThis, progress);
            }

            return string.Empty;
        }
    }
}