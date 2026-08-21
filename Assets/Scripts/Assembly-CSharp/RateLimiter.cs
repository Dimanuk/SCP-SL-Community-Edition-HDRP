public class RateLimiter
{
    private readonly float _instantCooldown;

    private readonly float _windowCooldown;

    private readonly global::System.Collections.Generic.Queue<global::System.Diagnostics.Stopwatch> _inputs;

    private readonly global::System.Diagnostics.Stopwatch _instInput;

    private readonly int _windowLength;

    public bool InstantReady
    {
        get
        {
            if (_instantCooldown != 0f && _instInput.IsRunning)
            {
                return _instInput.Elapsed.TotalSeconds >= (double)_instantCooldown;
            }
            return true;
        }
    }

    public bool RateReady
    {
        get
        {
            int num = _inputs.Count;
            if (num < _windowLength)
            {
                return true;
            }
            while (num > 0 && _inputs.Peek().Elapsed.TotalSeconds >= (double)_windowCooldown)
            {
                num--;
                _inputs.Dequeue();
            }
            return num < _windowLength;
        }
    }

    public bool AllReady
    {
        get
        {
            if (InstantReady)
            {
                return RateReady;
            }
            return false;
        }
    }

    public RateLimiter(float instantCooldown, int maxInputs, float timeWindow)
    {
        _instantCooldown = instantCooldown;
        _windowCooldown = timeWindow;
        _windowLength = maxInputs;
        _inputs = new global::System.Collections.Generic.Queue<global::System.Diagnostics.Stopwatch>();
        _instInput = new global::System.Diagnostics.Stopwatch();
    }

    public void RegisterInput()
    {
        _instInput.Restart();
        _inputs.Enqueue(global::System.Diagnostics.Stopwatch.StartNew());
    }
}
