using RemoteAdmin.Interfaces;
using RemoteAdmin.Menus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin.Elements
{
    public abstract class CustomButton : MonoBehaviour, ISelectableElement
    {
        [SerializeField]
        private TMP_Text _text;

        [SerializeField]
        private Outline _outline;

        [SerializeField]
        private RaCommandMenu _commandMenu;

        [SerializeField]
        private bool _magicButton = true;

        [SerializeField]
        private string _selectedText = "✓";

        [SerializeField]
        private string _unselectedText = "";

        [SerializeField]
        private bool _modifyText;

        private bool _isSelected;
        private string _oldText;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetState(value);
        }

        public TMP_Text Text
        {
            get => _text;
            set => _text = value;
        }

        public Outline Outline
        {
            get => _outline;
            set => _outline = value;
        }

        public RaCommandMenu CommandMenu
        {
            get => _commandMenu;
            set => _commandMenu = value;
        }

        protected virtual void Awake()
        {
            OnInitialize();
        }

        public virtual void Select()
        {
            SetState(!_isSelected);
        }

        public virtual void SetState(bool isSelected)
        {
            _isSelected = isSelected;

            if (_outline != null)
            {
                _outline.effectColor = isSelected
                    ? SubmenuSelector.Singleton?.c_selected ?? Color.white
                    : SubmenuSelector.Singleton?.c_deselected ?? Color.gray;
            }

            if (!_modifyText || _text == null)
                return;

            if (isSelected)
            {
                _oldText = _text.text;
                _text.text = _selectedText;
            }
            else
            {
                _text.text = string.IsNullOrEmpty(_unselectedText)
                    ? _oldText
                    : _unselectedText;
            }
        }

        protected virtual void OnInitialize()
        {
            if (_outline == null)
            {
                TryGetComponent(out _outline);
            }

            if (_commandMenu == null)
            {
                _commandMenu = GetComponentInParent<RaCommandMenu>();
            }

            if (_text == null)
            {
                if (!TryGetComponent(out _text) && transform.childCount > 0)
                {
                    var childTransform = transform.GetChild(0);
                    if (childTransform != null)
                        childTransform.TryGetComponent(out _text);
                }

                // The label often sits deeper than the first child in the ripped scene
                // (e.g. RoleElement's "Role Name"), leaving designer placeholder text
                // like "SCP-999" on every button — deep-search as a last resort.
                if (_text == null)
                    _text = GetComponentInChildren<TMP_Text>(true);
            }
        }
    }
}