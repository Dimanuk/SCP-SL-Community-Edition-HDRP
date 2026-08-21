using System.Diagnostics;
using Interactables.Interobjects.DoorUtils;
using Mirror;
using PlayerRoles;
using UnityEngine;

namespace Interactables.Interobjects
{
    public class Timed173PryableDoor : PryableDoor
    {
        private readonly Stopwatch _stopwatch = new();

        [SerializeField]
        private float _timeMark = 25f;

        [Tooltip("Automatically opens the gate when the time is over and SCP-173 is spawned.")]
        [SerializeField]
        private bool _smartOpen = true;

        protected override void Start()
        {
            base.Start();
            if (NetworkServer.active)
            {
                CharacterClassManager.OnRoundStarted += _stopwatch.Start;
                ServerChangeLock(DoorLockReason.SpecialDoorFeature, newState: true);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (NetworkServer.active)
            {
                CharacterClassManager.OnRoundStarted -= _stopwatch.Start;
            }
        }

        protected override void Update()
        {
            base.Update();
            if (!_stopwatch.IsRunning || _stopwatch.Elapsed.TotalSeconds < (double)_timeMark)
            {
                return;
            }
            _stopwatch.Stop();
            ServerChangeLock(DoorLockReason.SpecialDoorFeature, newState: false);
            if (!_smartOpen)
            {
                return;
            }
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.GetRoleId() == RoleTypeId.Scp173)
                {
                    base.TargetState = true;
                    break;
                }
            }
        }
    }
}
