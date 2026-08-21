
using AudioPooling;
using RemoteAdmin.Interfaces;
using UnityEngine;

namespace CustomPlayerEffects
{
	public class Invisible : StatusEffectBase, ISpectatorDataPlayerEffect, ICustomRADisplay
	{
		[SerializeField]
		private AudioClip _sfxEnable;

		[SerializeField]
		private AudioClip _sfxDisable;

		private bool _wasEverActive;

		public override EffectClassification Classification => EffectClassification.Positive;

		public string DisplayName => "Invisibility";

		public bool CanBeDisplayed => true;

        public bool GetSpectatorText(out string s)
        {
            s = "SCP-268";
            return true;
        }

        protected override void Enabled()
		{
			PlaySound(isEnabled: true);
			_wasEverActive = true;
		}

		protected override void Disabled()
		{
			if (_wasEverActive)
			{
				int isEnabled = 0;
				PlaySound((byte)isEnabled != 0);
			}
		}

		private void PlaySound(bool isEnabled)
		{
			if (Hub == null)
				return;

			if (!IsLocalPlayer && !IsSpectated)
				return;

			AudioClip clip = isEnabled ? _sfxEnable : _sfxDisable;
			AudioSourcePoolManager.PlaySound(clip, Hub.transform, 10f);
		}
	}
}
