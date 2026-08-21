public class BrowserLerp : global::UnityEngine.MonoBehaviour
{
    private global::UnityEngine.Vector3 prevPos;

    private global::UnityEngine.RectTransform rectTransform;

    private global::UnityEngine.Vector3 targetPos;

    public float speed = 2f;

    private void Start()
    {
        rectTransform = GetComponent<global::UnityEngine.RectTransform>();
    }

    private void LateUpdate()
    {
        targetPos += rectTransform.localPosition - prevPos;
        rectTransform.localPosition = prevPos;
        rectTransform.localPosition = global::UnityEngine.Vector3.Lerp(rectTransform.localPosition, targetPos, global::UnityEngine.Time.deltaTime * speed * 4f);
        prevPos = rectTransform.localPosition;
    }
}
