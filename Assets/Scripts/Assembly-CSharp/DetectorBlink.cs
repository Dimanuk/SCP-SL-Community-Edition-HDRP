public class DetectorBlink : global::UnityEngine.MonoBehaviour
{
    public global::UnityEngine.Material mat;

    private bool state;

    private void Start()
    {
        Blink();
    }

    private void Blink()
    {
        state = !state;
        int num = (state ? 2 : 0);
        mat.SetColor("_EmissionColor", new global::UnityEngine.Color(num, num, num));
        Invoke(nameof(Blink), state ? 0.2f : 1.3f);
    }
}
