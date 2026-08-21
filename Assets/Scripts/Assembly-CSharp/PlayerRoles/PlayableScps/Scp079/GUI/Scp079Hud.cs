using System;
using PlayerRoles.PlayableScps.HUDs;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace PlayerRoles.PlayableScps.Scp079.GUI
{
	public class Scp079Hud : ScpHudBase
	{
		public static Camera RaycastCamera { get; private set; }
		public static Scp079Role Instance { get; private set; }

		[SerializeField]
		private Camera[] _sceneCameras;

		[SerializeField]
		private GameObject[] _hideableGuiObjects;

		[SerializeField]
		private GameObject[] _aliveObjects;

		[SerializeField]
		private PostProcessVolume _deathVolume;

		[SerializeField]
		private Gradient _deathBarsColor;

		[SerializeField]
		private RawImage _deathBars;

		[SerializeField]
		private float _deathAnimSpeed;

		[SerializeField]
		private int _raycastCameraIndex;

		private float[] _defaultFovs;
		private int _camCount;
		private Scp079CurrentCameraSync _curCamSync;

		private Transform SceneCamera
		{
			get => MainCameraController._currentCamera;
			set => MainCameraController.CurrentCamera = value;
		}

		private void Awake()
		{
			PlayerRoleManager.OnRoleChanged += OnRoleChanged;
		}

		private void OnRoleChanged(ReferenceHub ply, PlayerRoleBase prev, PlayerRoleBase newRole)
		{
			if (ply != Hub)
				return;

			foreach (GameObject aliveObject in _aliveObjects)
				aliveObject.SetActive(newRole is Scp079Role);
		}

		internal override void Init(ReferenceHub hub)
		{
			base.Init(hub);

			PostProcessing.PostProcessChainUnity6Fix.ApplyToScp079Hud(transform);

			SceneCamera = transform;

			RaycastCamera = _sceneCameras[_raycastCameraIndex];

			Instance = hub.roleManager.CurrentRole as Scp079Role;

			Instance.SubroutineModule.TryGetSubroutine(out _curCamSync);

			_camCount = _sceneCameras.Length;
			_defaultFovs = new float[_camCount];

			for (int i = 0; i < _camCount; i++)
				_defaultFovs[i] = _sceneCameras[i].fieldOfView;

			var elements = GetComponentsInChildren<Scp079GuiElementBase>(true);
			foreach (Scp079GuiElementBase element in elements)
				element.Init(Instance, hub);

			ToggleHud(HideHUDController.IsHUDVisible);
		}

		protected override void Update()
		{
			if (!_deathVolume.enabled)
			{
				if (_curCamSync.TryGetCurrentCamera(out Scp079Camera cam)
					&& cam.ZoomAxis != null)
				{
					float currentZoom = cam.ZoomAxis.CurrentZoom;
					if (currentZoom > 0f)
					{
						float fovMultiplier = 1f / currentZoom;

						for (int i = 0; i < _camCount; i++)
							_sceneCameras[i].fieldOfView = fovMultiplier * _defaultFovs[i];
					}
				}
			}
			else
			{
				_deathVolume.weight = Mathf.Clamp01(_deathVolume.weight + Time.deltaTime * _deathAnimSpeed);
				_deathBars.color = _deathBarsColor.Evaluate(_deathVolume.weight);
			}
		}
		internal override void OnDied()
		{
			base.OnDied();
			_deathVolume.enabled = true;
		}

		protected override void ToggleHud(bool b)
		{
			foreach (GameObject hideable in _hideableGuiObjects)
				hideable.SetActive(b);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			PlayerRoleManager.OnRoleChanged -= OnRoleChanged;

			if (MainCameraController._currentCamera == transform)
				MainCameraController.CurrentCamera = null;
		}
	}
}
