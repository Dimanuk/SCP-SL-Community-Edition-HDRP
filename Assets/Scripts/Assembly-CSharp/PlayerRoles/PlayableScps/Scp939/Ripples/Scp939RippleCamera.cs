using CameraShaking;
using PlayerRoles.PlayableScps.Scp079.Pinging;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Spectating;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939.Ripples
{
	public class Scp939RippleCamera : ScpStandardSubroutine<Scp939Role>
	{
		[SerializeField]
		private Camera _cam;

		private Camera _instance;

		private bool _isActive;

		private const int RippleLayer = 22;

		public override void SpawnObject()
		{
			base.SpawnObject();
			RefreshState();
			Scp079PingInstance.OnSpawned += OnPingSpawned;
			SpectatorTargetTracker.OnTargetChanged += RefreshState;
		}

		public override void ResetObject()
		{
			base.ResetObject();
			_isActive = false;
			RefreshCamera();
			Scp079PingInstance.OnSpawned -= OnPingSpawned;
			SpectatorTargetTracker.OnTargetChanged -= RefreshState;
		}

		private void RefreshState()
		{
			bool flag = base.Owner.isLocalPlayer || base.Owner.IsLocallySpectated();
			if (_isActive != flag)
			{
				_isActive = flag;
				RefreshCamera();
			}
		}

		private void OnPingSpawned(Scp079PingInstance ping)
		{
			if (_isActive)
			{
				RelayerPing(ping);
			}
		}

		private void RefreshCamera()
		{
			if (!_isActive)
			{
				if (_instance != null)
				{
					_instance.enabled = false;
					Object.Destroy(_instance.gameObject);
				}
			}
			else
			{
				_instance = Object.Instantiate(_cam);
				Transform transform = _instance.transform;
				transform.SetParent(CameraShakeController.Singleton.transform);
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
				_instance.enabled = true;
			}
			foreach (Scp079PingInstance instance in Scp079PingInstance.Instances)
			{
				RelayerPing(instance);
			}
		}

		private void RelayerPing(Scp079PingInstance ping)
		{
			int layer = (_isActive ? RippleLayer : 0);
			foreach (SpriteRenderer componentsInChild in ping.GetComponentsInChildren<SpriteRenderer>(includeInactive: true))
			{
				componentsInChild.gameObject.layer = layer;
			}
		}

		private void LateUpdate()
		{
			if (_isActive)
			{
				_instance.fieldOfView = CameraShakeController.Singleton.CamerasFOV;
			}
		}
	}
}
