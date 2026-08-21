using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.GUI
{
	public class Scp079NotificationManager : Scp079GuiElementBase
	{
		[SerializeField]
		private Scp079NotificationEntry _template;

		[SerializeField]
		private Vector2 _defaultSize;

		[SerializeField]
		private AudioClip[] _sounds;

		private readonly Queue<Scp079NotificationEntry> _textPool = new Queue<Scp079NotificationEntry>();
		private readonly List<Scp079NotificationEntry> _spawnedTexts = new List<Scp079NotificationEntry>();

		private static Scp079NotificationManager _singleton;
		private static bool _singletonSet;

		private void Awake()
		{
			_singleton = this;
			_singletonSet = true;
		}

		private void OnDestroy()
		{
			if (_singleton == this)
			{
				_singletonSet = false;
			}
		}

		private void Update()
		{
			for (int i = 0; i < _spawnedTexts.Count; i++)
			{
				Scp079NotificationEntry entry = _spawnedTexts[i];
				IScp079Notification content = entry.Content;

				entry.Text.text = content.DisplayedText;
				entry.Text.alpha = Mathf.Clamp01(content.Opacity);
				entry.Text.rectTransform.sizeDelta = new Vector2(_defaultSize.x, content.Height);

				NotificationSound sound = content.Sound;
				if (sound != NotificationSound.None)
				{
					PlaySound(_sounds[(int)sound]);
				}

				if (content.Delete)
				{
					entry.gameObject.SetActive(false);
					_textPool.Enqueue(entry);
					_spawnedTexts.RemoveAt(i);
					i--;
				}
			}
		}

		private void SpawnNotification(IScp079Notification notification)
		{
			if (!CollectionExtensions.TryDequeue(_textPool, out var element))
			{
				element = Instantiate(_template, _template.transform.parent);
			}

			_spawnedTexts.Add(element);
			element.Content = notification;
			element.Text.text = string.Empty;
			element.Text.rectTransform.sizeDelta = _defaultSize;
			element.gameObject.SetActive(true);
			element.transform.SetAsLastSibling();
		}

		public static void AddNotification(IScp079Notification handler)
		{
			if (_singletonSet)
			{
				_singleton.SpawnNotification(handler);
			}
		}

		public static void AddNotification(string notification)
		{
			AddNotification(new Scp079SimpleNotification(notification));
		}

		public static void AddNotification(Scp079HudTranslation translation)
		{
			AddNotification(Translations.Get(translation));
		}

		public static void AddNotification(Scp079HudTranslation translation, params object[] format)
		{
			AddNotification(string.Format(Translations.Get(translation), format));
		}
	}
}
