public static class RandomElement
{
    public static T RandomItem<T>(this T[] array)
    {
        return array[global::UnityEngine.Random.Range(0, array.Length)];
    }

    public static T RandomItem<T>(this global::System.Collections.Generic.List<T> list)
    {
        return list[global::UnityEngine.Random.Range(0, list.Count)];
    }

    public static T PullRandomItem<T>(this global::System.Collections.Generic.List<T> list)
    {
        int index = global::UnityEngine.Random.Range(0, list.Count);
        T result = list[index];
        list.RemoveAt(index);
        return result;
    }
}
