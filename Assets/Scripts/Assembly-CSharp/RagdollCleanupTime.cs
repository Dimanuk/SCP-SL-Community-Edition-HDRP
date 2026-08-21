using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using MEC;
using PlayerRoles.Ragdolls;
using UnityEngine;
using UnityEngine.UI;

public class RagdollCleanupTime : MonoBehaviour
{
    [SerializeField]
    private InputField Text;

    [SerializeField]
    private Text SecondsText;

    [SerializeField]
    private Slider Slider;

    private string DisabledString;
    private string SecondsString;
    private string SecondString;

    private void Start()
    {
        DisabledString = TranslationReader.Get("NewMainMenu", 73, "Disabled");
        SecondsString = TranslationReader.Get("NewMainMenu", 74, "Seconds");
        SecondString = TranslationReader.Get("NewMainMenu", 86, "Second");

        int savedValue = PlayerPrefsSl.Get("ragdoll_cleanup", 0);

        if (savedValue > 0)
        {
            SecondsText.text = savedValue == 1 ? SecondString : SecondsString;
            Text.SetTextWithoutNotify(savedValue.ToString());
        }
        else
        {
            Text.SetTextWithoutNotify(string.Empty);
            SecondsText.text = DisabledString;
        }

        Timing.RunCoroutine(SetPositionOneFrameLater());
        Slider.value = savedValue;
    }

    public void OnValueChanged(float value)
    {
        value = Mathf.Clamp(value, 0f, 600f);

        int intValue = Mathf.RoundToInt(value);

        if (intValue > 0)
        {
            SecondsText.text = intValue == 1 ? SecondString : SecondsString;
            Text.SetTextWithoutNotify(intValue.ToString());
        }
        else
        {
            Text.SetTextWithoutNotify(string.Empty);
            SecondsText.text = DisabledString;
        }

        RagdollManager.CleanupTime = intValue;
        Timing.RunCoroutine(SetPositionOneFrameLater());
        Slider.value = intValue;
        PlayerPrefsSl.Set("ragdoll_cleanup", intValue);
    }

    public void OnStringValueChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            OnValueChanged(0f);
            return;
        }

        if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
        {
            OnValueChanged(result);
        }
    }

    private IEnumerator<float> SetPositionOneFrameLater()
    {
        yield return 0f;

        if (SecondsText == null || Text == null)
            yield break;

        RectTransform labelRect = SecondsText.rectTransform;
        RectTransform inputRect = Text.GetComponent<RectTransform>();
        Text textComponent = Text.textComponent;

        if (labelRect == null || inputRect == null || textComponent == null)
            yield break;

        float width = textComponent.preferredWidth;
        const float Padding = 5f;

        Vector2 pos = labelRect.anchoredPosition;
        pos.x = inputRect.anchoredPosition.x + inputRect.rect.width + Padding;
        labelRect.anchoredPosition = pos;
    }
}