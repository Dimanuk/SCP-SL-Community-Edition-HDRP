namespace InventorySystem.Items.Firearms
{
    [global::System.Serializable]
    public struct FirearmBaseStats
    {
        public float BaseDamage;

        [global::UnityEngine.Range(0f, 100f)]
        public int BasePenetrationPercent;

        public float FullDamageDistance;

        public float DamageFalloff;

        public float BulletInaccuracy;

        public float HipInaccuracy;

        public float AdsInaccuracy;

        public float BaseDrawTime;

        public float MaxDistance()
        {
            return FullDamageDistance + 100f / DamageFalloff;
        }

        public float DamageAtDistance(global::InventorySystem.Items.Firearms.Firearm firearm, float dis)
        {
            if (dis >= MaxDistance())
            {
                return 0f;
            }
            float num = BaseDamage * global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.AttachmentsValue(firearm, global::InventorySystem.Items.Firearms.Attachments.AttachmentParam.DamageMultiplier);
            if (dis > FullDamageDistance)
            {
                float num2 = 100f - DamageFalloff * (dis - FullDamageDistance);
                num *= num2 / 100f;
            }
            return num;
        }

        public float GetInaccuracy(global::InventorySystem.Items.Firearms.Firearm firearm, bool isAds, float movementSpeed, bool isGrounded)
        {
            if (!isGrounded)
            {
                movementSpeed = firearm.GlobalSettingsPreset.MaxWeaponMovementSpeed;
            }
            float num = (isAds ? global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.AttachmentsValue(firearm, global::InventorySystem.Items.Firearms.Attachments.AttachmentParam.AdsInaccuracyMultiplier) : global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.AttachmentsValue(firearm, global::InventorySystem.Items.Firearms.Attachments.AttachmentParam.HipInaccuracyMultiplier));
            float num2 = (isAds ? (AdsInaccuracy * num) : (HipInaccuracy * num));
            num2 += num * firearm.GlobalSettingsPreset.MovementSpeedToRunningInaccuracy.Evaluate(movementSpeed) * firearm.GlobalSettingsPreset.RunningInaccuracyCurve.Evaluate(firearm.Length * firearm.Weight) * firearm.GlobalSettingsPreset.OverallRunningInaccuracyMultiplier;
            if (!isGrounded)
            {
                num2 += firearm.GlobalSettingsPreset.AbsoluteJumpInaccuracy;
            }
            return num2 + BulletInaccuracy * global::InventorySystem.Items.Firearms.Attachments.AttachmentsUtils.AttachmentsValue(firearm, global::InventorySystem.Items.Firearms.Attachments.AttachmentParam.BulletInaccuracyMultiplier);
        }
    }
}
