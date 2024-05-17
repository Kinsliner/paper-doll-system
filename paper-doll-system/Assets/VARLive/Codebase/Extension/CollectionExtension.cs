using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CollectionExtension
{
    //try add list item extension
    public static bool TryAdd<T>(this List<T> list, T item)
    {
        if (list.Contains(item))
        {
            return false;
        }
        else
        {
            list.Add(item);
            return true;
        }
    }

    // try add dictionary item extension
    public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> list, TKey key, TValue value)
    {
        if (list.ContainsKey(key))
        {
            return false;
        }
        else
        {
            list.Add(key, value);
            return true;
        }
    }

    /// <summary>
    /// 只加還不存在的
    /// </summary>
    public static void TryAddRange<T>(this List<T> target, List<T> adds)
    {
        adds.ForEach(add => target.TryAdd(add));
    }

    public static bool Remove<T>(this List<T> list, Predicate<T> removePredicate)
    {
        T item = list.Find(removePredicate);
        if (item != null)
        {
            list.Remove(item);
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dict, Action<TValue> callback)
    {
        foreach (var item in dict)
        {
            callback.Invoke(item.Value);
        }
    }

    public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dict, Action<TKey, TValue> callback)
    {
        foreach (var item in dict)
        {
            callback.Invoke(item.Key, item.Value);
        }
    }

    public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dict, Action<KeyValuePair<TKey, TValue>> callback)
    {
        foreach (var item in dict)
        {
            callback.Invoke(item);
        }
    }

    public static void Map<T>(this List<T> target, Action<int, T> mapCallback)
    {
        for (int i = 0; i < target.Count; i++)
        {
            mapCallback(i, target[i]);
        }
    }

    /// <summary>
    /// 第三個參數回傳是否是最後一個item
    /// </summary>
    public static void Map<T>(this List<T> target, Action<int, T, bool> mapCallback)
    {
        for (int i = 0; i < target.Count; i++)
        {
            bool isLast = i == (target.Count - 1);

            mapCallback(i, target[i], isLast);
        }
    }

    public static List<T> GetRandom<T>(this List<T> collects, int count) where T : class
    {
        List<T> result = new List<T>();
        if (count > 0)
        {
            result.AddRange(collects);
            result.Shuffle();
            int min = Mathf.Min(result.Count, count);
            result.RemoveRange(min - 1, result.Count - min);
        }
        return result;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static List<T> RandomPick<T>(this List<T> list, int count)
    {
        List<T> result = new List<T>();
        if (count > 0 && list.Count > 0)
        {
            result.AddRange(list);
            result.Shuffle();
            int min = Mathf.Min(result.Count, count);
            result.RemoveRange(min - 1, result.Count - min);
        }
        return result;
    }
}
