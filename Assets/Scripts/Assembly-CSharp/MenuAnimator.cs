using System.Collections.Generic;

using MEC;
using UnityEngine;

public class MenuAnimator : MonoBehaviour
{
	[SerializeField]
	private GameObject sceneCamera;

	[SerializeField]
	private GameObject focusedPosition;

	[SerializeField]
	private GameObject unfocusedPosition;

	private Animator cameraSway;

	private MainMenuScript mms;

	[SerializeField]
	public static bool wasEverZoomed;

	public static bool retro;

	public bool retroSupported;

	public CameraFilterPack_TV_ARCADE_2 retroArcade;

	public CameraFilterPack_Colors_NewPosterize retroPosterize;

	private KeyCode[] konami = new KeyCode[11]
	{
		KeyCode.UpArrow,
		KeyCode.UpArrow,
		KeyCode.DownArrow,
		KeyCode.DownArrow,
		KeyCode.LeftArrow,
		KeyCode.RightArrow,
		KeyCode.LeftArrow,
		KeyCode.RightArrow,
		KeyCode.B,
		KeyCode.A,
		KeyCode.Return
	};

	private int konamiIndex;

	private void Start()
	{
		cameraSway = sceneCamera.GetComponent<Animator>();
		wasEverZoomed = false;
		Timing.RunCoroutine(_Animate());
		mms = GetComponent<MainMenuScript>();
	}

	private void Update()
	{
		GameObject target = (wasEverZoomed ? focusedPosition : unfocusedPosition);
		if (retro)
		{
			sceneCamera.transform.position = target.transform.position;
			sceneCamera.transform.rotation = target.transform.rotation;
		}
		else
		{
			sceneCamera.transform.position = Vector3.Lerp(sceneCamera.transform.position, target.transform.position, Time.deltaTime * 3f);
			sceneCamera.transform.rotation = Quaternion.Lerp(sceneCamera.transform.rotation, target.transform.rotation, Time.deltaTime * 2f);
		}

		if (cameraSway != null)
		{
			float speed;
			if (mms.submenus[1].activeSelf || retro)
			{
				speed = 0f;
			}
			else if (wasEverZoomed && retroArcade.enabled)
			{
				speed = 0.3f;
			}
			else
			{
				speed = 0.7f;
			}
			cameraSway.SetFloat("Speed", speed);
		}

		if (!retroSupported)
		{
			return;
		}
		retroArcade.enabled = retro;
		retroPosterize.enabled = retro;
		if (!Input.anyKeyDown)
		{
			return;
		}
		if (Input.GetKeyDown(konami[konamiIndex]))
		{
			konamiIndex = Mathf.Min(konamiIndex + 1, konami.Length);
		}
		else
		{
			konamiIndex = 0;
		}
		if (konamiIndex != konami.Length)
		{
			return;
		}
		Debug.Log("GAME START !");
		GameCore.Console.AddLog("<size=25>GAME START !</size>", (Color)new Color32(255, 255, 255, 255));
		retro = !retro;
		MainMenuSoundtrackController soundtrack = Object.FindObjectOfType<MainMenuSoundtrackController>();
		if (soundtrack != null)
		{
			soundtrack.SoundtrackState = ((soundtrack.SoundtrackState == MainMenuSoundtrackController.MenuSoundtrackState.Retro)
				? MainMenuSoundtrackController.MenuSoundtrackState.MenuJustLoaded
				: MainMenuSoundtrackController.MenuSoundtrackState.Retro);
		}
		konamiIndex = 0;
	}

	private IEnumerator<float> _Animate()
	{
		while (this != null)
		{
			int duration = Random.Range(2, 5);
			SignBlink[] signs = Object.FindObjectsOfType<SignBlink>();
			foreach (SignBlink sign in signs)
			{
				sign.Play(duration);
			}
			yield return Timing.WaitForSeconds(Random.Range(3, 10));
		}
	}
}
