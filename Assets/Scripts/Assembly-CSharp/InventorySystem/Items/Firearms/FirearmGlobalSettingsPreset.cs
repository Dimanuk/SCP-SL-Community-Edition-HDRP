namespace InventorySystem.Items.Firearms
{
    [global::UnityEngine.CreateAssetMenu(fileName = "New Global Settings Preset", menuName = "ScriptableObject/Firearms/Global Settings Preset")]
    public class FirearmGlobalSettingsPreset : global::UnityEngine.ScriptableObject
    {
        public float OverallRunningInaccuracyMultiplier;

        public float AbsoluteJumpInaccuracy;

        public global::UnityEngine.AnimationCurve AdsMovementSpeedCurve;

        public float MaxWeaponMovementSpeed;

        public global::UnityEngine.AnimationCurve AdsAnimationCurve;

        public global::UnityEngine.AnimationCurve MovementSpeedToRunningInaccuracy;

        public global::UnityEngine.AnimationCurve RunningInaccuracyCurve;

        public global::UnityEngine.AnimationCurve WeightToStaminaUsage;

        public global::UnityEngine.AnimationCurve WeightToMovementSpeed;
    }
}
