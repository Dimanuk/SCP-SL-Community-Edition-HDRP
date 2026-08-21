using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoles.RoleHelp
{
    public class KeyActionIcon : MonoBehaviour
    {

        [SerializeField]
        private ActionName _action;

        [SerializeField]
        private Image _keycodeBg;

        [SerializeField]
        private TMP_Text _keycodeText;

        [SerializeField]
        private TMP_Text _mouseText;

        [SerializeField]
        private GameObject[] _mouseIcons;

        private const KeyCode MouseMin = KeyCode.Mouse0;
        private const KeyCode MouseMax = KeyCode.Mouse6;

        private void Awake()
        {
            Refresh();
        }

        private void Refresh()
        {
            KeyCode key = NewInput.GetKey(_action);

            _mouseIcons.ForEach(x => x.SetActive(false));

            if (key >= MouseMin && key <= MouseMax)
            {
                HandleMouse((int)(key - MouseMin));
            }
            else
            {
                HandleKeycode(new ReadableKeyCode(key));
            }
        }

        private void HandleMouse(int buttonId)
        {
            _keycodeText.text = string.Empty;
            _mouseText.text = buttonId.ToString();

            int maxIndex = _mouseIcons.Length - 1;
            int index = Mathf.Min(buttonId, maxIndex);
            _mouseIcons[index].SetActive(true);

            _keycodeBg.enabled = false;
        }

        private void HandleKeycode(ReadableKeyCode rkc)
        {
            _keycodeText.text = rkc.NormalVersion;
            _keycodeBg.enabled = true;
        }
    }
}
