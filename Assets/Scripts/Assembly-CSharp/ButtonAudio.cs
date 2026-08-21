
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerClickHandler
{
	public AudioClip clickClip;

	public AudioClip hoverClip;

	private Button _button;

	private AudioSource _audioSource;

	private void Start()
	{
		_button = GetComponent<Button>();
		_audioSource = GetComponentInParent<UnityEngine.AudioSource>();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if(_button != null && _audioSource != null && hoverClip != null)
		{
			_audioSource.PlayOneShot(hoverClip);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if(_button != null && _audioSource != null && clickClip != null)
		{
			_audioSource.PlayOneShot(clickClip);
		}
	}
}
