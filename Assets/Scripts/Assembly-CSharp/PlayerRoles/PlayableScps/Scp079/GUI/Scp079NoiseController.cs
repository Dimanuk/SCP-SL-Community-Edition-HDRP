using System;
using MapGeneration;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using PostProcessing;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace PlayerRoles.PlayableScps.Scp079.GUI
{
	public class Scp079NoiseController : Scp079GuiElementBase
	{
		[Serializable]
		private class NoiseAnimation
		{
			[SerializeField]
			private float _animationSpeed;

			[SerializeField]
			private AnimationCurve _fadeCurve;

			[SerializeField]
			private AnimationCurve _additiveCurve;

			[SerializeField]
			private AnimationCurve _distortionCurve;

			[SerializeField]
			private AnimationCurve _darkenCurve;

			[SerializeField]
			private AnimationCurve _alphaCurve;

			public float PrevValue { get; set; }

			public void ApplyTowards(Scp079NoiseController inst, float target, bool force)
			{
				// Раскачка ограничена ~кадром 30fps: кадр спавна (инстанцирование HUD) длится
				// сотни мс, и сырой deltaTime съедал весь boot-up шум за один Update —
				// анимация появления не успевала попасть на экран.
				float dt = Mathf.Min(Time.deltaTime, 1f / 30f);
				float num = Mathf.MoveTowards(PrevValue, target, dt * _animationSpeed);
				if (PrevValue != num || force)
				{
					ApplyAnimation(inst, num);
				}
			}

			public void ApplyAnimation(Scp079NoiseController inst, float f)
			{
				inst._static.fade.value = _fadeCurve.Evaluate(f);
				inst._static.fadeAdditive.value = _additiveCurve.Evaluate(f);
				inst._static.fadeDistortion.value = _distortionCurve.Evaluate(f);
				inst._darken.intensity.value = _darkenCurve.Evaluate(f);
				float num = _alphaCurve.Evaluate(f);
				inst._overconFader.SetColor(ColorHash, Color.Lerp(TransparentWhite, Color.white, num));
				inst._canvasFader.alpha = num;
				PrevValue = f;
			}
		}

		[Serializable]
		private struct ZoneSwitchSounds
		{
			public AudioClip Clip;
			public FacilityZone Zone;
		}

		private Scp079CurrentCameraSync _camSync;
		private Scp079LostSignalHandler _lostSignalHandler;

		private Static _static;
		private Darken _darken;

		private Scp079CurrentCameraSync.ClientSwitchState _prevSwitchState;

		private static readonly Color TransparentWhite = new Color(1f, 1f, 1f, 0f);
		private static readonly int ColorHash = Shader.PropertyToID("_Color");

		[SerializeField]
		private AudioClip _noiseClip;

		[SerializeField]
		private ZoneSwitchSounds[] _zoneOverrides;

		[SerializeField]
		private float _pitchVariation;

		[SerializeField]
		private NoiseAnimation _lostSignalAnim;

		[SerializeField]
		private NoiseAnimation _regularSwitchAnim;

		[SerializeField]
		private NoiseAnimation _zoneSwitchAnim;

		[SerializeField]
		private CanvasGroup _canvasFader;

		[SerializeField]
		private CanvasGroup _zoneSwitchFader;

		[SerializeField]
		private Material _overconFader;

		internal override void Init(Scp079Role role, ReferenceHub owner)
		{
			base.Init(role, owner);
			Role.SubroutineModule.TryGetSubroutine(out _camSync);
			Role.SubroutineModule.TryGetSubroutine(out _lostSignalHandler);
			if (TryGetComponent(out PostProcessVolume component))
			{
				PostProcessProfile profile = component.profile;
				_static = profile.GetSetting<Static>();
				_darken = profile.GetSetting<Darken>();
				_regularSwitchAnim.ApplyAnimation(this, 1f);
			}
		}

		private void Update()
		{
			if (_lostSignalHandler.Lost)
			{
				_regularSwitchAnim.ApplyAnimation(this, 0f);
				_zoneSwitchAnim.ApplyAnimation(this, 0f);
				_lostSignalAnim.ApplyTowards(this, 1f, true);
				_zoneSwitchFader.alpha = 0f;
			}
			else
			{
				UpdateSwitchState();
				_lostSignalAnim.ApplyTowards(this, 0f, false);
			}
		}

		private void UpdateSwitchState()
		{
			Scp079CurrentCameraSync.ClientSwitchState curClientSwitchState = _camSync.CurClientSwitchState;
			bool flag = curClientSwitchState != _prevSwitchState;
			_prevSwitchState = curClientSwitchState;

			switch (curClientSwitchState)
			{
				case Scp079CurrentCameraSync.ClientSwitchState.SwitchingRoom:
					if (flag)
					{
						PlaySound(_noiseClip).pitch = UnityEngine.Random.Range(1f - _pitchVariation, 1f + _pitchVariation);
					}
					_regularSwitchAnim.ApplyTowards(this, 1f, true);
					break;

				case Scp079CurrentCameraSync.ClientSwitchState.SwitchingZone:
					_regularSwitchAnim.ApplyTowards(this, 0f, true);
					_zoneSwitchAnim.ApplyTowards(this, 1f, true);
					_zoneSwitchFader.alpha = _darken.intensity.value;
					if (flag)
					{
						foreach (ZoneSwitchSounds zoneSwitchSounds in _zoneOverrides)
						{
							if (zoneSwitchSounds.Zone == _camSync.CurClientTargetZone)
							{
								PlaySound(zoneSwitchSounds.Clip);
								return;
							}
						}
						PlaySound(_noiseClip);
					}
					break;

				case Scp079CurrentCameraSync.ClientSwitchState.None:
					_regularSwitchAnim.ApplyTowards(this, 0f, false);
					_zoneSwitchAnim.ApplyTowards(this, 0f, false);
					_zoneSwitchFader.alpha = _zoneSwitchAnim.PrevValue;
					break;
			}
		}
	}
}
