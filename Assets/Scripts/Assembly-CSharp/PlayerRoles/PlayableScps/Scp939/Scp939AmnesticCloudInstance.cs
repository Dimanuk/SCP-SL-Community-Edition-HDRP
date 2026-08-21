using System.Collections.Generic;
using CustomPlayerEffects;
using Hazards;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerRoles.Spectating;
using PlayerStatsSystem;
using RelativePositioning;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace PlayerRoles.PlayableScps.Scp939
{
	public class Scp939AmnesticCloudInstance : TemporaryHazard
	{
		private enum CloudState
		{
			Spawning,
			Created,
			Destroyed
		}

		public static readonly List<Scp939AmnesticCloudInstance> ActiveInstances = new List<Scp939AmnesticCloudInstance>();

		private readonly AbilityCooldown _overallCooldown = new AbilityCooldown();

		private readonly Dictionary<uint, AbilityCooldown> _individualCooldown = new Dictionary<uint, AbilityCooldown>();

		private Scp939AmnesticCloudAbility cloud;

		private Scp939LungeAbility _lunge;

		private Scp939Role _scpRole;

		private Transform _t;

		private bool _abilitiesSet;

		private float _targetDuration;

		private float _lastHoldTime;

		private float _prevRange;

		private bool _localOwner;

		private bool _alreadyCreated;

		[SyncVar]
		private byte _syncHoldTime;

		[SyncVar]
		private byte _syncState;

		[SyncVar]
		private uint _syncOwner;

		[SyncVar]
		private RelativePosition _syncPos;

		[Header("Balance")]
		[SerializeField]
		private float _minHoldTime;

		[SerializeField]
		private float _maxHoldTime;

		[SerializeField]
		private AnimationCurve _rangeOverHeldTime;

		[SerializeField]
		private AnimationCurve _durationOverHeldTime;

		[SerializeField]
		private float _amnesiaDuration;

		[SerializeField]
		private float _pauseDuration;

		[Header("Lights")]
		[SerializeField]
		private Light _radiusLight;

		[SerializeField]
		private Light _areaLight;

		[SerializeField]
		private AnimationCurve _radiusToLightAngle;

		[SerializeField]
		private AnimationCurve _radiusToLightRange;

		[Header("Other audiovisual")]
		[SerializeField]
		private float _destroyTime;

		[SerializeField]
		private float _soundDropRate;

		[SerializeField]
		private float _sizeLerpTime;

		[SerializeField]
		private float _colorLerpTime;

		[SerializeField]
		private AudioSource _deploySound;

		[SerializeField]
		private AudioSource _chargeupSound;

		[SerializeField]
		private AnimationCurve _chargeupVolumeOverSize;

		private CloudState State
		{
			get
			{
				return (CloudState)_syncState;
			}
			set
			{
				_syncState = (byte)value;
			}
		}

		private float NormalizedHoldTime => Mathf.Clamp01(cloud.HoldDuration / _maxHoldTime);

		protected override float HazardDuration => _targetDuration;

		protected override float DecaySpeed
		{
			get
			{
				if (State != CloudState.Created)
				{
					return 0f;
				}
				return 1f;
			}
		}

		public Vector2 MinMaxTime => new Vector2(_minHoldTime, _maxHoldTime);

		[Server]
		public override void ServerDestroy()
		{
			base.ServerDestroy();
			_abilitiesSet = false;
			State = CloudState.Destroyed;
		}

		public override void OnStay(ReferenceHub player)
		{
			base.OnStay(player);
			if (State == CloudState.Created && player.roleManager.CurrentRole is HumanRole)
			{
				PlayerEffectsController playerEffectsController = player.playerEffectsController;
				playerEffectsController.EnableEffect<AmnesiaItems>(_amnesiaDuration);
				if (_overallCooldown.IsReady && (!_individualCooldown.TryGetValue(player.netId, out var value) || value.IsReady))
				{
					playerEffectsController.EnableEffect<AmnesiaVision>(_amnesiaDuration);
				}
			}
		}

		protected override void Start()
		{
			_t = base.transform;
			ActiveInstances.Add(this);
			if (ReferenceHub.TryGetHubNetID(_syncOwner, out var hub) && hub.isLocalPlayer)
			{
				_localOwner = true;
				SetAbilityCache();
			}
			base.Start();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			ActiveInstances.Remove(this);
			PlayerStats.OnAnyPlayerDamaged -= OnAnyPlayerDamaged;
			if (_lunge != null)
			{
				_lunge.OnStateChanged -= OnLungeStateChanged;
			}
		}

		protected override void Update()
		{
			base.Update();
			if (_localOwner)
			{
				UpdateLocal();
			}
			else
			{
				UpdateVisuals((float)(int)_syncHoldTime / 255f, Time.deltaTime * _sizeLerpTime);
			}
			if (NetworkServer.active)
			{
				switch (State)
				{
				case CloudState.Spawning:
					ServerUpdateSpawning();
					break;
				case CloudState.Destroyed:
					ServerUpdateDestroyed();
					break;
				}
			}
		}

		private void TryGetPlayer(out bool is939, out bool isOwner)
		{
			is939 = false;
			isOwner = false;
			if (SpectatorTargetTracker.TryGetTrackedPlayer(out var hub) || ReferenceHub.TryGetLocalHub(out hub))
			{
				is939 = hub.roleManager.CurrentRole is Scp939Role;
				isOwner = hub.netId == _syncOwner;
			}
		}

		private void OnAnyPlayerDamaged(ReferenceHub hub, DamageHandlerBase dhb)
		{
			if (dhb is Scp939DamageHandler)
			{
				PauseAll();
			}
			else if (hub.netId == _syncOwner && dhb is AttackerDamageHandler attackerDamageHandler)
			{
				AbilityCooldown abilityCooldown = new AbilityCooldown();
				abilityCooldown.Trigger(_pauseDuration);
				uint attackerId = attackerDamageHandler.Attacker.NetId;
				_individualCooldown[attackerId] = abilityCooldown;
				if (base.AffectedPlayers.TryGetFirst((ReferenceHub x) => x.netId == attackerId, out var first) && first.playerEffectsController.TryGetEffect<AmnesiaVision>(out var playerEffect))
				{
					playerEffect.IsEnabled = false;
				}
			}
		}

		private void OnLungeStateChanged(Scp939LungeState state)
		{
			if (state != Scp939LungeState.None)
			{
				PauseAll();
			}
		}

		private void PauseAll()
		{
			foreach (ReferenceHub affectedPlayer in base.AffectedPlayers)
			{
				if (!affectedPlayer.playerEffectsController.TryGetEffect<AmnesiaVision>(out var playerEffect))
				{
					return;
				}
				playerEffect.IsEnabled = false;
			}
			_overallCooldown.Trigger(_pauseDuration);
		}

		private void SetAbilityCache()
		{
			_abilitiesSet = false;
			if (ReferenceHub.TryGetHubNetID(_syncOwner, out var hub) && hub.roleManager.CurrentRole is Scp939Role scpRole)
			{
				_scpRole = scpRole;
				_abilitiesSet = _scpRole.SubroutineModule.TryGetSubroutine<Scp939AmnesticCloudAbility>(out cloud) && _scpRole.SubroutineModule.TryGetSubroutine<Scp939LungeAbility>(out _lunge);
			}
		}

		private void RefreshPosition(ReferenceHub owner)
		{
			_t.position = owner.PlayerCameraReference.position;
		}

		private void UpdateLocal()
		{
			if (_abilitiesSet && ReferenceHub.TryGetLocalHub(out var hub))
			{
				switch (State)
				{
				case CloudState.Destroyed:
					cloud.ClientCancel(Scp939HudTranslation.CloudFailedSizeInsufficient);
					break;
				case CloudState.Created:
					cloud.ClientCancel(Scp939HudTranslation.PressKeyToLunge);
					break;
				}
				if (!cloud.ValidateFloor())
				{
					cloud.ClientCancel((cloud.HoldDuration < _minHoldTime) ? Scp939HudTranslation.CloudFailedSizeInsufficient : Scp939HudTranslation.PressKeyToLunge);
				}
				if (cloud.TargetState)
				{
					UpdateVisuals(NormalizedHoldTime, 1f);
					RefreshPosition(hub);
				}
				else if (State != CloudState.Spawning)
				{
					_localOwner = false;
				}
			}
		}

		private void UpdateVisuals(float normalizedSize, float lerpTime)
		{
			_deploySound.mute = (SpectatorTargetTracker.TryGetTrackedPlayer(out var hub) || ReferenceHub.TryGetLocalHub(out hub)) && hub.roleManager.CurrentRole is HumanRole;
			TryGetPlayer(out var @is, out var isOwner);
			_t.position = _syncPos.Position;
			float time = normalizedSize * _maxHoldTime;
			_prevRange = Mathf.Lerp(_prevRange, _rangeOverHeldTime.Evaluate(time), lerpTime);
			_areaLight.enabled = @is;
			_radiusLight.enabled = @is;
			SetLightRadius(_areaLight, _prevRange);
			SetLightRadius(_radiusLight, _prevRange);
			_chargeupSound.mute = !isOwner;
			switch (State)
			{
			case CloudState.Spawning:
				LerpLight(_radiusLight, Color.white);
				_chargeupSound.volume = _chargeupVolumeOverSize.Evaluate(normalizedSize);
				return;
			case CloudState.Destroyed:
				LerpLight(_radiusLight, Color.black);
				LerpLight(_areaLight, Color.black);
				break;
			case CloudState.Created:
				LerpLight(_radiusLight, Color.white);
				LerpLight(_areaLight, Color.white);
				break;
			}
			_chargeupSound.volume -= Time.deltaTime;
		}

		private void LerpLight(Light l, Color c)
		{
			l.color = Color.Lerp(l.color, c, Time.deltaTime * _colorLerpTime);
		}

		private void SetLightRadius(Light l, float range)
		{
			l.spotAngle = _radiusToLightAngle.Evaluate(range);
			l.range = _radiusToLightRange.Evaluate(range);
		}

		[Server]
		private void ServerUpdateSpawning()
		{
			if (!_abilitiesSet || !ReferenceHub.TryGetHubNetID(_syncOwner, out var hub) || _scpRole == null || _scpRole.Pooled)
			{
				ServerDestroy();
				return;
			}
			RefreshPosition(hub);
			_syncPos = new RelativePosition(_t.position);
			if (cloud.TargetState)
			{
				_lastHoldTime = cloud.HoldDuration;
				_syncHoldTime = (byte)Mathf.RoundToInt(NormalizedHoldTime * 255f);
				if (_lastHoldTime < _maxHoldTime)
				{
					return;
				}
			}
			if (_lastHoldTime < _minHoldTime)
			{
				cloud.ServerFailPlacement();
				ServerDestroy();
				return;
			}
			_targetDuration = _durationOverHeldTime.Evaluate(_lastHoldTime);
			cloud.ServerConfirmPlacement(_targetDuration);
			MaxDistance = _rangeOverHeldTime.Evaluate(_lastHoldTime);
			State = CloudState.Created;
			RpcPlayCreateSound();
		}

		[Server]
		private void ServerUpdateDestroyed()
		{
			_destroyTime -= Time.deltaTime;
			if (!(_destroyTime > 0f))
			{
				NetworkServer.Destroy(base.gameObject);
			}
		}

		[ClientRpc]
		private void RpcPlayCreateSound()
		{
			if (!_alreadyCreated)
			{
				_deploySound.Play();
				if (ReferenceHub.TryGetHubNetID(_syncOwner, out var hub) && hub.roleManager.CurrentRole is Scp939Role scp939Role && scp939Role.FpcModule.CharacterModelInstance is Scp939Model scp939Model)
				{
					_alreadyCreated = true;
					scp939Model.PlayCloudRelease();
				}
			}
		}

		[Server]
		public void ServerSetup(ReferenceHub owner)
		{
			_syncOwner = owner.netId;
			SetAbilityCache();
			_lunge.OnStateChanged += OnLungeStateChanged;
			PlayerStats.OnAnyPlayerDamaged += OnAnyPlayerDamaged;
		}
	}
}
