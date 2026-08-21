using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClassPresetChooser : MonoBehaviour
{
	[Serializable]
	public class PickerPreset
	{
		public string classID;

		public Texture icon;

		public int health;

		public float wSpeed;

		public float rSpeed;

		public float stamina;

		public Texture[] startingItems;

		public string en_additionalInfo;

		public string pl_additionalInfo;
	}

	public GameObject bottomMenuItem;

	public Transform bottomMenuHolder;

	public PickerPreset[] presets;

	private string curKey;

	private List<PickerPreset> curPresets;

	public Slider health;

	public Slider wSpeed;

	public Slider rSpeed;

	public RawImage[] startItems;

	public RawImage avatar;

	public TextMeshProUGUI addInfo;

	public void RefreshBottomItems(string key)
	{
		curKey = key;
		curPresets.Clear();
		int count = 0;
		for (int i = 0; i < presets.Length; i++)
		{
			if (presets[i].classID != key)
			{
				continue;
			}
			count++;
			curPresets.Add(presets[i]);
		}
		foreach (Transform item in bottomMenuHolder)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		for (int j = 0; j < count; j++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(bottomMenuItem, bottomMenuHolder);
			gameObject.GetComponent<BottomPickerItem>().SetupButton(key, j);
			gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "ABCDEFGHIJKL"[j].ToString();
		}
	}

	private void Update()
	{
		if (curPresets.Count <= 0)
		{
			return;
		}
		PickerPreset pickerPreset = curPresets[PlayerPrefsSl.Get(curKey, 0)];
		health.value = pickerPreset.health;
		wSpeed.value = pickerPreset.wSpeed;
		rSpeed.value = pickerPreset.rSpeed;
		avatar.texture = pickerPreset.icon;
		for (int i = 0; i < startItems.Length; i++)
		{
			if (i < pickerPreset.startingItems.Length)
			{
				startItems[i].color = Color.white;
				startItems[i].texture = pickerPreset.startingItems[i];
			}
			else
			{
				startItems[i].color = Color.clear;
			}
		}
	}

	public ClassPresetChooser()
	{
		curPresets = new List<PickerPreset>();
	}
}
