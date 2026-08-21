using System;

using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoles.PlayableScps.HUDs
{
	[Serializable]
	public class LoadingCircleHud
	{
		[SerializeField]
		public GameObject _parent;

		[SerializeField]
		public Image _loadingBar;

		[SerializeField]
		public Gradient _colorGradient;

		[SerializeField]
		public bool _inverseFill;

		[SerializeField]
		public bool _hideWhenFull;

		[SerializeField]
		private bool _hideWhenEmpty;

		public void Apply(float percent, bool forceHide = false)
        {
            percent = Mathf.Clamp01(percent);
            
            bool hide = forceHide 
                || (percent >= 1f && _hideWhenFull) 
                || (percent <= 0f && _hideWhenEmpty);

            _parent.SetActive(!hide);

            if (!hide)
            {
                _loadingBar.fillAmount = _inverseFill ? 1f - percent : percent;
                _loadingBar.color = _colorGradient.Evaluate(percent);
            }
        }
	}
}
