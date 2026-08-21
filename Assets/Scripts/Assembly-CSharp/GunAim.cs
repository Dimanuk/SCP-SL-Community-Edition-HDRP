public class GunAim : global::UnityEngine.MonoBehaviour
{
    public int borderLeft;

    public int borderRight;

    public int borderTop;

    public int borderBottom;

    private global::UnityEngine.Camera parentCamera;

    private bool isOutOfBounds;

    private void Start()
    {
        parentCamera = GetComponentInParent<global::UnityEngine.Camera>();
    }

    private void Update()
    {
        float x = global::UnityEngine.Input.mousePosition.x;
        float y = global::UnityEngine.Input.mousePosition.y;
        if (x <= (float)borderLeft || x >= (float)(global::UnityEngine.Screen.width - borderRight) || y <= (float)borderBottom || y >= (float)(global::UnityEngine.Screen.height - borderTop))
        {
            isOutOfBounds = true;
        }
        else
        {
            isOutOfBounds = false;
        }
        if (!isOutOfBounds)
        {
            base.transform.LookAt(parentCamera.ScreenToWorldPoint(new global::UnityEngine.Vector3(x, y, 5f)));
        }
    }

    public bool GetIsOutOfBounds()
    {
        return isOutOfBounds;
    }
}
