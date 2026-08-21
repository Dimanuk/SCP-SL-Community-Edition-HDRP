using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Items.Firearms.Attachments
{
    public class AttachmentSummaryEntry : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _label;

        [SerializeField]
        private TextMeshProUGUI[] _valuesBank;

        [SerializeField]
        private Color _oddColor;

        private bool _firstSetup = true;

        public void Setup(string label, string[] values, bool isOdd)
        {
            if (isOdd && _firstSetup)
            {
                GetComponent<Image>().color = _oddColor;
                _firstSetup = false;
            }

            _label.text = label;

            for (int i = 0; i < _valuesBank.Length; i++)
            {
                if (i < values.Length)
                {
                    _valuesBank[i].gameObject.SetActive(true);
                    _valuesBank[i].text = values[i];
                }
                else
                {
                    _valuesBank[i].gameObject.SetActive(false);
                }
            }
        }
    }
}