using UnityEngine;

public class SignBlink : MonoBehaviour
{
    public bool verticalText;
    private string startText;

    private const string alphabet = "QWERTYUIOPASDFGHJKLZXCVBNM01234567890!@#$%^&*()-_=+[]{}<>";

    public void Play(int duration)
    {
        if (startText != "")
            return;

        TMPro.TextMeshProUGUI tmp = GetComponent<TMPro.TextMeshProUGUI>();
        startText = tmp.text;
        StartCoroutine(Blink(duration));
    }

    private System.Collections.IEnumerator Blink(int duration)
    {
        TMPro.TextMeshProUGUI tmp = GetComponent<TMPro.TextMeshProUGUI>();

        for (int i = 0; i < duration; i++)
        {
            string scrambled = "";
            for (int c = 0; c < startText.Length; c++)
                scrambled += alphabet[UnityEngine.Random.Range(0, alphabet.Length)];

            tmp.text = scrambled;
            yield return new WaitForSeconds(0.05f);
        }

        tmp.text = startText;
    }
}