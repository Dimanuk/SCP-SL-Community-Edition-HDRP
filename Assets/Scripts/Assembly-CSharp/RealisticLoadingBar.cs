public class RealisticLoadingBar
{
    private float _lastProgress;

    private readonly global::System.Collections.Generic.Queue<global::UnityEngine.Vector2> _queue;

    public float Progress
    {
        get
        {
            while (_queue.Count > 0 && _queue.Peek().x <= global::UnityEngine.Time.realtimeSinceStartup)
            {
                _lastProgress += _queue.Dequeue().y;
            }
            if (_queue.Count != 0)
            {
                return _lastProgress;
            }
            return 1f;
        }
    }

    public RealisticLoadingBar(float targetTime, int numberOfSteps, float maxStepSizeVar, float maxTickVar)
    {
        _lastProgress = 0f;
        _queue = new global::System.Collections.Generic.Queue<global::UnityEngine.Vector2>();
        global::System.Collections.Generic.List<global::UnityEngine.Vector2> list = global::NorthwoodLib.Pools.ListPool<global::UnityEngine.Vector2>.Shared.Rent();
        float num = 0f;
        float num2 = 0f;
        for (int i = 0; i < numberOfSteps; i++)
        {
            float num3 = global::UnityEngine.Random.Range(1f, maxTickVar);
            num += num3;
            float num4 = global::UnityEngine.Random.Range(1f, maxStepSizeVar);
            num2 += num4;
            list.Add(new global::UnityEngine.Vector2(num, num4));
        }
        float num5 = num / targetTime;
        foreach (global::UnityEngine.Vector2 item in list)
        {
            _queue.Enqueue(new global::UnityEngine.Vector2(global::UnityEngine.Time.realtimeSinceStartup + item.x / num5, item.y / num2));
        }
    }
}
