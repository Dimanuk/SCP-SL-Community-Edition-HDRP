using RemoteAdmin.Interfaces;
using UnityEngine;

namespace RemoteAdmin.Elements
{
    public class ValueButton : CustomButton, IValueHolder<string>
    {
        [SerializeField]
        protected bool AllowMultipleSelection;

        [SerializeField]
        private int _choiceIdentifier;

        [SerializeField]
        private string _value;

        public string Value
        {
            get => _value;
            set => _value = value;
        }

        public int ChoiceId
        {
            get => _choiceIdentifier;
            set => _choiceIdentifier = value;
        }

        public override void SetState(bool isSelected)
        {
            base.SetState(isSelected);

            // Only the button that just became selected drives deselection of its group.
            if (!isSelected)
                return;

            var commandMenu = CommandMenu;
            if (commandMenu == null || commandMenu.Options == null)
                return;

            // Holding Ctrl on a multi-select button preserves the other multi-select
            // buttons; single-select buttons of the same ChoiceId are always deselected.
            bool canSelectMultiple = CanSelectMultiple();

            foreach (var option in commandMenu.Options)
            {
                if (option == null)
                    continue;

                if (canSelectMultiple && option.AllowMultipleSelection)
                    continue;

                if (option != this && option.IsSelected && option._choiceIdentifier == this._choiceIdentifier)
                {
                    option.SetState(false);
                }
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            var commandMenu = CommandMenu;
            if (commandMenu != null && commandMenu.Options != null)
            {
                if (!commandMenu.Options.Contains(this))
                {
                    commandMenu.Options.Add(this);
                }
            }
        }

        private bool CanSelectMultiple()
        {
            if (!AllowMultipleSelection)
                return false;

            return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        }
    }
}
