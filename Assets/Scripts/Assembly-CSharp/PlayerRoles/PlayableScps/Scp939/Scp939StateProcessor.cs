namespace PlayerRoles.PlayableScps.Scp939
{
    public class Scp939StateProcessor : global::PlayerRoles.FirstPersonControl.FpcStateProcessor
    {
        private readonly global::PlayerRoles.PlayableScps.Scp939.Scp939Motor _motor;

        public Scp939StateProcessor(ReferenceHub hub, global::PlayerRoles.PlayableScps.Scp939.Scp939Motor motor, global::PlayerRoles.FirstPersonControl.FirstPersonMovementModule fpmm, float useRate, float respawnImmunity, global::UnityEngine.AnimationCurve regenCurve)
            : base(hub, fpmm, useRate, respawnImmunity, regenCurve)
        {
            _motor = motor;
        }

        public override void ClientUpdateInput(global::PlayerRoles.FirstPersonControl.FirstPersonMovementModule moduleRef, float walkSpeed, out global::PlayerRoles.FirstPersonControl.PlayerMovementState valueToSend)
        {
            base.ClientUpdateInput(moduleRef, walkSpeed, out valueToSend);
            if (valueToSend == global::PlayerRoles.FirstPersonControl.PlayerMovementState.Sprinting && !_motor.MovingForwards)
            {
                valueToSend = global::PlayerRoles.FirstPersonControl.PlayerMovementState.Walking;
            }
        }
    }
}
