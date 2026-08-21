using System;
using System.Collections.Generic;
using MapGeneration;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace PlayerRoles.PlayableScps.Scp079.GUI
{
	public class Scp079Nightvision : Scp079GuiElementBase
	{
		[Serializable]
		private struct ZoneNightvisionPair
		{
			public PostProcessVolume PostProcess;
			public FacilityZone Zone;
		}

		[SerializeField]
		private ZoneNightvisionPair[] _pairs;

		private Scp079CurrentCameraSync _curCam;
		private Scp079LostSignalHandler _lostSignal;

		private readonly HashSet<PostProcessVolume> _volumes = new HashSet<PostProcessVolume>();
		private readonly Dictionary<FacilityZone, PostProcessVolume> _zoneTargets = new Dictionary<FacilityZone, PostProcessVolume>();

		private PostProcessVolume TargetVolume
		{
			get
			{
				if (_curCam.CurClientSwitchState != Scp079CurrentCameraSync.ClientSwitchState.None)
					return null;

				if (_lostSignal.Lost)
					return null;

				Scp079Camera currentCamera = Role.CurrentCamera;
				if (currentCamera == null)
					return null;

				if (!FlickerableLightController.IsInDarkenedRoom(currentCamera.Position))
					return null;

				if (!_zoneTargets.TryGetValue(currentCamera.Room.Zone, out var value))
					return null;

				return value;
			}
		}

		internal override void Init(Scp079Role role, ReferenceHub owner)
		{
			base.Init(role, owner);
			role.SubroutineModule.TryGetSubroutine(out _curCam);
			role.SubroutineModule.TryGetSubroutine(out _lostSignal);
			_curCam.OnCameraChanged += UpdateAll;

			foreach (ZoneNightvisionPair pair in _pairs)
			{
				_volumes.Add(pair.PostProcess);
				_zoneTargets[pair.Zone] = pair.PostProcess;
			}

			UpdateAll();
		}

		private void OnDestroy()
		{
			if (_curCam != null)
				_curCam.OnCameraChanged -= UpdateAll;
		}

		private void Update()
		{
			UpdateAll();
		}

		private void UpdateAll()
		{
			PostProcessVolume target = TargetVolume;
			foreach (PostProcessVolume volume in _volumes)
			{
				volume.enabled = volume == target;
			}
		}
	}
}
