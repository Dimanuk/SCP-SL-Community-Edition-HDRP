using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoles.PlayableScps.Scp079.GUI
{
    public class Scp079AuxGui : Scp079GuiElementBase
    {
        [SerializeField]
        private Image _slider;

        [SerializeField]
        private TextMeshProUGUI _textNormal;

        [SerializeField]
        private TextMeshProUGUI _textInverted;

        private Scp079AuxManager _auxManager;

        private const string Format = "{0} / {1}";

        private string ExpText
        {
            set
            {
                _textNormal.text = value;
                _textInverted.text = value;
            }
        }

        internal override void Init(Scp079Role role, ReferenceHub owner)
        {
            base.Init(role, owner);
            role.SubroutineModule.TryGetSubroutine(out _auxManager);
        }

        private void Update()
        {
            _slider.fillAmount = _auxManager.CurrentAux / _auxManager.MaxAux;
            ExpText = string.Format(Format, _auxManager.CurrentAuxFloored, _auxManager.MaxAux);
        }
    }
}
