namespace PlayerRoles.Spectating
{
    public class SpectatorAttachmentsWindowToggler : global::ToggleableMenus.SimpleToggleableMenu
    {
        [global::UnityEngine.SerializeField]
        private global::TMPro.TextMeshProUGUI _toggleHint;

        public override bool CanToggle => base.gameObject.activeInHierarchy;

        protected override void Awake()
        {
            base.Awake();
            _toggleHint.text = string.Format(Translations.Get(global::InventorySystem.Items.Firearms.Attachments.AttachmentEditorsTranslation.SpectatorEditorTip), new ReadableKeyCode(MenuActionKey));
        }

        private void OnDisable()
        {
            IsEnabled = false;
        }
    }
}
