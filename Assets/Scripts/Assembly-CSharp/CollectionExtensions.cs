public static class CollectionExtensions
{
    public static void ShuffleList<T>(this global::System.Collections.Generic.IList<T> list)
    {
        global::System.Random random = new global::System.Random();
        int num = list.Count;
        while (num > 1)
        {
            num--;
            int index = random.Next(num + 1);
            T value = list[index];
            list[index] = list[num];
            list[num] = value;
        }
    }

    public static void ShuffleListSecure<T>(this global::System.Collections.Generic.IList<T> list)
    {
        using (global::System.Security.Cryptography.RNGCryptoServiceProvider rNGCryptoServiceProvider = new global::System.Security.Cryptography.RNGCryptoServiceProvider())
        {
            int num = list.Count;
            while (num > 1)
            {
                byte[] array = new byte[1];
                do
                {
                    rNGCryptoServiceProvider.GetBytes(array);
                }
                while (array[0] >= num * (255 / num));
                int index = array[0] % num;
                num--;
                T value = list[index];
                list[index] = list[num];
                list[num] = value;
            }
        }
    }

    public static bool IsEmpty(this global::System.Array array)
    {
        return array.Length == 0;
    }

    public static bool IsEmpty<T>(this T[] array)
    {
        return array.Length == 0;
    }

    public static bool IsEmpty<T>(this global::System.ArraySegment<T> array)
    {
        return array.Count == 0;
    }

    public static bool IsEmpty<T>(this global::System.Collections.Generic.List<T> list)
    {
        return list.Count == 0;
    }

    public static bool IsEmpty<T>(this global::System.Collections.Generic.Queue<T> queue)
    {
        return queue.Count == 0;
    }

    public static bool IsEmpty<T>(this global::System.Collections.Generic.Stack<T> stack)
    {
        return stack.Count == 0;
    }

    public static bool IsEmpty<T>(this global::System.Collections.Generic.HashSet<T> set)
    {
        return set.Count == 0;
    }

    public static bool IsEmpty<T>(this global::System.Collections.Generic.SortedSet<T> set)
    {
        return set.Count == 0;
    }

    public static bool IsEmpty<T>(this global::Mirror.SyncList<T> list)
    {
        return list.Count == 0;
    }

    public static bool IsEmpty<T>(this global::Mirror.SyncSet<T> set)
    {
        return set.Count == 0;
    }

    public static bool IsEmpty<TKey, TValue>(this global::Mirror.SyncDictionary<TKey, TValue> dictionary)
    {
        return dictionary.Count == 0;
    }

    public static bool IsEmpty<T>(this global::System.Collections.Generic.ICollection<T> collection)
    {
        return collection.Count == 0;
    }

    public static bool IsEmpty<TKey, TValue>(this global::System.Collections.Generic.Dictionary<TKey, TValue> dictionary)
    {
        return dictionary.Count == 0;
    }

    public static bool IsEmpty<T>(this global::System.Collections.Generic.IEnumerable<T> iEnumerable)
    {
        return !global::System.Linq.Enumerable.Any(iEnumerable);
    }

    public static void EnsureCapacity<T>(this global::System.Collections.Generic.List<T> list, int capacity)
    {
        if (list.Capacity < capacity)
        {
            list.Capacity = capacity;
        }
    }

    public static int IndexOf<T>(this T[] array, T obj)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (global::System.Collections.Generic.EqualityComparer<T>.Default.Equals(array[i], obj))
            {
                return i;
            }
        }
        return -1;
    }

    public static int LastIndexOf<T>(this T[] array, T obj)
    {
        for (int num = array.Length - 1; num >= 0; num--)
        {
            if (global::System.Collections.Generic.EqualityComparer<T>.Default.Equals(array[num], obj))
            {
                return num;
            }
        }
        return -1;
    }

    public static bool Contains<T>(this T[] array, T obj)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (global::System.Collections.Generic.EqualityComparer<T>.Default.Equals(array[i], obj))
            {
                return true;
            }
        }
        return false;
    }

    public static void ForEach<T>(this T[] array, global::System.Action<T> obj)
    {
        for (int i = 0; i < array.Length; i++)
        {
            obj?.Invoke(array[i]);
        }
    }

    public static void Reverse<T>(this T[] array)
    {
        int num = 0;
        int num2 = array.Length;
        while (num < num2)
        {
            T val = array[num];
            array[num] = array[num2];
            array[num2] = val;
            num++;
            num2--;
        }
    }

    public static bool Contains(this string[] array, string str, global::System.StringComparison comparison = global::System.StringComparison.Ordinal)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (string.Equals(array[i], str, comparison))
            {
                return true;
            }
        }
        return false;
    }

    public static bool Contains(this global::System.Collections.Generic.List<string> list, string str, global::System.StringComparison comparison = global::System.StringComparison.Ordinal)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (string.Equals(list[i], str, comparison))
            {
                return true;
            }
        }
        return false;
    }

    public static bool TryGet<T>(this T[] array, int index, out T element)
    {
        if (index > -1 && index < array.Length)
        {
            element = array[index];
            return true;
        }
        element = default(T);
        return false;
    }

    public static bool TryGet<T>(this global::System.Collections.Generic.List<T> list, int index, out T element)
    {
        if (index > -1 && index < list.Count)
        {
            element = list[index];
            return true;
        }
        element = default(T);
        return false;
    }

    public static bool TryDequeue<T>(this global::System.Collections.Generic.Queue<T> queue, out T element)
    {
        if (queue.Count > 0)
        {
            element = queue.Dequeue();
            return true;
        }
        element = default(T);
        return false;
    }

    public static T[] ToArray<T>(this global::System.Array array)
    {
        T[] array2 = new T[array.Length];
        array.CopyTo(array2, 0);
        return array2;
    }

    public static int IndexOf(this global::UnityEngine.GameObject[] array, global::UnityEngine.GameObject obj)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == obj)
            {
                return i;
            }
        }
        return -1;
    }

    public static int IndexOf(this global::System.Collections.Generic.List<global::UnityEngine.GameObject> list, global::UnityEngine.GameObject obj)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == obj)
            {
                return i;
            }
        }
        return -1;
    }

    public static bool Contains(this global::UnityEngine.GameObject[] array, global::UnityEngine.GameObject obj)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == obj)
            {
                return true;
            }
        }
        return false;
    }

    public static bool Contains(this global::System.Collections.Generic.List<global::UnityEngine.GameObject> list, global::UnityEngine.GameObject obj)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == obj)
            {
                return true;
            }
        }
        return false;
    }

    [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static T FirstElement<T>(this global::System.ArraySegment<T> segment)
    {
        return segment.Array[segment.Offset];
    }

    [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static T At<T>(this global::System.ArraySegment<T> segment, int index)
    {
        return segment.Array[segment.Offset + index];
    }

    [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static global::System.ArraySegment<T> Segment<T>(this global::System.ArraySegment<T> segment, int offset)
    {
        return new global::System.ArraySegment<T>(segment.Array, segment.Offset + offset, segment.Count - offset);
    }

    [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static global::System.ArraySegment<T> Segment<T>(this global::System.ArraySegment<T> segment, int offset, int length)
    {
        return new global::System.ArraySegment<T>(segment.Array, segment.Offset + offset, length);
    }

    [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static global::System.ArraySegment<T> Segment<T>(this T[] array, int offset)
    {
        return new global::System.ArraySegment<T>(array, offset, array.Length - offset);
    }

    [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static global::System.ArraySegment<T> Segment<T>(this T[] array, int offset, int length)
    {
        return new global::System.ArraySegment<T>(array, offset, length);
    }

    public static TValue GetOrAdd<TKey, TValue>(this global::System.Collections.Generic.Dictionary<TKey, TValue> dictionary, TKey key, global::System.Func<TValue> factory) where TValue : class
    {
        if (!dictionary.TryGetValue(key, out var value))
        {
            return dictionary[key] = factory();
        }
        return value;
    }
}
