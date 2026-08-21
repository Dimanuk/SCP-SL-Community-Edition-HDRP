using System;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Attachments.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.GUI.Descriptions
{
    public class FirearmDescriptionGui : RadialDescriptionBase
    {
        [Serializable]
        public class FirearmStatBar
        {
            [SerializeField] private Image _targetImage;
            [SerializeField] private TextMeshProUGUI _valueText;
            [SerializeField] private TextMeshProUGUI _headerText;
            [SerializeField] private InventoryGuiTranslation _headerTranslation;

            private float _defaultWidth;
            private float _defaultHeight;
            private bool _defaultsSet;

            public void SetValue(float normalizedValue, string text, Color col)
            {
                if (!_defaultsSet)
                {
                    string header;
                    if (TranslationReader.TryGet("InventoryGUI", (int)_headerTranslation, out header))
                    {
                        _headerText.text = header;
                    }

                    _defaultHeight = _targetImage.rectTransform.sizeDelta.y;
                    _defaultWidth = _targetImage.rectTransform.sizeDelta.x;
                    _defaultsSet = true;
                }

                _valueText.text = text;
                _targetImage.color = col;

                float finalValue = Mathf.Clamp01(normalizedValue);
                _targetImage.rectTransform.sizeDelta = new Vector2(_defaultWidth * finalValue, _defaultHeight);
            }
        }

        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _ammoText;
        [SerializeField] private FirearmStatBar _damageBar;
        [SerializeField] private FirearmStatBar _firerateBar;
        [SerializeField] private FirearmStatBar _penetrationBar;
        [SerializeField] private FirearmStatBar _hipAccBar;
        [SerializeField] private FirearmStatBar _adsAccBar;
        [SerializeField] private FirearmStatBar _runAccBar;
        [SerializeField] private FirearmStatBar _staminaUsageBar;
        [SerializeField] private FirearmStatBar _movementSpeedBar;
        [SerializeField] private AnimationCurve _accuracyToValue;
        [SerializeField] private RawImage _bodyImage;
        [SerializeField] private RawImage[] _attachmentsPool;
        [SerializeField] private RectTransform _attachmentIconsBorders;

        public override void UpdateInfo(ItemBase targetItem, Color roleColor)
        {
            Firearm firearm = targetItem as Firearm;
            if (firearm == null) return;

            _title.text = firearm.Name;

            float damage = firearm.BaseStats.DamageAtDistance(firearm, 10f);
            _damageBar.SetValue(damage / 24.5f, damage.ToString("F0"), roleColor);

            float rpm = firearm.AttachmentsValue(AttachmentParam.FireRateMultiplier);
            _firerateBar.SetValue(rpm, (rpm * 10f).ToString("F1"), roleColor);

            float penetration = firearm.BaseStats.BasePenetrationPercent / 100f;
            _penetrationBar.SetValue(penetration, (penetration * 100f).ToString("F0") + "%", roleColor);

            float hipInacc = firearm.BaseStats.GetInaccuracy(firearm, false, 0f, true);
            float adsInacc = firearm.BaseStats.GetInaccuracy(firearm, true, 0f, true);
            float runInacc = firearm.BaseStats.GetInaccuracy(firearm, false, 4f, true);

            _hipAccBar.SetValue(_accuracyToValue.Evaluate(hipInacc), "", roleColor);
            _adsAccBar.SetValue(_accuracyToValue.Evaluate(adsInacc), "", roleColor);
            _runAccBar.SetValue(_accuracyToValue.Evaluate(runInacc), "", roleColor);

            _staminaUsageBar.SetValue(firearm.StaminaUsageMultiplier, (firearm.StaminaUsageMultiplier * 100f).ToString("F0") + "%", roleColor);
            _movementSpeedBar.SetValue(firearm.MovementSpeedMultiplier, (firearm.MovementSpeedMultiplier * 100f).ToString("F0") + "%", roleColor);

            _ammoText.text = FormatAmmo(firearm);

            // Attachments that are part of the weapon's default setup stay white; everything the
            // player picked themselves (preset / workstation) is tinted with the role colour.
            uint defaultCode = firearm.ValidateAttachmentsCode(0u);
            firearm.GenerateIcon(_bodyImage, _attachmentsPool, _attachmentIconsBorders.sizeDelta,
                x => (defaultCode & (1u << x)) == (1u << x) ? Color.white : roleColor);
        }

        private string FormatAmmo(Firearm firearm)
        {
            byte currentAmmo = firearm.Status.Ammo;
            string ammoName = InventoryItemLoader.TryGetItem<AmmoItem>(firearm.AmmoType, out AmmoItem ammoItem)
                ? ammoItem.Name
                : firearm.AmmoType.ToString();
            return $"<color=white>{ammoName}</color> ({currentAmmo})";
        }
    }
}