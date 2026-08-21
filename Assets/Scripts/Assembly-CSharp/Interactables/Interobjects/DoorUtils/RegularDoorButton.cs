using TMPro;
using UnityEngine;

namespace Interactables.Interobjects.DoorUtils
{
	public class RegularDoorButton : InteractableCollider
	{
		[SerializeField]
		private TextMeshProUGUI _mainText;

		[SerializeField]
		private MeshRenderer _mainRenderer;

		private bool _useText;

        protected override void Awake()
        {
            base.Awake();
            _useText = _mainText != null;
        }

        public void SetupButton(string text, global::UnityEngine.Material mat)
        {
            if (_useText)
            {
                _mainText.text = text;
            }
            _mainRenderer.sharedMaterial = mat;
        }
    }
}
