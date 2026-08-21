using UnityEngine;
using UnityEngine.EventSystems;

namespace OperationalGuide
{
	public class AnimationTextFader : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		public Animator Animator;

		public void OnPointerEnter(PointerEventData eventData)
		{
			Animator.SetBool("fade", true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			Animator.SetBool("fade", false);
		}
	}
}
