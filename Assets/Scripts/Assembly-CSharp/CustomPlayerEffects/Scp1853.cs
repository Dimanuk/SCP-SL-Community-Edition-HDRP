namespace CustomPlayerEffects
{
    public class Scp1853 : global::CustomPlayerEffects.StatusEffectBase, global::CustomPlayerEffects.ISpectatorDataPlayerEffect, global::CustomPlayerEffects.IHealablePlayerEffect, global::PlayerRoles.FirstPersonControl.IStaminaModifier, global::CustomPlayerEffects.IWeaponModifierPlayerEffect, global::InventorySystem.Searching.ISearchTimeModifier, global::RemoteAdmin.Interfaces.ICustomRADisplay, global::CustomPlayerEffects.IUsableItemModifierEffect
    {
        private const float SprintStaminaUsage = 1f;

        private const float SearchSpeed = 1.6f;

        private const float EquipAndUse = 1.2f;

        private static readonly global::InventorySystem.Items.Firearms.Attachments.AttachmentParameterValuePair[] WeaponModifiers = new global::InventorySystem.Items.Firearms.Attachments.AttachmentParameterValuePair[5]
        {
            new global::InventorySystem.Items.Firearms.Attachments.AttachmentParameterValuePair(global::InventorySystem.Items.Firearms.Attachments.AttachmentParam.OverallRecoilMultiplier, 0.75f),
            new global::InventorySystem.Items.Firearms.Attachments.AttachmentParameterValuePair(global::InventorySystem.Items.Firearms.Attachments.AttachmentParam.AdsInaccuracyMultiplier, 0.7f),
            new global::InventorySystem.Items.Firearms.Attachments.AttachmentParameterValuePair(global::InventorySystem.Items.Firearms.Attachments.AttachmentParam.ReloadSpeedMultiplier, 1.25f),
            new global::InventorySystem.Items.Firearms.Attachments.AttachmentParameterValuePair(global::InventorySystem.Items.Firearms.Attachments.AttachmentParam.DrawSpeedMultiplier, 1.2f),
            new global::InventorySystem.Items.Firearms.Attachments.AttachmentParameterValuePair(global::InventorySystem.Items.Firearms.Attachments.AttachmentParam.AdsSpeedMultiplier, 1.2f)
        };

        private static readonly ItemType[] AffectedItems = new ItemType[7]
        {
            ItemType.SCP018,
            ItemType.SCP500,
            ItemType.GrenadeFlash,
            ItemType.GrenadeHE,
            ItemType.Medkit,
            ItemType.Painkillers,
            ItemType.Adrenaline
        };

        private global::CustomPlayerEffects.Scp207 _scp207Reference;

        private float _staminaUsageMultiplier;

        private float _searchSpeedMultiplier;

        private global::System.Collections.Generic.Dictionary<global::InventorySystem.Items.Firearms.Attachments.AttachmentParam, float> _processedParams = new global::System.Collections.Generic.Dictionary<global::InventorySystem.Items.Firearms.Attachments.AttachmentParam, float>();

        public bool ParamsActive => base.IsEnabled;

        public bool StaminaModifierActive => base.IsEnabled;

        public float StaminaRegenMultiplier => 1f;

        public float StaminaUsageMultiplier => ProcessValue(_staminaUsageMultiplier, global::CustomPlayerEffects.RainbowTaste.CurrentMultiplier(base.Hub));

        public float ItemSpeedModifier { get; private set; }

        public override global::CustomPlayerEffects.StatusEffectBase.EffectClassification Classification => global::CustomPlayerEffects.StatusEffectBase.EffectClassification.Positive;

        public string DisplayName => "SCP-1853";

        public bool CanBeDisplayed => true;

        public bool SprintingDisabled => false;

        protected override void OnAwake()
        {
            _scp207Reference = base.Hub.playerEffectsController.GetEffect<global::CustomPlayerEffects.Scp207>();
        }

        protected override void OnEffectUpdate()
        {
            if (base.IsEnabled && _scp207Reference.IsEnabled)
            {
                base.Hub.playerEffectsController.EnableEffect<global::CustomPlayerEffects.Poisoned>();
            }
        }

        protected override void IntensityChanged(byte prevState, byte newState)
        {
            float num = 0f;
            float num2 = 1f;
            for (int i = 0; i < newState; i++)
            {
                num += num2;
                num2 /= 2f;
            }
            global::InventorySystem.Items.Firearms.Attachments.AttachmentParameterValuePair[] weaponModifiers = WeaponModifiers;
            for (int j = 0; j < weaponModifiers.Length; j++)
            {
                global::InventorySystem.Items.Firearms.Attachments.AttachmentParameterValuePair attachmentParameterValuePair = weaponModifiers[j];
                _processedParams[attachmentParameterValuePair.Parameter] = ProcessValue(attachmentParameterValuePair.Value, num);
            }
            _staminaUsageMultiplier = ProcessValue(1f, num);
            _searchSpeedMultiplier = ProcessValue(1.6f, num);
            ItemSpeedModifier = ProcessValue(1.2f, num);
        }

        public bool IsHealable(ItemType it)
        {
            return it == ItemType.SCP500;
        }

        public bool GetSpectatorText(out string s)
        {
            s = ((base.Intensity > 1) ? $"SCP-1853 (x{base.Intensity})" : "SCP-1853");
            return true;
        }

        public bool TryGetSpeed(ItemType item, out float speed)
        {
            speed = ItemSpeedModifier;
            if (base.IsEnabled)
            {
                return AffectedItems.Contains(item);
            }
            return false;
        }

        public float ProcessSearchTime(float val)
        {
            return val / global::UnityEngine.Mathf.Max(_searchSpeedMultiplier, 1f);
        }

        private float ProcessValue(float baseValue, float multiplier)
        {
            return 1f + (baseValue - 1f) * multiplier;
        }

        public bool TryGetWeaponParam(global::InventorySystem.Items.Firearms.Attachments.AttachmentParam param, out float val)
        {
            return _processedParams.TryGetValue(param, out val);
        }
    }
}
