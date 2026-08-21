public class LoadingCircle : global::UnityEngine.MonoBehaviour
{
    private int i;

    public int framesToNextRotation = 10;

    private void FixedUpdate()
    {
        i++;
        if (i > framesToNextRotation)
        {
            i = 0;
            base.transform.Rotate(global::UnityEngine.Vector3.forward * -45f, global::UnityEngine.Space.Self);
        }
    }
}
