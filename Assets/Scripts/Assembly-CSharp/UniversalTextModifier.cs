using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UniversalTextModifier : MonoBehaviour
{
    private TMP_Text _tmpText;
    private Text _unityText;
    private bool _usesTmp;
    private bool _usesUnity;
    private string _text;

    public string DisplayText
    {
        get
        {
            if (string.IsNullOrEmpty(_text))
            {
                if (_usesTmp)
                {
                    return _tmpText?.text ?? string.Empty;
                }

                if (_usesUnity)
                {
                    return _unityText?.text ?? string.Empty;
                }

                return string.Empty;
            }

            return _text;
        }
        set
        {
            if (_text == value) return;

            _text = value;

            if (_usesTmp && _tmpText != null)
            {
                _tmpText.text = value;
            }

            if (_usesUnity && _unityText != null)
            {
                _unityText.text = value;
            }
        }
    }

    protected virtual void Awake()
    {
        _usesTmp = TryGetComponent(out _tmpText);
        _usesUnity = TryGetComponent(out _unityText);
    }

    protected void SetText(string text)
    {
        DisplayText = text;
    }
}