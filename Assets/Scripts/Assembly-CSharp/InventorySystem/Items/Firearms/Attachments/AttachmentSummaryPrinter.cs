using InventorySystem.Items.Firearms.Attachments.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Attachments
{
    public class AttachmentSummaryPrinter : MonoBehaviour
    {
        [Serializable]
        private readonly struct ComparableConfiguration
        {
            public readonly string ConfigurationName;
            public readonly uint Code;

            public ComparableConfiguration(string label, uint code)
            {
                ConfigurationName = label;
                Code = code;
            }
        }

        [SerializeField]
        private AttachmentSelectorBase _selectorReference;

        [SerializeField]
        private AttachmentSummaryEntry _tableHeader;

        [SerializeField]
        private AttachmentSummaryEntry _entryTemplate;

        [SerializeField]
        private int _displayedPresets;

        [SerializeField]
        private string _goodColor;

        [SerializeField]
        private string _badColor;

        [SerializeField]
        private Color _oddEntryColor;

        private Firearm _prevFirearm;

        private readonly Queue<AttachmentSummaryEntry> _entryPool = new Queue<AttachmentSummaryEntry>();
        private readonly HashSet<AttachmentSummaryEntry> _spawnedEntires = new HashSet<AttachmentSummaryEntry>();
        private readonly HashSet<uint> _displayedCodes = new HashSet<uint>();
        private readonly List<ComparableConfiguration> _displayedConfigurations = new List<ComparableConfiguration>();

        private Firearm Firearm => _selectorReference.SelectedFirearm;

        private void OnEnable()
        {
            Refresh();
        }

        private void Update()
        {
            Firearm selectedFirearm = _selectorReference.SelectedFirearm;
            if (selectedFirearm != _prevFirearm)
            {
                _prevFirearm = selectedFirearm;
                Refresh();
            }
        }

        private void Refresh()
        {
            Firearm firearm = Firearm;
            if (firearm != null)
            {
                uint currentCode = AttachmentsUtils.GetCurrentAttachmentsCode(firearm);
                PrepareConfigurations(currentCode);

                string[] configNames = _displayedConfigurations.Select(c => c.ConfigurationName).ToArray();
                _tableHeader.Setup(firearm.name, configNames, false);

                List<Dictionary<string, string>> allConfigData = new List<Dictionary<string, string>>();
                foreach (ComparableConfiguration conf in _displayedConfigurations)
                {
                    KeyValuePair<string, string>[] data = GenerateConfigurationData(conf);
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> kvp in data)
                        dict[kvp.Key] = kvp.Value;
                    allConfigData.Add(dict);
                }

                if (allConfigData.Count > 0)
                {
                    string[] statLabels = allConfigData[0].Keys.ToArray();
                    for (int i = 0; i < statLabels.Length; i++)
                    {
                        string statLabel = statLabels[i];
                        string[] values = new string[allConfigData.Count];
                        for (int j = 0; j < allConfigData.Count; j++)
                        {
                            values[j] = allConfigData[j].TryGetValue(statLabel, out string v) ? v : string.Empty;
                        }

                        if (!_entryPool.TryDequeue(out AttachmentSummaryEntry entry))
                        {
                            entry = Instantiate(_entryTemplate, _entryTemplate.transform.parent);
                        }

                        entry.gameObject.SetActive(true);
                        bool isOdd = i % 2 != 0;
                        entry.Setup(statLabel, values, isOdd);
                        _spawnedEntires.Add(entry);
                    }
                }

                // ��������������� �������� ��� ������
                AttachmentsUtils.ApplyAttachmentsCode(firearm, currentCode, false);
                return;
            }

            // firearm == null � ����������� ������
            _selectorReference._summaryScreen.SetActive(false);
            _selectorReference._selectorScreen.SetActive(true);
            _selectorReference.OnSummaryToggled?.Invoke();
        }

        private KeyValuePair<string, string>[] GenerateConfigurationData(ComparableConfiguration conf)
        {
            Firearm firearm = Firearm;
            ComparableConfiguration baseline = _displayedConfigurations[0];

            AttachmentsUtils.ApplyAttachmentsCode(firearm, conf.Code, false);
            ExtractNonparams(out float confRunningInaccuracy, out float confLength, out float confWeight, out float confBaseAdsRecoil);
            float confAdsValue = AttachmentsUtils.AttachmentsValue(firearm, (AttachmentParam)6);
            float confRecoilPattern = AttachmentsUtils.AttachmentsValue(firearm, (AttachmentParam)13);
            float confHipAccuracy = AttachmentsUtils.AttachmentsValue(firearm, (AttachmentParam)10);

            AttachmentsUtils.ApplyAttachmentsCode(firearm, baseline.Code, false);
            ExtractNonparams(out float baseRunningInaccuracy, out float baseLength, out float baseWeight, out float baseBaseAdsRecoil);
            float baseAdsValue = AttachmentsUtils.AttachmentsValue(firearm, (AttachmentParam)6);
            float baseRecoilPattern = AttachmentsUtils.AttachmentsValue(firearm, (AttachmentParam)13);
            float baseHipAccuracy = AttachmentsUtils.AttachmentsValue(firearm, (AttachmentParam)10);

            KeyValuePair<string, string>[] result = new KeyValuePair<string, string>[13];

            result[0] = GetStatValuePair(baseRunningInaccuracy, (AttachmentParam)2, "%");
            result[1] = GetStatValuePair(baseBaseAdsRecoil, (AttachmentParam)3, "%");

            float fireRate = confAdsValue * 60f;  
            float baseFireRate = baseAdsValue * 60f;
            string fireRateStr = fireRate.ToString();
            if (!Mathf.Approximately(fireRate, baseFireRate) && TryGetFormatted((AttachmentParam)4, out string formattedFireRate))
                fireRateStr = string.Concat(fireRateStr, formattedFireRate);
            result[2] = new KeyValuePair<string, string>(
                TranslationReader.Get("AttachmentParameters", 4, "NO_TRANSLATION"),
                fireRateStr);

            result[3] = GetStatValuePair(baseAdsValue, (AttachmentParam)6, "%");

            result[4] = GetCustomValuePair(
                TranslationReader.Get("AttachmentParameters", 5, "NO_TRANSLATION"),
                baseAdsValue, confAdsValue, "%", 100f, 100f, true, false);

            result[5] = GetStatValuePair(baseHipAccuracy, (AttachmentParam)8, "");
            result[6] = GetStatValuePair(baseHipAccuracy, (AttachmentParam)7, "");
            result[7] = GetStatValuePair(baseHipAccuracy, (AttachmentParam)9, "");

            result[8] = GetCustomValuePair(
                TranslationReader.Get("InventoryGUI", 16, "NO_TRANSLATION"),
                baseRecoilPattern, confRecoilPattern, "", 100f, 100f, false, false);

            result[9] = GetCustomValuePair(
                TranslationReader.Get("AttachmentParameters", 13, "NO_TRANSLATION"),
                baseRunningInaccuracy, confRunningInaccuracy, "%", 100f, 100f, true, true);

            result[10] = GetCustomValuePair(
                TranslationReader.Get("InventoryGUI", 6, "NO_TRANSLATION"),
                baseLength, confLength, "cm", 10f, 10f, false, false);

            result[11] = GetCustomValuePair(
                TranslationReader.Get("InventoryGUI", 5, "NO_TRANSLATION"),
                baseWeight, confWeight, "kg", 100f, 100f, false, true);

            result[12] = GetStatValuePair(baseHipAccuracy, (AttachmentParam)10, "");
            return result;
        }

        private void ExtractNonparams(out float runningInaccuracy, out float length, out float weight, out float baseAdsRecoil)
        {
            Firearm firearm = Firearm;
            weight = firearm.PickupDropModel.Info.Weight;
            length = firearm.Length * 2.54f;

            runningInaccuracy = AttachmentsUtils.AttachmentsValue(firearm, (AttachmentParam)9);
            baseAdsRecoil = AttachmentsUtils.AttachmentsValue(firearm, (AttachmentParam)6) * 100f;
        }

        private KeyValuePair<string, string> GetStatValuePair(
            float defaultValue,
            AttachmentParam param,
            string unit = "",
            bool processValue = true,
            float rounding = 100f)
        {
            float value = AttachmentsUtils.AttachmentsValue(Firearm, param);
            AttachmentParameterDefinition definition = AttachmentsUtils.GetDefinitionOfParam((int)param);

            if (processValue)
            {
                if (definition.MixingMode == ParameterMixingMode.Additive)
                    value += definition.DefaultValue;      
                else
                    value *= definition.DefaultValue;     
            }

            float rounded = Mathf.Round(value * rounding) / rounding;
            string formatted = rounded.ToString() + unit;

            if (!Mathf.Approximately(value, defaultValue) && TryGetFormatted(param, out string colorFormatted))
                formatted = string.Concat(formatted, colorFormatted);

            string label = TranslationReader.Get("AttachmentParameters", (int)param, "NO_TRANSLATION");
            return new KeyValuePair<string, string>(label, formatted);
        }

        private KeyValuePair<string, string> GetCustomValuePair(
            string label,
            float baseValue,
            float otherValue,
            string unit,
            float mainAccuracy,
            float diffAccuracy,
            bool asPercent,
            bool posBad,
            bool inverse = false)
        {
            float mainRounded = Mathf.Round(baseValue * mainAccuracy) / mainAccuracy;
            string mainFormatted = string.Concat(mainRounded.ToString(), unit);

            float diff = inverse ? otherValue - baseValue : baseValue - otherValue;

            if (asPercent)
                diff *= 100f;

            float roundedDiff = Mathf.Round(diff * diffAccuracy) / diffAccuracy;
            bool isPositive = diff > 0f;
            bool isGood = posBad ? !isPositive : isPositive;
            string colorHex = isGood ? _goodColor : _badColor;
            string percentSign = asPercent ? "%" : "";
            string sign = roundedDiff >= 0f ? "+" : "-";

            object[] args = new object[5];
            args[0] = mainFormatted;
            args[1] = colorHex;
            args[2] = Mathf.Abs(roundedDiff).ToString();
            args[3] = percentSign;
            args[4] = sign;

            string result = string.Format("{0} (<color=#{1}>{4}{2}{3}</color>)", args);
            return new KeyValuePair<string, string>(label, result);
        }

        private bool TryGetFormatted(AttachmentParam param, out string str)
        {
            int paramIndex = (int)param;
            float currentValue = AttachmentsUtils.AttachmentsValue(Firearm, param);
            AttachmentParameterDefinition definition = AttachmentsUtils.GetDefinitionOfParam(paramIndex);

            if (AttachmentParameterFormatters.Formatters.TryGetValue(param, out IAttachmentsParameterFormatter formatter))
            {
                bool success = formatter.FormatParameter(
                    param,           
                    Firearm,        
                    0,               
                    currentValue,   
                    out string formattedText,  
                    out bool isGood           
                );

                if (success)
                {
                    str = formattedText;
                    return true;
                }
            }

            bool isPositive = currentValue >= definition.DefaultValue;
            string color = isPositive ? _goodColor : _badColor;
            string[] parts = new string[5];
            parts[0] = " (<color=#";
            parts[1] = color;
            parts[2] = ">";
            parts[3] = currentValue.ToString();
            parts[4] = "</color>)";
            str = string.Concat(parts);
            return true;
        }

        private void PrepareConfigurations(uint initial)
        {
            foreach (AttachmentSummaryEntry entry in _spawnedEntires)
            {
                if (entry != null)
                    _entryPool.Enqueue(entry);
            }
            _spawnedEntires.Clear();
            _displayedCodes.Clear();
            _displayedConfigurations.Clear();

            // ��������� Default (��� 0)
            AddConfiguration(0u, Translations.Get <AttachmentEditorsTranslation> (AttachmentEditorsTranslation.DefaultAttachments), true);

            Firearm firearm = Firearm;
            int preset = AttachmentPreferences.GetPreset(firearm.ItemTypeId);

            if (preset == 0)
            {
                AddConfiguration(initial, Translations.Get <AttachmentEditorsTranslation>(AttachmentEditorsTranslation.Custom), true);
            }

            for (int i = 1; i <= _displayedPresets; i++)
            {
                uint preferenceCode = AttachmentPreferences.GetPreferenceCodeOfPreset(firearm.ItemTypeId, i);
                string presetLabel = string.Format(
                    Translations.Get<AttachmentEditorsTranslation>(AttachmentEditorsTranslation.PresetId),
                    i);
                AddConfiguration(preferenceCode, presetLabel, false);
            }
        }

        private void AddConfiguration(uint attachmentCode, string label, bool forceAdd = false)
        {
            uint validCode = AttachmentsUtils.ValidateAttachmentsCode(Firearm, attachmentCode);

            if (!_displayedCodes.Add(validCode) && !forceAdd)
                return;

            _displayedConfigurations.Add(new ComparableConfiguration(label, validCode));
        }
    }
}
