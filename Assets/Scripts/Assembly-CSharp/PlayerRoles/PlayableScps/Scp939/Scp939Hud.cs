using Mirror;
using PlayerRoles.PlayableScps.HUDs;
using PlayerRoles.PlayableScps.Scp939.Mimicry;
using PlayerRoles.PlayableScps.Subroutines;
using PostProcessing;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace PlayerRoles.PlayableScps.Scp939
{
	public class Scp939Hud : ViewmodelScpHud
	{
		[SerializeField]
		private GameObject _hudRoot;

		[SerializeField]
		private TextMeshProUGUI _lungeReadyText;

		[SerializeField]
		private AbilityHud _amnesticCloudPlacedIcon;

		[SerializeField]
		private AbilityHud _amnesticCloudBuildupIcon;

		[SerializeField]
		private AbilityHud _mimicryCooldown;

		[SerializeField]
		private ScpWarningHud _warningElement;

		[SerializeField]
		private CanvasGroup _warningFader;

		[SerializeField]
		private PostProcessVolume _postProcessVolume;

		[SerializeField]
		private float _blurAdditive;

		[SerializeField]
		private float _effectsLerpSpeed;

		[SerializeField]
		private AnimationCurve _startDisOverRange;

		private EnvironmentalMimicry _envMimicry;

		private MimicPointController _mimicPoint;

		private Scp939VisibilityController _visController;

		private Scp939LungeAbility _lungeAbility;

		private Scp939AmnesticCloudAbility _cloudAbility;

		private FogEffect _ppFog;

		private BlurEffect _ppBlur;

		private Grayscale _ppGrayscale;

		private const float TextFadeSpeed = 5f;

		private const float WarningDuration = 5.5f;

		private const float ProlongedUpdateTime = 0.4f;

		private const string DefaultLungeFormat = "Press {0} or {1} to Lunge.";

		private void LateUpdate()
		{
			if (_lungeAbility == null || _cloudAbility == null)
				return;

			float num = (_lungeAbility.IsReady ? TextFadeSpeed : (0f - TextFadeSpeed));
			float num2 = Mathf.Clamp01(_lungeReadyText.alpha + Time.deltaTime * num);
			_lungeReadyText.alpha = num2;
			_warningFader.alpha = 1f - num2;
			_mimicryCooldown.Update();
			if (_cloudAbility.TargetState)
			{
				if (_cloudAbility.Cooldown.NextUse > NetworkTime.time - ProlongedUpdateTime)
				{
					UpdateAmnesticCloud(buildupHidden: true);
				}
			}
			else
			{
				UpdateAmnesticCloud(buildupHidden: false);
			}
			LerpEffects(Time.deltaTime * _effectsLerpSpeed);
		}

		private void UpdateAmnesticCloud(bool buildupHidden)
		{
			_amnesticCloudBuildupIcon.Update(buildupHidden);
			_amnesticCloudPlacedIcon.Update(!buildupHidden);
		}

		private void LerpEffects(float lerp)
		{
			if (!_hudRoot.activeSelf || _visController == null)
			{
				return;
			}
			float range = _visController.CurrentDetectionRange;
			if (_startDisOverRange != null)
			{
				float target = _startDisOverRange.Evaluate(range);
				LerpEffect(_ppFog, target, lerp);
				LerpEffect(_ppGrayscale, target, lerp);
				LerpEffect(_ppBlur, target + _blurAdditive, lerp);
			}
		}

		private void ShowWarning(Scp939HudTranslation val)
		{
			if (val == Scp939HudTranslation.PressKeyToLunge)
			{
				return;
			}
			if (_warningElement != null)
			{
				_warningElement.SetText(Translations.Get(val), WarningDuration);
			}
		}

		private void LerpEffect(DistanceEffect fx, float target, float lerp)
		{
			if (fx != null)
			{
				FloatParameter fogStartDistance = fx.fogStartDistance;
				if (fogStartDistance != null)
				{
					fogStartDistance.value = Mathf.Lerp(fogStartDistance.value, target, lerp);
				}
			}
		}

		private string ActionKeyName(ActionName an)
		{
			return string.Format("<color=red>{0}</color>", new ReadableKeyCode(an));
		}

		internal override void Init(ReferenceHub hub)
		{
			base.Init(hub);
			if (!(hub.roleManager.CurrentRole is Scp939Role scp939Role))
			{
				return;
			}
			SubroutineManagerModule subroutineModule = scp939Role.SubroutineModule;
			subroutineModule.TryGetSubroutine<Scp939LungeAbility>(out _lungeAbility);
			subroutineModule.TryGetSubroutine<Scp939AmnesticCloudAbility>(out _cloudAbility);
			subroutineModule.TryGetSubroutine<MimicPointController>(out _mimicPoint);
			subroutineModule.TryGetSubroutine<EnvironmentalMimicry>(out _envMimicry);
			_cloudAbility.OnDeployFailed += ShowWarning;
			_mimicPoint.OnMessageReceived += ShowWarning;
			_visController = scp939Role.VisibilityController as Scp939VisibilityController;
			_mimicryCooldown.Setup(_envMimicry.Cooldown, null);
			_amnesticCloudPlacedIcon.Setup(_cloudAbility.Cooldown, _cloudAbility.Duration);
			_amnesticCloudBuildupIcon.Setup(_cloudAbility.HudIndicatorMin, _cloudAbility.HudIndicatorMax);
			PostProcessProfile profile = _postProcessVolume.profile;
			if (profile != null)
			{
				_ppFog = profile.GetSetting<FogEffect>();
				_ppBlur = profile.GetSetting<BlurEffect>();
				_ppGrayscale = profile.GetSetting<Grayscale>();
				if (!hub.isLocalPlayer)
				{
					_lungeReadyText.enabled = false;
				}
				else
				{
					_lungeReadyText.text = string.Format(DefaultLungeFormat, ActionKeyName(ActionName.Shoot), ActionKeyName(ActionName.Jump));
				}
				LerpEffects(1f);
			}
		}

		internal override void OnDied()
		{
			_hudRoot.SetActive(value: false);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (_cloudAbility != null)
			{
				_cloudAbility.OnDeployFailed -= ShowWarning;
			}
			if (_mimicPoint != null)
			{
				_mimicPoint.OnMessageReceived -= ShowWarning;
			}
		}
	}
}
