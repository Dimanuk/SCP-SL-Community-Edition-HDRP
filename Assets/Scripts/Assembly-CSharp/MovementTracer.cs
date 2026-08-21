public class MovementTracer
{
    public byte Clock;

    public readonly global::RelativePositioning.RelativePosition[] Positions;

    private readonly byte _size;

    private readonly byte _cooldown;

    private readonly float _tpDis;

    private byte _cooldownTimer;

    public MovementTracer(byte size, byte cooldown, float teleportDistance)
    {
        _size = size;
        Positions = new global::RelativePositioning.RelativePosition[size];
        _cooldown = cooldown;
        _tpDis = teleportDistance;
        _cooldownTimer = 0;
    }

    public void Record(global::UnityEngine.Vector3 plyPosition)
    {
        if (_cooldownTimer > 0)
        {
            _cooldownTimer--;
            return;
        }
        if (++Clock >= _size)
        {
            Clock = 0;
        }
        Positions[Clock] = new global::RelativePositioning.RelativePosition(plyPosition);
        _cooldownTimer = _cooldown;
    }

    public global::UnityEngine.Bounds GenerateBounds(float time, bool ignoreTeleports)
    {
        int num = global::UnityEngine.Mathf.FloorToInt(time / global::UnityEngine.Time.fixedDeltaTime / (float)(_cooldown + 1));
        if (num <= 0)
        {
            global::UnityEngine.Debug.LogError($"MovementTracer was requested to generate Bounds for the last {time} seconds, but it's too short. Please access PlayerMovementSync.RealModelPosition directly.");
            num = 1;
        }
        else if (num > _size)
        {
            global::UnityEngine.Debug.LogError($"MovementTracer was requested to generate Bounds for the last {time} seconds, but it can't keep track of positions after {(((float)(int)_cooldown + 1f) * (float)(int)_size * global::UnityEngine.Time.fixedDeltaTime)}");
            num = _size;
        }
        global::UnityEngine.Bounds result = new global::UnityEngine.Bounds(Positions[Clock].Position, global::UnityEngine.Vector3.zero);
        for (int i = 1; i < num; i++)
        {
            int num2 = Clock - i;
            if (num2 < 0)
            {
                num2 += _size - 1;
            }
            global::UnityEngine.Vector3 position = Positions[num2].Position;
            if (!ignoreTeleports && global::UnityEngine.Vector3.Distance(result.ClosestPoint(position), position) > _tpDis)
            {
                break;
            }
            result.Encapsulate(position);
        }
        return result;
    }
}
