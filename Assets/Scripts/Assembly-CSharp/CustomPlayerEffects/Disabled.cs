namespace CustomPlayerEffects
{
    public class Disabled : global::CustomPlayerEffects.StatusEffectBase, global::PlayerRoles.FirstPersonControl.IMovementSpeedModifier
    {
        public float SpeedMultiplier = 0.8f;

        public override bool AllowEnabling => !global::CustomPlayerEffects.Vitality.CheckPlayer(base.Hub);

        public bool MovementModifierActive => base.IsEnabled;

        public float MovementSpeedMultiplier => (SpeedMultiplier - 1f) * global::CustomPlayerEffects.RainbowTaste.CurrentMultiplier(base.Hub) + 1f;

        public float MovementSpeedLimit => float.MaxValue;
    }
}
