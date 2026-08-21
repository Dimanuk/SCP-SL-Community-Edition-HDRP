using UnityEngine;

public sealed class TLRSubstringSelector : UniversalTextModifier
{
    [SerializeField]
    private int _index;

    [SerializeField]
    private char _divisionChar;

    protected override void Awake()
    {
        base.Awake();

        string displayText = DisplayText;
        string[] parts = displayText.Split(new[] { _divisionChar });

        if (_index < parts.Length)
            DisplayText = parts[_index];

        TextLanguageReplacer replacer = GetComponent<TextLanguageReplacer>();
        if (replacer != null)
            replacer.OnUpdated += Divide;
    }

    private void Divide()
    {
        string displayText = DisplayText;
        string[] parts = displayText.Split(new[] { _divisionChar });

        if (_index < parts.Length)
            DisplayText = parts[_index];
    }
}