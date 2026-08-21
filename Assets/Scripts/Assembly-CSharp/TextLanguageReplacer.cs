using System;
using UnityEngine;

public sealed class TextLanguageReplacer : UniversalTextModifier
{
    public static readonly string[] Files = new string[35]
    {
        "None", "Facility", "NewMainMenu", "SCP939_HUD", "Voicechat", "MainMenu", "MenuWarnings", "SCP079_HUD", "SCP049_HUD", "SCP106_HUD",
        "Version_7-0-0", "ReportForm", "Summary", "InventoryGUI", "AttachmentEditors", "Nuke", "DecontaminationDisplays", "Doors", "Class_Names", "Abilities",
        "Legacy_Interfaces", "HumanAbilities", "HumanBio", "SCP173_HUD", "SCP096_HUD", "Teams", "VideoSettings", "AudioSettings", "SettingsCategories", "ControlsSettings",
        "InterfaceSettings", "OtherSettings", "Hotkeys", "Overwatch_HUD", "SCP3114_HUD"
    };

    [SerializeField] private string _englishVersion;
    [SerializeField] private int _fileId;
    [SerializeField] private int _line;

    public bool Missing { get; private set; }

    public event Action OnUpdated;

    protected override void Awake()
    {
        base.Awake();

        UpdateText();

        TranslationReader.OnTranslationsRefreshed += UpdateText;
    }

    private void OnDestroy()
    {
        TranslationReader.OnTranslationsRefreshed -= UpdateText;
    }

    private void UpdateText()
    {
        string translated = _englishVersion ?? string.Empty;
        Missing = true;

        if (_fileId >= 0 && _fileId < Files.Length && _line >= 0)
        {
            string fileName = Files[_fileId];

            if (TranslationReader.TryGet(fileName, _line, out string result))
            {
                translated = result;
                Missing = false;
            }
        }

        SetText(translated);

        OnUpdated?.Invoke();
    }

    public string GetCurrentTranslatedText()
    {
        string translated = _englishVersion ?? string.Empty;

        if (_fileId >= 0 && _fileId < Files.Length && _line >= 0)
        {
            string fileName = Files[_fileId];
            if (TranslationReader.TryGet(fileName, _line, out string result))
            {
                translated = result;
            }
        }

        return translated;
    }
}