using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Disarming
{
	public class DisarmingGUI : MonoBehaviour
	{
		public static DisarmingGUI Singleton;

		public Image Circle;

		[SerializeField]
		private GameObject _disarmedMessageObject;

		[SerializeField]
		private TextMeshProUGUI _disarmedMessageText;

		[SerializeField]
		private TextMeshProUGUI _bottomText;

		private const string TranslationKey = "Disarming";

		private string _adminCuffed;

		private string _nicknameCuffedFormat;

        private void Start()
        {
            Singleton = this;

            _nicknameCuffedFormat = TranslationReader.Get(TranslationKey, 0, "You have been detained by {0}.");
            _adminCuffed = TranslationReader.Get(TranslationKey, 1, "an administrator");

            if (_bottomText != null)
            {
                _bottomText.text = TranslationReader.Get(TranslationKey, 2, "Follow orders or ask teammates for help.");
            }
        }

        private void Update()
        {
            if (!ReferenceHub.TryGetLocalHub(out ReferenceHub localHub)) return;

            bool isLocalDisarmed = false;
            string disarmerName = string.Empty;

            foreach (var entry in DisarmedPlayers.Entries)
            {
                if (entry.DisarmedPlayer == localHub.netId)
                {
                    isLocalDisarmed = true;

                    if (ReferenceHub.TryGetHubNetID(entry.Disarmer, out ReferenceHub disarmerHub))
                    {
                        disarmerName = disarmerHub.nicknameSync.MyNick;
                    }
                    else
                    {
                        disarmerName = _adminCuffed;
                    }
                    break;
                }
            }

            if (_disarmedMessageObject != null)
            {
                _disarmedMessageObject.SetActive(isLocalDisarmed);
            }

            if (isLocalDisarmed && _disarmedMessageText != null)
            {
                _disarmedMessageText.text = string.Format(_nicknameCuffedFormat, disarmerName);
            }
        }
    }
}
