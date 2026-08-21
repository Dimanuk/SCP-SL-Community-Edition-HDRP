using TMPro;
using UnityEngine;

namespace Hints
{
	public readonly struct SharedHintData
	{
		public readonly ReferenceHub PlayerHub;

		public readonly TextMeshProUGUI Textbox;

        public readonly float RawTime;

        public SharedHintData(GameObject self, TextMeshProUGUI textbox, float rawTime)
		{
			PlayerHub = ReferenceHub.GetHub(self);
			Textbox = textbox;
			RawTime = rawTime;
		}
	}
}
