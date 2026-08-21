using System;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using PlayerRoles.PlayableScps.Scp079.GUI;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Map
{
	public class Scp079MapGui : Scp079GuiElementBase
	{
		[Serializable]
		private struct MapAnimation
		{
			public AnimationCurve Background;
			public AnimationCurve Horizontal;
			public AnimationCurve Vertical;
			public AnimationCurve Compressor;
		}

		[SerializeField] private float _animDuration;
		[SerializeField] private CanvasGroup _background;
		[SerializeField] private CanvasGroup _compressor;
		[SerializeField] private RectTransform _scalable;
		[SerializeField] private MapAnimation _closeAnim;
		[SerializeField] private MapAnimation _openAnim;
		[SerializeField] private MonoBehaviour[] _zoneMaps;
		[SerializeField] private RectTransform _mapMover;
		[SerializeField] private RectTransform _mapScaler;

		private float _animValue;
		private float _prevAnimVal;
		private float _zoom;
		private bool _prevOpen;
		private Scp079CurrentCameraSync _curCamSync;
		private Vector3 _prevOffset;

		private const string AxisX = "Mouse X";
		private const string AxisY = "Mouse Y";
		private const string AxisScroll = "Mouse ScrollWheel";
		private const float MouseSensitivity = 30f;
		private const float SpectatorLerp = 15f;

		public static Scp079Camera HighlightedCamera { get; private set; }
		public static Vector3 SyncVars { get; internal set; }

		private void Update()
		{
			bool mapState = Scp079ToggleMapAbility.MapState;
			float delta = Time.deltaTime * (mapState ? 1f : -1f);
			_animValue = Mathf.Clamp(_animValue + delta, 0f, _animDuration);

			if (mapState)
			{
				if (!_prevOpen)
					OnOpened();
				UpdateOpen();
			}
			else if (_prevOpen)
			{
				HighlightedCamera = null;
			}

			_prevOpen = mapState;

			if (_prevAnimVal != _animValue)
			{
				EvaluateAll(mapState, _animValue);
				_prevAnimVal = _animValue;
			}
		}

		private void OnOpened()
		{
			if (!Scp079Role.LocalInstanceActive)
			{
				_mapMover.localPosition = (Vector2)SyncVars;
				_mapScaler.localScale = Vector3.one * SyncVars.z;
				return;
			}

			_zoom = 1f;
			_mapScaler.localScale = Vector3.one;

			Scp079Camera currentCamera = _curCamSync.CurrentCamera;
			MonoBehaviour[] zoneMaps = _zoneMaps;
			for (int i = 0; i < zoneMaps.Length; i++)
			{
				if (((IZoneMap)zoneMaps[i]).TryGetCenterTransform(currentCamera, out Vector3 center))
				{
					_prevOffset = center;
					break;
				}
			}

			_mapMover.anchoredPosition = _prevOffset;
		}

		private void UpdateOpen()
		{
			HighlightedCamera = null;

			bool isSpectator = !Scp079Role.LocalInstanceActive;
			bool canControl = !Cursor.visible && !isSpectator;

			if (canControl)
			{
				float scrollInput = SensitivitySettings.RawInput
					? Input.GetAxisRaw(AxisScroll)
					: Input.GetAxis(AxisScroll);

				_zoom = Mathf.Clamp(_zoom + scrollInput, 0.3f, 2.1f);
				_mapScaler.localScale = Vector3.one * _zoom;

				Vector2 mouseInput = SensitivitySettings.RawInput
					? new Vector2(Input.GetAxisRaw(AxisX), Input.GetAxisRaw(AxisY))
					: new Vector2(Input.GetAxis(AxisX), Input.GetAxis(AxisY));

				Vector3 mouseDelta = (Vector3)(mouseInput * MouseSensitivity * SensitivitySettings.SensMultiplier / _zoom);

				Vector3 localPos = _mapMover.localPosition;
				localPos -= mouseDelta;
				_mapMover.localPosition = localPos;

				SyncVars = new Vector3(localPos.x, localPos.y, _zoom);
			}

			if (isSpectator)
			{
				_mapScaler.localScale = Vector3.one * SyncVars.z;
				_mapMover.localPosition = Vector3.Lerp(_mapMover.localPosition, SyncVars, Time.deltaTime * SpectatorLerp);
			}

			MonoBehaviour[] zoneMaps = _zoneMaps;
			for (int i = 0; i < zoneMaps.Length; i++)
			{
				if (!canControl && !isSpectator)
					break;

				if (((IZoneMap)zoneMaps[i]).TryGetCamera(out Scp079Camera target))
				{
					HighlightedCamera = target;
				}
			}

			Scp079Camera currentCamera = _curCamSync.CurrentCamera;
			zoneMaps = _zoneMaps;
			for (int i = 0; i < zoneMaps.Length; i++)
			{
				((IZoneMap)zoneMaps[i]).UpdateOpened(currentCamera);
			}
		}

		private void EvaluateAll(bool open, float val)
		{
			MapAnimation anim = open ? _openAnim : _closeAnim;
			_background.alpha = anim.Background.Evaluate(val);
			_compressor.alpha = anim.Compressor.Evaluate(val);
			_scalable.localScale = new Vector3(
				anim.Horizontal.Evaluate(val),
				anim.Vertical.Evaluate(val),
				1f);
		}

		internal override void Init(Scp079Role role, ReferenceHub owner)
		{
			base.Init(role, owner);
			role.SubroutineModule.TryGetSubroutine<Scp079CurrentCameraSync>(out _curCamSync);

			MonoBehaviour[] zoneMaps = _zoneMaps;
			for (int i = 0; i < zoneMaps.Length; i++)
			{
				((IZoneMap)zoneMaps[i]).Generate();
			}
		}
	}
}
