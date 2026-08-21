public static class MECExtensionMethods2
{
    public static global::System.Collections.Generic.IEnumerator<float> Delay(this global::System.Collections.Generic.IEnumerator<float> coroutine, float timeToDelay)
    {
        yield return global::MEC.Timing.WaitForSeconds(timeToDelay);
        while (coroutine.MoveNext())
        {
            yield return coroutine.Current;
        }
    }

    public static global::System.Collections.Generic.IEnumerator<float> Delay(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::System.Func<bool> condition)
    {
        while (!condition())
        {
            yield return 0f;
        }
        while (coroutine.MoveNext())
        {
            yield return coroutine.Current;
        }
    }

    public static global::System.Collections.Generic.IEnumerator<float> Delay<T>(this global::System.Collections.Generic.IEnumerator<float> coroutine, T data, global::System.Func<T, bool> condition)
    {
        while (!condition(data))
        {
            yield return 0f;
        }
        while (coroutine.MoveNext())
        {
            yield return coroutine.Current;
        }
    }

    public static global::System.Collections.Generic.IEnumerator<float> DelayFrames(this global::System.Collections.Generic.IEnumerator<float> coroutine, int framesToDelay)
    {
        while (framesToDelay-- > 0)
        {
            yield return 0f;
        }
        while (coroutine.MoveNext())
        {
            yield return coroutine.Current;
        }
    }

    public static global::System.Collections.Generic.IEnumerator<float> CancelWith(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::UnityEngine.GameObject gameObject)
    {
        while (global::MEC.Timing.MainThread != global::System.Threading.Thread.CurrentThread || ((bool)gameObject && gameObject.activeInHierarchy && coroutine.MoveNext()))
        {
            yield return coroutine.Current;
        }
    }

    public static global::System.Collections.Generic.IEnumerator<float> CancelWith(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::UnityEngine.GameObject gameObject1, global::UnityEngine.GameObject gameObject2)
    {
        while (global::MEC.Timing.MainThread != global::System.Threading.Thread.CurrentThread || ((bool)gameObject1 && gameObject1.activeInHierarchy && (bool)gameObject2 && gameObject2.activeInHierarchy && coroutine.MoveNext()))
        {
            yield return coroutine.Current;
        }
    }

    public static global::System.Collections.Generic.IEnumerator<float> CancelWith<T>(this global::System.Collections.Generic.IEnumerator<float> coroutine, T script) where T : global::UnityEngine.MonoBehaviour
    {
        global::UnityEngine.GameObject myGO = script.gameObject;
        while (global::MEC.Timing.MainThread != global::System.Threading.Thread.CurrentThread || ((bool)myGO && myGO.activeInHierarchy && script != null && coroutine.MoveNext()))
        {
            yield return coroutine.Current;
        }
    }

    public static global::System.Collections.Generic.IEnumerator<float> CancelWith(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::System.Func<bool> condition)
    {
        if (condition != null)
        {
            while (global::MEC.Timing.MainThread != global::System.Threading.Thread.CurrentThread || (condition() && coroutine.MoveNext()))
            {
                yield return coroutine.Current;
            }
        }
    }

    public static global::System.Collections.Generic.IEnumerator<float> PauseWith(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::UnityEngine.GameObject gameObject)
    {
        while (global::MEC.Timing.MainThread == global::System.Threading.Thread.CurrentThread && (bool)gameObject)
        {
            if (gameObject.activeInHierarchy)
            {
                if (!coroutine.MoveNext())
                {
                    break;
                }
                yield return coroutine.Current;
            }
            else
            {
                yield return float.NegativeInfinity;
            }
        }
    }

    public static global::System.Collections.Generic.IEnumerator<float> PauseWith(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::UnityEngine.GameObject gameObject1, global::UnityEngine.GameObject gameObject2)
    {
        while (global::MEC.Timing.MainThread == global::System.Threading.Thread.CurrentThread && (bool)gameObject1 && (bool)gameObject2)
        {
            if (gameObject1.activeInHierarchy && gameObject2.activeInHierarchy)
            {
                if (!coroutine.MoveNext())
                {
                    break;
                }
                yield return coroutine.Current;
            }
            else
            {
                yield return float.NegativeInfinity;
            }
        }
    }

    public static global::System.Collections.Generic.IEnumerator<float> PauseWith<T>(this global::System.Collections.Generic.IEnumerator<float> coroutine, T script) where T : global::UnityEngine.MonoBehaviour
    {
        global::UnityEngine.GameObject myGO = script.gameObject;
        while (global::MEC.Timing.MainThread == global::System.Threading.Thread.CurrentThread && (bool)myGO && myGO.GetComponent<T>() != null)
        {
            if (myGO.activeInHierarchy && script.enabled)
            {
                if (!coroutine.MoveNext())
                {
                    break;
                }
                yield return coroutine.Current;
            }
            else
            {
                yield return float.NegativeInfinity;
            }
        }
    }

    public static global::System.Collections.Generic.IEnumerator<float> PauseWith(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::System.Func<bool> condition)
    {
        if (condition != null)
        {
            while (global::MEC.Timing.MainThread != global::System.Threading.Thread.CurrentThread || (condition() && coroutine.MoveNext()))
            {
                yield return coroutine.Current;
            }
        }
    }

    public static global::System.Collections.Generic.IEnumerator<float> KillWith(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::MEC.CoroutineHandle otherCoroutine)
    {
        while (otherCoroutine.IsRunning && coroutine.MoveNext())
        {
            yield return coroutine.Current;
        }
    }

    public static global::System.Collections.Generic.IEnumerator<float> Append(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::System.Collections.Generic.IEnumerator<float> nextCoroutine)
    {
        while (coroutine.MoveNext())
        {
            yield return coroutine.Current;
        }
        if (nextCoroutine != null)
        {
            while (nextCoroutine.MoveNext())
            {
                yield return nextCoroutine.Current;
            }
        }
    }

    public static global::System.Collections.Generic.IEnumerator<float> Append(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::System.Action onDone)
    {
        while (coroutine.MoveNext())
        {
            yield return coroutine.Current;
        }
        onDone?.Invoke();
    }

    public static global::System.Collections.Generic.IEnumerator<float> Prepend(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::System.Collections.Generic.IEnumerator<float> lastCoroutine)
    {
        if (lastCoroutine != null)
        {
            while (lastCoroutine.MoveNext())
            {
                yield return lastCoroutine.Current;
            }
        }
        while (coroutine.MoveNext())
        {
            yield return coroutine.Current;
        }
    }

    public static global::System.Collections.Generic.IEnumerator<float> Prepend(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::System.Action onStart)
    {
        onStart?.Invoke();
        while (coroutine.MoveNext())
        {
            yield return coroutine.Current;
        }
    }

    public static global::System.Collections.Generic.IEnumerator<float> Superimpose(this global::System.Collections.Generic.IEnumerator<float> coroutineA, global::System.Collections.Generic.IEnumerator<float> coroutineB)
    {
        return coroutineA.Superimpose(coroutineB, global::MEC.Timing.Instance);
    }

    public static global::System.Collections.Generic.IEnumerator<float> Superimpose(this global::System.Collections.Generic.IEnumerator<float> coroutineA, global::System.Collections.Generic.IEnumerator<float> coroutineB, global::MEC.Timing instance)
    {
        while (coroutineA != null || coroutineB != null)
        {
            if (coroutineA != null && !(instance.localTime < coroutineA.Current) && !coroutineA.MoveNext())
            {
                coroutineA = null;
            }
            if (coroutineB != null && !(instance.localTime < coroutineB.Current) && !coroutineB.MoveNext())
            {
                coroutineB = null;
            }
            if ((coroutineA != null && float.IsNaN(coroutineA.Current)) || (coroutineB != null && float.IsNaN(coroutineB.Current)))
            {
                yield return float.NaN;
            }
            else if (coroutineA != null && coroutineB != null)
            {
                yield return (coroutineA.Current < coroutineB.Current) ? coroutineA.Current : coroutineB.Current;
            }
            else if (coroutineA == null && coroutineB != null)
            {
                yield return coroutineB.Current;
            }
            else if (coroutineA != null)
            {
                yield return coroutineA.Current;
            }
        }
    }

    public static global::System.Collections.Generic.IEnumerator<float> Hijack(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::System.Func<float, float> newReturn)
    {
        if (newReturn != null)
        {
            while (coroutine.MoveNext())
            {
                yield return newReturn(coroutine.Current);
            }
        }
    }

    public static global::System.Collections.Generic.IEnumerator<float> RerouteExceptions(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::System.Action<global::System.Exception> exceptionHandler)
    {
        while (true)
        {
            try
            {
                if (!coroutine.MoveNext())
                {
                    break;
                }
            }
            catch (global::System.Exception obj)
            {
                exceptionHandler?.Invoke(obj);
                break;
            }
            yield return coroutine.Current;
        }
    }
}
