using UnityEngine;
using UnityEngine.UI;

public class AspectScaler : MonoBehaviour
{
    public CanvasScaler Scaler;

    private void Update()
    {
        float num = (float)Screen.width / (float)Screen.height;
        if (num > 1.8f)
        {
            Scaler.matchWidthOrHeight = 1f;
        }
        if (num < 1.65f)
        {
            Scaler.matchWidthOrHeight = 0f;
        }
    }
}
