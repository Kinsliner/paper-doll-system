using System.Collections.Generic;

public static class CheckExtension
{
    public static bool IsNull(this object value)
    {
        return value == null;
    }

    public static bool IsNotNull(this object value)
    {
        return value != null;
    }

    public static bool IsNull<T>(this T value) where T : class
    {
        return value == null;
    }

    public static bool IsNotNull<T>(this T value) where T : class
    {
        return value != null;
    }

    public static bool IsNull<T>(this T? value) where T : struct
    {
        return !value.HasValue;
    }

    public static bool IsNotNull<T>(this T? value) where T : struct
    {
        return value.HasValue;
    }

    public static bool IsNullOrEmpty<T>(this T[] value) where T : class
    {
        return value == null || value.Length == 0;
    }

    public static bool IsNotNullOrEmpty<T>(this T[] value) where T : class
    {
        return value != null && value.Length > 0;
    }

    public static bool IsNullOrEmpty<T>(this List<T> value) where T : class
    {
        return value == null || value.Count == 0;
    }

    public static bool IsNotNullOrEmpty<T>(this List<T> value) where T : class
    {
        return value != null && value.Count > 0;
    }

    public static bool IsNullOrEmpty<T>(this Dictionary<T, T> value) where T : class
    {
        return value == null || value.Count == 0;
    }

    public static bool IsNotNullOrEmpty<T>(this Dictionary<T, T> value) where T : class
    {
        return value != null && value.Count > 0;
    }

    public static bool IsNullOrEmpty<T>(this HashSet<T> value) where T : class
    {
        return value == null || value.Count == 0;
    }

    public static bool IsNotNullOrEmpty<T>(this HashSet<T> value) where T : class
    {
        return value != null && value.Count > 0;
    }

    public static bool IsNullOrEmpty<T>(this Queue<T> value) where T : class
    {
        return value == null || value.Count == 0;
    }

    public static bool IsNotNullOrEmpty<T>(this Queue<T> value) where T : class
    {
        return value != null && value.Count > 0;
    }

    //string is not null or empty
    public static bool IsNotNullOrEmpty(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return false;
        }
        return true;
    }

    //string is null or empty
    public static bool IsNullOrEmpty(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return true;
        }
        return false;
    }
}
