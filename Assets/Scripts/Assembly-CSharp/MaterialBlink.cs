using UnityEngine;

public class MaterialBlink : MonoBehaviour
{
    public Material materal;
    public Color lowestColor;
    public Color highestColor;
    public float speed;
    public float colorMultiplier;
    private float time;

    private void Update()
    {
        time += Time.deltaTime * speed;
        if (time > 1f)
        {
            time -= 1f;
        }

        float t = Mathf.Lerp(-1f, 1f, time);
        Color blendedColor = Color.Lerp(lowestColor, highestColor, t) * colorMultiplier;
        materal.SetColor("_EmissionColor", blendedColor);
    }

    private void OnDisable()
    {
        materal?.SetColor("_EmissionColor", highestColor);
    }

    public MaterialBlink()
    {
        lowestColor = Color.white;
        speed = 1f;
        colorMultiplier = 1f;
        highestColor = Color.white;
    }
}