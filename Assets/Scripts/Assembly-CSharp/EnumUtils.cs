using System;

public static class EnumUtils<T> where T : struct, Enum
{
    public static readonly T[] Values = (T[])Enum.GetValues(typeof(T));

    public static readonly string[] Names = Enum.GetNames(typeof(T));

    public static readonly Type UnderlyingType = Enum.GetUnderlyingType(typeof(T));

    public static readonly TypeCode TypeCode = Type.GetTypeCode(UnderlyingType);
    public static string GetName(T value)
    {
        return Enum.GetName(typeof(T), value);
    }
}