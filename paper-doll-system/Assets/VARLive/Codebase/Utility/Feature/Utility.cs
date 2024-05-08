using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static List<T> EnumToList<T>()
    {
        List<T> list = new List<T>();

        foreach (T t in System.Enum.GetValues(typeof(T)))
        {
            list.Add(t);
        }

        return list;
    }

    /// <summary>
    /// 根據Hierarchy排序，所有元素的父物件一樣才有用
    /// </summary>
    public static List<Transform> SortByHierarchy(List<Transform> sources)
    {
        List<Transform> sorts = new List<Transform>();
        List<KeyValuePair<int, Transform>> childIndexPairTransform = new List<KeyValuePair<int, Transform>>();

        sources.ForEach(p =>
        {
            childIndexPairTransform.Add(new KeyValuePair<int, Transform>(p.transform.GetSiblingIndex(), p));
        });

        childIndexPairTransform.Sort((a, b) =>
        {
            return a.Key.CompareTo(b.Key);
        });

        childIndexPairTransform.ForEach(pair => sorts.Add(pair.Value));

        return sorts;
    }

        /// <summary>
    /// 根據Hierarchy排序，所有元素的父物件一樣才有用
    /// </summary>
    public static List<T> SortByHierarchy<T>(List<T> source) where T : MonoBehaviour
    {
        List<Transform> targets = MonoToTransforms<T>(source);

        targets = SortByHierarchy(targets);

        return TransformToMonos<T>(targets);
    }

    public static List<Transform> MonoToTransforms<T>(List<T> monoBehaviours) where T : MonoBehaviour
    {
        List<Transform> results = new List<Transform>();

        monoBehaviours.ForEach(mono => results.Add(mono.transform));

        return results;
    }


    public static List<T> TransformToMonos<T>(List<Transform> targets) where T : MonoBehaviour
    {
        List<T> results = new List<T>();

        targets.ForEach(target => results.Add(target.GetComponent<T>()));

        return results;
    }

    public static void SwitchItem<T>(List<T> list1, List<T> list2)
    {
        List<T> _list1 = new List<T>(list2);
        List<T> _list2 = new List<T>(list1);

        list1.Clear();
        _list1.ForEach(item => list1.Add(item));

        list2.Clear();
        _list2.ForEach(item => list2.Add(item));
    }
}