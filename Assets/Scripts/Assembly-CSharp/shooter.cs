public class shooter : global::UnityEngine.MonoBehaviour
{
    public int mtpl = 5;

    private void Update()
    {
        if (global::UnityEngine.Input.GetKeyDown(global::UnityEngine.KeyCode.Return))
        {
            global::UnityEngine.ScreenCapture.CaptureScreenshot("Taken" + global::UnityEngine.Random.Range(0, 1000) + ".png", mtpl);
        }
    }
}
