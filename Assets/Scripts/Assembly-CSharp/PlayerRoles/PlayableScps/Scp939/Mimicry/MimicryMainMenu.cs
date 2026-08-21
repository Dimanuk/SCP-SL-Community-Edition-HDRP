using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerRoles.PlayableScps.Scp939.Mimicry
{
	public class MimicryMainMenu : MimicryMenuBase
	{
		[Serializable]
		private class MainMenuOption
		{
			public Scp939HudTranslation Title;

			public Scp939HudTranslation Description;

			public UnityEvent OnSelect;
		}

		[SerializeField]
		private MainMenuOption _stolenVoicesOption;

		[SerializeField]
		private MainMenuOption _envSoundsOption;

		[SerializeField]
		private MainMenuOption _placeMimicPointOption;

		[SerializeField]
		private MainMenuOption _removeMimicPointOption;

		[SerializeField]
		private TMP_Text _titleTemplate;

		private int _mimicPointIndex;

		private MainMenuOption[] _options;

		private TMP_Text[] _textInstances;

		private MimicPointController _mimicPointController;

		public override int Slots => _options.Length;

		protected override void Setup(Scp939Role role)
		{
			base.Setup(role);
			_options = new MainMenuOption[3] { _stolenVoicesOption, _placeMimicPointOption, _envSoundsOption };
			_mimicPointIndex = Array.IndexOf(_options, _placeMimicPointOption);
			role.SubroutineModule.TryGetSubroutine<MimicPointController>(out _mimicPointController);
			_mimicPointController.OnMessageReceived += HandleRpc;
			RegisterMimicPointButton(_placeMimicPointOption);
			RegisterMimicPointButton(_removeMimicPointOption);
		}

		protected override void OnSlotsNumberChanged(int prev, int cur)
		{
			base.OnSlotsNumberChanged(prev, cur);
			_textInstances = new TMP_Text[cur];
			for (int i = 0; i < cur; i++)
			{
				RectTransform rectTransform = Highlights[i].rectTransform;
				TMP_Text tMP_Text = UnityEngine.Object.Instantiate(_titleTemplate, rectTransform.parent);
				tMP_Text.rectTransform.localPosition = GetSlotPosition(i);
				_textInstances[i] = tMP_Text;
			}
		}

		protected override void Update()
		{
			base.Update();
			if (base.HighlightedSlot >= 0)
			{
				UpdateDescription(_options[base.HighlightedSlot].Description);
			}
			else
			{
				UpdateDescription(Scp939HudTranslation.PressKeyToLunge);
			}
			_options[_mimicPointIndex] = (_mimicPointController.Active ? _removeMimicPointOption : _placeMimicPointOption);
			for (int i = 0; i < Slots; i++)
			{
				_textInstances[i].text = Translations.Get(_options[i].Title);
			}
		}

		protected override void OnSelected()
		{
			base.OnSelected();
			if (base.HighlightedSlot >= 0 && base.HighlightedSlot < Slots)
			{
				_options[base.HighlightedSlot].OnSelect?.Invoke();
			}
		}

		private void RegisterMimicPointButton(MainMenuOption option)
		{
			option.OnSelect.AddListener(_mimicPointController.ClientToggle);
		}

		private void HandleRpc(Scp939HudTranslation msg)
		{
			if (msg != Scp939HudTranslation.PressKeyToLunge)
			{
				MimicryMenuController.Singleton.CurrentGroup = null;
			}
		}

		private void OnDestroy()
		{
			if (_mimicPointController != null)
			{
			_mimicPointController.OnMessageReceived -= HandleRpc;
			}
		}
	}
}
