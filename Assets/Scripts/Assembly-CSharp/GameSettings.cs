using System;

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
	[Serializable]
	public struct GameSettingsAudioSlider
	{
		public string Key;

		public Slider SliderReference;

		public Text OptionalText;

		public float DefaultValue;

		public void LoadState()
		{
			if (singleton == null) return;

			Slider localSlider = SliderReference;
			float savedVolume = PlayerPrefs.GetFloat(Key, 1f);
			localSlider.value = savedVolume;

			localSlider.onValueChanged.AddListener(v => singleton.OnSliderValueChange(localSlider));

			singleton.OnSliderValueChange(localSlider);
		}

		public void SaveState()
		{
			PlayerPrefs.SetFloat(Key, SliderReference.value);
		}
	}

	public AudioMixer MasterAudioMixer;

	public static GameSettings singleton;

	public GameSettingsAudioSlider[] AudioSliders;

	private void Awake()
	{
		singleton = this;
		foreach (var slider in AudioSliders)
		{
			slider.LoadState();
		}
	}

	public void OnSliderValueChange(Slider activator)
	{
		foreach (var audioSlider in AudioSliders)
		{
			if (audioSlider.SliderReference != activator)
				continue;

			if (audioSlider.OptionalText != null)
			{
				int percent = Mathf.RoundToInt(activator.value * 100f);
				audioSlider.OptionalText.text = percent + " %";
			}

			float volume;
			if (activator.value <= 0f)
			{
				volume = -80f;
			}
			else
			{
				volume = Mathf.Log10(activator.value) * 20f;
			}

			MasterAudioMixer.SetFloat(audioSlider.Key, volume);
			if (audioSlider.Key == "SFX")
			{
				MasterAudioMixer.SetFloat("No Ducking", -144f);
			}

			audioSlider.SaveState();
			break;
		}
	}
}
