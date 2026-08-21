using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Interactables;
using Interactables.Interobjects.DoorUtils;
using Mirror;
using PlayerStatsSystem;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp096
{
    public class Scp096HitHandler
    {
        private static readonly Collider[] Hits;

        private static readonly CachedLayerMask SolidObjectMask;

        private static readonly CachedLayerMask AttackHitMask;

        private readonly Scp096TargetsTracker _targetCounter;

        private readonly HashSet<uint> _hitNetIDs;

        private readonly Scp096Role _scpRole;

        private readonly float _windowDamage;

        private readonly float _doorDamage;

        private readonly float _humanTargetDamage;

        private readonly float _humanNontargetDamage;

        private readonly Scp096DamageHandler.AttackType _damageType;

        [CompilerGenerated]
        public event Action<ReferenceHub> OnPlayerHit;

        [CompilerGenerated]
        public event Action<BreakableWindow> OnWindowHit;

        [CompilerGenerated]
        public event Action<IDamageableDoor> OnDoorHit;

        public Scp096HitResult HitResult { get; private set; }

        public Scp096HitHandler(
            Scp096Role scpRole,
            Scp096DamageHandler.AttackType damageType,
            float windowDamage,
            float doorDamage,
            float humanTargetDamage,
            float humanNontargetDamage)
        {
            _scpRole = scpRole;
            _damageType = damageType;
            _windowDamage = windowDamage;
            _doorDamage = doorDamage;
            _humanTargetDamage = humanTargetDamage;
            _humanNontargetDamage = humanNontargetDamage;

            HitResult = Scp096HitResult.None;

            _hitNetIDs = new HashSet<uint>();

            _scpRole.SubroutineModule.TryGetSubroutine(out _targetCounter);
        }

        public void Clear()
        {
            _hitNetIDs.Clear();

            HitResult = Scp096HitResult.None;
        }

        // Listen-server only: the host player's own hitbox colliders are disabled
        // (CharacterModel.SpawnObject -> SetColliders(!isLocalPlayer)), so the physics
        // overlaps below can never detect the host. Briefly re-enable them, same pattern
        // as ExplosionGrenade.Explode / ScpAttackAbilityBase.ServerProcessCmd.
        public Scp096HitResult DamageSphere(Vector3 position, float radius)
        {
            ReferenceHub.TryGetHostHub(out ReferenceHub hostHub);
            bool restoreHostHitboxes = NetworkServer.active && !_scpRole.IsLocalPlayer && HitboxIdentity.SetOwnHitboxes(hostHub, true);

            int count = Physics.OverlapSphereNonAlloc(position, radius, Hits, AttackHitMask);
            Scp096HitResult result = ProcessHits(count);

            if (restoreHostHitboxes)
            {
                HitboxIdentity.SetOwnHitboxes(hostHub, false);
            }
            return result;
        }

        public Scp096HitResult DamageBox(Vector3 position, Vector3 halfExtents, Quaternion orientation)
        {
            ReferenceHub.TryGetHostHub(out ReferenceHub hostHub);
            bool restoreHostHitboxes = NetworkServer.active && !_scpRole.IsLocalPlayer && HitboxIdentity.SetOwnHitboxes(hostHub, true);

            int count = Physics.OverlapBoxNonAlloc(position, halfExtents, Hits, orientation, AttackHitMask);
            Scp096HitResult result = ProcessHits(count);

            if (restoreHostHitboxes)
            {
                HitboxIdentity.SetOwnHitboxes(hostHub, false);
            }
            return result;
        }

        private Scp096HitResult ProcessHits(int count)
        {
            Scp096HitResult result = Scp096HitResult.None;

            for (int i = 0; i < count; i++)
            {
                Collider collider = Hits[i];

                CheckDoorHit(collider);

                if (!collider.TryGetComponent<IDestructible>(out var destructible))
                    continue;

                int layerMask = (int)SolidObjectMask & ~(1 << collider.gameObject.layer);

                if (Physics.Linecast(_scpRole.CameraPosition, destructible.CenterOfMass, layerMask))
                    continue;

                if (!_hitNetIDs.Add(destructible.NetworkId))
                    continue;

                if (destructible is BreakableWindow window)
                {
                    if (DealDamage(window, _windowDamage))
                    {
                        result |= Scp096HitResult.Window;
                        OnWindowHit?.Invoke(window);
                    }
                    continue;
                }

                if (!(destructible is HitboxIdentity hitbox) || !IsHumanHitbox(hitbox))
                    continue;

                ReferenceHub hub = hitbox.TargetHub;

                bool isTarget = _targetCounter.HasTarget(hub);

                float damage = isTarget ? _humanTargetDamage : _humanNontargetDamage;

                if (DealDamage(hitbox, damage))
                {
                    result |= Scp096HitResult.Human;

                    OnPlayerHit?.Invoke(hub);

                    if (!hub.IsAlive())
                        result |= Scp096HitResult.Lethal;
                }
            }

            HitResult |= result;

            return result;
        }

        private bool DealDamage(IDestructible target, float dmg)
        {
            if (dmg <= 0f)
                return false;

            Scp096DamageHandler handler = new Scp096DamageHandler(_scpRole, dmg, _damageType);

            return target.Damage(dmg, handler, _scpRole.FpcModule.Position);
        }

        private void CheckDoorHit(Collider col)
        {
            if (!col.TryGetComponent(out InteractableCollider interactable))
                return;

            if (!(interactable.Target is IDamageableDoor door))
                return;

            if (!(door is NetworkBehaviour netBehaviour))
                return;

            if (!_hitNetIDs.Add(netBehaviour.netId))
                return;

            if (!door.ServerDamage(_doorDamage, DoorDamageType.Scp096))
                return;

            HitResult |= Scp096HitResult.Door;

            OnDoorHit?.Invoke(door);
        }

        private bool IsHumanHitbox(HitboxIdentity hid)
        {
            return hid.TargetHub.roleManager.CurrentRole is HumanRole;
        }

        static Scp096HitHandler()
        {
            Hits = new Collider[32];

            string[] solidLayers = new string[3];
            solidLayers[0] = "Default";
            solidLayers[1] = "Door";
            solidLayers[2] = "Glass";
            SolidObjectMask = new CachedLayerMask(solidLayers);

            string[] attackLayers = new string[3];
            attackLayers[0] = "Hitbox";
            attackLayers[1] = "Door";
            attackLayers[2] = "Glass";
            AttackHitMask = new CachedLayerMask(attackLayers);
        }
    }
}