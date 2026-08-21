namespace InventorySystem.Items.Firearms.Attachments.Components
{
    public abstract class Attachment : global::UnityEngine.MonoBehaviour
    {
        private bool _parentCached;

        private global::InventorySystem.Items.Firearms.Firearm _parentFirearm;

        internal bool[] _activeParameters;

        internal float[] _parameterValues;

        internal bool _initialized;

        private const string AttNamesFilename = "AttachmentNames";

        public abstract global::InventorySystem.Items.Firearms.Attachments.AttachmentName Name { get; }

        public abstract global::InventorySystem.Items.Firearms.Attachments.AttachmentSlot Slot { get; }

        public abstract float Weight { get; }

        public abstract float Length { get; }

        public abstract global::InventorySystem.Items.Firearms.Attachments.AttachmentDescriptiveAdvantages DescriptivePros { get; }

        public abstract global::InventorySystem.Items.Firearms.Attachments.AttachmentDescriptiveDownsides DescriptiveCons { get; }

        public virtual bool IsEnabled { get; set; }

        public int AttachmentId { get; internal set; }

        private void SetupArray()
        {
            if (!_initialized)
            {
                Initialize();
                _initialized = true;
            }
        }

        protected virtual void Initialize()
        {
            int totalNumberOfParams = global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.TotalNumberOfParams;
            _parameterValues = new float[totalNumberOfParams];
            _activeParameters = new bool[totalNumberOfParams];
        }

        protected void SetParameterValue(global::InventorySystem.Items.Firearms.Attachments.AttachmentParameterValuePair pair)
        {
            SetParameterValue(pair.Parameter, pair.Value);
        }

        protected void SetParameterValue(global::InventorySystem.Items.Firearms.Attachments.AttachmentParam param, float val)
        {
            SetParameterValue((int)param, val);
        }

        protected void SetParameterValue(int param, float val)
        {
            SetupArray();
            _parameterValues[param] = val;
            _activeParameters[param] = true;
        }

        protected void ResetParameter(global::InventorySystem.Items.Firearms.Attachments.AttachmentParam param)
        {
            SetupArray();
            _activeParameters[(int)param] = false;
        }

        public bool TryGetValue(int param, out float val)
        {
            SetupArray();
            val = _parameterValues[param];
            return _activeParameters[param];
        }

        public bool TryGetValue(global::InventorySystem.Items.Firearms.Attachments.AttachmentParam param, out float val)
        {
            return TryGetValue((int)param, out val);
        }

        public bool TryGetParentFirearm(out global::InventorySystem.Items.Firearms.Firearm firearm)
        {
            if (!_parentCached)
            {
                if (!base.transform.parent.TryGetComponent<global::InventorySystem.Items.Firearms.Firearm>(out var component))
                {
                    firearm = null;
                    return false;
                }
                _parentFirearm = component;
                _parentCached = true;
            }
            firearm = _parentFirearm;
            return true;
        }

        public void GetNameAndDescription(out string n, out string d)
        {
            if (TranslationReader.TryGet(AttNamesFilename, (int)Name, out var val))
            {
                string[] array = val.Split('~');
                n = array[0];
                d = ((array.Length == 1) ? string.Empty : array[1]);
            }
            else
            {
                n = Name.ToString();
                d = string.Empty;
            }
        }

        public void GetActiveParamsNonAlloc(global::InventorySystem.Items.Firearms.Attachments.AttachmentParam[] activeParams, out int len)
        {
            SetupArray();
            len = 0;
            for (int i = 0; i < global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.TotalNumberOfParams; i++)
            {
                if (_activeParameters[i])
                {
                    activeParams[len] = (global::InventorySystem.Items.Firearms.Attachments.AttachmentParam)i;
                    len++;
                }
            }
        }
    }
}
