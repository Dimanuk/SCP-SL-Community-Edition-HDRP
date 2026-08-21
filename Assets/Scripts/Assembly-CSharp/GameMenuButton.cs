using UnityEngine;

public class GameMenuButton : MonoBehaviour
{
    public Vector3 normalPos;
    public Vector3 focusedPos;
    public AnimationCurve anim;

    private bool isFocused;
    private float status;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent <RectTransform>();
    }

    public void Focus(bool b)
    {
        isFocused = b;
    }

    private void Update()
    {
        float direction = isFocused ? 1f : -1f;
        status += direction * Time.deltaTime;
        status = Mathf.Clamp01(status);

        if (anim != null && rectTransform != null)
        {
            Vector3 finalPos = normalPos + (focusedPos - normalPos) * anim.Evaluate(status);
            rectTransform.localPosition = finalPos;
        }
    }
}