using System.Collections.Generic;

using InventorySystem.Items.Pickups;
using Mirror;
using RelativePositioning;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939.Ripples
{
	public class PickupRippleTrigger : RippleTriggerBase
	{
		private const float MinVelSqr = 8.5f;

		private const float SoundRangeMin = 4f;

		private const float SoundRangeKg = 0.75f;

		private RelativePosition _syncPos;

        private readonly RateLimiter _rateLimiter = new RateLimiter(0f, 8, 0.2f);

        private static bool _anyInstances;

        private static readonly global::System.Collections.Generic.HashSet<global::PlayerRoles.PlayableScps.Scp939.Ripples.PickupRippleTrigger> ActiveInstances = new global::System.Collections.Generic.HashSet<global::PlayerRoles.PlayableScps.Scp939.Ripples.PickupRippleTrigger>();

        public override void SpawnObject()
        {
            base.SpawnObject();
            ActiveInstances.Add(this);
            _anyInstances = true;
        }


        public override void ResetObject()
		{
			base.ResetObject();
			RemoveSelf();
		}

        public override void ServerWriteRpc(global::Mirror.NetworkWriter writer)
        {
            base.ServerWriteRpc(writer);
            global::RelativePositioning.RelativePositionSerialization.WriteRelativePosition(writer, _syncPos);
        }

        public override void ClientProcessRpc(global::Mirror.NetworkReader reader)
        {
            base.ClientProcessRpc(reader);
            if (base.IsLocalOrSpectated)
            {
                base.Player.Play(global::RelativePositioning.RelativePositionSerialization.ReadRelativePosition(reader).Position, global::UnityEngine.Color.red);
            }
        }

        private void OnDestroy()
        {
            RemoveSelf();
        }

        private void RemoveSelf()
        {
            if (ActiveInstances.Remove(this))
            {
                _anyInstances = ActiveInstances.Count > 0;
            }
        }


        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            global::InventorySystem.Items.Pickups.ItemPickupBase.OnPickupAdded += OnPickupAdded;
        }

        private static void OnPickupAdded(global::InventorySystem.Items.Pickups.ItemPickupBase ipb)
        {
            global::InventorySystem.Items.Pickups.CollisionDetectionPickup cdp;
            if ((object)(cdp = ipb as global::InventorySystem.Items.Pickups.CollisionDetectionPickup) != null && global::Mirror.NetworkServer.active)
            {
                cdp.OnCollided += delegate (global::UnityEngine.Collision col)
                {
                    OnCollided(cdp, col);
                };
            }
        }

        private static void OnCollided(global::InventorySystem.Items.Pickups.CollisionDetectionPickup cdp, global::UnityEngine.Collision collision)
        {
            if (!_anyInstances || !global::Mirror.NetworkServer.active)
            {
                return;
            }
            float sqrMagnitude = collision.relativeVelocity.sqrMagnitude;
            if (sqrMagnitude < 8.5f || collision.contactCount == 0)
            {
                return;
            }
            float a = global::UnityEngine.Mathf.Max(4f, cdp.RigidBody.mass * 0.75f);
            a = global::UnityEngine.Mathf.Max(a, cdp.GetRangeOfCollisionVelocity(sqrMagnitude));
            foreach (global::PlayerRoles.PlayableScps.Scp939.Ripples.PickupRippleTrigger prt in ActiveInstances)
            {
                if (!prt._rateLimiter.RateReady)
                {
                    continue;
                }
                global::UnityEngine.Vector3 point = collision.GetContact(0).point;
                if (!((point - prt.ScpRole.FpcModule.Position).sqrMagnitude >= a * a))
                {
                    prt._rateLimiter.RegisterInput();
                    prt._syncPos = new global::RelativePositioning.RelativePosition(point);
                    prt.ServerSendRpc((ReferenceHub x) => x == prt.Owner || global::PlayerRoles.Spectating.SpectatorNetworking.IsSpectatedBy(prt.Owner, x));
                }
            }
        }
	}
}
