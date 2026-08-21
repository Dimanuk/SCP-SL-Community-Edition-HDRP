namespace Hints
{
    public abstract class HintParameter : NetworkObject<SharedHintData>
    {
        public string Formatted { get; private set; }

        protected HintParameter()
        {
        }

        public bool Update(float progress)
        {
            string text = this.UpdateState(progress);

            if (text == null)
            {
                return false;
            }

            this.Formatted = text;
            return true;
        }

        protected abstract string UpdateState(float progress);
    }
}