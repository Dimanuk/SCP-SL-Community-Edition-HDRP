using System;
using System.Collections.Generic;
using Interactables;
using Interactables.Interobjects;
using Mirror;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerStatsSystem;
using UnityEngine;
using CustomPlayerEffects;

namespace PlayerRoles.PlayableScps.Scp096
{
	public class Scp096ChargeAbility : ScpKeySubroutine<Scp096Role>
	{
		private static readonly Collider[] DoorDetections = new Collider[8];
		private static readonly CachedLayerMask ClientsideDoorDetectorMask = new CachedLayerMask("Door");
		private static readonly HashSet<Collider> DisabledColliders = new HashSet<Collider>();

		public const float DefaultChargeCooldown = 5f;
		private const float DefaultChargeDuration = 1f;
		private const float DamageObjects = 750f;
		private const float DamageTarget = 90f;
		private const float DamageNonTarget = 45f;
		private const float ConcussionDurationTargets = 10f;
		private const float ConcussionDurationNonTargets = 4f;

		private Scp096HitHandler _hitHandler;
		private Scp096TargetsTracker _targetsTracker;
		private Scp096AudioPlayer _audioPlayer;
		private Transform _tr;

		[SerializeField]
		private Vector3 _detectionOffset;

		[SerializeField]
		private Vector3 _detectionExtents;

		[SerializeField]
		private AudioClip[] _soundsLethal;

		[SerializeField]
		private AudioClip[] _soundsNonLethal;

		[SerializeField]
		private float _soundDistance;

		public readonly AbilityCooldown Cooldown = new AbilityCooldown();
		public readonly AbilityCooldown Duration = new AbilityCooldown();

		public bool CanCharge
		{
			get
			{
				if (base.ScpRole.IsRageState(Scp096RageState.Enraged) && base.ScpRole.IsAbilityState(Scp096AbilityState.None))
				{
					return Cooldown.IsReady;
				}
				return false;
			}
		}

		protected override ActionName TargetKey => ActionName.Zoom;

		public override void ClientProcessRpc(NetworkReader reader)
		{
			base.ClientProcessRpc(reader);
			Cooldown.ReadCooldown(reader);
			Duration.ReadCooldown(reader);
		}

		public override void ServerWriteRpc(NetworkWriter writer)
		{
			base.ServerWriteRpc(writer);
			Cooldown.WriteCooldown(writer);
			Duration.WriteCooldown(writer);
		}

		public override void ServerProcessCmd(NetworkReader reader)
		{
			base.ServerProcessCmd(reader);
			if (!CanCharge)
			{
				return;
			}
			base.Role.TryGetOwner(out ReferenceHub hub);

			_hitHandler.Clear();
			Duration.Trigger(DefaultChargeDuration);
			base.ScpRole.StateController.SetAbilityState(Scp096AbilityState.Charging);
			ServerSendRpc(toAll: true);
		}

		public override void SpawnObject()
		{
			base.SpawnObject();
			_hitHandler = new Scp096HitHandler(base.ScpRole, Scp096DamageHandler.AttackType.Charge, DamageObjects, DamageObjects, DamageTarget, DamageNonTarget);
			_hitHandler.OnPlayerHit += ply =>
			{
				ply.playerEffectsController.EnableEffect<Concussed>(_targetsTracker.HasTarget(ply) ? ConcussionDurationTargets : ConcussionDurationNonTargets);
			};
		}

		protected override void OnKeyDown()
		{
			base.OnKeyDown();
			ClientSendCmd();
		}

		protected override void Awake()
		{
			base.Awake();
			_tr = base.transform;
			GetSubroutine<Scp096AudioPlayer>(out _audioPlayer);
			GetSubroutine<Scp096TargetsTracker>(out _targetsTracker);
			base.ScpRole.StateController.OnAbilityUpdate += _ =>
			{
				foreach (Collider disabledCollider in DisabledColliders)
				{
					if (!(disabledCollider == null))
					{
						disabledCollider.enabled = true;
					}
				}
				DisabledColliders.Clear();
			};
		}

		protected override void Update()
		{
			base.Update();
			if (!base.ScpRole.IsAbilityState(Scp096AbilityState.Charging))
			{
				return;
			}
			if (NetworkServer.active)
			{
				UpdateServer();
			}
			if (base.Role.IsLocalPlayer)
			{
				UpdateLocalClient();
			}
		}

		private void UpdateServer()
		{
			if (Duration.IsReady || !base.ScpRole.IsRageState(Scp096RageState.Enraged))
			{
				base.ScpRole.ResetAbilityState();
				Cooldown.Trigger(DefaultChargeCooldown);
				ServerSendRpc(toAll: true);
				return;
			}
			Scp096HitResult scp096HitResult = _hitHandler.DamageBox(_tr.TransformPoint(_detectionOffset), _detectionExtents, _tr.rotation);
			if (scp096HitResult != Scp096HitResult.None)
			{
				Hitmarker.SendHitmarker(base.Owner, 1f);
				_audioPlayer.ServerPlayAttack(scp096HitResult);
			}
		}

		private void UpdateLocalClient()
		{
			int num = Physics.OverlapBoxNonAlloc(_tr.TransformPoint(_detectionOffset), _detectionExtents, DoorDetections, _tr.rotation, ClientsideDoorDetectorMask);
			for (int i = 0; i < num; i++)
			{
				if (DoorDetections[i].TryGetComponent<InteractableCollider>(out InteractableCollider component))
				{
					CheckDoor(component.Target as IInteractable);
				}
			}
		}

		private void CheckDoor(IInteractable inter)
		{
			if (inter == null)
			{
				return;
			}
			if (inter is BreakableDoor breakableDoor)
			{
				foreach (Collider scp106Collider in breakableDoor.Scp106Colliders)
				{
					if (scp106Collider.enabled)
					{
						scp106Collider.enabled = false;
						DisabledColliders.Add(scp106Collider);
					}
				}
				return;
			}
			if (inter is PryableDoor pryableDoor)
			{
				GetSubroutine<Scp096PrygateAbility>(out Scp096PrygateAbility sr);
				sr.ClientTryPry(pryableDoor);
			}
		}
	}
}