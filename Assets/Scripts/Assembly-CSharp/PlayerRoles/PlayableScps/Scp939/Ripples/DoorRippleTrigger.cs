using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using PlayerRoles.Spectating;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939.Ripples
{
    public class DoorRippleTrigger : RippleTriggerBase
    {
        private static readonly Vector3 PosOffset = Vector3.up * 1.25f;
        public override void SpawnObject()
        {
            base.SpawnObject();
            DoorEvents.OnDoorAction += OnDoorAction;
        }

        public override void ResetObject()
        {
            base.ResetObject();
            DoorEvents.OnDoorAction -= OnDoorAction;
        }

        private void OnDoorAction(DoorVariant dv, DoorAction da, ReferenceHub hub)
        {
            if (da > DoorAction.Opened)
                return;

            if (!base.IsLocalOrSpectated)
                return;

            if (!(dv is BasicDoor basicDoor))
                return;

            Vector3 pos = dv.transform.position + PosOffset;

            float range = basicDoor.MainSource.maxDistance;

            PlayInRangeSqr(pos, range * range, Color.red);
        }
    }
}