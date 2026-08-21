namespace CustomPlayerEffects
{
    public class Ensnared : global::CustomPlayerEffects.StatusEffectBase, global::PlayerRoles.FirstPersonControl.IMovementSpeedModifier
    {
        public bool MovementModifierActive => base.IsEnabled;

        public float MovementSpeedMultiplier => 0f;

        public float MovementSpeedLimit => 0f;
    }
}
