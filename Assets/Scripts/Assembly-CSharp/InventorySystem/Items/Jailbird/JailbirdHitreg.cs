using System;
using System.Collections.Generic;

using Mirror;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace InventorySystem.Items.Jailbird
{
	[Serializable]
	public class JailbirdHitreg
	{
		private const int MaxDetections = 128;

        private static readonly global::UnityEngine.Collider[] DetectedColliders = new global::UnityEngine.Collider[128];

        private static readonly IDestructible[] DetectedDestructibles = new IDestructible[128];

        private static readonly CachedLayerMask DetectionMask = new CachedLayerMask("Hitbox", "Glass");

        private static readonly CachedLayerMask LinecastMask = new CachedLayerMask("Default");

        private static readonly global::System.Collections.Generic.HashSet<uint> DetectedNetIds = new global::System.Collections.Generic.HashSet<uint>();

        private static readonly global::System.Collections.Generic.HashSet<global::PlayerRoles.FirstPersonControl.FpcBacktracker> BacktrackedPlayers = new global::System.Collections.Generic.HashSet<global::PlayerRoles.FirstPersonControl.FpcBacktracker>();

        private static int _detectionsLen;

		[SerializeField]
		private float _hitregOffset;

		[SerializeField]
		private float _hitregRadius;

		[SerializeField]
		private float _damageMelee;

		[SerializeField]
		private float _damageCharge;

		[SerializeField]
		private float _flashDuration;

		private JailbirdItem _item;

		public float TotalMeleeDamageDealt { get; internal set; }

        public bool AnyDetected
        {
            get
            {
                DetectDestructibles();
                return _detectionsLen > 0;
            }
        }

        public void Setup(global::InventorySystem.Items.Jailbird.JailbirdItem target)
        {
            _item = target;
        }

        public bool ClientTryAttack()
        {
            if (!(_item.Owner.roleManager.CurrentRole is global::PlayerRoles.FirstPersonControl.IFpcRole fpcRole))
            {
                return false;
            }
            global::Mirror.NetworkWriter writer;
            using (new global::InventorySystem.Items.Autosync.AutosyncCmd(_item, out writer))
            {
                writer.WriteByte(4);
                global::RelativePositioning.RelativePositionSerialization.WriteRelativePosition(writer, new global::RelativePositioning.RelativePosition(fpcRole.FpcModule.Position));
                global::Mirror.NetworkWriterExtensions.WriteQuaternion(writer, _item.Owner.PlayerCameraReference.rotation);
                DetectDestructibles();
                for (int i = 0; i < _detectionsLen; i++)
                {
                    if (DetectedDestructibles[i] is HitboxIdentity hitboxIdentity)
                    {
                        ReferenceHub targetHub = hitboxIdentity.TargetHub;
                        global::Utils.Networking.ReferenceHubReaderWriter.WriteReferenceHub(writer, targetHub);
                        global::RelativePositioning.RelativePositionSerialization.WriteRelativePosition(writer, new global::RelativePositioning.RelativePosition(targetHub));
                    }
                }
            }
            return true;
        }

        public bool ServerAttack(bool isCharging, global::Mirror.NetworkReader reader)
        {
            ReferenceHub owner = _item.Owner;
            bool result = false;
            if (reader != null)
            {
                global::RelativePositioning.RelativePosition relativePosition = global::RelativePositioning.RelativePositionSerialization.ReadRelativePosition(reader);
                global::UnityEngine.Quaternion claimedRot = global::Mirror.NetworkReaderExtensions.ReadQuaternion(reader);
                BacktrackedPlayers.Add(new global::PlayerRoles.FirstPersonControl.FpcBacktracker(owner, relativePosition.Position, claimedRot));
                while (reader.Position < reader.Capacity)
                {
                    bool num = global::Utils.Networking.ReferenceHubReaderWriter.TryReadReferenceHub(reader, out ReferenceHub hub);
                    global::UnityEngine.Vector3 position = global::RelativePositioning.RelativePositionSerialization.ReadRelativePosition(reader).Position;
                    if (num)
                    {
                        BacktrackedPlayers.Add(new global::PlayerRoles.FirstPersonControl.FpcBacktracker(hub, position));
                    }
                }
            }
            DetectDestructibles();
            global::UnityEngine.Vector3 forward = _item.Owner.PlayerCameraReference.forward;
            float num2 = (isCharging ? _damageCharge : _damageMelee);
            for (int i = 0; i < _detectionsLen; i++)
            {
                IDestructible destructible = DetectedDestructibles[i];
                bool damaged = destructible.Damage(num2, new global::PlayerStatsSystem.JailbirdDamageHandler(owner, num2, forward), destructible.CenterOfMass);
                if (damaged)
                {
                    result = true;
                    if (!isCharging)
                    {
                        TotalMeleeDamageDealt += num2;
                    }
                    else if (destructible is HitboxIdentity hitboxIdentity)
                    {
                        hitboxIdentity.TargetHub.playerEffectsController.EnableEffect<global::CustomPlayerEffects.Flashed>(_flashDuration, addDuration: true);
                    }
                }
            }
            global::Utils.NonAllocLINQ.HashsetExtensions.ForEach(BacktrackedPlayers, delegate (global::PlayerRoles.FirstPersonControl.FpcBacktracker x)
            {
                x.RestorePosition();
            });
            BacktrackedPlayers.Clear();
            return result;
        }

        private void DetectDestructibles()
        {
            global::UnityEngine.Transform playerCameraReference = _item.Owner.PlayerCameraReference;
            global::UnityEngine.Vector3 position = playerCameraReference.position + playerCameraReference.forward * _hitregOffset;
            _detectionsLen = 0;

            // Listen-server only: the host player's own hitbox colliders are disabled
            // (CharacterModel.SpawnObject -> SetColliders(!isLocalPlayer)), so the overlap
            // sphere below can never detect the host. Briefly re-enable them, same pattern
            // as ExplosionGrenade.Explode / ScpAttackAbilityBase.ServerProcessCmd. Skipped
            // when the host itself is swinging the jailbird (victim is remote then).
            ReferenceHub.TryGetHostHub(out ReferenceHub hostHub);
            bool restoreHostHitboxes = NetworkServer.active && !_item.Owner.isLocalPlayer && HitboxIdentity.SetOwnHitboxes(hostHub, true);

            int num = global::UnityEngine.Physics.OverlapSphereNonAlloc(position, _hitregRadius, DetectedColliders, DetectionMask);
            if (num != 0)
            {
                DetectedNetIds.Clear();
                for (int i = 0; i < num; i++)
                {
                    if (DetectedColliders[i].TryGetComponent<IDestructible>(out var component) && (!global::UnityEngine.Physics.Linecast(playerCameraReference.position, component.CenterOfMass, out var hitInfo, LinecastMask) || !(hitInfo.collider != DetectedColliders[i])) && DetectedNetIds.Add(component.NetworkId))
                    {
                        DetectedDestructibles[_detectionsLen++] = component;
                    }
                }
            }

            if (restoreHostHitboxes)
            {
                HitboxIdentity.SetOwnHitboxes(hostHub, false);
            }
        }
	}
}
