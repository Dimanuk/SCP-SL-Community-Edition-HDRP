using System;
using System.Text;
using NorthwoodLib.Pools;
using PlayerRoles.PlayableScps.Subroutines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoles.PlayableScps.Scp079.GUI
{
	public class Scp079TierGui : Scp079GuiElementBase
	{
		[SerializeField]
		private Image _slider;

		[SerializeField]
		private TextMeshProUGUI _textTier;

		[SerializeField]
		private TextMeshProUGUI _textExpNormal;

		[SerializeField]
		private TextMeshProUGUI _textExpInverted;

		private bool _uiDirty;
		private Scp079TierManager _tierManager;

		private string _tierFormat;
		private string _expFormat;
		private string _maxTierText;
		private string _levelUpNotification;

		public const string NewLineFormat = "\n  - ";

		private string ExpText
		{
			set
			{
				_textExpNormal.text = value;
				_textExpInverted.text = value;
			}
		}

		internal override void Init(Scp079Role role, ReferenceHub owner)
		{
			base.Init(role, owner);
			_tierFormat = Translations.Get(Scp079HudTranslation.AccessTier);
			_expFormat = Translations.Get(Scp079HudTranslation.Experience);
			_maxTierText = Translations.Get(Scp079HudTranslation.MaxTierReached);
			_levelUpNotification = Translations.Get(Scp079HudTranslation.AccessTierUnlocked);
			role.SubroutineModule.TryGetSubroutine(out _tierManager);
			_tierManager.OnExpChanged += SetDirty;
			_tierManager.OnLevelledUp += OnLevelledUp;
			_uiDirty = true;
		}

		private void OnDestroy()
		{
			_tierManager.OnExpChanged -= SetDirty;
			_tierManager.OnLevelledUp -= OnLevelledUp;
		}

		private void Update()
		{
			if (!_uiDirty)
				return;

			_textTier.text = string.Format(_tierFormat, _tierManager.AccessTierLevel);

			if (_tierManager.NextLevelThreshold <= 0)
			{
				_slider.fillAmount = 1f;
				ExpText = _maxTierText;
			}
			else
			{
				_slider.fillAmount = (float)_tierManager.RelativeExp / _tierManager.NextLevelThreshold;
				ExpText = string.Format(_expFormat, _tierManager.RelativeExp, _tierManager.NextLevelThreshold);
			}

			_uiDirty = false;
		}

		private void SetDirty()
		{
			_uiDirty = true;
		}

		private void OnLevelledUp()
		{
			SetDirty();

			bool flag = false;
			StringBuilder sb = StringBuilderPool.Shared.Rent();
			sb.AppendFormat(_levelUpNotification, _tierManager.AccessTierLevel);

			foreach ( SubroutineBase subroutine in Role.SubroutineModule.AllSubroutines)
			{
				if (subroutine is IScp079LevelUpNotifier notifier)
				{
					if (!flag)
					{
						sb.Append(NewLineFormat);
					}
					flag = !notifier.WriteLevelUpNotification(sb, _tierManager.AccessTierIndex);
				}
			}

			string text = StringBuilderPool.Shared.ToStringReturn(sb);
			if (flag)
			{
				text = text.Substring(0, text.Length - NewLineFormat.Length);
			}

			Scp079NotificationManager.AddNotification(new Scp079AccentedNotification(text));
		}
	}
}
