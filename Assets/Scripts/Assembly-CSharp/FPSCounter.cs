using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    private double _framerate;
    private double _frametime;
    private ushort _time = 24;

    public Text FpsText;

    private void Start()
    {
        if (ServerStatic.IsDedicated)
            enabled = false;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        _framerate = System.Math.Round(1.0 / deltaTime, 1);
        _frametime = System.Math.Round(deltaTime * 1000.0, 1);
    }

    private void FixedUpdate()
    {
        _time++;
        if (_time == 25)
        {
            _time = 0;
            if (FpsText != null)
            {
                FpsText.text = string.Concat("Framerate: ", _framerate, "   ", _frametime, "ms");
            }
        }
    }
}