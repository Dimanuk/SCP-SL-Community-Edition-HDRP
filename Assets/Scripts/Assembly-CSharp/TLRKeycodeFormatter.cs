using System;
using UnityEngine;

public sealed class TLRKeycodeFormatter : UniversalTextModifier
{
    [Serializable]
    private struct Key
    {
        public ActionName Action;
        public int FormatIndex;
        public string AdditionalFormat;
    }

    [SerializeField] private Key[] _keys;
    [SerializeField] private int _maxChars;
    [SerializeField] private string _missingFormat;

    private TextLanguageReplacer _cachedtlr;
    private bool _cacheSet;

    private TextLanguageReplacer TextLangReplacer
    {
        get
        {
            if (!_cacheSet)
            {
                _cachedtlr = GetComponent<TextLanguageReplacer>();
                _cacheSet = true;
            }
            return _cachedtlr;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        Format();

        var tlr = TextLangReplacer;
        if (tlr != null)
            tlr.OnUpdated += Format;
    }

    private void Format()
    {
        if (_keys == null || _keys.Length == 0)
            return;

        int maxIndex = 0;
        for (int i = 0; i < _keys.Length; i++)
            maxIndex = Mathf.Max(maxIndex, _keys[i].FormatIndex);

        string[] formatArgs = new string[maxIndex + 1];

        for (int i = 0; i < _keys.Length; i++)
        {
            var key = _keys[i];

            string keyName = GetKeyDisplayName(key.Action);

            if (_maxChars > 0 && !string.IsNullOrEmpty(keyName) && keyName.Length > _maxChars)
                keyName = keyName.Substring(0, _maxChars);

            if (!string.IsNullOrEmpty(key.AdditionalFormat))
                keyName = string.Format(key.AdditionalFormat, keyName);

            if (key.FormatIndex >= 0 && key.FormatIndex < formatArgs.Length)
                formatArgs[key.FormatIndex] = keyName;
        }

        var tlr = TextLangReplacer;
        if (tlr != null && tlr.Missing)
            base.DisplayText = _missingFormat;

        base.DisplayText = string.Format(base.DisplayText, formatArgs);
    }

    private static string GetKeyDisplayName(ActionName action)
    {
        return NewInput.GetKey(action).ToString();
    }
}