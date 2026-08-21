
using Interactables;
using Interactables.Verification;
using TMPro;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Attachments
{
    public class AttachmentPresetSelector : MonoBehaviour, IClientInteractable, IInteractable
    {
        [SerializeField]
        public AttachmentSelectorBase _selectorRef;

        [SerializeField]
        public GameObject _rootObject;

        [SerializeField]
        public TextMeshProUGUI[] _saveButtons;

        [SerializeField]
        public TextMeshProUGUI[] _currentPresetIndicators;

        [SerializeField]
        public Color _normalColor;

        [SerializeField]
        public Color _currentColor;

        public const byte SaveOffset = 100;

        public const byte ResetAttachmentsCode = 254;

        public const byte SummaryToggleCode = 253;

        public IVerificationRule VerificationRule => StandardDistanceVerification.Default;

        public void Start()
        {
            for (int i = 0; i < _currentPresetIndicators.Length; i++)
            {
                _currentPresetIndicators[i].text = ((i == 0) ? Translations.Get(AttachmentEditorsTranslation.Custom) : string.Format(Translations.Get(AttachmentEditorsTranslation.PresetId), i));
            }

            string text = "[ " + Translations.Get(AttachmentEditorsTranslation.SaveAttachments) + " ]";
            TextMeshProUGUI[] saveButtons = _saveButtons;
            for (int j = 0; j < saveButtons.Length; j++)
            {
                saveButtons[j].text = text;
            }
        }

        public void ProcessButton(int id)
        {
            if (id != 253)
            {
                if (id == 254)
                {
                    _selectorRef.ResetAttachments();
                }
                else if (id > 100)
                {
                    _selectorRef.SaveAsPreset(id - 100);
                }
                else
                {
                    _selectorRef.LoadPreset(id);
                }
            }
            else
            {
                _selectorRef.ToggleSummaryScreen();
            }
        }

        public void LateUpdate()
        {
            if (_selectorRef.SelectedFirearm == null)
            {
                _rootObject.SetActive(value: false);
                return;
            }

            int preset = AttachmentPreferences.GetPreset(_selectorRef.SelectedFirearm.ItemTypeId);
            for (int i = 0; i < Mathf.Min(_currentPresetIndicators.Length, _saveButtons.Length); i++)
            {
                _saveButtons[i].gameObject.SetActive(_selectorRef.CanSaveAsPreference(i));
                _currentPresetIndicators[i].color = ((preset == i) ? _currentColor : _normalColor);
            }

            _rootObject.SetActive(value: true);
        }

        public void ClientInteract(InteractableCollider collider)
        {
            ProcessButton(collider.ColliderId);
        }
    }
}
