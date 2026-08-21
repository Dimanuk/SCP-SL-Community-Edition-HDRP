using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HideHUDController : MonoBehaviour
{
    private static HideHUDController _singleton;

    private bool _showHUDElements = true;

    private KeyCode _hideUIKey;

    public static bool IsHUDVisible
    {
        get
        {
            if (_singleton == null)
                return true;
            return _singleton._showHUDElements;
        }
    }

    public static event Action<bool> ToggleHUD;

    private void Awake()
    {
        _singleton = this;
        _hideUIKey = NewInput.GetKey(ActionName.HideGUI, default);
    }

    private void Update()
    {
        GameCore.RoundStart singleton = GameCore.RoundStart.singleton;
        if (singleton == null)
            return;

        if (singleton.Timer != -1)
            return;

        if (!Input.GetKeyDown(_hideUIKey))
            return;

        if (_showHUDElements)
        {
            InputField[] inputFields = FindObjectsOfType<InputField>();
            for (int i = 0; i < inputFields.Length; i++)
            {
                if (inputFields[i].isFocused)
                    return;
            }

            TMP_InputField[] tmpInputFields = FindObjectsOfType<TMP_InputField>();
            for (int i = 0; i < tmpInputFields.Length; i++)
            {
                if (tmpInputFields[i].isFocused)
                    return;
            }
        }

        _showHUDElements = !_showHUDElements;

        ToggleHUD?.Invoke(_showHUDElements);

        GameMenu.singleton.hideHUDText.SetActive(!_showHUDElements);
    }
}