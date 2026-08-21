using RadialMenus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoles.PlayableScps.Scp939.Mimicry
{
	public abstract class MimicryMenuBase : RadialMenuBase
	{
		[SerializeField]
		private CanvasGroup _descriptionFader;

		[SerializeField]
		private TMP_Text _descriptionInfo;

		private float _angleStep;

		private float _halfStep;

		private Scp939HudTranslation _displayedDesc;

		private const float DescriptionFadeSpeed = 15f;

		private const float IconOffset = 280f;

		[field: SerializeField]
		public CanvasGroup Fader { get; private set; }

		protected string DescriptionText
		{
			get
			{
				return _descriptionInfo.text;
			}
			set
			{
				_descriptionInfo.text = value;
			}
		}

		protected virtual void Awake()
		{
			if (ReferenceHub.TryGetLocalHub(out var hub) && hub.roleManager.CurrentRole is Scp939Role role)
			{
				Setup(role);
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}

		protected virtual void Setup(Scp939Role role)
		{
		}

		protected override void Update()
		{
			base.Update();
			int slots = Slots;
			for (int i = 0; i < slots; i++)
			{
				MimicryMenuController.UpdateHighlight(Highlights[i], i == base.HighlightedSlot);
			}
			if (Input.GetKeyDown(KeyCode.Mouse0) && !(Fader.alpha < 1f))
			{
				OnSelected();
			}
		}

		protected void UpdateDescription(Scp939HudTranslation translation)
		{
			float num = 15f * Time.deltaTime;
			if (_displayedDesc == translation)
			{
				_descriptionFader.alpha = Mathf.MoveTowards(_descriptionFader.alpha, Mathf.Min(1, (int)_displayedDesc), num);
				return;
			}
			if (_descriptionFader.alpha > 0f)
			{
				_descriptionFader.alpha -= num;
				return;
			}
			OnDescriptionUpdated(translation);
			_displayedDesc = translation;
		}

		protected virtual void OnDescriptionUpdated(Scp939HudTranslation newDesc)
		{
			DescriptionText = Translations.Get(newDesc);
		}

		protected Vector3 GetSlotPosition(int slot)
		{
			return Quaternion.Euler(0f, 0f, (0f - _angleStep) * (float)slot - _halfStep) * Vector3.up * 280f;
		}

		protected virtual void OnSelected()
		{
			if (base.HighlightedSlot < 0 && !InRingRange(out var _) && !MimicryExternalButton.TryGetHighlighted(out var _))
			{
				MimicryMenuController.Singleton.IsEnabled = false;
			}
		}

		protected override void OnSlotsNumberChanged(int prev, int cur)
		{
			_angleStep = 360f / (float)Mathf.Max(1, cur);
			_halfStep = ((cur == 1) ? 0f : (_angleStep / 2f));
			if (cur != 0)
			{
				base.OnSlotsNumberChanged(prev, cur);
			}
			Image[] highlights = Highlights;
			foreach (Image image in highlights)
			{
				if (image == null)
				{
					break;
				}
				image.color = Color.clear;
			}
			base.OnSlotsNumberChanged(prev, cur);
		}

		public void Open()
		{
			MimicryMenuController.Singleton.CurrentGroup = Fader;
			int num = Slots;
			while (--num >= 0)
			{
				MimicryMenuController.UpdateHighlight(Highlights[num], isHighlighted: false, 1f);
			}
		}
	}
}
