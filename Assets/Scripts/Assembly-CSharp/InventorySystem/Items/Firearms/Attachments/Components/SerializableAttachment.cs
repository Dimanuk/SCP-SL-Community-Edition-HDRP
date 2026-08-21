namespace InventorySystem.Items.Firearms.Attachments.Components
{
    public class SerializableAttachment : global::InventorySystem.Items.Firearms.Attachments.Components.Attachment, global::InventorySystem.Items.Firearms.Attachments.Components.IDisplayableAttachment
    {
        [global::UnityEngine.SerializeField]
        private global::InventorySystem.Items.Firearms.Attachments.AttachmentName _name;

        [global::UnityEngine.SerializeField]
        private global::InventorySystem.Items.Firearms.Attachments.AttachmentSlot _slot;

        [global::UnityEngine.Space]
        [global::UnityEngine.SerializeField]
        private float _weight;

        [global::UnityEngine.SerializeField]
        private float _length;

        [global::UnityEngine.Space]
        [global::UnityEngine.SerializeField]
        private global::InventorySystem.Items.Firearms.Attachments.AttachmentDescriptiveAdvantages _extraPros;

        [global::UnityEngine.SerializeField]
        private global::InventorySystem.Items.Firearms.Attachments.AttachmentDescriptiveDownsides _extraCons;

        [global::UnityEngine.Space]
        [global::UnityEngine.SerializeField]
        private global::UnityEngine.Texture _icon;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.Vector2 _iconOffset;

        [global::UnityEngine.SerializeField]
        private int _parentId;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.Vector2 _parentOffset;

        [global::UnityEngine.Space]
        [global::UnityEngine.SerializeField]
        private global::InventorySystem.Items.Firearms.Attachments.AttachmentParameterValuePair[] _params;

        public override global::InventorySystem.Items.Firearms.Attachments.AttachmentName Name => _name;

        public override global::InventorySystem.Items.Firearms.Attachments.AttachmentSlot Slot => _slot;

        public override float Weight => _weight;

        public override float Length => _length;

        public override global::InventorySystem.Items.Firearms.Attachments.AttachmentDescriptiveAdvantages DescriptivePros => _extraPros;

        public override global::InventorySystem.Items.Firearms.Attachments.AttachmentDescriptiveDownsides DescriptiveCons => _extraCons;

        public global::UnityEngine.Texture Icon => _icon;

        public global::UnityEngine.Vector2 IconOffset => _iconOffset;

        public int ParentId => _parentId;

        public global::UnityEngine.Vector2 ParentOffset => _parentOffset;

        private void Awake()
        {
            global::InventorySystem.Items.Firearms.Attachments.AttachmentParameterValuePair[] array = _params;
            foreach (global::InventorySystem.Items.Firearms.Attachments.AttachmentParameterValuePair parameterValue in array)
            {
                SetParameterValue(parameterValue);
            }
        }

        private bool _isValidating;

        private void OnValidate()
        {
            if (_isValidating)
                return;

            if (TryGetComponent<global::InventorySystem.Items.Firearms.Attachments.Components.SerializableAttachment>(out var component) && !(component == this))
            {
                if (GetInstanceID() > component.GetInstanceID())
                {
                    _isValidating = true;
                    try
                    {
                        _name = component._name;
                        _slot = component._slot;
                        _weight = component._weight;
                        _length = component._length;
                        _extraPros = component._extraPros;
                        _extraCons = component._extraCons;
                        _icon = component._icon;
                        _iconOffset = component._iconOffset;
                        _parentId = component._parentId;
                        _parentOffset = component._parentOffset;
                        _params = component._params;
                    }
                    finally
                    {
                        _isValidating = false;
                    }
                }
            }
        }
    }
}