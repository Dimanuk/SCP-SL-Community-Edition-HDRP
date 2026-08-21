using System;
using System.Collections.Generic;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.HumeShield;
using PlayerRoles.PlayableScps.Scp049;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace PlayerRoles.PlayableScps.Scp049.Zombies
{
    public class ZombieShieldController : DynamicHumeShieldController
    {
        [SerializeField]
        private float _maxShield;
        [SerializeField]
        private float _maxActivateDistanceSqr;
        private static readonly HashSet<Scp049CallAbility> CallSubroutines = new HashSet<Scp049CallAbility>();

        private FirstPersonMovementModule _fpc;
        public override float HsMax => _maxShield;
        public override float HsRegeneration
        {
            get
            {
                if (!HashsetExtensions.Any(CallSubroutines, x => x.IsMarkerShown && CheckDistanceTo(x.ScpRole)))
                    return 0f;

                return base.HsRegeneration;
            }
        }

        public override void SpawnObject()
        {
            base.SpawnObject();
            _fpc = (base.Owner.roleManager.CurrentRole as IFpcRole).FpcModule;
        }

        private bool CheckDistanceTo(Scp049Role role)
        {
            return (role.FpcModule.Position - _fpc.Position).sqrMagnitude <= _maxActivateDistanceSqr;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += CallSubroutines.Clear;
            PlayerRoleManager.OnRoleChanged += (hub, oldRole, newRole) =>
            {
                if (!NetworkServer.active)
                    return;

                if (TryGetCallSubroutine(oldRole, out var sr))
                    CallSubroutines.Remove(sr);

                if (TryGetCallSubroutine(newRole, out var sr2))
                    CallSubroutines.Add(sr2);
            };
        }

        private static bool TryGetCallSubroutine(PlayerRoleBase prb, out Scp049CallAbility sr)
        {
            if (prb is Scp049Role scp049Role)
                return scp049Role.SubroutineModule.TryGetSubroutine(out sr);

            sr = null;
            return false;
        }
    }
}