using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using OperationalGuide;

public class Credits : MonoBehaviour
{
	private static bool _skip;
	private static int _timer;

	[SerializeField] private TextMeshProUGUI Header;
	[SerializeField] private TextMeshProUGUI[] RecordTitles;
	[SerializeField] private TextMeshProUGUI[] Records;

	private void OnEnable()
	{
		StopAllCoroutines();
		StartCoroutine(PlayEnumerator());
	}

	public void Skip()
	{
		_skip = true;
	}

	private IEnumerator Play()
	{
		return PlayEnumerator();
	}

	private IEnumerator PlayEnumerator()
	{
		CreditsCategory[] categories = CreditsData.Data;
		if (categories == null) yield break;

		foreach (var category in categories)
		{
			CreditsEntry[] recordsToShow = category.Records;
			if (category.Header == "$50+ Patreon Supporters")
			{
				recordsToShow = ParsePatreonsArray();
			}

			Clear();

			if (Header != null)
				Header.text = category.Header;

			for (int i = 0; i < recordsToShow.Length; i++)
			{
				if (_skip)
				{
					_skip = false;
					yield break;
				}

				if (RecordTitles != null && i < RecordTitles.Length && recordsToShow[i] != null)
					RecordTitles[i].text = recordsToShow[i].Title ?? "";
				if (Records != null && i < Records.Length && recordsToShow[i] != null)
					Records[i].text = recordsToShow[i].Name ?? "";

				yield return new WaitForSeconds(0.02f);
			}

			yield return new WaitForSeconds(0.02f);
		}

		var menu = GetComponentInParent<MainMenuScript>();
		if (menu != null)
			menu.ChangeMenu(0);
	}

	private void Clear()
	{
		if (RecordTitles != null)
		{
			foreach (var title in RecordTitles)
			{
				if (title != null) title.text = "";
			}
		}
		if (Records != null)
		{
			foreach (var record in Records)
			{
				if (record != null) record.text = "";
			}
		}
	}

	private CreditsEntry[] ParsePatreonsArray()
	{
		string patreonsText = CreditsData.Patreons ?? "";
		var lines = patreonsText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
		var entries = new List<CreditsEntry>();
		foreach (var line in lines)
		{
			string trimmed = line.Trim();
			if (!string.IsNullOrEmpty(trimmed))
				entries.Add(new CreditsEntry(trimmed));
		}
		return entries.ToArray();
	}

}