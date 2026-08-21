using System.Collections.Generic;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939.Ripples
{
    public class GrenadeRippleTrigger : RippleTriggerBase
    {
        private class ThrownGrenadeHandler
        {
            private readonly GrenadeRippleTrigger _tr;
            private readonly float _startTime;
            private int _nextTime;

            public ThrownGrenadeHandler(GrenadeRippleTrigger trigger)
            {
                _tr = trigger;
                _startTime = Time.timeSinceLevelLoad;
            }

            public bool UpdateSound()
            {
                if (_nextTime >= _tr._rippleTimes.Length)
                    return false;

                float elapsed = Time.timeSinceLevelLoad - _startTime;

                if (_tr._rippleTimes[_nextTime] > elapsed)
                    return false;

                _nextTime++;

                while (UpdateSound()) { }

                return true;
            }
        }

        [SerializeField]
        private float[] _rippleTimes;

        [SerializeField]
        private float _audibleRangeSqr;

        private readonly Dictionary<ThrownProjectile, ThrownGrenadeHandler> _trackedGrenades = new Dictionary<ThrownProjectile, ThrownGrenadeHandler>();

        public override void SpawnObject()
        {
            base.SpawnObject();
            ThrownProjectile.OnProjectileSpawned += OnProjectileSpawned;
            ItemPickupBase.OnPickupDestroyed += OnPickupDestroyed;
        }
        public override void ResetObject()
        {
            base.ResetObject();
            ThrownProjectile.OnProjectileSpawned -= OnProjectileSpawned;
            ItemPickupBase.OnPickupDestroyed -= OnPickupDestroyed;
        }

        private void OnProjectileSpawned(ThrownProjectile tp)
        {
            if (tp == null || tp.Info.ItemId != ItemType.GrenadeHE)
                return;

            _trackedGrenades.Add(tp, new ThrownGrenadeHandler(this));
        }

        private void OnPickupDestroyed(ItemPickupBase ipb)
        {
            if (ipb is not ThrownProjectile thrownProjectile)
                return;

            _trackedGrenades.Remove(thrownProjectile);
        }

        private void Update()
        {
            if (!IsLocalOrSpectated)
                return;

            List<ThrownProjectile> toRemove = null;

            foreach (var kvp in _trackedGrenades)
            {
                if (kvp.Key == null)
                {
                    (toRemove ??= new List<ThrownProjectile>()).Add(kvp.Key);
                    continue;
                }

                if (kvp.Value.UpdateSound())
                {
                    PlayInRangeSqr(kvp.Key.RigidBody.position, _audibleRangeSqr, Color.red);
                }
            }

            if (toRemove != null)
            {
                foreach (var key in toRemove)
                    _trackedGrenades.Remove(key);
            }
        }
    }
}