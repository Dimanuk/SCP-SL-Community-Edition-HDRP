using UnityEngine;
using UnityEngine.UI;

namespace OperationalGuide
{
	public class OperationalPage : MonoBehaviour
	{
		[Header("UI References")]
		public GameObject MainPage;
		public GameObject DescriptionPage;
		public Image BackgroundImage;

		[Header("State & Animation")]
		public bool F1PageActive;
		public Animator PageAnimator;

		public virtual void ToggleDescriptionMenu()
		{
			F1PageActive = !F1PageActive;
			if (PageAnimator != null)
				PageAnimator.SetBool("main", F1PageActive);

			TurnOn();
		}

		public void TurnOn()
		{
			if (!gameObject.activeSelf)
			{
				var guide = OperationalGuide.Instance;
				if (guide != null && guide.FadeAnimator != null)
				{
					guide.FadeAnimator.Play("Base Layer.PageFade");
				}
			}

			if (PageAnimator != null)
			{
				PageAnimator.SetBool("main", F1PageActive);
			}

			if (MainPage != null) MainPage.SetActive(true);
			if (BackgroundImage != null) BackgroundImage.gameObject.SetActive(true);

			var instance = OperationalGuide.Instance;
			if (instance != null && instance.Back != null)
			{
				instance.Back.SetActive(true);
			}
		}

		public void ForceTurnOff()
		{
			if (MainPage != null) MainPage.SetActive(false);
			if (BackgroundImage != null) BackgroundImage.gameObject.SetActive(false);
		}

		public virtual void OnPageEnable() { }

		public virtual void OnPageDisable() { }

		// Unity Lifecycle
		private void OnEnable() => OnPageEnable();
		private void OnDisable() => OnPageDisable();
	}
}
