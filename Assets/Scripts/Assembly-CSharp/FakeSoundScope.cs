public class FakeSoundScope : global::UnityEngine.MonoBehaviour
{
    public global::UnityEngine.AnimationCurve highOverVolume;

    public int numOfPos;

    private global::UnityEngine.LineRenderer line;

    public float maxH;

    private void Awake()
    {
        line = GetComponent<global::UnityEngine.LineRenderer>();
    }

    private void LateUpdate()
    {
        global::UnityEngine.Vector3[] array = new global::UnityEngine.Vector3[numOfPos];
        float value = global::UnityEngine.Random.value;
        float num = 0f;
        for (int i = 0; i < numOfPos; i++)
        {
            float num2 = (float)i / (float)numOfPos;
            float num3 = global::UnityEngine.Mathf.Abs(1f - global::UnityEngine.Mathf.Abs(num2 - 0.5f) * 2f);
            array[i][0] = num2 * 100f;
            array[i][2] = global::UnityEngine.Mathf.Sin((float)(i * 7) * value) * num3 * maxH * (global::UnityEngine.Mathf.Sin(i) / 3f) * num;
        }
        line.SetPositions(array);
    }
}
