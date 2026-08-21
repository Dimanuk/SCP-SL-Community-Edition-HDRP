public class Rotate : global::UnityEngine.MonoBehaviour
{
    private global::UnityEngine.Vector3 speed;

    private void Update()
    {
        base.transform.Rotate(speed * global::UnityEngine.Time.deltaTime);
    }
}
